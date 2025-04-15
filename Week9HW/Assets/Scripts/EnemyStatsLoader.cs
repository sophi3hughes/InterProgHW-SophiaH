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

    public EnemyData[] enemyScriptableObjects;

    void Start()
    {
        LoadJsonAndApply();
    }

    void LoadJsonAndApply()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "EnemyStats.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("No EnemyStats.json found");
            return;
        }

        string jsonString = File.ReadAllText(path);
        EnemyStatsJson allStats = JsonUtility.FromJson<EnemyStatsJson>(jsonString);

        // match json
        foreach (var jsonEnemy in allStats.enemies)
        {
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

        Debug.Log("json updated enemy stats");
    }
}
