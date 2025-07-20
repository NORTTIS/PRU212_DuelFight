using UnityEngine;

public class StarPointController : MonoBehaviour
{
    [SerializeField] private int manaAmount = 4;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.AddMana(manaAmount);
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Pet"))
        {
            PetController pet = other.GetComponent<PetController>();
            if (pet != null)
            {
                pet.player.GetComponent<PlayerStats>().AddMana(manaAmount);
                Destroy(gameObject);
            }
        }
    }
}
