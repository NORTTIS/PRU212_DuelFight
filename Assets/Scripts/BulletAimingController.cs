using System.Collections;
using UnityEngine;

public class ArrowAimingController : MonoBehaviour
{
    [Header("Cài đặt góc bắn")]
    [SerializeField] private float upperAngle = 60f;   // Góc bắn tối đa (lên trên)
    [SerializeField] private float lowerAngle = 0f;    // Góc bắn tối thiểu (thấp nhất)

    [SerializeField] private float upperAngleLeft = 120f;      // Góc bắn tối đa (lên trên)
    [SerializeField] private float lowerAngleLeft = 180f;      // Góc bắn tối thiểu (thấp nhất)

    private float upAngle;  // Góc bắn tối đa hiện tại
    private float lowAngle;  // Góc bắn tối thiểu hiện tại
    [SerializeField] private float rotateSpeed = 2f;   // Tốc độ xoay góc
    [SerializeField] private bool isFacingRight = true; // Hướng nhân vật đang quay mặt

    [SerializeField] GameObject ArrowDirection;


    private bool isLocked = false;         // Trạng thái khóa góc bắn (đã bắn)
    private float lockEndTime;             // Thời điểm kết thúc lock
    private float cachedAngle = 0f;        // Góc bắn được lưu lại
    private float lastShotTime = -Mathf.Infinity; // Thời gian bắn trước đó

    private const float lockThreshold = 0.2f;  // Khoảng thời gian cần giãn cách giữa 2 lần lock

    // Biến dùng để tính toán tiếp tục xoay mượt sau khi unlock
    private float unlockedOffset = 0f;
    private float unlockedStartTime = 0f;

    private void Update()
    {
        updateFireAngle();
        MoveArrowAlongArc();
    }


    // Cập nhật góc xoay mũi tên (dựa theo thời gian hoặc khi đã lock)
    private void updateFireAngle()
    {
        if (isLocked)
        {
            transform.rotation = Quaternion.Euler(0, 0, cachedAngle);
            ArrowDirection.transform.rotation = Quaternion.Euler(0, 0, cachedAngle);

            if (Time.time >= lockEndTime)
            {
                isLocked = false;

                // Sau khi hết thời gian lock, tiếp tục xoay lại mượt từ đúng vị trí cũ
                unlockedStartTime = Time.time;

                // Tính toán t (giá trị đầu vào cho sin) từ cachedAngle
                float normalized = ((cachedAngle - lowerAngle) / (upperAngle - lowerAngle) - 0.5f) * 2f;
                normalized = Mathf.Clamp(normalized, -1f, 1f);

                unlockedOffset = Mathf.Asin(normalized) / rotateSpeed;

                if (float.IsNaN(unlockedOffset))
                    unlockedOffset = 0f; // fallback nếu lỗi
            }
        }
        else
        {
            // Nếu không bị lock thì liên tục xoay qua lại theo sin
            float t = Mathf.Sin((Time.time - unlockedStartTime + unlockedOffset) * rotateSpeed) * 0.5f + 0.5f;
            float angle = Mathf.Lerp(lowerAngle, upperAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            ArrowDirection.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        // DrawArc();
    }


    // Hàm lấy hướng bắn hiện tại (dạng Vector2)
    public Vector2 GetDirection()
    {
        return transform.right.normalized;
    }

    // Khóa góc bắn trong 1 khoảng thời gian ngắn sau khi bắn
    public void LockAimingDirection(float duration = 0.2f)
    {
        // Chỉ lưu lại góc nếu thời gian từ lần bắn trước >= ngưỡng
        if (Time.time - lastShotTime >= lockThreshold)
        {
            cachedAngle = transform.rotation.eulerAngles.z;
        }

        lastShotTime = Time.time;
        isLocked = true;
        lockEndTime = Time.time + duration;
    }
    // Thiết lập hướng quay mặt(true = phải, false = trái)
    // Và cập nhật lại góc bắn tương ứng cho mỗi bên
    public void SetFacingDirection(bool facingRight)
    {
        // Nếu thay đổi hướng so với trước đó thì cập nhật lại góc
        if (isFacingRight ^ facingRight)
        {
            if (upperAngle > lowerAngle)
            {
                // Đang quay phải → chuyển sang trái
                upAngle = upperAngleLeft;
                lowAngle = lowerAngleLeft;
            }
            else
            {
                // Đang quay trái → chuyển sang phải
                upAngle = upperAngle;
                lowAngle = lowerAngle;
            }

            isFacingRight = facingRight;
        }
    }
    void OnDrawGizmos()
    {
        float length = 2f;
        Vector3 origin = transform.position;
        Vector3 currentDir = transform.right;

        // Hướng hiện tại
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + currentDir * length);

        // Giới hạn trên
        Vector3 upperDir = Quaternion.Euler(0, 0, upperAngle) * Vector3.right;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + upperDir.normalized * length);

        // Giới hạn dưới
        Vector3 lowerDir = Quaternion.Euler(0, 0, lowerAngle) * Vector3.right;
        Gizmos.DrawLine(origin, origin + lowerDir.normalized * length);

        // Vẽ cung giữa 2 giới hạn
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // cam mờ
        DrawArc(origin, Vector3.right, lowerAngle, upperAngle, length, 20);
    }
    void DrawArc(Vector3 center, Vector3 startDir, float startAngle, float endAngle, float radius, int segments)
    {
        Vector3 prevPoint = center + Quaternion.Euler(0, 0, startAngle) * startDir.normalized * radius;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            Vector3 nextPoint = center + Quaternion.Euler(0, 0, angle) * startDir.normalized * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    // void DrawArc()
    // {
    //     Vector3 center = transform.position;
    //     Vector3 startDir = Vector3.right;

    //     for (int i = 0; i <= arcSegments; i++)
    //     {
    //         float t = (float)i / arcSegments;
    //         float angle = Mathf.Lerp(lowerAngle, upperAngle, t);
    //         Vector3 point = center + Quaternion.Euler(0, 0, angle) * startDir * arcLength;
    //     }
    // }
    private void MoveArrowAlongArc()
    {
        if (ArrowDirection == null) return;

        Vector3 center = transform.position;
        Vector3 startDir = Vector3.right;

        float t = Mathf.Sin((Time.time - unlockedStartTime + unlockedOffset) * rotateSpeed) * 0.5f + 0.5f;
        float angle = isLocked
                    ? cachedAngle
                    : Mathf.Lerp(lowerAngle, upperAngle, t);

        // Vị trí mới theo cung tròn
        Vector3 currentPos = center + Quaternion.Euler(0, 0, angle) * startDir * 2f;

        // Gán vị trí
        ArrowDirection.transform.position = currentPos;

        // Hướng thẳng theo góc bắn (dựa trên góc angle)
        ArrowDirection.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
