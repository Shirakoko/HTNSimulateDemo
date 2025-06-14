using System.Collections.Generic;

public class CompoundTask : IBaseTask
{
    // 当前选中的有效方法
    public Method ValidMethod { get; private set; }

    // 子任务（方法）列表
    private readonly List<Method> methods;

    public CompoundTask()
    {
        methods = new List<Method>();
    }

    /// <summary>
    /// 添加子任务（仅支持添加方法）
    /// </summary>
    /// <param name="nextTask">待添加的任务</param>
    public void AddNextTask(IBaseTask nextTask)
    {
        // 检查任务类型，仅添加方法
        if (nextTask is Method method)
        {
            methods.Add(method);
        }
    }

    /// <summary>
    /// 检查复合任务是否满足条件（顺序选择逻辑）
    /// </summary>
    /// <param name="worldState">世界状态</param>
    /// <returns>
    /// true：至少有一个方法满足条件
    /// false：所有方法均不满足条件
    /// </returns>
    public bool MetCondition_Sequential(Dictionary<string, object> worldState)
    {
        // 遍历所有方法，按顺序检查条件
        for (int i = 0; i < methods.Count; ++i)
        {
            if (methods[i].MetCondition(worldState))
            {
                // 记录第一个满足条件的方法
                ValidMethod = methods[i];
                return true;
            }
        }

        // 没有方法满足条件
        return false;
    }

    /// <summary>
    /// 检查复合任务是否满足条件（随机选择逻辑）
    /// </summary>
    /// <param name="worldState">世界状态</param>
    /// <returns>
    /// true：至少有一个方法满足条件
    /// false：所有方法均不满足条件
    /// </returns>
    public bool MetCondition_Random(Dictionary<string, object> worldState)
    {
        // 收集所有满足条件的方法
        var validMethods = new List<Method>();
        for (int i = 0; i < methods.Count; ++i)
        {
            if (methods[i].MetCondition(worldState))
            {
                validMethods.Add(methods[i]);
            }
        }

        // 如果有满足条件的方法，则随机选择一个
        if (validMethods.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, validMethods.Count);
            ValidMethod = validMethods[randomIndex];
            return true;
        }

        // 没有方法满足条件
        return false;
    }

    /// <summary>
    /// 默认的条件检查方法（可根据需求选择顺序或随机逻辑）
    /// </summary>
    public bool MetCondition(Dictionary<string, object> worldState)
    {
        // 默认使用顺序选择逻辑
        // return MetCondition_Sequential(worldState);

        // 如果需要随机选择逻辑，可以改为：
        return MetCondition_Random(worldState);
    }
}
