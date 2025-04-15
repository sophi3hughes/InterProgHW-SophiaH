using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f; // destroy after 2 seconds

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward (relative to its own transform)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If it hits something, you could apply damage or destroy the bullet
        Destroy(gameObject);
    }
}
