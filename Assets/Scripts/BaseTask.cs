using System.Collections.Generic;

// 用于描述运行结果的枚举
public enum EStatus
{
    Failure, Success, Running, 
}

public interface IBaseTask
{
    // 判断是否满足条件
    bool MetCondition(Dictionary<string, object> worldState);
    // 添加子任务
    void AddNextTask(IBaseTask nextTask);
}
