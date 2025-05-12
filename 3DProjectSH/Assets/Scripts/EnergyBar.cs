using UnityEngine;
using UnityEngine.UI;  
using TMPro;

public class UIEnergyBar : MonoBehaviour
{
    [Header("UI References")]
    public Image energyFillImage;       // the foreground image, set to Filled/Horizontal
    public TextMeshProUGUI energyLabel; // optional label

    [Header("Game Settings")]
    public int maxEnergy = 100;         // how many units full bar represents

    void Update()
    {
        int cur = GameManager.instance != null
                  ? GameManager.instance.mysticEnergy
                  : 0;

        // clamp
        cur = Mathf.Clamp(cur, 0, maxEnergy);

        energyFillImage.fillAmount = cur / (float)maxEnergy;

        // 2) optional text
        if (energyLabel)
            energyLabel.text = $"{cur}/{maxEnergy}";
    }
}
