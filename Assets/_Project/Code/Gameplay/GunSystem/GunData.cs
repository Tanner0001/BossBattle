using UnityEngine;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public enum FireMode
    {
        Projectile,
        Hitscan
    }

    [CreateAssetMenu(fileName = "NewGunData", menuName = "Guns/Gun Data")]
    public class GunData : ScriptableObject
    {
        [Header("Info")]
        public string GunName;

        [Header("Stats")]
        public FireMode FireMode = FireMode.Projectile;
        public float Damage = 10f;
        public int MaxAmmo = 10;
        public float FireRate = 0.2f;
        public float ReloadTime = 1.5f;

        [Header("Projectile Settings (if FireMode is Projectile)")]
        public GameObject ProjectilePrefab;

        [Header("Hitscan Settings (if FireMode is Hitscan)")]
        public float HitscanRange = 100f;
        public LayerMask HitscanLayers;

        [Header("Recoil")]
        public float RecoilAmount = 0.1f;
        public float RecoilSpeed = 10f;
        public float RecoilRecoverySpeed = 5f;
    }
}
