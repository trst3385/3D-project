using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Data", menuName = "ScriptableObject/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Attack 나무")]
    public float AttackInterval = 1f;//n초마다 공격
    public float Range = 10f;//트리거 콜라이더 크기 조절용

    [Header("발사체 설정")]
    public GameObject BulletPrefab;
    public float BulletSpeed = 10f;
    public int BulletDamage = 5;

    [Header("Slow 나무")]
    public float SlowAmount = 0.5f; //0.5면 50% 느려짐

}
