using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour//DontDestroyOnLoad를 쓴 오브젝트는 부모 오브젝트가 파괴될 때 씬에서 강제로 튕겨 나와(Root로 이동)
{
    public static GameManager Instance;//싱글톤

    //외부에서 읽을 수만 있는 카운트다운 진행 여부 플래그(카운트다운 중 일시정지창 못켜게 막기)
    public bool IsGameReady { get; private set; } = false;

    //외부에서 현재 라운드 데이터(SO)를 읽을 수만 있게 보호
    public RoundData currentRoundData { get; private set; }

    public List<Enemy> activeEnemies = new List<Enemy>();//현재 살아있는 적들

    [Header("라운드 설정")]
    [SerializeField] private RoundData roundData;//이 씬에서 쓸 라운드 SO 하나만 연결

    //----게임 골드----
    [Header("게임 골드 데이터")]
    public GameGold gamegold;//인스펙터에서 GameGold 에셋(SO) 연결
    [Header("골드 시스템")]
    public int CurrentGold { get; private set; }//외부에서 읽을 수만 있게 보호
    //-----------------

    [Header("게임 시작 카운트 연출 UI(UI수동연결)")]
    [SerializeField] private TextMeshProUGUI readyText;   //UI 텍스트(ReadyText) 연결
    [SerializeField] private float gameReadyDelay = 3.0f; //시작 전 대기시간. 3초 카운트다운

    private int defeatedEnemyCount = 0;//처치한 몬스터 수

    //------------이벤트------------------------------
    public event Action<int, int> OnEnemyCountChanged;
    public event Action OnGameClear;
    public event Action OnBossSpawn;
    public event Action<int> OnGoldChanged;
    public event Action<RoundData> OnRoundChanged;
    public event Action OnGameOver;
    //------------------------------------------------


    void Awake()
    {
        //같은 씬에 중복으로 배치된 경우만 정리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentRoundData = roundData;
        if (currentRoundData == null)
        {
            Debug.LogWarning("RoundData SO가 GameManager에 연결되지 않았어!");
        }

        if (gamegold != null)//게임 시작 시 SO로부터 초기 골드 설정
        {
            CurrentGold = gamegold.startGold;
        }
        else Debug.LogWarning("GameGold SO가 GameManager에 연결되지 않았어!");

        if (readyText != null)//ReadyText는 인스펙터에서 직접 연결 (비활성 상태여도 안전하게 시작 시 숨김 처리)
        {
            readyText.gameObject.SetActive(false);
        }
        else Debug.LogWarning("ReadyText가 GameManager에 연결되지 않았어!");
    }

    void Start()
    {
        StartCoroutine(GameReadyRoutine());//시작 전 대기시간
    }

    IEnumerator GameReadyRoutine()
    {
        IsGameReady = true;//카운트다운 시작 (일시정지창은 카운트 종료까지 작동 불가 상태)
        Time.timeScale = 0f;//게임 시작하자마자 시간 정지 (씬 전체 멈춤)
        float timer = gameReadyDelay;//카운트다운 동안 텍스트를 띄우며 1초씩 대기 (현실 시간 기준)

        while (timer > 0f)
        {
            //텍스트에 남은 시간 표시 (소수점 올림해서 3, 2, 1로 보이게)
            int displayNum = Mathf.CeilToInt(timer);
            if (readyText != null)
            {
                readyText.text = displayNum.ToString();
                readyText.gameObject.SetActive(true);
            }

            //1초 대기 (timeScale = 0이어도 멈추지 않게 Realtime 사용)
            yield return new WaitForSecondsRealtime(1f);//현실 시간 기준 1초 대기 (일시정지 중에도 작동)
            timer -= 1f;
        }

        if (readyText != null)//카운트 끝난 후 "START!" 같은 문구를 잠깐 보여주고 싶다면?
        {
            readyText.text = "START!";
            yield return new WaitForSecondsRealtime(1f);//1초 동안 START 표시
            readyText.gameObject.SetActive(false);//텍스트 숨기기
        }

        //대기 시간이 끝나면 시간 다시 흐르게 복구
        Time.timeScale = 1f;
        IsGameReady = false;//카운트다운 종료 (이제 일시정지창 생성 가능)
        Debug.Log("게임 시작!");

        if (currentRoundData != null)
        {
            OnRoundChanged?.Invoke(currentRoundData);
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
        enemy.OnEnemyDie += (isBoss) =>
        {
            if (isBoss)
            {
                OnGameClear?.Invoke();//보스 잡으면 클리어 이벤트만 발생
            }
            else EnemyDefeated();//일반 몬스터면 카운트 증가
        };
    }


    public void EnemyDefeated()
    {
        if (currentRoundData == null)
        {
            Debug.LogWarning("currentRoundData가 비어 있어서 처치 카운트를 올릴 수 없어!");
            return;
        }

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

    public void AddGold(int amount)//몬스터 처치로 골드를 획득(추가)하는 함수
    {
        CurrentGold += amount;
        Debug.Log($"골드 획득 성공! 현재 골드: {CurrentGold}");
        //나중에 필요하면 여기에 골드 획득 이벤트도 추가할 수 있어!

        OnGoldChanged?.Invoke(CurrentGold);//구독자들에게 바뀐 골드 전파(UiManager)
    }

    public bool UseGold(int amount)//골드를 사용(차감)하는 함수
    {
        if (CurrentGold >= amount)
        {
            CurrentGold -= amount;
            Debug.Log($"골드 차감 성공! 남은 골드: {CurrentGold}");
            //나중에 골드 UI가 바뀌면 여기서 이벤트를 쏴줄 수도 있어!

            OnGoldChanged?.Invoke(CurrentGold);//구독자들에게 바뀐 골드 전파(UiManager)
            return true;
        }

        Debug.Log("골드가 부족해서 사용할 수 없어!");
        return false;
    }
}