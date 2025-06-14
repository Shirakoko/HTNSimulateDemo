using System.Collections.Generic;
using UnityEngine;

public enum Task
{
    ChaseCock,
    Destroy,
    Drink,
    Eat,
    EatCock,
    Idle,
    LickFur,
    Meow,
    Parkour,
    Poop,
    RubMaster,
    Sleep,
}

public abstract class PrimitiveTask : IBaseTask
{
    protected Task _task;
    protected float _startTime = -1f;
    protected float _duration = 0.0f;
    private bool _isMoving;

    // 任务类型与中文名的映射字典
    private static readonly Dictionary<Task, string> TaskNameMap = new Dictionary<Task, string>
    {
        { Task.ChaseCock, "追蟑螂" },
        { Task.Destroy, "拆家" },
        { Task.Drink, "喝水" },
        { Task.Eat, "吃饭" },
        { Task.EatCock, "吃蟑螂" },
        { Task.Idle, "发呆" },
        { Task.LickFur, "舔毛" },
        { Task.Meow, "叫唤" },
        { Task.Parkour, "跑酷" },
        { Task.Poop, "拉屎" },
        { Task.RubMaster, "蹭主人" },
        { Task.Sleep, "睡觉" },
    };

    public PrimitiveTask(float duration)
    {
        this._duration = duration;
        // this._operation = operation;
    }

    // 获取任务的中文名
    public string GetTaskName()
    {
        return TaskNameMap[_task];
    }

    //原子任务不可以再分解为子任务，所以AddNextTask方法不必实现
    void IBaseTask.AddNextTask(IBaseTask nextTask)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 执行前判断条件是否满足，传入null时直接修改HTNWorld
    /// </summary>
    /// <param name="worldState">用于plan的世界状态副本</param>
    public bool MetCondition(Dictionary<string, object> worldState = null)
    {
        if (worldState == null)
        {
            // 运行时直接检查真实世界状态
            return MetCondition_OnRun();
        }
        
        // 规划时检查副本状态，若条件满足则立即应用效果
        if (MetCondition_OnPlan(worldState))
        {
            Effect_OnPlan(worldState);
            return true;
        }
        return false;
    }

    protected virtual bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        return true;
    }
    protected virtual bool MetCondition_OnRun()
    {
        return true;
    }
    
    // 任务的具体运行逻辑
    public virtual EStatus Operator()
    {
        if (_startTime < 0)
        {
            if (_isMoving)
            {
                // 如果正在移动，返回 Running 状态
                return EStatus.Running;
            }

            // 开始移动
            _isMoving = true;
            CatHTN.Instance.MoveToNextPosition(this._task, () =>
            {
                // 移动完成后，设置 _startTime 并执行任务逻辑
                _startTime = Time.time;
                Debug.Log($"开始{GetTaskName()}...");
                CatHTN.Instance.ShowDialog($"开始{GetTaskName()}...");

                this.TaskStartOperation(); // 调用任务开始方法（材质、UI、特效、音效等）
                _isMoving = false; // 移动完成
            });

            return EStatus.Running;
        }

        // 如果任务已经完成，返回 Success 状态
        if (Time.time - _startTime >= this._duration)
        {
            this.TaskEndOperation(); // 调用任务结束方法（材质、UI、特效、音效等）

            Debug.Log($"{GetTaskName()}完毕，耗时{this._duration}");
            CatHTN.Instance.ShowDialog($"{GetTaskName()}完毕，耗时{this._duration}s");
            CatHTN.Instance.HideDialog();
            _startTime = -1; // 重置计时器
            return EStatus.Success;
        }

        // 任务正在执行中
        return EStatus.Running;
    }

    //执行成功后的影响
    public void Effect()
    {
        Effect_OnRun();
    }
    
    // 规划模式效果应用（子类可覆盖）
    protected virtual void Effect_OnPlan(Dictionary<string, object> worldState) { }

    // 运行时效果应用（子类可覆盖）
    protected virtual void Effect_OnRun() { }

    /// <summary>
    /// 任务开始操作，子类可覆写
    /// </summary>
    protected virtual void TaskStartOperation() {}

    /// <summary>
    /// 任务结束操作，子类可覆写
    /// </summary>
    protected virtual void TaskEndOperation() {}
}
