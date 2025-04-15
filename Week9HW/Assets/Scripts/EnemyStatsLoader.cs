using System;
using System.IO;
using UnityEngine;

public class EnemyStatsLoader : MonoBehaviour
{
    [Serializable]
    public class EnemyStatsJson
    {
        public EnemyJson[] enemies;
    }

    [Serializable]
    public class EnemyJson
    {
        public string enemyName;
        public float health;
        public float attackPower;
        public float speed;
    }

    // Reference your ScriptableObjects in the inspector
    public EnemyData[] enemyScriptableObjects;

    void Start()
    {
        LoadJsonAndApply();
    }

    void LoadJsonAndApply()
    {
        // If using StreamingAssets:
        string path = Path.Combine(Application.streamingAssetsPath, "EnemyStats.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("No EnemyStats.json found at " + path);
            return;
        }

        string jsonString = File.ReadAllText(path);
        EnemyStatsJson allStats = JsonUtility.FromJson<EnemyStatsJson>(jsonString);

        // Match each JSON entry to the corresponding ScriptableObject by name (or other logic)
        foreach (var jsonEnemy in allStats.enemies)
        {
            // Find matching ScriptableObject
            foreach (var so in enemyScriptableObjects)
            {
                if (so.enemyName == jsonEnemy.enemyName)
                {
                    so.health = jsonEnemy.health;
                    so.attackPower = jsonEnemy.attackPower;
                    so.speed = jsonEnemy.speed;
                }
            }
        }

        Debug.Log("Enemy stats updated from JSON!");
    }
}
