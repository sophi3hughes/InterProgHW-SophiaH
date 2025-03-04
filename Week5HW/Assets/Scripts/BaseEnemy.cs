using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{

    [SerializeField] 
    protected float health = 100f;
    [SerializeField] 
    protected float moveSpeed = 2f;


    protected virtual void Start()
    {
        
    }
    
    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Vector3 direction = (GameObject.FindWithTag("Player").transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        GameManager.Instance.AddScore(10);
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>()?.TakeDamage(10f);
        }
    }

}
