using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleGem : MonoBehaviour
{
    [Header("Collectible Settings")]
    public AudioClip collectSound;   // assign in Inspector
    public int mysticEnergyValue = 5;
    public int collectibleCountValue = 1;

    private bool isCollected = false;

    private void Start()
    {
        // Ensure the gem’s collider is set as “isTrigger”
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        // check if player
        if (other.CompareTag("Player"))
        {
            isCollected = true;

            GameManager.instance.collectibleCount += collectibleCountValue;
            GameManager.instance.mysticEnergy += mysticEnergyValue;
            Debug.Log("You picked up: " + gameObject.name);

            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            Destroy(gameObject);
        }
    }
}
