using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    //private bool facingRight = true;

    [Header("Components")]
    public Rigidbody2D rb;


    [Header("Physics")]
    public float maxSpeed = 12f;

    //[Header("Collision")]


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");
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
    }
}
