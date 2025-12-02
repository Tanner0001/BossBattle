using UnityEngine;
using System.Collections;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float lifeTime = 3f;
        
        [Header("Impact Effects")]
        [SerializeField] private GameObject defaultImpactPrefab;
        [SerializeField] private GameObject playerImpactPrefab;
        // Optionally, could add environmentalImpactPrefab, etc.

        public float Damage { get; set; }
        public LayerMask PlayerLayer { get; set; } // Added PlayerLayer

        private bool _isVisualOnly = false;

        private void Start()
        {
            // Only self-destruct if not a visual tracer, as tracers have a controlled lifetime.
            if (!_isVisualOnly)
            {
                Destroy(gameObject, lifeTime);
            }
        }

        private void Update()
        {
            // Default projectile movement, ignored if it's a visual tracer.
            if (!_isVisualOnly)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }

        public void InitializeAsVisualTracer(Vector3 targetPoint, float tracerSpeed = 150f)
        {
            _isVisualOnly = true;
            StartCoroutine(MoveToTarget(targetPoint, tracerSpeed));
        }

        private IEnumerator MoveToTarget(Vector3 target, float speed)
        {
            float travelTime = Vector3.Distance(transform.position, target) / speed;
            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;

            while (elapsedTime < travelTime)
            {
                transform.position = Vector3.Lerp(startPosition, target, elapsedTime / travelTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = target;
            
            // Spawn impact, but don't deal damage
            // For tracers, we use the default impact visual for now
            if (defaultImpactPrefab != null)
            {
                // Note: For a simple tracer, we don't have a real collision normal.
                // We'll spawn the effect facing away from the impact point.
                Quaternion rotation = Quaternion.LookRotation(transform.position - startPosition);
                var impactEffectInstance = Instantiate(defaultImpactPrefab, transform.position, rotation);
                Destroy(impactEffectInstance, 2f); // Simplified lifetime management
            }

            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // If this is just a visual tracer, do not process collisions or damage.
            if (_isVisualOnly) return;
            
            GameObject impactPrefabToUse = defaultImpactPrefab;
            Quaternion impactRotation = Quaternion.LookRotation(collision.contacts[0].normal);
            Transform parentTransform = collision.transform;

            // Conditional impact effect based on what was hit (using LayerMask for performance)
            if (((1 << collision.gameObject.layer) & PlayerLayer) != 0)
            {
                impactPrefabToUse = playerImpactPrefab;
                // For blood splatter, might not want to align with normal directly
                impactRotation = Quaternion.identity; // Or align to camera/fixed up
            }
            // else if (collision.gameObject.CompareTag("Environmental")) { impactPrefabToUse = environmentalImpactPrefab; }
            // else { impactPrefabToUse = defaultImpactPrefab; }


            // Instantiate the impact effect at the point of impact and align it with the surface normal
            if (impactPrefabToUse != null)
            {
                ContactPoint contact = collision.contacts[0];
                var impactEffectInstance = Instantiate(impactPrefabToUse, contact.point, impactRotation, parentTransform);

                if (impactEffectInstance.TryGetComponent<ParticleSystem>(out var particleSystem))
                {
                    Destroy(impactEffectInstance, particleSystem.main.duration);
                }
                else
                {
                    // If there's no particle system, destroy it after a default duration
                    Destroy(impactEffectInstance, 2f);
                }
            }

            if (collision.collider.TryGetComponent<Hitbox>(out var hitbox))
            {
                hitbox.ApplyDamage(Damage);
            }

            Destroy(gameObject);
        }
    }
}
