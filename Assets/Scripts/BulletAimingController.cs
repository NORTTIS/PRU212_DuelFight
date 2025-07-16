using UnityEngine;

public class ArrowAimingController : MonoBehaviour
{
    [Header("Setting right aiming angles")]
    [SerializeField] private float rightUpperAngle = 60f;
    [SerializeField] private float rightLowerAngle = -20f;

    [Header("Setting left aiming angles")]
    [SerializeField] private float leftUpperAngle = 120f;
    [SerializeField] private float leftLowerAngle = 200f;

    [SerializeField] private float rotateSpeed = 50f; // Tốc độ xoay (độ/giây)
    [SerializeField] private bool isFacingRight = true;

    [SerializeField] private GameObject ArrowDirection;

    private float upperAngle;
    private float lowerAngle;

    private bool isLocked = false;
    private float lockEndTime;
    private float cachedAngle = 0f;
    private float lastShotTime = -Mathf.Infinity;

    private const float lockThreshold = 0.2f;

    private float currentAngle; // Góc hiện tại của mũi tên
    private int aimingDirection = 1; // +1 = tiến tới upperAngle, -1 = tiến tới lowerAngle
    private bool isFlipping = false; // Đang chuyển hướng
    private float targetAngle; // Góc mục tiêu khi đổi hướng

    private void Awake()
    {
        ApplyFacingDirection(isFacingRight);
        currentAngle = (upperAngle + lowerAngle) / 2f; // Khởi tạo góc ở giữa phạm vi
    }

    private void Update()
    {
        UpdateFireAngle();
        MoveArrowAlongArc();
    }

    private void UpdateFireAngle()
    {
        if (isLocked)
        {
            // Giữ góc cố định khi khóa
            transform.rotation = Quaternion.Euler(0, 0, cachedAngle);
            if (ArrowDirection != null)
                ArrowDirection.transform.rotation = Quaternion.Euler(0, 0, cachedAngle);

            // Mở khóa khi hết thời gian
            if (Time.time >= lockEndTime)
            {
                isLocked = false;
                currentAngle = cachedAngle;
                ClampCurrentAngle();
            }

            return; // Dừng tại đây nếu đang locked
        }
        else if (isFlipping)
        {
            // Gán góc đích khi đổi hướng
            currentAngle = targetAngle;

            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            if (ArrowDirection != null)
                ArrowDirection.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 0.01f)
            {
                isFlipping = false;
                ClampCurrentAngle();
            }

            return; // Dừng tại đây nếu đang flipping
        }

        // === Nếu không locked và không flipping, thì tiếp tục xoay tự động ===

        // Kiểm tra giới hạn và đảo hướng nếu chạm biên
        if (isFacingRight)
        {
            if (currentAngle >= upperAngle)
            {
                currentAngle = upperAngle;
                aimingDirection = -1;
            }
            else if (currentAngle <= lowerAngle)
            {
                currentAngle = lowerAngle;
                aimingDirection = 1;
            }
        }
        else
        {
            if (currentAngle >= lowerAngle)
            {
                currentAngle = lowerAngle;
                aimingDirection = -1;
            }
            else if (currentAngle <= upperAngle)
            {
                currentAngle = upperAngle;
                aimingDirection = 1;
            }
        }

        // Di chuyển theo hướng hiện tại
        currentAngle += aimingDirection * rotateSpeed * Time.deltaTime;

        // Cập nhật xoay
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        if (ArrowDirection != null)
            ArrowDirection.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }

    private void MoveArrowAlongArc()
    {
        if (ArrowDirection == null) return;

        Vector3 center = transform.position;
        float angle = isLocked ? cachedAngle : currentAngle;
        angle = ClampAngle(angle, lowerAngle, upperAngle); // Giới hạn góc trước khi sử dụng

        Vector3 currentPos = center + Quaternion.Euler(0, 0, angle) * Vector3.right * 2f;
        ArrowDirection.transform.position = currentPos;
        ArrowDirection.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public Vector2 GetDirection()
    {
        return transform.right.normalized;
    }

    public void LockAimingDirection(float duration = 0.2f)
    {
        if (Time.time - lastShotTime >= lockThreshold)
        {
            cachedAngle = currentAngle; // Lưu góc hiện tại
            isLocked = true;
            lockEndTime = Time.time + duration;
            lastShotTime = Time.time;
        }
    }

    public void SetFacingDirection(bool facingRight)
    {
        if (isFacingRight != facingRight)
        {
            isFacingRight = facingRight;
            ApplyFacingDirection(facingRight);

            // Ánh xạ góc qua pháp tuyến 90°
            float mirrored = 180f - currentAngle;
            targetAngle = ClampAngle(mirrored, lowerAngle, upperAngle);
            currentAngle = targetAngle;

            // Đảo hướng di chuyển: xu hướng cũ đối xứng sang bên kia
            aimingDirection *= -1;

            // Bắt đầu quá trình chuyển hướng
            isFlipping = true;
        }
    }

    private void ApplyFacingDirection(bool facingRight)
    {
        if (facingRight)
        {
            upperAngle = rightUpperAngle;
            lowerAngle = rightLowerAngle;
        }
        else
        {
            upperAngle = leftUpperAngle;
            lowerAngle = leftLowerAngle;
        }
    }

    private void OnDrawGizmos()
    {
        float length = 2f;
        Vector3 origin = transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + transform.right * length);

        Vector3 upperDir = Quaternion.Euler(0, 0, upperAngle) * Vector3.right;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + upperDir * length);

        Vector3 lowerDir = Quaternion.Euler(0, 0, lowerAngle) * Vector3.right;
        Gizmos.DrawLine(origin, origin + lowerDir * length);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        DrawArcGizmo(origin, Vector3.right, Mathf.Min(lowerAngle, upperAngle), Mathf.Max(lowerAngle, upperAngle), length, 20);
    }

    private void DrawArcGizmo(Vector3 center, Vector3 startDir, float startAngle, float endAngle, float radius, int segments)
    {
        Vector3 prevPoint = center + Quaternion.Euler(0, 0, startAngle) * startDir * radius;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            Vector3 nextPoint = center + Quaternion.Euler(0, 0, angle) * startDir * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (min < max)
            return Mathf.Clamp(angle, min, max);
        else
            return Mathf.Clamp(angle, max, min); // Xử lý trường hợp min > max (hướng trái)
    }

    private void ClampCurrentAngle()
    {
        currentAngle = ClampAngle(currentAngle, lowerAngle, upperAngle);
    }
}