using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<GameObject> itemPrefabs;
    public float spawnInterval = 10f;
    public float spawnHeight = 5f; // Khoảng cách từ đỉnh màn hình

    void Start()
    {
        StartCoroutine(SpawnItemLoop());
    }

    IEnumerator SpawnItemLoop()
    {
        while (true)
        {
            SpawnRandomItemInCameraView();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandomItemInCameraView()
    {
        if (itemPrefabs == null || itemPrefabs.Count == 0) return;

        int index = Random.Range(0, itemPrefabs.Count);
        GameObject prefab = itemPrefabs[index];

        // Tính giới hạn camera theo chiều ngang
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        float minX = cam.transform.position.x - camWidth / 2f;
        float maxX = cam.transform.position.x + camWidth / 2f;

        // Ước lượng chiều rộng của item để tránh bị cắt khi spawn sát mép
        float itemWidth = 0.5f;
        Collider2D prefabCol = prefab.GetComponent<Collider2D>();
        if (prefabCol != null)
        {
            itemWidth = prefabCol.bounds.size.x;
        }

        float spawnX = Random.Range(minX + itemWidth / 2f, maxX - itemWidth / 2f);
        float spawnY = cam.transform.position.y + cam.orthographicSize + spawnHeight;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Add Rigidbody2D nếu chưa có
        if (item.GetComponent<Rigidbody2D>() == null)
        {
            var rb = item.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
        }

        // Add Collider2D nếu chưa có
        if (item.GetComponent<Collider2D>() == null)
        {
            item.AddComponent<CircleCollider2D>();
        }

        Debug.Log($"Spawned item: {prefab.name} at {spawnPos}");
    }
}
