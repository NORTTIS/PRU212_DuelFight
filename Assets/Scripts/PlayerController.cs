using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;

    [SerializeField] float jumpForce = 5f;

    [SerializeField] public float maxHealth = 100f;

    [SerializeField] public float atk = 10f;

    [SerializeField] LayerMask groundCheck;
    [SerializeField] Transform groundCheckPoint;

    [SerializeField] public bool isPlayer1;

    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    [SerializeField] ArrowAimingController arrowAiming;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] float arrowSpeed = 10f;
    //[SerializeField] float arrowLifetime = 2f;

    private bool isGrounded = false;


    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        HandleJump();
        Fire();
    }

    void Movement()
    {
        float moveInput = 0f;
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, 0.2f, groundCheck);

        if (isPlayer1)
        {
            // Player 1: A/D
            if (Input.GetKey(KeyCode.A)) moveInput = -1f;
            if (Input.GetKey(KeyCode.D)) moveInput = 1f;

            // Nhảy bằng W
            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            // Player 2: ← / →
            if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1f;

            // Nhảy bằng ↑
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

    }
    void HandleJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            // Đang rơi xuống -> tăng trọng lực
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            // Kiểm tra người chơi có thả nút nhảy sớm không
            bool releasedJump = (isPlayer1 && !Input.GetKey(KeyCode.Space)) ||
                                (!isPlayer1 && !Input.GetKey(KeyCode.UpArrow));

            if (releasedJump)
            {
                // Rơi sớm -> nhảy thấp hơn
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
            }
        }
    }

    public void Fire()
    {
        bool shouldFire;
        if (isPlayer1)
        {
            // Player 1: Space để bắn
            shouldFire = Input.GetKeyDown(KeyCode.Space);
        }
        else
        {
            // Player 2: Chuột phải để bắn
            shouldFire = Input.GetMouseButtonDown(0); // 1 = chuột trái
        }

        if (shouldFire)
        {
            // lấy hướng bắn
            Vector2 direction = arrowAiming.GetDirection();

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // Đặt hướng mũi tên dựa trên hướng nhắm
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Xoay mũi tên về hướng nhắm
            proj.transform.rotation = Quaternion.Euler(0, 0, angle);

            Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();

            // set isPlayer1 cho mũi tên
            ArrowProjectile arrowProjectile = proj.GetComponent<ArrowProjectile>();
            arrowProjectile.isPlayer1 = isPlayer1;
            // Thêm lực vào mũi tên theo hướng nhắm
            projRb.AddForce(direction.normalized * arrowSpeed, ForceMode2D.Impulse);

            Destroy(proj, 2f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, 0.2f);
        }
    }
}
