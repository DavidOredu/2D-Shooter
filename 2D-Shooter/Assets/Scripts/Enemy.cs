using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Alert,
    Chase,
    Attack,
}
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public EnemyState enemyState;

    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField]
    private EnemyData enemyData;

    [SerializeField]
    private Transform attackCheck;
    [SerializeField]
    private Transform patrolPointA;
    [SerializeField]
    private Transform patrolPointB;

    private Transform target = null;

    private int health = 100;
    private int facingDirection = -1;

    private float _alertTime;
    private float _confusionTime;
    private float _attackCooldownTime;

    private bool canAttack;

    private AttackInfo attackInfo;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        target = patrolPointA;
        enemyState = EnemyState.Patrol;
        facingDirection = -1;
        _alertTime = enemyData.alertTime;
        _confusionTime = enemyData.confusionTime;
        _attackCooldownTime = enemyData.attackCooldownTime;
        canAttack = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SwitchEnemyStates();
        HandleAnimations();
    }
    private void HandleAnimations()
    {
        anim.SetBool("move", rb.velocity.x != 0);
    }
    private void SwitchEnemyStates()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                SearchForPlayer();
                break;
            case EnemyState.Patrol:
                Patrol();
                CheckIfShouldFlip();
                SearchForPlayer();
                break;
            case EnemyState.Alert:
                AlertOnPlayer();
                CheckIfShouldFlip();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                CheckIfShouldFlip();
                CheckPlayerInAttackRange();
                break;
            case EnemyState.Attack:
                CheckPlayerInAttackRange();
                CheckIfCanAttack();
                SetAttacking();
                break;
        }
    }
    private void Patrol()
    {
        if (target == patrolPointA)
        {
            if (transform.position.x <= target.position.x)
            {
                target = patrolPointB;
            }
            else
            {
                rb.velocity = transform.right * enemyData.moveSpeed;
            }
        }
        else if (target == patrolPointB)
        {
            if (transform.position.x >= target.position.x)
            {
                target = patrolPointA;
            }
            else
            {
                rb.velocity = transform.right * enemyData.moveSpeed;
            }
        }
    }
    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingDirection *= -1;
    }
    private RaycastHit2D CanSeePlayer()
    {
        return Physics2D.Raycast(transform.position, transform.right, enemyData.playerSearchDistance, LayerMask.GetMask("Player"));
    }
    private void SearchForPlayer()
    {
        var hit = CanSeePlayer();

        if (hit)
        {
            enemyState = EnemyState.Alert;
            target = hit.transform;
        }
    }
    private void AlertOnPlayer()
    {
        if (CanSeePlayer())
        {
            _confusionTime = enemyData.confusionTime;
            if (_alertTime <= 0)
            {
                enemyState = EnemyState.Chase;
            }
            else
            {
                _alertTime -= Time.deltaTime;
            }
        }
        else
        {
            if (_confusionTime <= 0)
            {
                enemyState = EnemyState.Patrol;
                target = patrolPointA;
                _alertTime = enemyData.alertTime;
                _confusionTime = enemyData.confusionTime;
            }
            else
            {
                _confusionTime -= Time.deltaTime;
            }
        }
    }
    private void ChasePlayer()
    {
        if (Mathf.Abs(target.position.x - transform.position.x) <= enemyData.minAgroRange)
        {
            enemyState = EnemyState.Attack;
        }
        else
        {
            rb.velocity = transform.right * (enemyData.moveSpeed + (enemyData.chaseSpeedMultiplier * enemyData.moveSpeed));
        }
    }
    private void SetAttacking()
    {
        if (canAttack)
        {
            anim.SetTrigger("attack");
            canAttack = false;
        }
    }
    private void CheckPlayerInAttackRange()
    {
        if (Mathf.Abs(target.position.x - transform.position.x) >= enemyData.maxAgroRange)
        {
            enemyState = EnemyState.Alert;
            target = patrolPointA;
        }
        else if (Mathf.Abs(target.position.x - transform.position.x) >= enemyData.minAgroRange)
        {
            enemyState = EnemyState.Chase;
        }
    }
    private void Attack()
    {
        var hits = Physics2D.OverlapCircleAll(attackCheck.position, enemyData.attackCheckRadius, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            attackInfo.damageAmount = enemyData.damageAmount;
            attackInfo.knockbackDirection = enemyData.knockbackDirection * facingDirection;

            hit.transform.SendMessage("TakeDamage", attackInfo);
        }
    }
    private void FinishAttack()
    {
        canAttack = false;
    }
    private void CheckIfCanAttack()
    {
        if (!canAttack)
        {
            if (_attackCooldownTime <= 0)
            {
                canAttack = true;
                _attackCooldownTime = enemyData.attackCooldownTime;
            }
            else
            {
                _attackCooldownTime -= Time.deltaTime;
            }
        }
    }
    private void CheckIfShouldFlip()
    {
        if((target.position.x > transform.position.x && facingDirection == -1) || target.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
    }
    private void TakeDamage(AttackInfo attackInfo)
    {
        health -= attackInfo.damageAmount;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + enemyData.minAgroRange * facingDirection, transform.position.y), 0.2f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + enemyData.maxAgroRange * facingDirection, transform.position.y), 0.2f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + enemyData.playerSearchDistance * facingDirection, transform.position.y), 0.2f);
        Gizmos.DrawWireSphere(attackCheck.position, enemyData.attackCheckRadius);
    }
}
