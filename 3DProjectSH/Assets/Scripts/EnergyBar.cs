using UnityEngine;
using UnityEngine.UI;  
using TMPro;

public class UIEnergyBar : MonoBehaviour
{
    [Header("UI References")]
    public Image energyFillImage;
    public TextMeshProUGUI energyLabel;

    [Header("Game Settings")]
    public int maxEnergy = 100;

    void Update()
    {
        int cur = GameManager.instance != null
                  ? GameManager.instance.mysticEnergy
                  : 0;

        // clamp
        cur = Mathf.Clamp(cur, 0, maxEnergy);

        energyFillImage.fillAmount = cur / (float)maxEnergy;

        if (energyLabel)
            energyLabel.text = $"{cur}/{maxEnergy}";
    }
}
