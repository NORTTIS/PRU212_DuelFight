using UnityEngine;
using Pathfinding;

public class PetController : MonoBehaviour
{
    // Layer dùng để kiểm tra pet có đang đứng trên mặt đất hay không
    [SerializeField] LayerMask groundCheck;

    // Tham chiếu đến player để pet có thể theo dõi và di chuyển gần
    [SerializeField] public PlayerController player;

    // Tốc độ di chuyển tối đa của pet
    public float moveSpeed = 3f;

    // Vị trí dùng để kiểm tra mặt đất
    [SerializeField] Transform groundCheckPoint;

    private AIDestinationSetter destinationSetter; // Component giúp gán target cho AI
    private AIPath aiPath; // AIPath dùng để pathfinding và di chuyển
    private GameObject targetMarker; // Một object tạm để làm điểm đích khi không có item

    private bool isGrounded = true; // Kiểm tra pet có đang chạm đất

    void Start()
    {
        // Lấy các component cần thiết
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();

        // Nếu có AIPath thì cấu hình các thông số ban đầu
        if (aiPath != null)
        {
            aiPath.maxSpeed = moveSpeed;
            aiPath.canMove = true; // Cho phép di chuyển lúc đầu
        }
        else
        {
            Debug.LogWarning("AIPath component not found on pet!");
        }

        // Lặp lại hàm UpdateTarget mỗi 0.5 giây
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);

        // Tạo đối tượng target marker để làm điểm đứng gần player
        targetMarker = new GameObject("TargetMarker");
    }

    void Update()
    {
        Movement(); // Kiểm tra pet có đang chạm đất
        CheckDistanceToPlayer(); // Dừng hoặc tiếp tục di chuyển khi đến gần player
        FlipSprite();
    }

    /// <summary>
    /// Cập nhật target của pet mỗi 0.5s. Ưu tiên item, nếu không có thì đi gần player
    /// </summary>
    void UpdateTarget()
    {
        if (destinationSetter == null || player == null) return;

        // Nếu target cũ bị destroy thì gỡ bỏ
        if (destinationSetter.target != null && destinationSetter.target.gameObject == null)
        {
            destinationSetter.target = null;
        }

        // Tìm item gần nhất
        GameObject nearestItem = FindClosestItemWithTag("Item");
        if (nearestItem != null)
        {
            // Nếu có item, di chuyển đến item đó
            destinationSetter.target = nearestItem.transform;
            if (aiPath != null)
            {
                aiPath.canMove = true;
                Debug.Log("New item found, pet moving to " + nearestItem.transform.position + " at " + System.DateTime.Now.ToString("hh:mm:ss tt"));
            }
        }
        else
        {
            // Nếu không có item thì di chuyển gần player
            destinationSetter.target = null;
            if (aiPath != null)
            {
                // Tính toán vị trí cách player 1 đơn vị (đứng cạnh player chứ không trùng)
                Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
                Vector3 targetPosition = player.transform.position - directionToPlayer * 1f;

                // Chỉ cập nhật vị trí nếu khác biệt đáng kể (tránh tính lại path nhiều lần)
                if (Vector3.Distance(targetMarker.transform.position, targetPosition) > 0.1f)
                {
                    targetMarker.transform.position = targetPosition;
                    aiPath.SetPath(null); // Reset lại đường đi nếu thay đổi vị trí
                }

                // Gán target là targetMarker (gần player)
                destinationSetter.target = targetMarker.transform;
                aiPath.canMove = true;

                Debug.Log("Moving to near player at " + targetMarker.transform.position);
            }
        }
    }

    /// <summary>
    /// Kiểm tra khoảng cách đến player. Nếu đã đến gần (<=1f) thì dừng lại
    /// </summary>
    void CheckDistanceToPlayer()
    {
        if (aiPath != null && destinationSetter.target == targetMarker.transform && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Nếu đã đến gần player thì dừng lại
            if (distanceToPlayer <= 1f)
            {
                aiPath.canMove = false;
                destinationSetter.target = null;
                Debug.Log("Reached 1f from player, pet stopped at " + System.DateTime.Now.ToString("hh:mm:ss tt"));
            }
            // Nếu bị dừng mà player lại chạy xa thì tiếp tục di chuyển
            else if (distanceToPlayer > 1f && !aiPath.canMove)
            {
                aiPath.canMove = true;
            }
        }
    }

    /// <summary>
    /// Tìm object gần nhất có tag chỉ định (ví dụ: "Item")
    /// </summary>
    GameObject FindClosestItemWithTag(string tag)
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject item in items)
        {
            float dist = Vector3.Distance(item.transform.position, currentPos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = item;
            }
        }

        return closest;
    }

    /// <summary>
    /// Kiểm tra pet có đang đứng trên mặt đất hay không
    /// </summary>
    void Movement()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, 0.2f, groundCheck);
    }

    /// <summary>
    /// Vẽ gizmo kiểm tra vùng chạm đất trong editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, 0.2f);
        }
    }

    /// <summary>
    /// Cleanup: xóa targetMarker khi pet bị destroy
    /// </summary>
    void OnDestroy()
    {
        if (targetMarker != null)
        {
            Destroy(targetMarker);
        }
    }

    void FlipSprite()
    {
        if (aiPath == null) return;

        float moveX = aiPath.desiredVelocity.x;

        // Lật hướng theo trục X nếu đang di chuyển đủ xa
        if (Mathf.Abs(moveX) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveX) * Mathf.Abs(scale.x); // Giữ nguyên chiều cao
            transform.localScale = scale;
        }
    }
}
