using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData; 
    private Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemyData.speed;
    }

    void Update()
    {
        if (player == null) return;

        // Basic chase:
        //float step = enemyData.speed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, player.position, step);
        //transform.LookAt(player.position);
        agent.SetDestination(player.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision");
        if (collision.gameObject.CompareTag("Bullet"))
        {
            enemyData.health -= 5f;
            if (enemyData.health <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}