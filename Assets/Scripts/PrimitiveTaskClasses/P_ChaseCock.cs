using System;
using System.Collections.Generic;
using UnityEngine;

public class P_ChaseCock : PrimitiveTask
{
    public P_ChaseCock(float duration) : base(duration)
    {
        this._task = Task.ChaseCock;
    }

    protected override bool MetCondition_OnRun()
    {
        int energy = HTNWorld.GetWorldState<int>("_energy");
        return energy >= 5; // 追蟑螂条件：精力值 >= 5
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        int energy = (int)worldState["_energy"];
        return energy >= 5; // 规划时条件：精力值 >= 5
    }

    protected override void Effect_OnRun()
    {
        int energy = HTNWorld.GetWorldState<int>("_energy");
        int full = HTNWorld.GetWorldState<int>("_full");
        int mood = HTNWorld.GetWorldState<int>("_mood");

        // 确保 _energy 和 _full 不低于最小值 0，_mood 不超过最大值 10
        HTNWorld.UpdateState("_energy", Math.Max(energy - 3, 0));
        HTNWorld.UpdateState("_full", Math.Max(full - 2, 0));
        HTNWorld.UpdateState("_mood", Math.Min(mood + 1, 10));
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int energy = (int)worldState["_energy"];
        int full = (int)worldState["_full"];
        int mood = (int)worldState["_mood"];

        // 确保 _energy 和 _full 不低于最小值 0，_mood 不超过最大值 10
        worldState["_energy"] = Math.Max(energy - 3, 0);
        worldState["_full"] = Math.Max(full - 2, 0);
        worldState["_mood"] = Math.Min(mood + 1, 10);
    }

    protected override void TaskStartOperation()
    {
        // 出现蟑螂
        CatHTN.Instance.CockGo.SetActive(true);
    }

    protected override void TaskEndOperation()
    {
        // 隐藏蟑螂
        CatHTN.Instance.CockGo.SetActive(false);
    }
}