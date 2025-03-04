using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    public int score = 0;
    public int waveNumber = 0;

    public GameObject[] enemyPrefabs;

    public float spawnInterval = 10f;
    private float timer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        InvokeRepeating("SpawnEnemies", 0f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemies();
            timer = 0f;
        }
    }

    public void SpawnEnemies()
    {
        waveNumber++;
        int enemyCount = waveNumber + 1;

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        Vector3 randomPos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyToSpawn = enemyPrefabs[randomIndex];

        Instantiate(enemyToSpawn, randomPos, Quaternion.identity);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score " + score);
    }
}
