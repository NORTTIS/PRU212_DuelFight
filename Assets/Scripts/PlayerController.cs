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
    [SerializeField] Transform petFirePoint;
    [SerializeField] float fireLockRate = 0.2f;

    [SerializeField] float arrowSpeed = 10f;
    [SerializeField] float arrowLifetime = 2f;
    [SerializeField] bool isFacingRight;

    [SerializeField] public Animator animator;
    [SerializeField] public GameObject pet;
    [SerializeField] public int UltimateMana = 40;
    private bool isGrounded = false;

    private PlayerStats playerStats;


    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        HandleJump();
        Fire();
        HandleSkillInput();
    }

    void HandleSkillInput()
    {
        if (isPlayer1)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                playerStats.UseSkill(Enums.SkillType.Block);
            if (Input.GetKeyDown(KeyCode.E))
                playerStats.UseSkill(Enums.SkillType.Heal);
            if (Input.GetKeyDown(KeyCode.R))
                playerStats.UseSkill(Enums.SkillType.TrackingBullet);
            if (Input.GetKeyDown(KeyCode.F) && playerStats.mana >= UltimateMana)
            {
                if (playerStats.mana >= UltimateMana)
                {
                    pet.SetActive(true);
                    playerStats.mana -= UltimateMana;
                }
                else
                {
                    Debug.Log($"{playerStats.playerName} not enough mana for Pet summon");
                }
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.U))
                playerStats.UseSkill(Enums.SkillType.Block);
            if (Input.GetKeyDown(KeyCode.I))
                playerStats.UseSkill(Enums.SkillType.Heal);
            if (Input.GetKeyDown(KeyCode.O))
                playerStats.UseSkill(Enums.SkillType.TrackingBullet);
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (playerStats.mana >= UltimateMana)
                {
                    pet.SetActive(true);
                    playerStats.mana -= UltimateMana;
                }
                else
                {
                    Debug.Log($"{playerStats.playerName} not enough mana for Pet summon");
                }
            }

        }
        if (playerStats.requestTrackingBullet)
        {
            FireTrackingBullet();
            playerStats.requestTrackingBullet = false;
        }
    }
    void FireTrackingBullet()
    {
        PlayerStats target = isPlayer1 ? GameManager.Instance.player2 : GameManager.Instance.player1;
        GameObject proj = Instantiate(GameManager.Instance.trackingProjectilePrefab, firePoint.position, Quaternion.identity);
        TrackingProjectile tp = proj.GetComponent<TrackingProjectile>();
        tp.SetTarget(target.transform);
        tp.isPlayer1 = isPlayer1;
        tp.shooter = this.transform;

        if (pet.activeSelf)
        {
            GameObject projPet = Instantiate(GameManager.Instance.trackingProjectilePrefab, petFirePoint.position, Quaternion.identity);
            TrackingProjectile tpPet = projPet.GetComponent<TrackingProjectile>();
            tpPet.SetTarget(target.transform);
            tpPet.isPlayer1 = isPlayer1;
            tpPet.shooter = this.transform;
        }
        // Bỏ qua va chạm với chính người bắn
        Collider2D playerCollider = GetComponent<Collider2D>();
        Collider2D projCollider = proj.GetComponent<Collider2D>();
        Collider2D petCollider = pet.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, projCollider, true);
        Physics2D.IgnoreCollision(petCollider, projCollider, true);
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
        if (moveInput > 0 && !isFacingRight)
            Flip();
        else if (moveInput < 0 && isFacingRight)
            Flip();
        animator.SetFloat("Horizontal", moveInput);

        animator.SetBool("isRunning", moveInput != 0);
        if (moveInput > 0)
            animator.SetBool("ishor", true);
        else if (moveInput < 0)
            animator.SetBool("ishor", false);

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
            bool releasedJump = (isPlayer1 && !Input.GetKey(KeyCode.W)) ||
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
            // Player 2: P để bắn
            shouldFire = Input.GetKeyDown(KeyCode.P);
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

            // Bỏ qua va chạm với chính người bắn
            Collider2D playerCollider = GetComponent<Collider2D>();
            Collider2D projCollider = proj.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(playerCollider, projCollider, true);
            arrowAiming.LockAimingDirection(fireLockRate);
            Destroy(proj, arrowLifetime);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        arrowAiming.SetFacingDirection(isFacingRight);
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
