using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(DragonAI))]
public class DragonCombat : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Animator anim;
    public NavMeshAgent agent;
    public DragonAI patrol;
    public DragonMount mount;
    public UIBossBar bossBar;

    [Header("Settings")]
    public float engageRadius = 25f;
    public int maxStamina = 7;

    [Header("Attack settings")]
    public float attackInterval = 9f;
    public int damageEnergy = 5;
    public string[] attackTriggers = { "Bite", "Claw", "Flame" };

    public float attackRange = 4f;
    public float approachSlack = 0.3f;
    public float attackFreeze = 3f;

    int currentStamina;
    bool inCombat;
    float attackTimer;
    float freezeTimer;
    PlayerShield playerShield;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        playerShield = player.GetComponent<PlayerShield>();
        currentStamina = maxStamina;
        attackTimer = attackInterval;

        agent.stoppingDistance = attackRange - approachSlack;
    }

    void Update()
    {
        if (IsDefeated) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!inCombat && dist < engageRadius)
            EnterCombat();
        else if (inCombat && dist > engageRadius * 1.5f)
            ExitCombat();

        if (!inCombat) return;

        freezeTimer -= Time.deltaTime;

        if (freezeTimer <= 0f)
        {
            if (dist > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                if (!agent.isStopped)
                    agent.ResetPath();

                agent.isStopped = true;
                transform.LookAt(player);
            }
        }
        else
        {
            if (!agent.isStopped)
                agent.ResetPath();

            agent.isStopped = true;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attackTimer = attackInterval;
            freezeTimer = attackFreeze;

            string trig = attackTriggers[Random.Range(0, attackTriggers.Length)];
            anim.SetTrigger(trig);                             // play animation

            if (dist <= attackRange &&
                (playerShield == null || !playerShield.IsShielding))
            {
                GameManager.instance.mysticEnergy =
                    Mathf.Max(0, GameManager.instance.mysticEnergy - damageEnergy);
            }
        }
    }

    void EnterCombat()
    {
        inCombat = true;
        patrol.enabled = false;
        agent.isStopped = false;

        bossBar.Show(name, maxStamina);
        bossBar.UpdateValue(currentStamina);

        anim.SetTrigger("Roar");
    }

    void ExitCombat()
    {
        inCombat = false;
        patrol.enabled = true;
        agent.ResetPath();

        bossBar.Hide();

        attackTimer = attackInterval;
        freezeTimer = 0f;
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

    [ContextMenu("Debug - TakeHit")]
    void DebugTakeHit() => TakeHit(1);
}
