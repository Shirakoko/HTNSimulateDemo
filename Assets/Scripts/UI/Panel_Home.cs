using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Panel_Home : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("Btn_Back").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Start");
        });
    }
}
