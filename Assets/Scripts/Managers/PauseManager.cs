using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;//싱글톤

    [Header("연결된 UI (자동연결)")]
    public GameObject pausePanel;//일시정지 창 패널
    public Button ResumeButton;  //씬에 있을 일시정지 버튼

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //1. 하이어라키에서 이름으로 패널 찾기 (비활성화 상태여도 찾음)
        GameObject panelObj = GameObject.Find("PausePanel");
        if (panelObj != null)
        {
            pausePanel = panelObj;
            pausePanel.SetActive(false);//시작 시 무조건 끄기(버튼 눌러야만 창이 떠야하기에)
        }
    }

    void Update()//Update에서 Esc 키 감지
    {
        //최신 방식(Input System Package), Input.GetKeyDown를 사용하지 않은 대신 사용.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (pausePanel == null)
        {
            return;
        }

        // 현재 패널 상태의 반대값을 구함 (켜져 있으면 false, 꺼져 있으면 true)
        bool isPaused = !pausePanel.activeSelf;

        pausePanel.SetActive(isPaused);//버튼 클릭, esc 누르면 켜거나 끔 (토글)

        Time.timeScale = isPaused ? 0f : 1f;//일시정지 중이면 시간을 멈추고0, 아니면 정상 흐름1 으로 복구
    }
}
