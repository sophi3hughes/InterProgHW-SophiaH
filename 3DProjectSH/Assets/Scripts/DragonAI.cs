using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DragonAI : MonoBehaviour
{
    [Header("Patrol Waypoints")]
    public Transform[] waypoints;
    private int _current = 0;

    NavMeshAgent _agent;
    Animator _anim;
    DragonMount _mount;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _mount = GetComponent<DragonMount>();
    }

    void Start()
    {
        if (waypoints.Length > 0)
            _agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        // if someone is riding, pause all AI
        if (_mount.isMounted)
        {
            if (_agent.enabled && _agent.isOnNavMesh)
                _agent.isStopped = true;

            _anim.SetFloat("Speed", 0f);
            return;
        }

        // otherwise let the agent patrol
        if (_agent.enabled && _agent.isOnNavMesh)
            _agent.isStopped = false;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _current = (_current + 1) % waypoints.Length;
            _agent.SetDestination(waypoints[_current].position);
        }

        float rawSpeed = _agent.velocity.magnitude;
        float normalized = rawSpeed / _agent.speed;
        //_anim.SetFloat("Speed", _agent.velocity.magnitude);
        _anim.SetFloat("Speed", Mathf.Clamp01(normalized));

    }
}
