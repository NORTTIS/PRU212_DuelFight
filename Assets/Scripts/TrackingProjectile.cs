using UnityEngine;

public class TrackingProjectile : MonoBehaviour
{
    public float speed = 8f;
    public Transform target;
    public bool isPlayer1;
    public Transform shooter;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == shooter) return; // Không va chạm với người bắn
        if (other.CompareTag("Player") && other.GetComponent<PlayerController>().isPlayer1 != isPlayer1)
        {
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
                stats.TakeDamage(GameManager.Instance.player1.baseAttack, "TrackingProjectile");
            Destroy(gameObject);
        }
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}