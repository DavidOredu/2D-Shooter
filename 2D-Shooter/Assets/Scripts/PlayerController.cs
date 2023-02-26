using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private Transform groundCheck = null;

    private float moveDirection;

    private int facingDirection = 1;

    private bool jumpInput;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetKeyDown(KeyCode.Space);

        Jump();
    }
    private void FixedUpdate()
    {
        Move();
    }
    
    private void Move()
    {
        rb.velocity = new Vector2(moveDirection * playerData.moveSpeed * Time.deltaTime, rb.velocity.y);

        if (moveDirection != facingDirection && moveDirection != 0)
            Flip();

        if (moveDirection != 0 && CheckIfGrounded())
            anim.SetBool("move", true);
        else
            anim.SetBool("move", false);
    }
    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingDirection *= -1;
    }
    private void Jump()
    {
        if (jumpInput && CheckIfGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, playerData.jumpSpeed);
            anim.SetTrigger("jump");
        }

        anim.SetBool("inAir", !CheckIfGrounded());
        anim.SetFloat("yVelocity", Mathf.CeilToInt(rb.velocity.normalized.y));
    }
    private bool CheckIfGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, playerData.groundCheckDistance, LayerMask.GetMask("Ground"));
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + (Vector3)(Vector2.down * playerData.groundCheckDistance));
    }
}
