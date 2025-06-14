using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum SpawnLocation
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public SpawnLocation spawnLocation = SpawnLocation.Top;

    public static GameManager instance;
    public int nextNumber = -1;
    public int currentPoiont = 0;
    public int Point;
    public float playTimer;
    public int blockCount;

    [Header("UI")]
    public Text pointText;
    public SettingUI settingUI;
    public GameOverUI gameOverUI;
    public Button SettingsButton;
    public Button rotateButton;
    public Button lockButton;
    public GameObject mobileUi;

    [Header("Sound")]
    public AudioSource downSound;
    public AudioSource LineClearSound;

    private void Awake()
    {
        instance = this;

        if (SettingsButton != null && settingUI != null)
        {
            SettingsButton.onClick.AddListener(() =>
            {
                settingUI.Show();
            });
        }
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android && mobileUi != null)
        {
            mobileUi.SetActive(true);
        }

        if (settingUI != null && settingUI.masterSlider != null && settingUI.bgmSlider != null && settingUI.effectsSlider != null)
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
        }
    }

    private void Update()
    {
        playTimer += Time.deltaTime;

        if (pointText != null)
        {
            pointText.text = $"포인트 : {currentPoiont}점";
        }

        if (Input.GetKeyDown(KeyCode.Escape) && settingUI != null)
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
}
