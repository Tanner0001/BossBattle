using UnityEngine;
using UnityEngine.AI;
using BossBattle.Gameplay.Enemies.Data;
using _Project.Code.Gameplay.Combat; // Added for Hitbox
using System; // Added for Action event
using _Project.Code.Core.Events; // Added for EventBus
using _Project.Code.Gameplay.VFX; // Added for MaterialController
using TMPro; // For the vulnerability text

namespace BossBattle.Gameplay.Enemies.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Hitbox))]
    public class WardenBossController : MonoBehaviour
    {
        [Header("Data")]
        public WardenData WardenData;

        [Header("State Machine")]
        private WardenBaseState _currentState;
        [HideInInspector] public int nextPhaseToEnter; // Used by TransitionState to know where to go
        public WardenPhase1State Phase1State = new WardenPhase1State();
        public WardenTransitionState TransitionState = new WardenTransitionState();
        public WardenPhase2State Phase2State = new WardenPhase2State();
        public WardenPhase3State Phase3State = new WardenPhase3State();

        [Header("Warden Properties")]
        public Transform Player;
        public float CurrentHealth => _hitbox != null ? _hitbox.CurrentHealth : 0f;
        public NavMeshAgent Agent;

        public event Action OnHealthChanged; // Event for UI and other systems to subscribe to

        [Header("Phase 1 Properties")]
        public Transform Phase1StartPoint;
        public GameObject SingleGun;

        [Header("Transition Properties")]
        public Transform RetreatPoint;
        public GameObject DronePrefab;
        public Transform[] DroneSpawnPoints;

        [Header("Phase 2 Properties")]
        public Transform Phase2StartPoint;
        public GameObject LeftGun;
        public GameObject RightGun;
        [Tooltip("A dedicated stopping distance for Phase 2 to keep the Warden further away.")]
        public float Phase2StoppingDistance = 15f;
        [Tooltip("The maximum range the Warden will pursue the player in Phase 2 before disengaging.")]
        public float Phase2EngagementRange = 30f;

        [Header("Phase 3 Properties")]
        public Transform Phase3StartPoint;
        public Transform WeakSpotTransform;
        public Renderer WeakSpotRenderer;
        public TextMeshProUGUI VulnerabilityTextElement;

        [Header("Sight Config")]
        public float viewRadius = 20f;
        [Range(0, 360)] public float viewAngle = 90f;
        public LayerMask playerMask;
        public LayerMask obstacleMask;

        [Header("AI Config")]
        public float CombatStoppingDistance = 10f;
        [Tooltip("The transform representing the visual root of the warden's model. Used for rotation and view origin. If not set, defaults to the main transform.")]
        [SerializeField] private Transform modelRoot;

        private Hitbox _hitbox; // Reference to the attached Hitbox component

        // Public properties for states to access core components
        public Hitbox Hitbox => _hitbox;
        public MaterialController MaterialController { get; private set; }

        void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            MaterialController = GetComponent<MaterialController>();
            _hitbox = GetComponent<Hitbox>(); // Get the Hitbox component
            _hitbox.Initialize(WardenData.MaxHealth); // Initialize Hitbox with WardenData's MaxHealth
            _hitbox.OnHealthChanged += () => OnHealthChanged?.Invoke(); // Subscribe to hitbox health changes

            if (modelRoot == null)
                modelRoot = transform;
            
            EventBus.Instance.Subscribe<WardenDiedEvent>(this, OnWardenDied);
        }

        private void OnDestroy()
        {
            EventBus.Instance.Unsubscribe<WardenDiedEvent>(this);
        }

        void Start()
        {
            // Set player transform
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            Agent.speed = WardenData.MovementSpeed;
            Agent.angularSpeed = WardenData.TurnSpeed;
            Agent.stoppingDistance = CombatStoppingDistance; // Set initial stopping distance

            // Do NOT transition to state here. BossTrigger will call StartCombat().
        }

        void Update()
        {
            if (_currentState != null)
            {
                _currentState.UpdateState(this);
            }
        }

        public void TransitionToState(WardenBaseState state)
        {
            _currentState = state;
            _currentState.EnterState(this);

            // Publish UI event based on the new state
            if (state is WardenPhase1State)
                EventBus.Instance.Publish(new PhaseChangedEvent { PhaseIndex = 1, DisplayText = "Phase 1" });
            else if (state is WardenTransitionState)
                EventBus.Instance.Publish(new PhaseChangedEvent { PhaseIndex = 0, DisplayText = "Warden Retreating" });
            else if (state is WardenPhase2State)
                EventBus.Instance.Publish(new PhaseChangedEvent { PhaseIndex = 2, DisplayText = "Phase 2" });
            else if (state is WardenPhase3State)
                EventBus.Instance.Publish(new PhaseChangedEvent { PhaseIndex = 3, DisplayText = "Phase 3" });
        }

        public void StartCombat()
        {
            TransitionToState(Phase1State);
            Agent.isStopped = false; // Allow movement when combat starts
        }
        
        private void OnWardenDied(WardenDiedEvent e)
        {
            // Stop all logic
            enabled = false;
            Agent.isStopped = true;
            _currentState = null;
            
            EventBus.Instance.Publish(new GameWonEvent());
        }

        public Transform ModelRoot => modelRoot; // Public property for ModelRoot

        public Vector3 GetPlayerNavMeshPosition()
        {
            if (Player == null) return Vector3.zero; // Or some default safe position

            if (NavMesh.SamplePosition(Player.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return Player.position; // Fallback if no NavMesh found nearby, though this should be rare.
        }
    }
}
