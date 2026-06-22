using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalEnemyCount = 10;                   //총 등장할 몬스터 수
    [SerializeField]private int defeatedEnemyCount = 0;//처치한 몬스터 수

    public event System.Action<int, int> OnEnemyCountChanged;//UI한테 "나 숫자 바뀌었어!"라고 알려주는 이벤트
    public event System.Action OnGameClear;//UI한테 "게임 끝났어!"라고 알려주는 이벤트

    void Awake() => Instance = this;

    public void EnemyDefeated()
    {
        defeatedEnemyCount++;
        // 이벤트를 통해 알림 발송 (구독자가 없어도 에러 안 나게 ? 사용)
        OnEnemyCountChanged?.Invoke(defeatedEnemyCount, totalEnemyCount);

        if (defeatedEnemyCount >= totalEnemyCount)
        {
            OnGameClear?.Invoke();
        }
    }
}
