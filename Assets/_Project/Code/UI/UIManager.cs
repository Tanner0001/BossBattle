using _Project.Code.Core.Events;
using UnityEngine;

namespace _Project.Code.UI
{
    public class UIManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(this, OnPhaseChanged);
            EventBus.Instance.Subscribe<GameWonEvent>(this, OnGameWon);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<PhaseChangedEvent>(this);
            EventBus.Instance.Unsubscribe<GameWonEvent>(this);
        }

        private void OnPhaseChanged(PhaseChangedEvent e)
        {
            // In a real implementation, this would update a Text UI element.
            Debug.Log($"[UIManager] Received Phase Change: {e.DisplayText}");
            // Example: phaseText.text = e.DisplayText;
            // Example: phaseText.gameObject.SetActive(true);
            // Example: Invoke("HidePhaseText", 4.0f);
        }

        private void OnGameWon(GameWonEvent e)
        {
            // In a real implementation, this would show a victory screen.
            Debug.Log("[UIManager] Received Game Won event! Displaying victory screen.");
            // Example: victoryPanel.SetActive(true);
        }
    }
}
