using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//싱글톤
    public RoundData currentRoundData;//이 스크립트에서만 SO연결, 다른 매니저 스크립트에서 GameManager에 연결된 SO 값을 받아옴
    public List<RoundData> roundDatas;//여러 라운드 데이터를 미리 SO로 만들어두고 리스트에 담기
    public int currentRoundIndex = 0;


    private int defeatedEnemyCount = 0;//처치한 몬스터 수

    //------------이벤트------------------------------
    public event Action<int, int> OnEnemyCountChanged;//UI한테 "몬스터 처치해서 카운트 올릴게!"라고 알려주는 이벤트
    public event Action OnGameClear;                  //UI한테 "게임 끝났어!"라고 알려주는 이벤트
    public event Action OnBossSpawn;                  //보스 몬스터 등장 이벤트

    public event Action<RoundData> OnRoundChanged;//다음 라운드 이동 이벤트
    public event Action OnGameOver;//게임 오버 이벤트
    //------------------------------------------------

    void Awake()
    {
        Instance = this;

        if (roundDatas != null && roundDatas.Count > 0)//방어적 설계: 리스트에 SO를 아무것도 안 넣었을 때 에러나는 걸 방지
        {
            currentRoundData = roundDatas[currentRoundIndex];//게임 시작 시 0번째(1라운드) 데이터를 현재 라운드 데이터로 설정
        }
        else
        {
            Debug.LogError("GameManager: roundDatas 리스트가 비어있어! SO를 인스펙터에서 넣어줘!");
        }
    } 

  
    public void RegisterEnemy(EnemyHealth enemy)//몬스터가 자기 자신(EnemyHealth)을 넘겨주면, 
    {                                           //그 몬스터의 사망 이벤트를 우리(GameManager)가 구독함
        enemy.OnEnemyDie += (isBoss) => {
            if (isBoss)
            {
                OnGameClear?.Invoke();//보스면 바로 클리어!
            }
            else
            {
                EnemyDefeated();//일반 몬스터면 카운트 증가
            }
        };  
    }

    public void EnemyDefeated()
    {
        defeatedEnemyCount++;
        OnEnemyCountChanged?.Invoke(defeatedEnemyCount, currentRoundData.enemyCount);//이벤트를 통해 알림 발송 (구독자가 없어도 에러 안 나게 ? 사용)

        if (defeatedEnemyCount >= currentRoundData.enemyCount)//totalEnemyCount.처치 수 달성 시 보스 소환 신호 발송!
        {
            OnBossSpawn?.Invoke();//여기서 보스 소환 신호를 보내거나, 다음 라운드 로직을 실행
        }
    }

    public void TriggerGameOver()//플레이어 사망 시 게임 오버 창 생성 이벤트
    {
        Debug.Log("게임 오버 이벤트 발생!");
        OnGameOver?.Invoke();
    }

    public void LoadNextRound()
    {
        currentRoundIndex++;
        if (currentRoundIndex < roundDatas.Count)
        {
            currentRoundData = roundDatas[currentRoundIndex];
            defeatedEnemyCount = 0;//카운트 초기화

            OnRoundChanged?.Invoke(currentRoundData);//라운드 시작 알림 (UI나 스포너가 구독하도록)
        }
        else
        {
            Debug.Log("모든 라운드 클리어! 진짜 게임 끝!");
            //여기서 메인 메뉴로 가거나 축하 화면 표시
        }
    }
}