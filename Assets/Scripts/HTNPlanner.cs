using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HTNPlanner
{
    // 最终分解完成的所有原子任务存放的列表
    public Stack<PrimitiveTask> FinalTasks { get; private set; }

    // 分解过程中，用来缓存被分解出的任务（复合任务和原子任务）的栈
    private readonly Stack<IBaseTask> taskOfProcess;
    private readonly CompoundTask rootTask; //根任务

    public HTNPlanner(CompoundTask rootTask)
    {
        this.rootTask = rootTask;
        taskOfProcess = new Stack<IBaseTask>();
        FinalTasks = new Stack<PrimitiveTask>();
    }

    /// <summary>
    /// 执行规划（核心逻辑）
    /// </summary>
    public void Plan()
    {
        // 复制一份世界状态
        var worldState = HTNWorld.CopyWorldState();
        // 将存储列表清空，避免上次计划结果的影响
        FinalTasks.Clear();
        // 将根任务压进栈中，准备分解
        taskOfProcess.Push(rootTask);
        // 只要栈还没空，就继续分解
        while(taskOfProcess.Count > 0)
        {
            // 拿出栈顶的元素
            var task = taskOfProcess.Pop();
            // 元素是复合任务
            if(task is CompoundTask cTask)
            {
                // 判断是否可以执行
                if(cTask.MetCondition(worldState))
                {
                    // 如果可以执行，就肯定有可用的方法，将该方法的子任务都压入栈中，以继续分解
                    var subTask = cTask.ValidMethod.SubTask;
                    foreach(var t in subTask)
                    {
                        taskOfProcess.Push(t);
                    }
                    /* 通过上面的步骤可知，能被压进栈中的只有
                    复合任务和原子任务，方法本身并不会入栈 */
                }
            }
            else
            {
                // 元素是原子任务；将该元素转为原子任务加入存放分解完成的任务列表
                var pTask = task as PrimitiveTask;
                FinalTasks.Push(pTask);
            }
        }
        
        string taskNames = string.Join(", ", FinalTasks.Select(task => task.GetTaskName()));
        Debug.Log($"计划完毕，队列中的任务有: {taskNames}");
        CatHTN.Instance.SetStateText($"计划完毕，队列中的任务有: {taskNames}");
    }
}
