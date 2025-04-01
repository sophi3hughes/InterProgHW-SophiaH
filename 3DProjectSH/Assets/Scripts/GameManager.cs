/*
 * Game manager that isn't really used
 * - stores the global variables for collectible count and energy count
 * - singleton
 */
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int collectibleCount = 0;
    public int mysticEnergy = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
