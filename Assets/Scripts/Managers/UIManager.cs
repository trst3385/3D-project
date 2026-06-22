using UnityEngine;
using TMPro;
using System.Collections;


public class UIManager : MonoBehaviour
{
    [Header("WaveText연결. 자동연결")]
    public TextMeshProUGUI WaveText;
    [Header("GameClearPanel연결. 자동연결")]
    public GameObject GameClearPanel;


    void Start()
    {
        FindUI();//UI 자동찾기

        //GameManager가 던지는 신호를 구독(Listen)하기
        GameManager.Instance.OnEnemyCountChanged += UpdateUI;
        GameManager.Instance.OnGameClear += ShowGameClear;


        UpdateUI(0, GameManager.Instance.totalEnemyCount);//실행할 때 초기값 전달
    }                                                     //GameManager.Instance에 직접 접근해서 데이터를 넣어줘

    void OnDestroy()
    {
        //구독 해제 (이거 안 하면 나중에 버그 발생)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyCountChanged -= UpdateUI;
            GameManager.Instance.OnGameClear -= ShowGameClear;
        }
    }

    void ShowGameClear()
    {
        StartCoroutine(ShowGameClearRoutine());
    }
    IEnumerator ShowGameClearRoutine()//1초 대기 후 클리어 창을 띄우는 코루틴
    {
        yield return new WaitForSeconds(1.0f);//1초 대기

        if (GameClearPanel != null)
        {
            GameClearPanel.SetActive(true);//클리어 창 활성화
            Time.timeScale = 0f;//게임 시간을 멈춰서 정지 화면처럼 만들 수도 있어
            Debug.Log("게임 클리어!");
        }
    }
    void UpdateUI(int current, int total)
    {
        WaveText.text = $"처치: {current} / {total}";
    }
    

    void FindUI()//UI 자동으로 찾기
    {
        GameObject uiObj = GameObject.Find("WaveText");//몬스터 처치 수, 자동 연결
        if (uiObj != null)
        {
            WaveText = uiObj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("WaveText를 찾을 수 없어! 씬에 오브젝트가 있는지 확인해!");
        }

        //GameClearPanel은 하이어라키에 비활성화 된 상태라 다른 방식으로 찾아 자동연결(transform.Find)
        GameObject canvasObj = GameObject.Find("UI Canvas");
        if (canvasObj != null)//UI Canvas 안에서 이름으로 자식 오브젝트 찾기
        {
            Transform panelTransform = canvasObj.transform.Find("GameClearPanel");//transform.Find는 비활성화된 자식도 찾아냄
            if (panelTransform != null)
            {
                GameClearPanel = panelTransform.gameObject;
                GameClearPanel.SetActive(false);//창이 등장할 상황이 아닐땐 끈 상태 유지
            }
        }
        else
        {
            Debug.LogWarning("Canvas를 찾을 수 없어!");
        }
    }
}
