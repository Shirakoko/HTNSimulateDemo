using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Diagnostics;

public class CatHTN : MonoBehaviour
{
    private static CatHTN _instance;
    public static CatHTN Instance => _instance;

    #region 猫猫的状态
    public int energy;
    public int full;
    public int mood;
    public bool masterBeside;
    #endregion

    [Header("猫猫移动速度")]
    public float moveSpeed = 2.0f;

    #region UI
    private GameObject _panelDialogGo;
    private Text _textDialog;
    public Text _textState;
    #endregion

    #region 其他角色
    private GameObject _cockGo;
    public GameObject CockGo => _cockGo;
    private GameObject _masterGo;
    #endregion

    private Animator animator;
    private Dictionary<Task, AnimationClip> clipDict;

    // 存储任务类型和任务执行位置的字典
    private Dictionary<Task, Vector3> _taskPositions = new Dictionary<Task, Vector3>();

    private HTNPlanBuilder htnBuilder;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // 初始化UI
        _panelDialogGo = transform.Find("Cat").Find("Canvas").Find("Panel_Dialog").gameObject;
        _textDialog = _panelDialogGo.transform.Find("Text_Dialog").GetComponent<Text>();
        _panelDialogGo.SetActive(false);

        _textState = GameObject.Find("Canvas/Panel_Home/Img_State/Text_State").GetComponent<Text>();

        _cockGo = transform.Find("Cat").Find("Cock").gameObject;
        _masterGo = transform.Find("Master").gameObject;

        animator = transform.Find("Cat").GetComponent<Animator>();
        clipDict = new Dictionary<Task, AnimationClip>();

        // 遍历动画片段，加入字典
        foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            Task result;
            if(Enum.TryParse<Task>(clip.name, out result))
            {
                clipDict.Add(result, clip);
            }
        }
    }

    private void Start()
    {
        // 查找 Positions 空物体
        GameObject positionsParent = GameObject.Find("Positions");
        // 遍历 Positions 下的所有子物体
        foreach (Transform child in positionsParent.transform)
        {
            // 尝试将子物体名称解析为 Task 枚举
            if (Enum.TryParse(child.name, out Task task))
            {
                _taskPositions[task] = child.position;
            }
        }
        // 初始化世界状态
        InitWorldState();
        if(masterBeside) {
            this._masterGo.SetActive(true);
        }
        
        // 注册世界状态
        HTNWorld.AddState("_energy", () => energy, value => energy = (int)value);
        HTNWorld.AddState("_mood", () => mood, value => mood = (int)value);
        HTNWorld.AddState("_full", () => full, value => full = (int)value);
        // masterBeside状态会影响游戏物体_masterGo的显隐
        HTNWorld.AddState("_masterBeside", () => masterBeside, value => {masterBeside = (bool)value; _masterGo.SetActive(masterBeside); });

        htnBuilder = new HTNPlanBuilder();

        // 构建猫猫的 HTN 网络结构
        htnBuilder.AddCompoundTask() // 生活（终极任务）
            .AddMethod(() => true) // 维持生命
                .AddCompoundTask() // 维持生命复合任务
                    .AddMethod(() => true) // 进食
                        .AddPrimitiveTask(new P_Eat(3.0f))  // 吃饭
                        .AddPrimitiveTask(new P_Drink(2.0f)) // 喝水
                        .Back()
                    .AddMethod(() => true) // 排泄
                        .AddPrimitiveTask(new P_Poop(5.0f)) // 拉屎
                        .Back()
                    .Back()
                .Back()
            .AddMethod(() => true) // 运动
                .AddCompoundTask() // 运动复合任务
                    .AddMethod(() => true) // 玩耍
                        .AddPrimitiveTask(new P_Parkour(6.0f)) // 跑酷
                        .AddPrimitiveTask(new P_ChaseCock(4.0f)) // 追蟑螂
                        .AddPrimitiveTask(new P_EatCock(1.5f)) // 吃蟑螂
                        .Back()
                    .AddMethod(() => true) // 拆家
                        .AddPrimitiveTask(new P_Destroy(3.0f)) // 拆家
                        .Back()
                    .Back()
                .Back()
            .AddMethod(() => true) // 叫唤
                .AddPrimitiveTask(new P_Meow(3.0f)) // 叫唤
                .Back()
            .AddMethod(() => HTNWorld.GetWorldState<bool>("_masterBeside")) // 撒娇
                .AddPrimitiveTask(new P_RubMaster(4.0f)) // 蹭主人
                .AddPrimitiveTask(new P_Meow(2.0f)) // 叫唤
                .Back()
            .AddMethod(() => true) // 休息
                .AddPrimitiveTask(new P_Sleep(7.0f)) // 睡觉
                .AddPrimitiveTask(new P_Idle(2.5f)) // 发呆
                .Back()
            .AddMethod(() => true) // 清洁
                .AddPrimitiveTask(new P_LickFur(3.5f)) // 舔毛
                .Back()
            .End();
    }

    void Update()
    {
        // 循环执行计划
        htnBuilder.RunPlan();
    }

    private void InitWorldState()
    {
        this.energy = GameManager.Instance.Energy;
        this.full = GameManager.Instance.Full;
        this.mood = GameManager.Instance.Mood;
        this.masterBeside = GameManager.Instance.MasterBeside;
    }

    public void ShowDialog(string text)
    {
        this._textDialog.text = text;
        this._panelDialogGo.SetActive(true);
    }

    public void HideDialog()
    {
        this._textDialog.text = "";
        this._panelDialogGo.SetActive(false);
    }

    public void MoveToNextPosition(Task task, Action finishAction)
    {
        if (_taskPositions.TryGetValue(task, out Vector3 position))
        {
            // 先停止动画
            animator.Play("Empty");
            // 计算当前位置和目标位置之间的距离
            float distance = Vector3.Distance(this.transform.position, position);
            // 根据距离和速度计算移动时间
            float moveTime = distance / moveSpeed;

            this.transform.DOMove(position, moveTime).OnComplete(() => {
                // 完成之后执行的函数
                finishAction?.Invoke();
                PlayAnim(task);
            });
        } else {
            finishAction?.Invoke();
            PlayAnim(task);
        }
    }

    public void PlayAnim(Task task)
    {
        if(clipDict.ContainsKey(task))
        {
            animator.Play(task.ToString());
        }
        else
        {
            animator.Play("Empty");
        }
    }

    public void SetStateText(string text)
    {
        _textState.text = text;
    }
}