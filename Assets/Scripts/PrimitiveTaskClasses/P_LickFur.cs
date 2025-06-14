using System;
using System.Collections.Generic;

public class P_LickFur : PrimitiveTask
{
    public P_LickFur(float duration) : base(duration)
    {
        this._task = Task.LickFur;
    }

    protected override bool MetCondition_OnRun()
    {
        int mood = HTNWorld.GetWorldState<int>("_mood");
        return mood <= 5; // 舔毛条件：心情值 <= 5
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        int mood = (int)worldState["_mood"];
        return mood <= 5; // 规划时条件：心情值 <= 5
    }

    protected override void Effect_OnRun()
    {
        int mood = HTNWorld.GetWorldState<int>("_mood");
        int energy = HTNWorld.GetWorldState<int>("_energy");

        // 确保 _mood 不超过最大值 10
        HTNWorld.UpdateState("_mood", Math.Min(mood + 1, 10));
        // 减少能量
        HTNWorld.UpdateState("_energy", Math.Max(energy - 1, 0));
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int mood = (int)worldState["_mood"];
        int energy = (int)worldState["_energy"];

        // 确保 _mood 不超过最大值 10
        worldState["_mood"] = Math.Min(mood + 1, 10);
        // 减少能量
        worldState["_energy"] = Math.Max(energy - 1, 0);
    }
}