using System;
using System.Collections.Generic;


// 世界状态管理器，使用静态类实现单例模式
public static class HTNWorld
{
    // 用于读取世界状态的字典，键为状态名，值为获取状态的委托
    private static readonly Dictionary<string, Func<object>> get_WorldState;
    // 用于写入世界状态的字典，键为状态名，值为设置状态的委托
    private static readonly Dictionary<string, Action<object>> set_WorldState;
    
    static HTNWorld()
    {
        get_WorldState = new Dictionary<string, Func<object>>();
        set_WorldState = new Dictionary<string, Action<object>>();
    }

    /// <summary>
    /// 添加一个世界状态
    /// </summary>
    /// <param name="key">状态名称</param>
    /// <param name="getter">获取状态值的委托</param>
    /// <param name="setter">设置状态值的委托</param>
    public static void AddState(string key, Func<object> getter, Action<object> setter)
    {
        get_WorldState[key] = getter;
        set_WorldState[key] = setter;
    }

    /// <summary>
    /// 移除一个世界状态
    /// </summary>
    /// <param name="key">状态名称</param>
    public static void RemoveState(string key)
    {
        get_WorldState.Remove(key);
        set_WorldState.Remove(key);
    }

    /// <summary>
    /// 更新某个状态的值
    /// </summary>
    /// <param name="key">状态名称</param>
    /// <param name="value">新的状态值</param>
    public static void UpdateState(string key, object value)
    {
        if (set_WorldState.ContainsKey(key))
        {
            set_WorldState[key].Invoke(value);
        }
    }

    /// <summary>
    /// 获取某个状态的值，并转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="key">状态名称</param>
    /// <returns>状态值</returns>
    public static T GetWorldState<T>(string key) 
    {
        if (get_WorldState.ContainsKey(key))
        {
            return (T)get_WorldState[key].Invoke();
        }
        else
        {
            throw new KeyNotFoundException($"状态 '{key}' 未找到。");
        }
    }

    /// <summary>
    /// 复制当前世界状态，生成一个快照
    /// </summary>
    /// <returns>包含所有状态名称和值的字典</returns>
    public static Dictionary<string, object> CopyWorldState()
    {
        var copy = new Dictionary<string, object>();
        foreach(var state in get_WorldState)
        {
            copy.Add(state.Key, state.Value.Invoke());
        }
        return copy;
    }
}
