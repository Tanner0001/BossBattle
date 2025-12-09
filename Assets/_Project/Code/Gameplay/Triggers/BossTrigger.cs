using UnityEngine;
using BossBattle.Gameplay.Enemies.AI; // Added this line

namespace BossBattle.Gameplay.Triggers
{
    public class BossTrigger : MonoBehaviour
    {
        public GameObject WardenBoss;
        public bool HasBeenTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !HasBeenTriggered)
            {
                HasBeenTriggered = true;
                WardenBoss.SetActive(true); // Still activate the GameObject first
                WardenBossController wardenController = WardenBoss.GetComponent<WardenBossController>();
                if (wardenController != null)
                {
                    wardenController.StartCombat();
                }
                else
                {
                    Debug.LogError("WardenBossController not found on WardenBoss GameObject.");
                }
                Debug.Log("Boss fight triggered!");
            }
        }
    }
}
