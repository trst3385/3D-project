    using UnityEngine;

    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "ScriptableObject/Enemy Data")]

public class EnemyData : ScriptableObject
{
    [Header("몬스터 스탯")]
    public int MaxHealth = 20;
    public float Speed = 5f;
    public int Damage = 10;//몬스터가 WayPoint 끝에 닿으면 플레이어 HP 감소

    [Header("공격 데이터")]
    public float AttackRange = 5f;
    public float AttackInterval = 2f;
    public int TreeDamage = 1;//나무에게 줄 데미지 (혹은 맞은 횟수 처리)

    [Header("공격 시각 효과")]
    public GameObject attackEffectPrefab;//나무 공격시 피격 이펙트

    [Header("보상 설정")]
    public int RewardGold = 10;//몬스터 처치 시 획득할 골드

}
