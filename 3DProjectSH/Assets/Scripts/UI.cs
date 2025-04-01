using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI energyText;

    void Update()
    {
        if (GameManager.instance != null)
        {
            energyText.text = "Mystic Energy: " + GameManager.instance.mysticEnergy.ToString();
        }
        else
        {
            energyText.text = "Mystic Energy: 0";
        }
    }
}
