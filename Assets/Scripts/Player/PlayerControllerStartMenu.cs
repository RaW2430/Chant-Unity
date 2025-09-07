using System.Collections;
using UnityEngine;

public class PlayerControllerStartMenu : MonoBehaviour
{
    public float jumpForce = 5f;      // 跳跃力度
    public float jumpInterval = 2f;   // 跳跃间隔（秒）

    private Rigidbody2D rb;
    private bool isGrounded = true;   
    private float nextJumpTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nextJumpTime = Time.time + jumpInterval;
    }

    void Update()
    {
        // 到了时间就跳跃
        if (Time.time >= nextJumpTime)
        {
            Jump();
            nextJumpTime = Time.time + jumpInterval;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
