using System;
using System.Collections.Generic;

public class P_Sleep : PrimitiveTask
{
    public P_Sleep(float duration) : base(duration)
    {
        this._task = Task.Sleep;
    }

    protected override bool MetCondition_OnRun()
    {
        int energy = HTNWorld.GetWorldState<int>("_energy");
        return energy <= 4; // 睡觉条件：精力值 <= 4
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        int energy = (int)worldState["_energy"];
        return energy <= 4; // 规划时条件：精力值 <= 4
    }

    protected override void Effect_OnRun()
    {
        int energy = HTNWorld.GetWorldState<int>("_energy");
        int mood = HTNWorld.GetWorldState<int>("_mood");

        // 确保 _energy 和 _mood 不超过最大值 10
        HTNWorld.UpdateState("_energy", Math.Min(energy + 4, 10));
        HTNWorld.UpdateState("_mood", Math.Max(mood - 1, 0)); // 心情值不低于 0
        HTNWorld.UpdateState("_masterBeside", false);
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int energy = (int)worldState["_energy"];
        int mood = (int)worldState["_mood"];

        // 确保 _energy 和 _mood 不超过最大值 10
        worldState["_energy"] = Math.Min(energy + 4, 10);
        worldState["_mood"] = Math.Max(mood - 1, 0); // 心情值不低于 0
        worldState["_masterBeside"] = false;
    }
}