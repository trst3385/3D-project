using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private GameObject gameOverPanel;//UI 자동연결

    void Awake()
    {
        //1. 씬 전체에서 Canvas라는 타입의 오브젝트들을 싹 다 찾아서...
        Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
        Canvas mainCanvas = null;
        foreach (var canvas in canvases)//1-1. 찾은 Canvas에서 이름이 "UI Canvas"인 녀석을 딱 골라내
        {
            if (canvas.gameObject.scene.name != null && canvas.name == "UI Canvas")
            {
                mainCanvas = canvas;
                break;
            }
        }

        if (mainCanvas != null)//2. 찾은 UI Canvas란 이름의 오브젝트의 자식에서 GameOverPanel을 찾아
        {
            Transform found = mainCanvas.transform.Find("GameOverPanel");
            if (found != null)
            {
                gameOverPanel = found.gameObject;
                gameOverPanel.SetActive(false);//시작 시 비활성화
            }
            else
            {
                Debug.LogError("UI Canvas 자식 중에 'GameOverPanel'을 찾을 수 없어!");
            }
        }
        else
        {
            Debug.LogError("씬에 'UI Canvas'라는 이름의 캔버스를 찾을 수 없어!");
        }

        if (GameManager.Instance != null)//3. 이벤트 구독
        {
            GameManager.Instance.OnGameOver += ShowGameOver;
        }
    }

    void Start()
    {
        GameManager.Instance.OnGameOver += ShowGameOver;//이벤트 구독: 게임 오버 이벤트가 발생하면 ShowGameOver 메서드 실행
    }

    void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;//게임 일시정지
    }

    public void ReStartButton()//ReStartButton 버튼. 재시작
    {
        Time.timeScale = 1f;//1. 게임오버 창 등장으로 멈춘 시간 다시 정상으로 돌리기 (이거 안 하면 멈춘 채로 시작됨)

        //2. 현재 씬 이름으로 다시 불러오기
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        //이렇게 씬을 다시 불러오면 GameManager나 다른 데이터들도 싹 초기화되어서 처음부터 다시 시작하게 될 거야.
        //만약 나중에 "데이터(점수, 아이템 등)는 유지하고 몬스터만 초기화하고 싶어!"라는 고민이 들 때가 올 수도 있을텐데,
        //그때는 SceneManager.LoadScene 대신 '스테이트 패턴(State Pattern)'이나 '데이터 매니저 초기화 함수'를 공부하면 돼.
    }

    void OnDestroy()//구독 해제 (중요!) - 오브젝트 파괴 시 메모리 누수 방지
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowGameOver;
        }
    }
}
