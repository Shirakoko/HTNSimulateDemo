using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class PrimitiveTaskGenerator : EditorWindow
{
    private const string ConfigPath = "Assets/Configs/HTNConfig.json";
    private const string OutputFolder = "Assets/Scripts/PrimitiveTaskClasses/";
    private static Dictionary<string, WorldStateField> _worldStateFields = new Dictionary<string, WorldStateField>();
    
    [MenuItem("Tools/Generate Primitive Tasks")]
    public static void Generate()
    {
        if (!File.Exists(ConfigPath))
        {
            Debug.LogError($"Config file not found: {ConfigPath}");
            return;
        }

        string json = File.ReadAllText(ConfigPath);
        var config = JsonUtility.FromJson<PrimitiveTaskConfig>(json);

        // 初始化 WorldState 字段信息
        _worldStateFields.Clear();
        foreach (var field in config.WorldState)
        {
            _worldStateFields[field.name] = field;
        }

        // 生成枚举
        GenerateTaskEnum(config.PrimitiveClasses);

        // 生成原子任务类
        if (!Directory.Exists(OutputFolder))
        {
            Directory.CreateDirectory(OutputFolder);
        }

        foreach (var task in config.PrimitiveClasses)
        {
            string className = $"P_{task.name}";
            string filePath = Path.Combine(OutputFolder, $"{className}.cs");
            
            if (File.Exists(filePath))
            {
                Debug.Log($"Skipping existing file: {filePath}");
                continue;
            }

            string code = GenerateClassCode(task);
            File.WriteAllText(filePath, code);
            Debug.Log($"Generated: {filePath}");
        }

        AssetDatabase.Refresh();
    }

    private static string GenerateClassCode(PrimitiveTaskClass task)
    {
        
        var sb = new StringBuilder();
        
        // Using directives
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();

        // Class declaration
        sb.AppendLine($"public class {task.GetClassName()} : PrimitiveTask");
        sb.AppendLine("{");

        // Constructor
        sb.AppendLine($"    public {task.GetClassName()}(float duration) : base(duration)");
        sb.AppendLine("    {");
        sb.AppendLine($"        this._task = Task.{task.name};");
        sb.AppendLine("    }");
        sb.AppendLine();

        // MetCondition_OnRun
        sb.AppendLine("    protected override bool MetCondition_OnRun()");
        sb.AppendLine("    {");
        if (task.conditions == null || task.conditions.Count == 0)
        {
            sb.AppendLine("        return true; // 无条件");
        }
        else
        {
            sb.AppendLine(GenerateConditionChecks(task.conditions));
        }
        sb.AppendLine("    }");
        sb.AppendLine();

        // MetCondition_OnPlan
        sb.AppendLine("    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)");
        sb.AppendLine("    {");
        if (task.conditions == null || task.conditions.Count == 0)
        {
            sb.AppendLine("        return true; // 无条件");
        }
        else
        {
            sb.AppendLine(GeneratePlanningConditionChecks(task.conditions));
        }
        sb.AppendLine("    }");
        sb.AppendLine();

        // Effect_OnRun
        sb.AppendLine("    protected override void Effect_OnRun()");
        sb.AppendLine("    {");
        sb.AppendLine(GenerateEffectCode(task.effects, false));
        sb.AppendLine("    }");
        sb.AppendLine();

        // Effect_OnPlan
        sb.AppendLine("    protected override void Effect_OnPlan(Dictionary<string, object> worldState)");
        sb.AppendLine("    {");
        sb.AppendLine(GenerateEffectCode(task.effects, true));
        sb.AppendLine("    }");

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateConditionChecks(List<Condition> conditions)
    {
        if (conditions == null || conditions.Count == 0) return "        return true;";
        
        var checks = new List<string>();
        foreach (var cond in conditions)
        {
            checks.Add(ParseCondition(cond, false));
        }
        return $"        return {string.Join(" && ", checks)};";
    }

    private static string GeneratePlanningConditionChecks(List<Condition> conditions)
    {
        if (conditions == null || conditions.Count == 0) return "        return true;";
        
        var checks = new List<string>();
        foreach (var cond in conditions)
        {
            checks.Add(ParseCondition(cond, true));
        }
        return $"        return {string.Join(" && ", checks)};";
    }

    private static string ParseCondition(Condition cond, bool isPlanning)
    {
        if (cond == null || string.IsNullOrEmpty(cond.expression) || string.IsNullOrEmpty(cond.field))
        {
            Debug.LogError("Invalid condition: condition is null or missing required fields.");
            return "false";
        }

        var parts = cond.expression.Split(' ');
        if (parts.Length < 2)
        {
            Debug.LogError($"Invalid condition expression: {cond.expression}");
            return "false";
        }

        var varName = cond.field;
        var op = parts[0].Trim();
        var value = parts[1].Trim();

        if (isPlanning)
        {
            // 根据字段类型生成正确的类型转换
            return cond.type.ToLower() switch
            {
                "int" => $"(int)worldState[\"{varName}\"] {op} {value}",
                "bool" => $"(bool)worldState[\"{varName}\"] {op} {value}",
                _ => throw new System.NotImplementedException($"Unsupported type: {cond.type}")
            };
        }

        return $"HTNWorld.GetWorldState<{GetCSharpType(cond.type)}>(\"{varName}\") {op} {value}";
    }

    private static string GenerateEffectCode(List<Effect> effects, bool isPlanning)
    {
        var sb = new StringBuilder();
        foreach (var effect in effects)
        {
            var varName = effect.field;
            var operation = effect.operation;
            var value = effect.value;

            if (isPlanning)
            {
                sb.AppendLine($"        worldState[\"{varName}\"] = {GetEffectExpression(varName, operation, value, true)};");
            }
            else
            {
                sb.AppendLine($"        HTNWorld.UpdateState(\"{varName}\", {GetEffectExpression(varName, operation, value, false)});");
            }
        }
        return sb.ToString();
    }

    private static string GetEffectExpression(string varName, string operation, string value, bool isPlanning)
    {
        if (!_worldStateFields.TryGetValue(varName, out WorldStateField field))
        {
            Debug.LogError($"WorldState field {varName} not found.");
            return "0";
        }

        string currentValue = isPlanning 
            ? $"(int)worldState[\"{varName}\"]" 
            : $"HTNWorld.GetWorldState<{GetCSharpType(field.type)}>(\"{varName}\")";

        // 处理不同类型字段
        if (field.type.ToLower() == "bool")
        {
            return operation switch
            {
                "=" => value.ToLower(),
                _ => throw new System.NotImplementedException()
            };
        }
        else // int 类型
        {
            return operation switch
            {
                "+" => ApplyRange($"{currentValue} + {value}", field.max, field.min),
                "-" => ApplyRange($"{currentValue} - {value}", field.max, field.min),
                "=" => ApplyRange(value, field.max, field.min),
                _ => throw new System.NotImplementedException()
            };
        }
    }

    private static string ApplyRange(string expression, int max, int min)
    {
        return $"Math.Clamp({expression}, {min}, {max})";
    }

    private static string GetCSharpType(string type) => type.ToLower() switch
    {
        "int" => "int",
        "bool" => "bool",
        _ => "object"
    };

    private static void GenerateTaskEnum(List<PrimitiveTaskClass> tasks)
    {
        string enumFilePath = "Assets/Scripts/PrimitiveTask.cs";
        if (!File.Exists(enumFilePath))
        {
            Debug.LogError($"PrimitiveTask.cs not found at: {enumFilePath}");
            return;
        }

        // 读取文件内容
        var lines = File.ReadAllLines(enumFilePath).ToList();

        // 找到枚举定义的起始和结束行
        int enumStartIndex = lines.FindIndex(line => line.Contains("public enum Task"));
        int enumEndIndex = lines.FindIndex(enumStartIndex, line => line.Contains("}"));

        if (enumStartIndex == -1 || enumEndIndex == -1)
        {
            Debug.LogError("Failed to find Task enum in PrimitiveTask.cs.");
            return;
        }

        // 生成新的枚举内容
        var enumLines = new List<string>
        {
            "public enum Task",
            "{"
        };

        foreach (var task in tasks)
        {
            enumLines.Add($"    {task.name},");
        }

        enumLines.Add("}");

        // 替换旧的枚举内容
        lines.RemoveRange(enumStartIndex, enumEndIndex - enumStartIndex + 1);
        lines.InsertRange(enumStartIndex, enumLines);

        // 写回文件
        File.WriteAllLines(enumFilePath, lines);
        Debug.Log($"Updated Task enum in {enumFilePath}");
    }

}

// JSON数据模型
[System.Serializable]
public class PrimitiveTaskConfig
{
    public List<WorldStateField> WorldState;
    public List<PrimitiveTaskClass> PrimitiveClasses;
}

[System.Serializable]
public class WorldStateField
{
    public string name;
    public string type;
    public int min;
    public int max;
}

[System.Serializable]
public class PrimitiveTaskClass
{
    public string name;
    public string cnName;
    public List<Condition> conditions;
    public List<Effect> effects;

    public string GetClassName() => $"P_{name}";
}

[System.Serializable]
public class Condition
{
    public string field;
    public string type;
    public string expression;
}

[System.Serializable]
public class Effect
{
    public string field;
    public string operation;
    public string value;
}