/*Objectives Met in this Script:
 * Ternary operator in UpdateScore()
 * array of spawnpoints
 * score set/get
 * prefabs made for popcorn and feedback text
*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopcornManager : MonoBehaviour
{
    // references
    public GameObject popcornPrefab;
    //public Transform spawnPoint;
    public Transform[] spawnPoints;

    // UI
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public GameObject feedbackTextPrefab;
    public Canvas mainCanvas;

    // game timer
    public float totalTime = 120f;
    private float timeLeft;
    private bool gameOver = false;

    // spawn
    public float startSpawnInterval = 1.0f;
    public float minSpawnInterval = 0.2f;
    private float spawnTimer = 0f;
    private float currentSpawnInterval;

    // score
    private int score;
    public int Score
    {
        get { return score; }
        set
        {
            score = Mathf.Max(value, 0);
            if (scoreText) scoreText.text = "Score: " + score;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Score = 0;

        timeLeft = totalTime;
        if (gameOverText) gameOverText.gameObject.SetActive(false);

        currentSpawnInterval = startSpawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame();
            return;
        }

        if (timerText)
        {
            // formatting timer text
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // increase spawn rate over time
        // scale from startSpawnInterval down to minSpawnInterval
        float timeElapsed = totalTime - timeLeft;
        float t = timeElapsed / totalTime;
        currentSpawnInterval = Mathf.Lerp(startSpawnInterval, minSpawnInterval, t);

        // spawn popcorn
        spawnTimer += Time.deltaTime;
        if (spawnTimer > currentSpawnInterval)
        {
            SpawnPopcorn();
            spawnTimer = 0f;
        }
    }
    private void EndGame()
    {
        gameOver = true;
        if (gameOverText)
        {
            gameOverText.gameObject.SetActive(false);
            gameOverText.text = "GAME OVER!\n Final Score: " + score;
        }
    }

    private void SpawnPopcorn()
    {
        // random spawn point chosen from array spawnPoints[]
        if (spawnPoints == null || spawnPoints.Length == 0) Debug.Log("no spawn points found in array");
        
        foreach (Transform sp in spawnPoints)
        {
            Vector3 pos = sp.position;
            pos.x += Random.Range(-0.25f, 0.25f);
            pos.z += Random.Range(-0.25f, 0.25f);
            Instantiate(popcornPrefab, pos, Quaternion.identity);
        }    
        
        /*Transform chosenSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        // random offset around spawnpoint
        Vector3 pos = chosenSpawn.position;
        pos.x += Random.Range(-0.25f, 0.25f);
        pos.z += Random.Range(0.25f, 0.25f);

        Instantiate(popcornPrefab, pos, Quaternion.identity);*/
    }

    public void UpdateScore(int amount)
    {
        Score += amount;
        // TERNARY OPERATOR example
        bool isNegative = (amount < 0) ? true : false;
    }

    public void SpawnText(string message, Vector3 worldPosition)
    {
        if (feedbackTextPrefab && mainCanvas)
        {
            // 3d to screen position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            // instantiate feedback prefab
            GameObject textObj = Instantiate(feedbackTextPrefab, mainCanvas.transform);
            textObj.transform.position = screenPos;

            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            if (tmp) tmp.text = message;

            Destroy(textObj, 1f);
        }
    }
}
