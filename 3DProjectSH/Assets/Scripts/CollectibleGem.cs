/*
 * For incrementing energy count, collectible count, playing sound, and destroying the gameobject
 * - the gemstone collectibles are prefab
 * 
 */

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleGem : MonoBehaviour
{
    [Header("Collectible Settings")]
    public AudioClip collectSound;
    public int mysticEnergyValue = 5;
    public int collectibleCountValue = 1;

    private bool isCollected = false;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // do not add to count if its already collected
        if (isCollected) return;

        // check if player
        if (other.CompareTag("Player") || other.CompareTag("Dragon"))
        {
            isCollected = true;

            GameManager.instance.collectibleCount += collectibleCountValue;
            GameManager.instance.mysticEnergy += mysticEnergyValue;
            Debug.Log("You picked up: " + gameObject.name);

            if (collectSound != null)
            {
                //AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position, 5f);

                GameObject tmp = new GameObject("GemPickupSFX");
                tmp.transform.position = Camera.main.transform.position;

                AudioSource src = tmp.AddComponent<AudioSource>();
                src.clip = collectSound;
                src.volume = 1f;
                src.spatialBlend = 0f;
                src.dopplerLevel = 0f;
                src.Play();

                Destroy(tmp, collectSound.length);

            }
            Destroy(gameObject);
        }
    }
}
