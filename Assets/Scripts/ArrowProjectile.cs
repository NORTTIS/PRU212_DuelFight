using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public bool isPlayer1; // Variable to determine which player shot the arrow
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Shield"))
        {
            var shieldOwner = other.gameObject.GetComponent<ShieldOwner>();
            if (shieldOwner != null && shieldOwner.isPlayer1 != isPlayer1)
            {
                // Only destroy if shield belongs to opponent
                Destroy(gameObject);
                return;
            }
        }
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