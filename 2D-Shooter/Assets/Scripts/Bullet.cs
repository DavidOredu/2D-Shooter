using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerCombatController combatController;

    [SerializeField]
    private float bulletSpeed = 10f;

    private Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void SetVariables(PlayerCombatController combatController, Vector2 direction)
    {
        this.combatController = combatController;
        this.direction = direction;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = direction * bulletSpeed * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            combatController.Damage(other.transform);
            Destroy(gameObject);
        }
    }
}
