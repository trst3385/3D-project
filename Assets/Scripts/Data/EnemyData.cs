using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "ScriptableObject/Enemy Data")]

public class EnemyData : ScriptableObject
{
    public int MaxHealth = 10;
    public float Speed = 5f;
    public int Damage = 10;
}
