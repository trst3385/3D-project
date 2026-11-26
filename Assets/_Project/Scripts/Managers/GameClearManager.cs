using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    [Header("GameClearPanel 자동연결")]
    [SerializeField] private GameObject gameClearPanel;

    [Header("다음 씬 설정")]
    [SerializeField] private string nextSceneName;

    void Awake()
    {
        FindGameClearPanel();//1. UI 자동 연결 (캐싱으로 Find()를 한번만 찾아 클래스 메모리에 저장, 잦은 Find() 사용은 CPU 부하 원인!)
    }

    void Start()
    {
        //2. 이벤트 구독
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameClear += Show;
        }
    }

    void OnDestroy()
    {
        //3. 구독 해제
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameClear -= Show;
        }
    }

    void FindGameClearPanel()
    {
        //UI Canvas 오브젝트를 찾아 그 자식 중 GameClearPanel을 찾음
        GameObject canvasObj = GameObject.Find("UI Canvas");
        if (canvasObj != null)
        {
            Transform panelTransform = canvasObj.transform.Find("GameClearPanel");
            if (panelTransform != null)
            {
                gameClearPanel = panelTransform.gameObject;
                gameClearPanel.SetActive(false);//초기 상태는 비활성화
            }
            else
            {
                Debug.LogError("UI Canvas 자식에서 'GameClearPanel'을 찾을 수 없어!");
            }
        }
        else
        {
            Debug.LogError("씬에 'UI Canvas' 오브젝트가 없어!");
        }
    }

    void Show()
    {
        StartCoroutine(ShowRoutine());
    }
    IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("게임 클리어!");
        }
    }

    public void OnClickNextRound()
    {
        Time.timeScale = 1f;//시간 다시 흐르게 하기

        if (gameClearPanel != null)//게임클리어 패널 끄기
        {
            gameClearPanel.SetActive(false);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("nextSceneName이 비어 있어!");
        }
    }
}
