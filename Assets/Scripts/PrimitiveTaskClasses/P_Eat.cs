using System;
using System.Collections.Generic;
using UnityEngine;

public class P_Eat : PrimitiveTask
{
    public P_Eat(float duration) : base(duration)
    {
        this._task = Task.Eat;
    }

    protected override bool MetCondition_OnRun()
    {
        int full = HTNWorld.GetWorldState<int>("_full");
        return full <= 8; // 吃饭条件：饱腹值 <= 8
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        int full = (int)worldState["_full"];
        return full <= 8; // 规划时条件：饱腹值 <= 8
    }

    protected override void Effect_OnRun()
    {
        int full = HTNWorld.GetWorldState<int>("_full");
        int mood = HTNWorld.GetWorldState<int>("_mood");

        // 确保 _full 和 _mood 不超过最大值 10
        HTNWorld.UpdateState("_full", Math.Min(full + 2, 10));
        HTNWorld.UpdateState("_mood", Math.Min(mood + 1, 10));
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int full = (int)worldState["_full"];
        int mood = (int)worldState["_mood"];

        // 确保 _full 和 _mood 不超过最大值 10
        worldState["_full"] = Math.Min(full + 2, 10);
        worldState["_mood"] = Math.Min(mood + 1, 10);
    }
}