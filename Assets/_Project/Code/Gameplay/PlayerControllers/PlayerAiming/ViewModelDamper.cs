using UnityEngine;
using _Project.Code.Core.Events; // Make sure you have this using statement

public class ViewModelDamper : MonoBehaviour
{
    [Tooltip("How much the arms follow the camera's pitch. 0.2 = 20%.")]
    [SerializeField] private float pitchDamping = 0.2f;
    
    [Tooltip("How far the arms can rotate up.")]
    [SerializeField] private float maxPitch = 15f;
    
    [Tooltip("How far the arms can rotate down.")]
    [SerializeField] private float minPitch = -15f;
    
    [Tooltip("How fast the arms catch up to the target rotation.")]
    [SerializeField] private float rotationSpeed = 10f;

    private float _targetPitch;

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<CameraPitchChangedEvent>(this, OnCameraPitchChanged);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<CameraPitchChangedEvent>(this);
    }

    private void OnCameraPitchChanged(CameraPitchChangedEvent evt)
    {
        _targetPitch = evt.Pitch * pitchDamping;
        _targetPitch = Mathf.Clamp(_targetPitch, minPitch, maxPitch);
    }

    private void Update()
    {
        var targetRotation = Quaternion.Euler(_targetPitch, 0, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
