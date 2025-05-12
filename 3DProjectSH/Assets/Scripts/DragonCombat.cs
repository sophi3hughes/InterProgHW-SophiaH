using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(DragonAI))]
public class DragonCombat : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;                // assign at runtime
    public Animator anim;                   // roar / hit etc.
    public NavMeshAgent agent;              // from DragonAI
    public DragonAI patrol;                 // the existing patrol script
    public DragonMount mount;               // to block mounting
    public UIBossBar bossBar;

    [Header("Settings")]
    public float engageRadius = 25f;
    public int maxStamina = 7;

    int currentStamina;
    bool inCombat = false;

    void Start()
    {
        currentStamina = maxStamina;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (IsDefeated) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!inCombat && dist < engageRadius)
            EnterCombat();
        else if (inCombat && dist > engageRadius * 1.5f)   // hysteresis
            ExitCombat();

        if (inCombat)
        {
            // face & walk at player (simple placeholder)
            agent.SetDestination(player.position);
        }
    }

    void EnterCombat()
    {
        inCombat = true;
        patrol.enabled = false;
        agent.isStopped = false;

        bossBar.Show(name, maxStamina);
        anim.SetTrigger("Roar");
    }

    void ExitCombat()
    {
        inCombat = false;
        patrol.enabled = true;
        agent.ResetPath();

        bossBar.Hide();
    }

    public void TakeHit(int dmg = 1)
    {
        currentStamina = Mathf.Max(0, currentStamina - dmg);
        bossBar.UpdateValue(currentStamina);
        anim.SetTrigger("Hit");  

        if (currentStamina == 0)
            Defeated();
    }

    void Defeated()
    {
        inCombat = false;
        patrol.enabled = true;
        agent.isStopped = true;
        agent.ResetPath();

        bossBar.Hide();
        mount.MarkTamed();
        anim.SetTrigger("Defeated");
    }

    public bool IsInCombat => inCombat;
    public bool IsDefeated => currentStamina == 0;

    [ContextMenu("Test TakeHit (lose 1 stamina)")]
    private void DebugTakeHit()
    {
        TakeHit(1);
    }
}
