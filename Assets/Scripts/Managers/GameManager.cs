using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour//DontDestroyOnLoad를 쓴 오브젝트는 부모 오브젝트가 파괴될 때 씬에서 강제로 튕겨 나와(Root로 이동)
{
    public static GameManager Instance;//싱글톤


    //외부에서 현재 라운드 번호를 '읽을 수는 있지만', 함부로 값을 바꿀 수 없도록(private set) 보호하고 인스펙터 창에서 숨김
    public int CurrentRoundIndex { get; private set; }
    //외부에서 현재 라운드 데이터(SO)를 '읽을 수는 있지만', 함부로 교체할 수 없도록 보호하고 인스펙터 창에서 숨김
    public RoundData currentRoundData { get; private set; }//이 스크립트에서만 SO연결, 다른 매니저 스크립트에서 GameManager에 연결된 SO 값을 받아옴


    public List<Enemy> activeEnemies = new List<Enemy>();//현재 살아있는 적들


    [Header("라운드 설정")]
    public List<RoundData> roundDatas;//여러 라운드 데이터를 미리 SO로 만들어두고 리스트에 담기


    //----게임 골드----
    [Header("게임 설정 데이터")]
    public GameGold gamegold;//인스펙터에서 GameGold에셋(SO)을 꽂아줄 변수
    [Header("골드 시스템")]
    public int CurrentGold { get; private set; } // 외부에서 읽을 수만 있게 보호
    //-----------------


    [SerializeField] private int startRoundIndex = 0;//숫자를 바꿔 실행 시 웨이브 순서 설정(0부터 1웨이브 시작)
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
        //1. 이미 인스턴스가 있는데 나(this) 자신이 아니라면? (중복 삭제)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //2. 이게 첫 번째 인스턴스라면 씬이 바뀌어도 파괴되지 않게!
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentRoundIndex = startRoundIndex;//게임 시작 시, 지정한 시작 라운드 인덱스로 초기화

        //기존 초기화 로직...
        if (roundDatas != null && roundDatas.Count > 0)
        {
            currentRoundData = roundDatas[CurrentRoundIndex];
        }

        if (gamegold != null)//게임 시작 시 SO로부터 초기 골드 설정
        {
            CurrentGold = gamegold.startGold;
        }
        else
        {
            CurrentGold = 50;//혹시 몰라 예외 처리
            Debug.LogWarning("GameGold SO가 GameManager에 연결되지 않았어!");
        }
    }

    public void AddEnemy(Enemy enemy)//적 리스트에 등록
    {
        if (!activeEnemies.Contains(enemy))//중복 방지: 리스트에 등록된 적이 실수로 또 추가되는 걸 막음
        {
            activeEnemies.Add(enemy);//리스트에 추가: 새로운 적이 생성(스폰)될 때 이 함수를 호출해서 리스트에 집어넣어
        }
    }

    public void RemoveEnemy(Enemy enemy)//적 리스트에 제거
    {
        if (activeEnemies.Contains(enemy))//존재 확인: 리스트에 실제로 해당 적이 들어있는지 안전하게 확인한 뒤에 지움
        {
            activeEnemies.Remove(enemy);//리스트에 제거: 몬스터가 죽거나 파괴될 때 리스트에서 빼내서 명단과 메모리에서 비워줘
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
        CurrentRoundIndex++;
        if (CurrentRoundIndex < roundDatas.Count)
        {
            currentRoundData = roundDatas[CurrentRoundIndex];
            defeatedEnemyCount = 0;//카운트 초기화

            OnRoundChanged?.Invoke(currentRoundData);//라운드 시작 알림 (UI나 스포너가 구독하도록)
        }
        else
        {
            Debug.Log("모든 라운드 클리어! 진짜 게임 끝!");
            //여기서 메인 메뉴로 가거나 축하 화면 표시
        }
    }

    public void AddGold(int amount)//몬스터 처치로 골드를 획득(추가)하는 함수
    {
        CurrentGold += amount;
        Debug.Log($"골드 획득 성공! 현재 골드: {CurrentGold}");
        //나중에 필요하면 여기에 골드 획득 이벤트도 추가할 수 있어!
    }

    public bool UseGold(int amount)//골드를 사용(차감)하는 함수
    {
        if (CurrentGold >= amount)
        {
            CurrentGold -= amount;
            Debug.Log($"골드 차감 성공! 남은 골드: {CurrentGold}");
            //나중에 골드 UI가 바뀌면 여기서 이벤트를 쏴줄 수도 있어!
            return true;
        }

        Debug.Log("골드가 부족해서 사용할 수 없어!");
        return false;
    }
}