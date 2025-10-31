using UnityEngine;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private GameObject impactEffect;

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

            if (impactEffect)
                Instantiate(impactEffect, transform.position, Quaternion.identity);

            if (collision.collider.TryGetComponent<Hitbox>(out var hitbox))
            {
                hitbox.ApplyDamage(Damage);
            }

            Destroy(gameObject);
        }
    }
}
