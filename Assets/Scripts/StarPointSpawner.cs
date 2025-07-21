using System.Collections;
using UnityEngine;

public class StarPointSpawner : MonoBehaviour
{
    public GameObject starPointPrefab;
    public float spawnInterval = 5f;

    // Điều chỉnh 4 cạnh
    public float leftBound = -3f;
    public float rightBound = 3f;
    public float topBound = 2f;
    public float bottomBound = -2f;

    // Bán kính để kiểm tra va chạm
    public float checkRadius = 0.2f;
    public LayerMask obstacleLayers; // Layer của các object cần tránh (ví dụ: Ground, Player)

    private void Start()
    {
        StartCoroutine(SpawnStarPoints());
    }

    IEnumerator SpawnStarPoints()
    {
        while (true)
        {
            Vector3 randomPos = GetSafeRandomPosition();
            Instantiate(starPointPrefab, randomPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Vector3 GetSafeRandomPosition()
    {
        Vector3 randomPos;
        int maxAttempts = 30; // Giới hạn số lần thử để tránh vòng lặp vô hạn
        int attempts = 0;

        do
        {
            randomPos = transform.position + new Vector3(
                Random.Range(leftBound, rightBound),
                Random.Range(bottomBound, topBound),
                0f
            );

            // Kiểm tra va chạm
            bool isColliding = Physics2D.OverlapCircle(randomPos, checkRadius, obstacleLayers) != null;
            if (!isColliding) break;

            attempts++;
        } while (attempts < maxAttempts);

        return randomPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = transform.position + new Vector3(
            (leftBound + rightBound) / 2f,
            (bottomBound + topBound) / 2f,
            0f
        );

        Vector3 size = new Vector3(
            Mathf.Abs(rightBound - leftBound),
            Mathf.Abs(topBound - bottomBound),
            0.1f
        );

        Gizmos.DrawWireCube(center, size);
    }
}