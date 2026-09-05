using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;//씬 이동을 위해 선언

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;//싱글톤

    [Header("연결된 UI (자동연결)")]
    public GameObject pausePanel;//일시정지 창 패널

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }


    void Start()
    {
        //하이어라키에서 이름으로 패널 찾기 (비활성화 상태여도 찾음)
        Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (Canvas canvas in allCanvases)//캔버스 안에서 해당 UI를 반복해서 찾아보기
        {
            if (canvas.gameObject.scene.name != null)//씬에 포함된 녀석들만 골라내기 (프리팹 제외)
            {
                Transform found = canvas.transform.Find("PausePanel");//자식 중에 PausePanel 이름이 있는지 확인
                if (found != null)
                {
                    pausePanel = found.gameObject;
                    pausePanel.SetActive(false);//찾으면 일단 비활성화 상태로 유지
                    break;
                }
            }
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

        //게임 준비(카운트다운) 중이라면 일시정지 창을 띄우지 않고 UIManager를 통해 안내 메시지 출력!
        if (GameManager.Instance != null && GameManager.Instance.IsGameReady)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPauseWarning();//텍스트 출력은 UIManager가 담당
            }
            return;
        }

        bool isPaused = !pausePanel.activeSelf;//현재 패널 상태의 반대값을 구함 (켜져 있으면 false, 꺼져 있으면 true)
        pausePanel.SetActive(isPaused);//버튼 클릭, esc 누르면 켜거나 끔 (토글)
        Time.timeScale = isPaused ? 0f : 1f;//일시정지 중이면 시간을 멈추고0, 아니면 정상 흐름1 으로 복구
    }

    public void ToMainScene()//메인 씬으로 이동
    {
        Time.timeScale = 1f;//씬 넘어가기 전에 반드시 시간 흐름을 1로 복구해야 해
        SceneManager.LoadScene("MainScene");//메인 씬 이름
    }

    public void QuitGame()//게임종료
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//에디터에서 플레이 중일 때 멈춤
        #else
        Application.Quit();//빌드된 실제 게임에서 종료
        #endif
    }
}