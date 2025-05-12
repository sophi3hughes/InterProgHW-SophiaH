using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public float activeTime = 1f;
    public float cooldownTime = 4f;
    public KeyCode shieldKey = KeyCode.Q;

    float cooldownTimer;
    float activeTimer;
    public bool IsShielding => activeTimer > 0f;  

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(shieldKey) && cooldownTimer <= 0f)
        {
            activeTimer = activeTime;
            cooldownTimer = cooldownTime;
        }

        if (activeTimer > 0f)
            activeTimer -= Time.deltaTime;
    }
}
