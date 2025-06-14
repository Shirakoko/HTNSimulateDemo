# 项目说明

本项目是一个基于 **Unity 2022.3.47f1c1** 开发的应用程序。以下是项目的详细说明和使用指南。

------

## 项目概述

- **开发引擎**：Unity 2022.3.47f1c1
- **第三方库**：DOTWEEN（用于动画和过渡效果）
- 分支说明
  - **Assets 分支**：包含项目的完整资源文件（Assets 文件夹）。
  - **Scripts 分支**：仅包含项目的脚本部分。

------

## 运行方式

### 1. 下载 Windows 版本

从 Releases 下载 Windows 版本：[![GitHub release (latest by date)](https://img.shields.io/github/v/release/Shirakoko/HTNSimulateDemo)](https://github.com/yourusername/Shirakoko/HTNSimulateDemo/latest)，解压后双击运行可执行文件 HTNSimulateDemo.exe。

### 2. 在线运行 WebGL 版本

访问 itch.io，直接在浏览器中运行 WebGL 版本：[HTNSimulateDemo by Yuki](https://yukilovesgames.itch.io/htnsimulatedemo)

------

## 优化空间

### 1.配置文件驱动的 HTN 网络生成

目前的世界状态、任务条件和HTN网络结构都是写死在代码里的；后续可通过JSON或YAML配置文件动态生成HTN网络（包括**世界状态定义**、**原子任务类**、**HTN网络构建**的代码），支持用户定义世界状态、任务和条件，自动生成原子任务具体类和网络结构，提升灵活性和可维护性。

例如，通过以下json对象：

```json
{
  "name": "Bat",
  "cnName": "打人",
  "conditions": [
      {
          "field": "_mood",
          "type": "int",
          "expression": "<= 3"
      },
      {
          "field": "_masterBeside",
          "type": "bool",
          "expression": "== true"
      }
  ],
  "effects": [
      { 
          "field": "_mood", 
          "operation": "+", 
          "value": "1" 
      },
      { 
          "field": "_masterBeside", 
          "operation": "=", 
          "value": "false" 
      }
  ]
}
```

生成`P_Bat.cs`原子任务类脚本：

```csharp
using System;
using System.Collections.Generic;

public class P_Bat : PrimitiveTask
{
    public P_Bat(float duration) : base(duration)
    {
        this._task = Task.Bat;
    }

    protected override bool MetCondition_OnRun()
    {
        return HTNWorld.GetWorldState<int>("_mood") <= 3 && HTNWorld.GetWorldState<bool>("_masterBeside") == true;
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        return (int)worldState["_mood"] <= 3 && (bool)worldState["_masterBeside"] == true;
    }

    protected override void Effect_OnRun()
    {
        HTNWorld.UpdateState("_mood", Math.Clamp(HTNWorld.GetWorldState<int>("_mood") + 1, 0, 10));
        HTNWorld.UpdateState("_masterBeside", false);

    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        worldState["_mood"] = Math.Clamp((int)worldState["_mood"] + 1, 0, 10);
        worldState["_masterBeside"] = false;

    }
}
```

部分功能已实现：


生成工具：[PrimitiveTaskGenerator-Shirakoko/HTNSimulateDemo](https://github.com/Shirakoko/HTNSimulateDemo/tree/Assets/Editor/PrimitiveTaskGenerator)

配置文件：[Configs-Shirakoko/HTNSimulateDemo](https://github.com/Shirakoko/HTNSimulateDemo/tree/Assets/Configs)


### 2.动态世界状态管理

引入动态世界状态机制（昼夜交替、饱腹度随时间流逝等），支持游戏进程自动更新状态，且允许用户通过配置文件或编辑器动态添加、修改、删除变量，增强HTN网络的适应性。

### 3.方法优先级与权重

目前方法的选择仅有顺序选择和随机选择两种模式，可为方法添加优先级和权重参数，使HTN网络行为更符合预期。
