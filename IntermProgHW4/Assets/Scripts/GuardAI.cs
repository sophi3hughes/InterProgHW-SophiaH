using UnityEngine;
using UnityEngine.AI;

namespace StealthGame
{
    public class GuardAI : MonoBehaviour
    {
        [Header("Waypoints")]
        public Transform[] waypoints;
        private int currentWaypointIndex;

        [Header("Speeds")]
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4f;

        [Header("Detection/Chase")]
        public float detectionRange = 10f;
        public float chaseDuration = 3f;
        private float chaseTimer;

        private NavMeshAgent agent;
        private Transform playerTransform;

        private Animator animator;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerTransform = player.transform;
        }

        void Update()
        {
            if (playerTransform == null) return;

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
            {
                animator.SetBool("IsChasing", false);

                agent.speed = patrolSpeed;
                Patrol();
            }
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
}