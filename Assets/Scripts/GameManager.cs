using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    #region 状态初始值
    public int Energy { get; set; } = 8;
    public int Full { get; set; } = 5;
    public int Mood { get; set; } = 4;
    public bool MasterBeside { get; set; } = true;
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
