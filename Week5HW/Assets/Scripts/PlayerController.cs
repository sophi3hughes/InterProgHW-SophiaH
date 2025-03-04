using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats stats = new PlayerStats(100f, 7f);

    // shooting
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public LayerMask groundLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0, v).normalized;

        transform.Translate(direction * stats.speed * Time.deltaTime);

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }

        if(stats.health <= 0)
        {
            Debug.Log("Player has died.");
        }
    }

    private void Shoot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
        {
            Vector3 targetPoint = hit.point;
            targetPoint.y = firePoint.position.y;
            Vector3 direction = (targetPoint - firePoint.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            Instantiate(bulletPrefab, firePoint.position, lookRotation);
        }
    }

    public void TakeDamage(float amount)
    {
        stats.health -= amount;
        Debug.Log("Player health: " + stats.health);

        if (stats.health <= 0)
        {
            Debug.Log("Player died!");
        }
    }
}
