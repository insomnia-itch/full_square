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
    public bool crouching = false;

    [Header("Components")]
    public Rigidbody2D rb;
    public LayerMask groundLayer;
    public GameObject character;
    Vector3 originalSize;

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

    private Vector3 scaleChange, positionChange;

    private void Awake()
    {
        scaleChange = new Vector3(-0.01f, -0.01f, -0.01f);
        positionChange = new Vector3(0.0f, -0.2f, 0.0f);
    }
    // Start is called before the first frame update
    void Start()
    {
        originalSize = Vector3.one;

    }

    // Update is called once per frame
    void Update()
    {
        bool wasOnGround = onGround;
        bool wasCrouching = crouching;
        crouching = (onGround && Input.GetButtonDown("Crouch")) ? true : false;
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        if (!wasOnGround && onGround)
        {
            StartCoroutine(JumpSqueeze(1.25f, 0.8f, 0.05f));
        }
        if (Input.GetButtonDown("Crouch"))
        {
            Debug.Log("crouch down");
        }
        if (crouching)
        {
            Debug.Log("crouch DOWN");
            character.transform.position += Vector3.down* 0.02f;
            character.transform.localScale = new Vector3(1.25f, 0.8f, 1f);
            character.transform.position += positionChange;

            crouching = true;
        }
        if(Input.GetButtonUp("Crouch"))
        {
            Debug.Log("crouch uP????");
            character.transform.localScale = new Vector3(1, 1, 1);
            crouching = false;
        }

        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }

        //if (Input.GetKeyDown(KeyCode.DownArrow) && onGround && !crouching)
        //{
        //    Crouch();
        //}
        //if (Input.GetKeyUp(KeyCode.DownArrow) && onGround && crouching)
        //{
        //    Uncrouch();
        //}


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


    void Crouch()
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(0.8f, 1.25f, originalSize.z);
        character.transform.localScale = Vector3.Lerp(originalSize, newSize, 1);
        crouching = true;
    }

    void Uncrouch()
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(1.25f, 0.8f, originalSize.z);
        character.transform.localScale = Vector3.Lerp(originalSize, newSize, 1);
        crouching = false;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
    }


    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            character.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            character.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }
}
