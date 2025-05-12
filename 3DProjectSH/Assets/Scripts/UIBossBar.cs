// UIBossBar.cs  (attach to BossStaminaBar in Canvas)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBossBar : MonoBehaviour
{
    [Header("UI refs")]
    public Slider slider;
    public CanvasGroup group;
    public TextMeshProUGUI nameLabel; 

    void Awake()
    {
        group.alpha = 0f;
        group.interactable = group.blocksRaycasts = false;
    }

    public void Show(string bossName, int max)   // called on fight start
    {
        if (nameLabel) nameLabel.text = bossName;
        slider.maxValue = max;
        slider.value = max;
        group.alpha = 1f;
    }

    public void UpdateValue(int cur) => slider.value = cur;

    public void Hide() => group.alpha = 0f;
}
