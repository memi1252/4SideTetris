using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class SettingUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider effectsSlider;
    public Button restartButton;
    public Button continueButton;
    public Button Lobbybutton;
    public Button CloseButton;
    
    public AudioMixer audioMixer;

    public enum EAudioMixerType{Master,bgm,effect}

    private void Awake()
    {
        restartButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetFloat("Master", masterSlider.value);
            PlayerPrefs.SetFloat("bgm", bgmSlider.value);
            PlayerPrefs.SetFloat("effect", effectsSlider.value);
            SceneManager.LoadScene("Tetris");
        });
        continueButton.onClick.AddListener(() =>
        {
            Hide();
        });
        Lobbybutton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetFloat("Master", masterSlider.value);
            PlayerPrefs.SetFloat("bgm", bgmSlider.value);
            PlayerPrefs.SetFloat("effect", effectsSlider.value);
            SceneManager.LoadScene("LobbyScene");
        });
        CloseButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        SetAudioVolume(EAudioMixerType.Master, masterSlider.value);
        SetAudioVolume(EAudioMixerType.bgm, bgmSlider.value);
        SetAudioVolume(EAudioMixerType.effect, effectsSlider.value);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void Hide()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    
    private void SetAudioVolume(EAudioMixerType audioMixerType,float volume)
    {
        // 오디오 믹서의 값은 -80 ~ 0까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 한다.
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }
}
