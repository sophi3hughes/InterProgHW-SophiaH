using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;  // Drag a matching ScriptableObject here
    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Basic chase:
        float step = enemyData.speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);
        transform.LookAt(player.position);
    }

    // Optional: If the bullet hits us, reduce health, etc.
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision");
        if (collision.gameObject.CompareTag("Bullet"))
        {
            enemyData.health -= 5f; // example bullet damage
            if (enemyData.health <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}