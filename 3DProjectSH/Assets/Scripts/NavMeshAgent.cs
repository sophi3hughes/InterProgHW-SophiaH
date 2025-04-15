/*using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgent : MonoBehaviour
{
    NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                agent.destination = hit.point;
            }
        }
    }
}*/

using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    private int currentWaypointIndex;

    [Header("Speeds")]
    public float patrolSpeed = 2f;
    //public float chaseSpeed = 4f;

    [Header("Detection/Chase")]
    //public float detectionRange = 10f;
    //public float chaseDuration = 3f;
    //private float chaseTimer;

    private NavMeshAgent agent;

    //private Transform playerTransform;

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //if (player) playerTransform = player.transform;
    }

    void Update()
    {
        /*if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            chaseTimer = chaseDuration;
        }

        if (chaseTimer > 0)
        {
            chaseTimer -= Time.deltaTime;
            animator.SetBool("IsChasing", true);

            agent.speed = chaseSpeed;
            agent.SetDestination(playerTransform.position);
        }
        else
        {*/
        //animator.SetBool("IsChasing", false);
            float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
            agent.speed = patrolSpeed;
            Patrol();
        //}
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentWaypointIndex].position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}