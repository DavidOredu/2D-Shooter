using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [SerializeField]
    private PlayerData playerData;

    private int health = 100;

    private AttackInfo attackInfo;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
            SetShooting();
    }
    private void SetShooting()
    {
        anim.SetTrigger("shoot");
    }
    private void TakeDamage(AttackInfo attackInfo)
    {
        health -= attackInfo.damageAmount;
        rb.velocity = attackInfo.knockbackDirection;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void Damage(Transform transform)
    {
        attackInfo.damageAmount = playerData.damageAmount;
        transform.SendMessage("TakeDamage", attackInfo);
    }
}
