using UnityEngine;
using TMPro;
using _Project.Code.Core.Events;

namespace _Project.Code.UI
{
    public class AmmoUIController : MonoBehaviour
    {
        public TMP_Text ammoText;

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<AmmoChangedEvent>(this, OnAmmoChanged);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<AmmoChangedEvent>(this);
        }

        private void OnAmmoChanged(AmmoChangedEvent evt)
        {
            ammoText.text = $"{evt.current}/{evt.max}";
        }
    }
}
