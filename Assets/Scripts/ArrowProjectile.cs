using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public bool isPlayer1; // Biến để xác định Player nào bắn mũi tên
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            collision.gameObject.GetComponent<PlayerController>().isPlayer1 != isPlayer1)
        {
            GameManager.Instance.PlayerTakeDamage(!isPlayer1, GameManager.damageType.BASIC);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}