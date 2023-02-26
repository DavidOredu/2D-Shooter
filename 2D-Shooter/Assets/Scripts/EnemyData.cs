using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Data/Enemy Data")]
public class EnemyData : GunnerData
{
    [Header("AI")]
    public float playerSearchDistance = .6f;
    public float attackCheckRadius = .5f;
    public float alertTime = .5f;
    public float confusionTime = 1f;
    public float attackCooldownTime = 1f;

    [Header("CHASE")]
    public float chaseSpeedMultiplier = .2f;

    [Header("COMBAT")]
    public float minAgroRange = .5f;
    public float maxAgroRange = 1f;
    public int damageAmount = 30;
    public float criticalDamageMultiplier = .25f;
    public float criticalDamageChance = .1f;
    public Vector2 knockbackDirection;
}
