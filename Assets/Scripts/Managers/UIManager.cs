using UnityEngine;
using TMPro;
using System.Collections;


public class UIManager : MonoBehaviour//InGameUI(WaveText 등)와 GameClearPanelUI 스크립트 분리 예정
{
    [Header("WaveText연결. 자동연결")]
    public TextMeshProUGUI WaveText;


    void Start()
    {
        FindUI();//UI 자동찾기

        GameManager.Instance.OnEnemyCountChanged += UpdateUI;//GameManager가 던지는 신호를 구독
        UpdateUI(0, GameManager.Instance.currentRoundData.enemyCount);//GameManager의 SO 데이터의 enemyCount를 가져와서 실행할 때 초기값 전달

        //라운드가 바뀌면 WaveText UI도 새 데이터에 맞게 초기화
        GameManager.Instance.OnRoundChanged += (newData) => {UpdateUI(0, newData.enemyCount);};
    }                                                     


    void OnDestroy()
    {
        if (GameManager.Instance != null)//구독 해제 (이거 안 하면 나중에 버그 발생)
        {
            GameManager.Instance.OnEnemyCountChanged -= UpdateUI;
        }
    }

 
    void UpdateUI(int current, int total)
    {
        if (current >= total)
        {
            WaveText.text = "보스 등장!";
        }
        else
        {
            WaveText.text = $"처치: {current} / {total}";
        }
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
    }
}
