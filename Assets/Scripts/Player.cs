using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    //private bool facingRight = true;


    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    public float airDragMultiplier = 0.65f;
    private float jumpTimer;

    [Header("Components")]
    public Rigidbody2D rb;
    public LayerMask groundLayer;


    [Header("Physics")]
    public float maxSpeed = 12f;
    public float linearDrag = 4f;
    public float gravity = 1.4f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool onGround = false;
    public float groundLength = 0.6f;
    // Edit this value in the inspector
    // * Should be at the left/right edges of the sprite
    public Vector3 colliderOffset;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        bool wasOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }
        direction = new Vector2(horizontalDirection, verticalDirection);
    }

    void movePlayer(float horizontal)
    {
        Vector2 horizonatlForce = Vector2.right * horizontal * moveSpeed;
        rb.AddForce(horizonatlForce);

        if(Mathf.Abs(rb.velocity.x) > maxSpeed) {
            float playerDirection = Mathf.Sign(rb.velocity.x);
            rb.velocity = new Vector2(playerDirection * maxSpeed, rb.velocity.y);
        }
    }

    private void FixedUpdate()
    {
        movePlayer(direction.x);
        if (jumpTimer > Time.time && onGround)
        {
            Jump();
        }
        modifyPhysics();
    }


    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {

            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0;
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * airDragMultiplier;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }


    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }
}
