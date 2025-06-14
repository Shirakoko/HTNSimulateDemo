using System;
using System.Collections.Generic;
using UnityEngine;

public class P_Idle : PrimitiveTask
{
    public P_Idle(float duration) : base(duration)
    {
        this._task = Task.Idle;
    }

    protected override bool MetCondition_OnRun()
    {
        // 无条件执行
        return true;
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        // 无条件执行
        return true;
    }

    protected override void Effect_OnRun()
    {
        int mood = HTNWorld.GetWorldState<int>("_mood");

        // 确保 _mood 不超过最大值 10
        HTNWorld.UpdateState("_mood", Math.Min(mood + 1, 10));
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int mood = (int)worldState["_mood"];

        // 确保 _mood 不超过最大值 10
        worldState["_mood"] = Math.Min(mood + 1, 10);
    }
}