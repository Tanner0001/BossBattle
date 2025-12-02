using _Project.Code.Core.StateMachine;
using _Project.Code.Gameplay.Combat;
using _Project.Code.Gameplay.Combat.GunSystem;
using _Project.Code.Gameplay.Enemies.AI.States;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Code.Gameplay.Enemies.AI
{
    [RequireComponent(typeof(NavMeshAgent), typeof(GunBase), typeof(Hitbox))]
    public class DroneAIController : MonoBehaviour
    {
        // Dependencies
        private NavMeshAgent _agent;
        private GunBase _gun;
        private Hitbox _hitbox;
        private FiniteStateMachine<BaseState> _stateMachine;

        // Configuration
        [Header("Sight Config")]
        [SerializeField] private float viewRadius = 15f;
        [SerializeField, Range(0, 360)] private float viewAngle = 90f;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private LayerMask obstacleMask;
        
        [Header("AI Config")]
        [SerializeField] private Transform playerTransform; // TEMP: Should use a targeting system
        [SerializeField] private bool isWarden = false; // Flag to determine if this is a warden

        [Header("Patrol Config")]
        [SerializeField] private PatrolRoute patrolRoute;
        
        [Header("Retreat Config")]
        [SerializeField] private Transform retreatPoint;
        [SerializeField] private float retreatHealthPercentage = 0.75f;


        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _gun = GetComponent<GunBase>();
            _hitbox = GetComponent<Hitbox>();

            // TODO: Find player transform properly via a system
            if (playerTransform == null)
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            var patrolState = new PatrolState(this);
            var combatState = new CombatState(this);
            
            _stateMachine = new FiniteStateMachine<BaseState>(patrolState);
            _stateMachine.AddState(combatState);
            
            if (isWarden)
            {
                var retreatState = new RetreatState(this);
                _stateMachine.AddState(retreatState);
            }
        }

        private void Update()
        {
            _stateMachine?.Update();
            HandleTransitions();
        }

        private void HandleTransitions()
        {
            if (_stateMachine.CurrentState is PatrolState)
            {
                if (CanSeePlayer())
                {
                    _stateMachine.TransitionTo<CombatState>();
                }
            }
            else if (_stateMachine.CurrentState is CombatState)
            {
                if (isWarden)
                {
                    float healthPercent = _hitbox.CurrentHealth / _hitbox.MaxHealth;
                    if (healthPercent <= retreatHealthPercentage)
                    {
                        _stateMachine.TransitionTo<RetreatState>();
                        return; // Exit to avoid other transitions
                    }
                }
                
                if (!CanSeePlayer())
                {
                    _stateMachine.TransitionTo<PatrolState>();
                }
            }
        }
        
        public bool CanSeePlayer()
        {
            if (playerTransform == null) return false;

            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

            foreach (var target in targetsInViewRadius)
            {
                Transform targetTransform = target.transform;
                Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float distToTarget = Vector3.Distance(transform.position, targetTransform.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                    {
                        return true; // Player is visible
                    }
                }
            }
            return false;
        }

        private void FixedUpdate()
        {
            _stateMachine?.FixedUpdate();
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

            if (CanSeePlayer())
            {
                Gizmos.color = Color.green;
                if(playerTransform != null)
                    Gizmos.DrawLine(transform.position, playerTransform.position);
            }
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
        
        // Public properties/methods for states to use
        public NavMeshAgent Agent => _agent;
        public GunBase Gun => _gun;
        public Hitbox Hitbox => _hitbox;
        public Transform PlayerTransform => playerTransform;
        public PatrolRoute GetPatrolRoute() => patrolRoute;
        public Transform GetRetreatPoint() => retreatPoint;
    }
}
