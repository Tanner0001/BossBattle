using UnityEngine;
using TMPro; // Required for TextMeshPro

namespace BossBattle.Gameplay.UI
{
    public class WardenHealthDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Enemies.AI.WardenBossController wardenBossController;
        [SerializeField] private TextMeshProUGUI healthText;

        private void Awake()
        {
            if (wardenBossController == null)
            {
                Debug.LogError("WardenHealthDisplay: No WardenBossController assigned!", this);
                enabled = false; // Disable script if no controller
                return;
            }

            if (healthText == null)
            {
                Debug.LogError("WardenHealthDisplay: No Health Text (TextMeshProUGUI) assigned!", this);
                enabled = false; // Disable script if no text element
                return;
            }
        }

        private void Update()
        {
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            if (wardenBossController != null && healthText != null && wardenBossController.WardenData != null)
            {
                healthText.text = $"Warden HP: {Mathf.CeilToInt(wardenBossController.CurrentHealth)} / {Mathf.CeilToInt(wardenBossController.WardenData.MaxHealth)}";
            }
            else
            {
                // Optionally disable the display if the boss is dead or data is missing
                healthText.text = "Warden HP: N/A";
            }
        }
    }
}
