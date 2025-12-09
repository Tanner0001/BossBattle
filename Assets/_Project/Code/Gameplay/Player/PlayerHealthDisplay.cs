using UnityEngine;
using UnityEngine.UI; // Required for UI components
using UnityEngine.SceneManagement; // Required for scene management
using TMPro; // Required for TextMeshPro

namespace _Project.Code.Gameplay.Player
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Combat.Hitbox playerHitbox;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image bloodOverlayImage;

        [Header("Blood Overlay Settings")]
        [SerializeField] private float bloodThresholdPercentage = 0.5f; // Blood effect starts at 50% health
        [SerializeField] private float bloodFadeSpeed = 1f;

        private Color _bloodColor;
        private bool _isDead = false;

        private void Awake()
        {
            if (playerHitbox == null)
            {
                playerHitbox = GetComponent<Combat.Hitbox>();
                if (playerHitbox == null)
                {
                    Debug.LogError("PlayerHealthDisplay: No Hitbox found on this GameObject or assigned!", this);
                    enabled = false; // Disable script if no hitbox
                    return;
                }
            }

            if (bloodOverlayImage != null)
            {
                _bloodColor = bloodOverlayImage.color;
                _bloodColor.a = 0f; // Start fully transparent
                bloodOverlayImage.color = _bloodColor;
            }
            else
            {
                Debug.LogWarning("PlayerHealthDisplay: No Blood Overlay Image assigned. Blood effect will not play.", this);
            }
        }

        private void Start()
        {
            UpdateHealthUI();
        }

        private void Update()
        {
            if (_isDead) return;

            UpdateHealthUI();
            UpdateBloodOverlay();

            if (playerHitbox.CurrentHealth <= 0)
            {
                _isDead = true;
                GameOver();
            }
        }

        private void UpdateHealthUI()
        {
            if (healthText != null)
            {
                string healthString = $"HP: {Mathf.CeilToInt(playerHitbox.CurrentHealth)} / {Mathf.CeilToInt(playerHitbox.MaxHealth)}";
                if (playerHitbox.HasShield)
                {
                    healthString = $"SH: {Mathf.CeilToInt(playerHitbox.CurrentShield)} / {Mathf.CeilToInt(playerHitbox.MaxShield)}\n" + healthString;
                }
                healthText.text = healthString;
            }
        }

        private void UpdateBloodOverlay()
        {
            if (bloodOverlayImage == null || playerHitbox == null) return;

            float currentEffectiveHealth = playerHitbox.HasShield ? playerHitbox.CurrentShield : playerHitbox.CurrentHealth;
            float maxEffectiveHealth = playerHitbox.HasShield ? playerHitbox.MaxShield : playerHitbox.MaxHealth;

            // If there's a shield, blood overlay only appears when shield is depleted and health is low
            if (playerHitbox.HasShield && playerHitbox.CurrentShield > 0)
            {
                _bloodColor.a = Mathf.Lerp(_bloodColor.a, 0f, Time.deltaTime * bloodFadeSpeed); // Fade out blood if shield is up
                bloodOverlayImage.color = _bloodColor;
                return;
            }

            float currentHealthPercentage = playerHitbox.CurrentHealth / playerHitbox.MaxHealth;
            float targetAlpha = 0f;

            if (currentHealthPercentage <= bloodThresholdPercentage)
            {
                // Linearly increase alpha as health drops from threshold to 0
                targetAlpha = 1f - (currentHealthPercentage / bloodThresholdPercentage);
            }

            // Smoothly lerp towards the target alpha
            _bloodColor.a = Mathf.Lerp(_bloodColor.a, targetAlpha, Time.deltaTime * bloodFadeSpeed);
            bloodOverlayImage.color = _bloodColor;
        }

        private void GameOver()
        {
            Debug.Log("Player Died! Game Over.");
            Time.timeScale = 0; // Pause game
            // Example: SceneManager.LoadScene("GameOverScene"); 
            // The user already has a Game Over scene and logic.
            // I'll just pause the game, they can wire up their existing logic.
        }
    }
}
