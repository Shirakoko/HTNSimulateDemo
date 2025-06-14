using System.Collections.Generic;

public partial class HTNPlanBuilder
{
    private HTNPlanner planner; 
    private HTNPlanRunner runner;
    private readonly Stack<IBaseTask> taskStack; // 辅助栈，用于构建HTN
    
    public HTNPlanBuilder()
    {
        taskStack = new Stack<IBaseTask>();
    }
    
    private void AddTask(IBaseTask task)
    {
        if (planner != null)
        {
            // 当前计划器不为空，将新任务作为构造栈顶元素的子任务
            taskStack.Peek().AddNextTask(task);
        }
        else 
        {
            // 当前计划器为空，初始化规划器和执行器
            planner = new HTNPlanner(task as CompoundTask);
            runner = new HTNPlanRunner(planner);
        }
        
        if (task is not PrimitiveTask)
        {
            //如果新任务是原子任务，就不需要进栈了，因为原子任务不会有子任务
            taskStack.Push(task);
        }
    }

    public void RunPlan()
    {
        runner.RunPlan();
    }
    public HTNPlanBuilder Back()
    {
        taskStack.Pop();
        return this;
    }
    public HTNPlanner End()
    {
        taskStack.Clear();
        return planner;
    }

    /// <summary>
    /// 添加原子任务
    /// </summary>
    /// <param name="task">原子任务</param>
    /// <returns>构造器本身</returns>
    public HTNPlanBuilder AddPrimitiveTask(PrimitiveTask task)
    {
        AddTask(task);
        return this;
    }

    /// <summary>
    /// 添加复合任务
    /// </summary>
    /// <returns>构造器本身</returns>
    public HTNPlanBuilder AddCompoundTask()
    {
        var task = new CompoundTask();
        AddTask(task);
        return this;
    }

    /// <summary>
    /// 添加方法
    /// </summary>
    /// <param name="condition">方法执行的条件</param>
    /// <returns>构造器本身</returns>
    public HTNPlanBuilder AddMethod(System.Func<bool> condition)
    {
        var task = new Method(condition);
        AddTask(task);
        return this;
    }
}