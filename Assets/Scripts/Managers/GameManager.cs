using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalEnemyCount = 10;                   //총 등장할 몬스터 수
    [SerializeField] private int defeatedEnemyCount = 0;//처치한 몬스터 수

    public event System.Action<int, int> OnEnemyCountChanged;//UI한테 "나 숫자 바뀌었어!"라고 알려주는 이벤트
    public event System.Action OnGameClear;//UI한테 "게임 끝났어!"라고 알려주는 이벤트
    public event System.Action OnBossSpawn;//보스 몬스터 등장 이벤트

    void Awake() => Instance = this;



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
        OnEnemyCountChanged?.Invoke(defeatedEnemyCount, totalEnemyCount);//이벤트를 통해 알림 발송 (구독자가 없어도 에러 안 나게 ? 사용)

        if (defeatedEnemyCount == totalEnemyCount)//totalEnemyCount.처치 수 달성 시 보스 소환 신호 발송!
        {
            OnBossSpawn?.Invoke();
        }
    }
}