using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Text currentPointText;
    public Text maxPointText;
    public Text playTimeText;
    public Text blockCountText;
    public Button restartButton;
    public Button lobbyButton;

    private void Awake()
    {
        if (GameManager.instance != null && GameManager.instance.settingUI != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                PlayerPrefs.SetFloat("Master", GameManager.instance.settingUI.masterSlider.value);
                PlayerPrefs.SetFloat("bgm", GameManager.instance.settingUI.bgmSlider.value);
                PlayerPrefs.SetFloat("effect", GameManager.instance.settingUI.effectsSlider.value);
                SceneManager.LoadScene("Tetris");
            });
            lobbyButton.onClick.AddListener(() =>
            {
                PlayerPrefs.SetFloat("Master", GameManager.instance.settingUI.masterSlider.value);
                PlayerPrefs.SetFloat("bgm", GameManager.instance.settingUI.bgmSlider.value);
                PlayerPrefs.SetFloat("effect", GameManager.instance.settingUI.effectsSlider.value);
                SceneManager.LoadScene("LobbyScene");
            });
        }
        else
        {
            Debug.LogError("GameManager 또는 settingUI가 null입니다.");
        }
    }
    
    private void Start()
    {
        Hide();
        restartButton.interactable = false;
        lobbyButton.interactable = false;
    }

    private void Update()
    {
        if (GameManager.instance != null && currentPointText != null)
        {
            currentPointText.text = $"현재 포인트 : {GameManager.instance.currentPoiont}점";
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        currentPointText.text = $"현재 포인트 : {GameManager.instance.currentPoiont}점";
        blockCountText.text = $"사용한 블럭 갯수 : {GameManager.instance.blockCount}개";
        if (!PlayerPrefs.HasKey("maxPoint"))
        {
            PlayerPrefs.SetInt("maxPoint", GameManager.instance.currentPoiont);
            PlayerPrefs.SetFloat("playTime", GameManager.instance.playTimer);
            PlayerPrefs.SetInt("blockCount", GameManager.instance.blockCount);
        }
        if (GameManager.instance.currentPoiont > PlayerPrefs.GetInt("maxPoint"))
        {
            PlayerPrefs.SetInt("maxPoint", GameManager.instance.currentPoiont);
            PlayerPrefs.SetFloat("playTime", GameManager.instance.playTimer);
            PlayerPrefs.SetInt("blockCount", GameManager.instance.blockCount);
        }
        maxPointText.text = $"최고 포인트 : {PlayerPrefs.GetInt("maxPoint")}점";
        playTimeText.text = $"플레이 타임 : {(int)GameManager.instance.playTimer / 60:00} : {(int)GameManager.instance.playTimer % 60:00}";
        StartCoroutine(dd());
    }

    IEnumerator dd()
    {
        yield return new WaitForSecondsRealtime(2f);
        BackendGameData.Instance.GameDataInsert(BackendLogin.Instance.GetNickName(), GameManager.instance.currentPoiont, GameManager.instance.playTimer, GameManager.instance.blockCount);
        restartButton.interactable = true;
        lobbyButton.interactable = true;
    }

    private void Hide()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
