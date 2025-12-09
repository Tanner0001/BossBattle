using UnityEngine;

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
                WardenBoss.SetActive(true);
                Debug.Log("Boss fight triggered!");
            }
        }
    }
}
