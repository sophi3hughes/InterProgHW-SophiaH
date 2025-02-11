/*Objectives Met in this Script:
 * Ternary operator in UpdateScore()
 * array of spawnpoints
 * score set/get
 * prefabs made for popcorn and feedback text
*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopcornManager : MonoBehaviour
{
    // references
    public GameObject popcornPrefab;
    public Transform[] spawnPoints;

    // UI
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public GameObject feedbackTextPrefab;
    public TextMeshProUGUI countersText;
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

    // popcorn speed
    public float popcornSpeedMultiplier = 1f;

    // score
    private int score;
    public int Score
    {
        get { return score; }
        set
        {
            score = Mathf.Max(value, 0);
            if (scoreText) scoreText.text = "Score:" + score;
        }
    }

    // count for each popcorn type popped
    public int kernelCount;
    public int wellPoppedCount;
    public int perfectCount;
    public int burntCount;
    public int rainbowCount;

    // popcorn combo 
    private int comboCount = 0;
    private bool lastWasComboEligible = false;

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
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "GAME OVER!\n Final Score: " + score
                + "\n\nCounters:\n"
                + $"Kernel: {kernelCount}\n"
                + $"Well: {wellPoppedCount}\n"
                + $"Perfect: {perfectCount}\n"
                + $"Burnt: {burntCount}\n"
                + $"Rainbow: {rainbowCount}\n";
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

            GameObject newPop = Instantiate(popcornPrefab, pos, Quaternion.identity);

            // rainbow rng
            bool isRainbow = (Random.value < 0.05f); // 5% for rainbow popcorn
            Popcorn popScript = newPop.GetComponent<Popcorn>();
            // NEW TERNARY OPERATOR!
            popScript.variant = isRainbow ? Popcorn.PopcornVariant.Rainbow : Popcorn.PopcornVariant.Normal;

            //Instantiate(popcornPrefab, pos, Quaternion.identity);
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

    // popcorn type count tracker
    public void AddPopcornCount(Popcorn.PopcornStates state, Popcorn.PopcornVariant variant)
    {
        if (variant == Popcorn.PopcornVariant.Rainbow && state != Popcorn.PopcornStates.Burnt)
        {
            rainbowCount++;
        }
        else
        {
            switch (state)
            {
                case Popcorn.PopcornStates.Kernel:
                    kernelCount++;
                    break;
                case Popcorn.PopcornStates.WellPopped:
                    wellPoppedCount++;
                    break;
                case Popcorn.PopcornStates.PerfectlyPopped:
                    perfectCount++;
                    break;
                case Popcorn.PopcornStates.Burnt:
                    burntCount++;
                    break;
            }
        }
        UpdateCounters();
    }
    public void SlowPopcornStates(float duration)
    {
        StartCoroutine(SlowPopcornCoroutine(duration));
    }
    private IEnumerator SlowPopcornCoroutine(float duration)
    {
        popcornSpeedMultiplier = 0.5f; // half speed
        yield return new WaitForSeconds(duration);
        popcornSpeedMultiplier = 1f; // back to normal
    }

    // combos
    public void CheckCombo(Popcorn.PopcornStates currentState, Popcorn.PopcornVariant variant, ref int scoreChange)
    {
        bool isEligible = false;

        // if rainbow & not burnt, or if perfectly popped
        if ((variant == Popcorn.PopcornVariant.Rainbow && currentState != Popcorn.PopcornStates.Burnt)
            || currentState == Popcorn.PopcornStates.PerfectlyPopped)
        {
            isEligible = true;
        }

        if (isEligible && lastWasComboEligible)
        {
            comboCount++;
            // +5 points
            scoreChange += 5;
        }
        else if (isEligible)
        {
            // start a new combo
            comboCount = 1;
        }
        else
        {
            comboCount = 0;
        }

        // tracking last popcorn type
        lastWasComboEligible = isEligible;
    }

    public void UpdateCounters()
    {
        if (!countersText) return;
        countersText.text = 
            "Kernel: " + kernelCount + "\n" +
            "Good: " + wellPoppedCount + "\n" +
            "Perfect: " + perfectCount + "\n" +
            "Burnt: " + burntCount + "\n" +
            "Rainbow: " + rainbowCount + "\n";
    }
}