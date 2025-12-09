using UnityEngine;

namespace BossBattle.Gameplay.Enemies.Data
{
    [CreateAssetMenu(fileName = "WardenData", menuName = "BossBattle/Warden Data")]
    public class WardenData : ScriptableObject
    {
        [Header("Stats")]
        public float MaxHealth = 1000f;
        public float MovementSpeed = 5f;
        public float TurnSpeed = 120f;

        [Header("Phase 1")]
        public float Phase1HealthThreshold = 500f;

        [Header("Combat")]
        public float AttackRate = 1f;
        public float AttackDamage = 10f;
        public float AttackRange = 20f;

        [Header("Transition")]
        public int DronesToSpawn = 5;

        [Header("Phase 2")]
        public float Phase2HealthThreshold = 250f;

        [Header("Phase 3")]
        public float WeakSpotMultiplier = 2.0f;
        public float Phase3NewAbilityCooldown = 6f;
        public int Phase3MinionsToSpawn = 3;
        public float Phase3HealthThreshold = 50f;
    }
}
