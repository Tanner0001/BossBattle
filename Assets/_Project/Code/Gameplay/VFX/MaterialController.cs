using UnityEngine;

namespace _Project.Code.Gameplay.VFX
{
    public class MaterialController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Renderer targetRenderer; // The renderer to modify

        [Header("Materials")]
        [SerializeField] private Material invulnerableMaterial; // The "glowing blue" material

        private Material _originalMaterial;
        private bool _isEffectActive = false;

        void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
                if (targetRenderer == null)
                {
                    Debug.LogError("MaterialController: No Renderer found or assigned!", this);
                    enabled = false;
                    return;
                }
            }
            _originalMaterial = targetRenderer.material;
        }

        public void ApplyInvulnerableMaterial()
        {
            if (_isEffectActive || invulnerableMaterial == null) return;
            
            _isEffectActive = true;
            targetRenderer.material = invulnerableMaterial;
            Debug.Log("Applied invulnerable material.");
        }

        public void ClearInvulnerableMaterial()
        {
            if (!_isEffectActive) return;

            _isEffectActive = false;
            targetRenderer.material = _originalMaterial;
            Debug.Log("Cleared invulnerable material, reverting to original.");
        }
    }
}
