using System;
using System.Collections.Generic;

public class Method : IBaseTask
{
    // 子任务列表，支持复合任务和原子任务
    public List<IBaseTask> SubTask { get; private set; }

    // 方法的前提条件
    private readonly Func<bool> condition;

    public Method(Func<bool> condition)
    {
        SubTask = new List<IBaseTask>();
        this.condition = condition;
    }

    /// <summary>
    /// 检查方法是否满足条件
    /// </summary>
    /// <param name="worldState">世界状态（传入副本）</param>
    /// <returns>
    /// true：方法自身条件和所有子任务条件均满足
    /// false：方法自身条件或任意子任务条件不满足
    /// </returns>
    public bool MetCondition(Dictionary<string, object> worldState = null)
    {
        // 创建临时世界状态副本，用于追踪子任务的效果
        var tpWorld = new Dictionary<string, object>(worldState);

        // 检查方法自身的前提条件
        if (condition())
        {
            // 检查所有子任务的条件
            for (int i = 0; i < SubTask.Count; ++i)
            {
                if (!SubTask[i].MetCondition(tpWorld))
                {
                    // 任意子任务条件不满足，方法失败
                    return false;
                }
            }

            // 所有子任务条件满足，将临时状态应用到真实世界状态
            worldState = tpWorld;
            return true;
        }

        // 方法自身条件不满足
        return false;
    }

    /// <summary>
    /// 添加子任务
    /// </summary>
    /// <param name="nextTask">子任务（复合任务或原子任务）</param>
    public void AddNextTask(IBaseTask nextTask)
    {
        SubTask.Add(nextTask);
    }
}
