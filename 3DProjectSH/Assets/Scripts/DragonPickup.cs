// script for dragon collider to pick up collectibles
using UnityEngine;

public class DragonPickup : MonoBehaviour
{

    public AudioClip collectSound;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Debug.Log("Dragon picked up: " + other.name);

            GameManager.instance.collectibleCount++;
            GameManager.instance.mysticEnergy += 5;
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

            Destroy(other.gameObject);
        }
    }
}
