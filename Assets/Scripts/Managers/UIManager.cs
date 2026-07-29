using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UIManager : MonoBehaviour
{

    [Header("WaveText연결. 자동연결")]
    public TextMeshProUGUI WaveText;//현재/최대 처치 수 텍스트

    [Header("GoldText 연결. 자동연결")]
    public TextMeshProUGUI GoldText;//보유 골드 텍스트

    [Header("GoldWarningText 연결. 자동연결")] 
    public TextMeshProUGUI GoldWarningText;//골드 부족 시 뜰 텍스트

    private Coroutine warningCoroutine;//중복 실행 방지용 코루틴 변수

    void Start()
    {
        FindUI();//UI 자동찾기

        GameManager.Instance.OnEnemyCountChanged += UpdateUI;//GameManager가 던지는 신호를 구독
        UpdateUI(0, GameManager.Instance.currentRoundData.enemyCount);//GameManager의 SO 데이터의 enemyCount를 가져와서 실행할 때 초기값 전달

        //라운드가 바뀌면 WaveText UI도 새 데이터에 맞게 초기화
        GameManager.Instance.OnRoundChanged += (newData) => {UpdateUI(0, newData.enemyCount);};

        
        //PlacementManager의 '골드 부족 신호'를 구독!
        if (PlacementManager.Instance != null)
        {
            PlacementManager.Instance.OnGoldShortage += HandleGoldShortage;
        }
        if (GoldWarningText != null)//게임 시작 시 골드 부족 경고 텍스트는 비활성화(작동 시에만 활성화 되게)
        {
            GoldWarningText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (GoldText != null && GameManager.Instance != null)//골드 텍스트는 매 프레임 업데이트 (싱글톤 접근)
        {
            GoldText.text = $"Gold: {GameManager.Instance.CurrentGold}";
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)//구독 해제 (이거 안 하면 나중에 버그 발생)
        {
            GameManager.Instance.OnEnemyCountChanged -= UpdateUI;
        }
    }

    private void HandleGoldShortage()//신호가 도착했을 때 실행될 메서드
    {
        if (GoldWarningText != null)
        {
            GoldWarningText.text = "골드가 부족합니다!";

            //만약 이미 경고 텍스트가 떠 있는 상태에서 또 골드를 클릭했다면?
            //기존에 돌고 있던 n초짜리 타이머를 멈추고 새로 n초를 세게 만들어야 글자가 끊기지 않고 자연스러움!
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }

            warningCoroutine = StartCoroutine(ShowWarningRoutine());//텍스트를 켜고, n초 뒤에 끄는 코루틴 시작
        }
    }
    private IEnumerator ShowWarningRoutine()//n초 동안 텍스트를 띄우고 다시 숨기는 코루틴
    {
        GoldWarningText.gameObject.SetActive(true);//텍스트 켜기

        yield return new WaitForSeconds(1.5f);//(n)f초 동안 출력

        GoldWarningText.gameObject.SetActive(false);//텍스트 끄기
        warningCoroutine = null;//코루틴 변수 초기화
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
        //WaveText 자동 찾기
        GameObject waveObj = GameObject.Find("WaveText");
        if (waveObj != null) WaveText = waveObj.GetComponent<TextMeshProUGUI>();
        else Debug.LogWarning("WaveText를 찾을 수 없어!");

        //GoldText 자동 찾기
        GameObject goldObj = GameObject.Find("GoldText");
        if (goldObj != null) GoldText = goldObj.GetComponent<TextMeshProUGUI>();
        else Debug.LogWarning("GoldText를 찾을 수 없어! 씬에 오브젝트가 있는지 확인해!");
        
        //GoldWarningText 자동 찾기(UI가 비활성화면 찾지 못해!)
        GameObject warningObj = GameObject.Find("GoldWarningText");
        if (warningObj != null) GoldWarningText = warningObj.GetComponent<TextMeshProUGUI>();
        else Debug.LogWarning("GoldWarningText를 찾을 수 없어! 씬에 오브젝트가 있는지 확인해!");
    }
}