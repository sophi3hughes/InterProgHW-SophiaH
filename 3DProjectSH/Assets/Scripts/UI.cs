// connects to UI elements
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI energyText;

    void Update()
    {
        if (GameManager.instance != null)
        {
            energyText.text = GameManager.instance.mysticEnergy.ToString();
        }
        else
        {
            energyText.text = "Mystic Energy: 0";
        }
    }
}
