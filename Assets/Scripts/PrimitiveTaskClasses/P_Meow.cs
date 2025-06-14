using System;
using System.Collections.Generic;

public class P_Meow : PrimitiveTask
{
    public P_Meow(float duration) : base(duration)
    {
        this._task = Task.Meow;
    }

    protected override bool MetCondition_OnRun()
    {
        int mood = HTNWorld.GetWorldState<int>("_mood");
        int full = HTNWorld.GetWorldState<int>("_full");
        return mood >= 8 && full >= 5; // 叫唤条件：心情值 >= 8 且饱腹值 >= 5
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        int mood = (int)worldState["_mood"];
        int full = (int)worldState["_full"];
        return mood >= 7 && full >= 5; // 规划时条件：心情值 >= 8 且饱腹值 >= 5
    }

    protected override void Effect_OnRun()
    {
        int mood = HTNWorld.GetWorldState<int>("_mood");
        int full = HTNWorld.GetWorldState<int>("_full");

        HTNWorld.UpdateState("_mood", Math.Max(mood - 1, 0));
        HTNWorld.UpdateState("_full", Math.Max(full - 1, 0));
        HTNWorld.UpdateState("_masterBeside", true);
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int mood = (int)worldState["_mood"];
        int full = HTNWorld.GetWorldState<int>("_full");


        // 确保 _mood 不低于最小值 0
        worldState["_mood"] = Math.Max(mood - 1, 0);
        worldState["_full"] = Math.Max(full - 1, 0);
        worldState["_masterBeside"] = true;
    }
}