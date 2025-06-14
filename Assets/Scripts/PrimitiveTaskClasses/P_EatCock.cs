using System;
using System.Collections.Generic;

public class P_EatCock : PrimitiveTask
{
    public P_EatCock(float duration) : base(duration)
    {
        this._task = Task.EatCock;
    }

    protected override bool MetCondition_OnRun()
    {
        int full = HTNWorld.GetWorldState<int>("_full");
        return full <= 7; // 吃蟑螂条件：饱腹值 <= 7
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        int full = (int)worldState["_full"];
        return full <= 7; // 规划时条件：饱腹值 <= 7
    }

    protected override void Effect_OnRun()
    {
        int full = HTNWorld.GetWorldState<int>("_full");
        int mood = HTNWorld.GetWorldState<int>("_mood");

        // 确保 _full 不超过最大值 10，_mood 不低于最小值 0
        HTNWorld.UpdateState("_full", Math.Min(full + 1, 10));
        HTNWorld.UpdateState("_mood", Math.Max(mood - 3, 0));
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int full = (int)worldState["_full"];
        int mood = (int)worldState["_mood"];

        // 确保 _full 不超过最大值 10，_mood 不低于最小值 0
        worldState["_full"] = Math.Min(full + 1, 10);
        worldState["_mood"] = Math.Max(mood - 3, 0);
    }
}