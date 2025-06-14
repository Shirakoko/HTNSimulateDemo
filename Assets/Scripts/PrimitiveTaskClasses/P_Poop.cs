using System;
using System.Collections.Generic;
using UnityEngine;

public class P_Poop : PrimitiveTask
{
    public P_Poop(float duration) : base(duration)
    {
        this._task = Task.Poop;
    }

    protected override bool MetCondition_OnRun()
    {
        int full = HTNWorld.GetWorldState<int>("_full");
        return full >= 6; // 拉屎条件：饱腹值 >= 6
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        int full = (int)worldState["_full"];
        return full >= 6; // 规划时条件：饱腹值 >= 6
    }

    protected override void Effect_OnRun()
    {
        int full = HTNWorld.GetWorldState<int>("_full");
        int mood = HTNWorld.GetWorldState<int>("_mood");

        // 确保 _full 不低于最小值 0
        HTNWorld.UpdateState("_full", Math.Max(full - 2, 0));
        HTNWorld.UpdateState("_mood", Math.Max(mood - 2, 0));
        HTNWorld.UpdateState("_masterBeside", false);
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int full = (int)worldState["_full"];
        int mood = (int)worldState["_mood"];

        // 确保 _full 不低于最小值 0
        worldState["_full"] = Math.Max(full - 2, 0);
        worldState["_mood"] = Math.Max(mood - 2, 0);
        worldState["_masterBeside"] = false;
    }
}