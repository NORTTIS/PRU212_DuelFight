using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public bool isPlayer1; // Biến để xác định Player nào bắn mũi tên
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.gameObject.GetComponent<PlayerController>().isPlayer1 != isPlayer1)
        {
            GameManager.Instance.PlayerTakeDamageFromOther(!isPlayer1);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}