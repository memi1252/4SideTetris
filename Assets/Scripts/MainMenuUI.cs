using System.Collections.Generic;
using System.Linq;
using BackEnd;
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
    public Button logoutButton;

    private void Awake()
    {
        HelpPanel.SetActive(false);
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
        logoutButton.onClick.AddListener(() =>
        {
            BackendLogin.Instance.CustomLogOut();
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
        
        var bro = Backend.GameData.Get("USER_DATA", new Where());
        if (bro.IsSuccess())
        {
            LitJson.JsonData gameDataJson = bro.FlattenRows();
            if (gameDataJson.Count > 0)
            {
                
                List<UserData>  userDataList = new List<UserData>();
                
                
                UserData userData = new UserData();
                for (int i = 0; i < gameDataJson.Count; i++)
                {
                    if (gameDataJson[i]["name"].ToString() == BackendLogin.Instance.GetNickName())
                    {
                        userData.point = int.Parse(gameDataJson[i]["point"].ToString());
                        userData.playTime = float.Parse(gameDataJson[i]["playTime"].ToString());
                        userData.blockCount = int.Parse(gameDataJson[i]["blockCount"].ToString());
                        userDataList.Add(userData);
                        userData = new UserData();
                    }
                    
                }
                var sortedList = userDataList.OrderByDescending(user => user.point).ToList();
                
                if(sortedList.Count == 0)
                {
                    Debug.LogWarning("랭킹 데이터가 존재하지 않습니다.");
                    maxpoint.text = $"최대 기록 없음";
                    playTime.text = $"게임을 시작해";
                    blockcount.text = $"기록을 세우세요";
                    return;
                }
                else
                {
                    maxpoint.text = $"최대 포인트 : {sortedList[0].point}점";;
                    float time = sortedList[0].playTime;
                    playTime.text =
                        $"플레이 타임 : {(int)time / 60:00} : {(int)time % 60:00}";
                    blockcount.text = $"사용한 블럭 갯수 : {sortedList[0].blockCount}개";
                }
            }
            else
            {
                Debug.LogWarning("랭킹 데이터가 존재하지 않습니다.");
                maxpoint.text = $"최대 기록 없음";
                playTime.text = $"게임을 시작해";
                blockcount.text = $"기록을 세우세요";
            }
        }
        else
        {
            maxpoint.text = $"최대 기록 없음";
            playTime.text = $"게임을 시작해";
            blockcount.text = $"기록을 세우세요";
            Debug.LogError("랭킹 데이터 조회 실패: " + bro);
        }
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
