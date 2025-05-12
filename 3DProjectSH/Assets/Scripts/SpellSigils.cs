using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellSigils : MonoBehaviour
{
    [Header("Sigil buttons")]
    public Button[] sigils;

    [Header("Spell settings")]
    public float timeLimit = 5f;
    public int energyCost = 10;
    public Vector2 padding = new Vector2(50, 50);

    PlayerMove playerMove;
    RectTransform panelRect;
    int nextIndex;
    float timer;
    bool inSpell;

    void Awake()
    {
        // hide panel
        gameObject.SetActive(false);

        panelRect = GetComponent<RectTransform>();
        playerMove = FindAnyObjectByType<PlayerMove>();  // works while inactive

        foreach (var btn in sigils)
            btn.onClick.AddListener(OnSigilClick);
    }

    void Update()
    {
        if (!inSpell) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
            EndSpell(false);
    }

    //public API
    public void BeginSpell()
    {
        if (inSpell) return; // already casting

        if (GameManager.instance.mysticEnergy < energyCost)
        {
            Debug.Log("Not enough energy to cast");
            return;
        }

        // spend energy immediately
        GameManager.instance.mysticEnergy -= energyCost;

        // randomize sigil position
        foreach (var btn in sigils)
            RandomisePos(btn.GetComponent<RectTransform>());

        nextIndex = 0;
        timer = timeLimit;
        inSpell = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerMove.enabled = false;

        gameObject.SetActive(true);
    }

    void RandomisePos(RectTransform rt)
    {
        float w = panelRect.rect.width - 2 * padding.x;
        float h = panelRect.rect.height - 2 * padding.y;

        float x = Random.Range(-w * 0.5f, w * 0.5f);
        float y = Random.Range(-h * 0.5f, h * 0.5f);

        rt.anchoredPosition = new Vector2(x, y);
    }

    void OnSigilClick()
    {
        if (!inSpell) return;

        GameObject clicked = EventSystem.current.currentSelectedGameObject;
        int idx = System.Array.FindIndex(sigils, b => b.gameObject == clicked);

        if (idx == nextIndex)
        {
            nextIndex++;
            if (nextIndex >= sigils.Length)
                EndSpell(true);
        }
        else
        {
            EndSpell(false);
        }
    }

    //end spell
    void EndSpell(bool success)
    {
        inSpell = false;
        gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerMove.enabled = true;

        if (success)
        {
            DragonCombat target = FindTarget();
            if (target != null)
            {
                Debug.Log($"Spell hit {target.name}");
                target.TakeHit(1);
            }
            else
            {
                Debug.Log("Spell cast, but no dragon in range");
            }
        }
        else
        {
            Debug.Log("Spell failed");
            GameManager.instance.mysticEnergy += energyCost / 2;
        }
    }

    DragonCombat FindTarget()
    {
        const float maxSpellRange = 50f;
        DragonCombat[] all = FindObjectsByType<DragonCombat>(FindObjectsSortMode.None);

        DragonCombat best = null;
        float bestDist = float.MaxValue;

        foreach (var dc in all)
        {
            if (!dc.IsInCombat) continue;

            float d = Vector3.Distance(playerMove.transform.position, dc.transform.position);
            if (d > maxSpellRange) continue;

            if (d < bestDist)
            {
                bestDist = d;
                best = dc;
            }
        }
        return best;
    }
}