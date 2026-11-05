using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;//MainMixer 연결(수동연결)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioMixer == null)//오디오믹서 인스펙터 연결 누락 방어 코드
        {
            Debug.LogError("[SoundManager] AudioMixer가 인스펙터에 연결되지 않았어!", this);
        }
            
    }

    public void SetBGMVolume(float sliderValue)//슬라이더 값을 받아 믹서의 BGMVolume을 조절하는 함수
    {
        //0.0001 ~ 1 값을 데시벨(-80dB ~ 0dB)로 변환
        float db = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("BGMVolume", db);
    }
}
