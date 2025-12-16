using _Project.Code.Core.Events;
using _Project.Code.Gameplay.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Code.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Player UI")]
        [SerializeField] private Hitbox playerHitbox;
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private Image bloodOverlayImage;
        [SerializeField] private float bloodThresholdPercentage = 0.5f;
        [SerializeField] private float bloodFadeSpeed = 1f;
        
        [Header("Warden UI")]
        [SerializeField] private Hitbox wardenHitbox;
        [SerializeField] private TextMeshProUGUI wardenHealthText;
        
        [Header("Weapon UI")]
        [SerializeField] private TMP_Text ammoText;
        
        [Header("Encounter UI")]
        [SerializeField] private TextMeshProUGUI phaseText;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private float phaseTextDisplayDuration = 4.0f;

        private Color _bloodColor;

        private void OnEnable()
        {
            // Subscribe to global events
            EventBus.Instance.Subscribe<AmmoChangedEvent>(this, OnAmmoChanged);
            EventBus.Instance.Subscribe<PlayerDiedEvent>(this, OnPlayerDied);
            EventBus.Instance.Subscribe<PhaseChangedEvent>(this, OnPhaseChanged);
            EventBus.Instance.Subscribe<GameWonEvent>(this, OnGameWon);
            
            // Subscribe to direct C# events from specific components
            if (playerHitbox != null) playerHitbox.OnHealthChanged += UpdatePlayerHealthUI;
            if (wardenHitbox != null) wardenHitbox.OnHealthChanged += UpdateWardenHealthUI;

            // Initial UI setup
            UpdatePlayerHealthUI();
            UpdateWardenHealthUI();
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (phaseText != null) phaseText.gameObject.SetActive(false);

            if (bloodOverlayImage != null)
            {
                _bloodColor = bloodOverlayImage.color;
                _bloodColor.a = 0f; // Start fully transparent
                bloodOverlayImage.color = _bloodColor;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from all events to prevent memory leaks
            EventBus.Instance.Unsubscribe<AmmoChangedEvent>(this);
            EventBus.Instance.Unsubscribe<PlayerDiedEvent>(this);
            EventBus.Instance.Unsubscribe<PhaseChangedEvent>(this);
            EventBus.Instance.Unsubscribe<GameWonEvent>(this);
            
            if (playerHitbox != null) playerHitbox.OnHealthChanged -= UpdatePlayerHealthUI;
            if (wardenHitbox != null) wardenHitbox.OnHealthChanged -= UpdateWardenHealthUI;
        }

        private void Update()
        {
            // Blood overlay is the only thing that needs polling for smooth fading
            UpdateBloodOverlay();
        }

        // --- Player Health & Death ---
        private void UpdatePlayerHealthUI()
        {
            if (playerHitbox == null || playerHealthText == null) return;
            
            string healthString = $"HP: {Mathf.CeilToInt(playerHitbox.CurrentHealth)} / {Mathf.CeilToInt(playerHitbox.MaxHealth)}";
            if (playerHitbox.HasShield)
            {
                healthString = $"SH: {Mathf.CeilToInt(playerHitbox.CurrentShield)} / {Mathf.CeilToInt(playerHitbox.MaxShield)}\n" + healthString;
            }
            playerHealthText.text = healthString;
        }

        private void UpdateBloodOverlay()
        {
            if (bloodOverlayImage == null || playerHitbox == null) return;

            // If there's a shield, blood overlay only appears when shield is depleted and health is low
            if (playerHitbox.HasShield && playerHitbox.CurrentShield > 0)
            {
                _bloodColor.a = Mathf.Lerp(_bloodColor.a, 0f, Time.deltaTime * bloodFadeSpeed); // Fade out blood if shield is up
                bloodOverlayImage.color = _bloodColor;
                return;
            }

            float currentHealthPercentage = playerHitbox.CurrentHealth / playerHitbox.MaxHealth;
            float targetAlpha = (currentHealthPercentage <= bloodThresholdPercentage)
                ? 1f - (currentHealthPercentage / bloodThresholdPercentage)
                : 0f;

            // Smoothly lerp towards the target alpha
            _bloodColor.a = Mathf.Lerp(_bloodColor.a, targetAlpha, Time.deltaTime * bloodFadeSpeed);
            bloodOverlayImage.color = _bloodColor;
        }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            Debug.Log("Player Died! Game Over.");
            Time.timeScale = 0; // Pause game
            // In a real implementation, you might load a GameOver scene or show a specific UI panel.
        }

        // --- Warden Health ---
        private void UpdateWardenHealthUI()
        {
            if (wardenHitbox == null || wardenHealthText == null) return;
            wardenHealthText.text = $"Warden HP: {Mathf.CeilToInt(wardenHitbox.CurrentHealth)} / {Mathf.CeilToInt(wardenHitbox.MaxHealth)}";
        }
        
        // --- Weapon UI ---
        private void OnAmmoChanged(AmmoChangedEvent evt)
        {
            if (ammoText == null) return;
            ammoText.text = $"{evt.current}/{evt.max}";
        }
        
        // --- Encounter UI ---
        private void OnPhaseChanged(PhaseChangedEvent e)
        {
            if (phaseText == null) return;
            phaseText.text = e.DisplayText;
            phaseText.gameObject.SetActive(true);
            Invoke(nameof(HidePhaseText), phaseTextDisplayDuration);
        }

        private void HidePhaseText()
        {
            if (phaseText != null) phaseText.gameObject.SetActive(false);
        }

        private void OnGameWon(GameWonEvent e)
        {
            if (victoryPanel == null) return;
            Debug.Log("[UIManager] Received Game Won event! Displaying victory screen.");
            victoryPanel.SetActive(true);
        }
    }
}
