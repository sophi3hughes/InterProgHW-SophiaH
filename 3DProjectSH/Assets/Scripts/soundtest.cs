using UnityEngine;

public class soundtest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.M)) 
        {
            SoundSystem.instance.PlaySound("background");
        }

    }
}
