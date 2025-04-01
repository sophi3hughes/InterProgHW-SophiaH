using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToHome : MonoBehaviour
{
    [SerializeField] private string startSceneName = "StartScene";

    void Update()
    {
        // Check if the player presses "M"
        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene(startSceneName);
        }
    }
}
