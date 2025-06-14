using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelStart : MonoBehaviour
{
    private Button _btnStart;
    private Button _btnSettings;
    private GameObject _panelSettings;

    private void Awake()
    {
        _btnStart = transform.Find("Btn_Start").GetComponent<Button>();
        _btnSettings = transform.Find("Btn_Settings").GetComponent<Button>();
        _panelSettings = transform.parent.Find("Panel_Settings").gameObject;

        _panelSettings.SetActive(false);

        _btnStart.onClick.AddListener(() => {
            SceneManager.LoadScene("Game");
        });

        _btnSettings.onClick.AddListener(() => {
            _panelSettings.SetActive(true);
        });
    }
}
