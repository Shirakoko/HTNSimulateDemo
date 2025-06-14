using System;
using System.Collections.Generic;

public class P_Drink : PrimitiveTask
{
    public P_Drink(float duration) : base(duration)
    {
        this._task = Task.Drink;
    }

    // 无条件
    protected override bool MetCondition_OnRun() => true; // 无条件

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState) => true; // 无条件

    protected override void Effect_OnRun()
    {
        int full = HTNWorld.GetWorldState<int>("_full");
        int mood = HTNWorld.GetWorldState<int>("_mood");

        // 确保 _full 和 _mood 不超过最大值 10
        HTNWorld.UpdateState("_full", Math.Min(full + 1, 10));
        HTNWorld.UpdateState("_mood", Math.Min(mood + 1, 10));
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int full = (int)worldState["_full"];
        int mood = (int)worldState["_mood"];

        // 确保 _full 和 _mood 不超过最大值 10
        worldState["_full"] = Math.Min(full + 1, 10);
        worldState["_mood"] = Math.Min(mood + 1, 10);
    }
}