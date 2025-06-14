using System;
using System.Collections.Generic;

public class P_RubMaster : PrimitiveTask
{
    public P_RubMaster(float duration) : base(duration)
    {
        this._task = Task.RubMaster;
    }

    protected override bool MetCondition_OnRun()
    {
        bool masterBeside = HTNWorld.GetWorldState<bool>("_masterBeside");
        return masterBeside; // 蹭主人条件：主人在旁边
    }

    protected override bool MetCondition_OnPlan(Dictionary<string, object> worldState)
    {
        bool masterBeside = (bool)worldState["_masterBeside"];
        return masterBeside; // 规划时条件：主人在旁边
    }

    protected override void Effect_OnRun()
    {
        int mood = HTNWorld.GetWorldState<int>("_mood");

        // 确保 _mood 不超过最大值 10
        HTNWorld.UpdateState("_mood", Math.Max(mood - 2, 0));
    }

    protected override void Effect_OnPlan(Dictionary<string, object> worldState)
    {
        int mood = (int)worldState["_mood"];

        // 确保 _mood 不超过最大值 10
        worldState["_mood"] = Math.Max(mood - 2, 0);
    }
}