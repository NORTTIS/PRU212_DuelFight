using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class ArrowAimingController : MonoBehaviour
{
    [SerializeField] float upperAngle = 60f;  // Giới hạn trên của góc
    [SerializeField] float lowerAngle = 0f;   // Giới hạn dưới của góc
    [SerializeField] float rotateSpeed = 2f;
    [SerializeField] bool isFacingRight = true;


    void Update()
    {
        // Lắc qua lại từ lowerAngle đến upperAngle theo dạng sin
        float t = Mathf.Sin(Time.time * rotateSpeed) * 0.5f + 0.5f; // Từ 0 đến 1
        float angle = Mathf.Lerp(lowerAngle, upperAngle, t);        // Interpolate từ dưới lên trên
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public Vector2 GetDirection()
    {
        return transform.right.normalized;
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


    public void SetFacingDirection(bool facingRight)
    {

        if (isFacingRight ^ facingRight)
        {
            if (upperAngle > lowerAngle)
            {
                upperAngle = 120f;
                lowerAngle = 180f;
            }
            else
            {
                upperAngle = 60f;
                lowerAngle = 0f;
            }
            isFacingRight = facingRight;
        }

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

}
