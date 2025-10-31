using UnityEngine;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private GameObject impactEffectPrefab;

        public float Damage { get; set; }

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player")) return;

            // Instantiate the impact effect and destroy it after its duration
            if (impactEffectPrefab != null)
            {
                var impactEffectInstance = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
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
