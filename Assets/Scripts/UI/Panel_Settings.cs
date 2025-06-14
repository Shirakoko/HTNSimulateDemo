using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Settings : MonoBehaviour
{
    private Button _btnClose;
    private Slider _sliderEnergy;
    private Slider _sliderFull;
    private Slider _sliderMood;
    private Toggle _toggleMasterBeside;

    private Text _textEnergy;
    private Text _textFull;
    private Text _textMood;
    
    private void Awake()
    {
        _btnClose = transform.Find("Btn_Close").GetComponent<Button>();
        _sliderEnergy = transform.Find("Slider_Energy").GetComponent<Slider>();
        _sliderFull = transform.Find("Slider_Full").GetComponent<Slider>();
        _sliderMood = transform.Find("Slider_Mood").GetComponent<Slider>();
        _toggleMasterBeside = transform.Find("Toggle_MasterBeside").GetComponent<Toggle>();

        _textEnergy = transform.Find("Text_Energy").GetComponent<Text>();
        _textFull = transform.Find("Text_Full").GetComponent<Text>();
        _textMood = transform.Find("Text_Mood").GetComponent<Text>();

        _btnClose.onClick.AddListener(() => {
            this.gameObject.SetActive(false);
        });

        // 初始化UI上的值
        _sliderEnergy.value = GameManager.Instance.Energy;
        _sliderFull.value = GameManager.Instance.Full;
        _sliderMood.value = GameManager.Instance.Mood;
        _toggleMasterBeside.isOn = GameManager.Instance.MasterBeside;

        _textEnergy.text = _sliderEnergy.value.ToString();
        _textFull.text = _sliderFull.value.ToString();
        _textMood.text = _sliderMood.value.ToString();

        _sliderEnergy.onValueChanged.AddListener((value) => {
            _textEnergy.text = _sliderEnergy.value.ToString();
            GameManager.Instance.Energy = (int)value;
        });

        _sliderFull.onValueChanged.AddListener((value) => {
            _textFull.text = _sliderFull.value.ToString();
            GameManager.Instance.Full = (int)value;
        });

        _sliderMood.onValueChanged.AddListener((value) => {
            _textMood.text = _sliderMood.value.ToString();
            GameManager.Instance.Mood = (int)value;
        });

        _toggleMasterBeside.onValueChanged.AddListener((value) => {
            GameManager.Instance.MasterBeside = value;
        });
    }
}
