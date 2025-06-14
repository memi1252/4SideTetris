using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button StartButton;
    public Button exitButton;
    public Text maxpoint;
    public Text playTime;
    public Text blockcount;
    public SettingUI settingUI;
    public Button settingsButton;
    public Button RankingButton;
    public GameObject RankingPanel;
    public GameObject HelpPanel;
    public Button HelpCloseButton;
    public Button HelpButton;

    private void Awake()
    {
        StartButton.onClick.AddListener(() =>
        {
            
            PlayerPrefs.SetFloat("Master", settingUI.masterSlider.value);
            PlayerPrefs.SetFloat("bgm", settingUI.bgmSlider.value);
            PlayerPrefs.SetFloat("effect", settingUI.effectsSlider.value);
            SceneManager.LoadScene("Tetris");
        });
        exitButton.onClick.AddListener(() =>
        {
            
            PlayerPrefs.SetFloat("Master", settingUI.masterSlider.value);
            PlayerPrefs.SetFloat("bgm", settingUI.bgmSlider.value);
            PlayerPrefs.SetFloat("effect", settingUI.effectsSlider.value);
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        });
        settingsButton.onClick.AddListener(() =>
        {
            settingUI.Show();
        });
        RankingButton.onClick.AddListener(() =>
        {
            if (RankingPanel != null)
            {
                RankingPanel.GetComponent<RankingUI>().Show();
            }
            else
            {
                Debug.LogError("RankingPanel is not assigned in the inspector.");
            }
        }); 
        HelpCloseButton.onClick.AddListener(() =>
        {
            if (HelpPanel != null)
            {
                HelpPanel.SetActive(false);
            }
        });
        HelpButton.onClick.AddListener(() =>
        {
            if (HelpPanel != null)
            {
                HelpPanel.SetActive(true);
            }
        });
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Master"))
        {
            PlayerPrefs.SetFloat("Master", settingUI.masterSlider.value);
            PlayerPrefs.SetFloat("bgm", settingUI.bgmSlider.value);
            PlayerPrefs.SetFloat("effect", settingUI.effectsSlider.value);
        }
        else
        {
           settingUI.masterSlider.value = PlayerPrefs.GetFloat("Master");
           settingUI.bgmSlider.value = PlayerPrefs.GetFloat("bgm");
           settingUI.effectsSlider.value = PlayerPrefs.GetFloat("effect");
        }
        
        if (PlayerPrefs.HasKey("maxPoint"))
        {
            maxpoint.text = $"최대 포인트 : {PlayerPrefs.GetInt("maxPoint")}";
            float time = PlayerPrefs.GetFloat("playTime");
            playTime.text =
                $"플레이 타임 : {(int)time / 60:00} : {(int)time % 60:00}";
            blockcount.text = $"사용한 블럭 갯수 : {PlayerPrefs.GetInt("blockCount")}개";
        }
        else
        {
            maxpoint.text = $"최대 기록 없음";
            playTime.text = $"게임을 시작해";
            blockcount.text = $"기록을 세우세요";
        }
        HelpPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingUI.gameObject.activeSelf)
            {
                settingUI.Hide();
            }
            else
            {
                settingUI.Show();
            }
        }
    }

    public void RankingUIOpen()
    {
        GameObject.Find("RankingUI").GetComponent<RankingUI>().Show();
    }
}
