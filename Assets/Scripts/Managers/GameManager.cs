using UnityEngine;
using TMPro;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//싱글톤 설정

    public int totalEnemyCount = 10;//라운드별 총 몬스터 수
    public int defeatedEnemyCount = 0;//처치한 몬스터 수

    [Header("WaveText연결. 자동연결")]
    public TextMeshProUGUI WaveText;
    [Header("GameClearPanel연결. 자동연결")]
    public GameObject GameClearPanel;

    void Awake()
    {
        if (Instance == null)//싱글톤이 없다면 이걸 연결
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);//이미 있다면 기존의 것은 삭제
        }
    }

    void Start()
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

        UpdateUI();//게임 시작하자마자 WaveText UI 초기화
    }

    public void OnEnemyDefeated()
    {
        defeatedEnemyCount++;
        UpdateUI();

        if (defeatedEnemyCount >= totalEnemyCount)//클리어 조건 체크: 처치 수가 총합과 같거나 많아지면 실행
        {
            StartCoroutine(ShowGameClearRoutine());
        }
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
    void UpdateUI()
    {
        if (WaveText != null)
        {
            WaveText.text = $"처치: {defeatedEnemyCount} / {totalEnemyCount}";
        }
    }
}
