using _Project.Code.Core.Events;

namespace _Project.Code.Gameplay.Combat.GunSystem
{
    public struct CameraRecoilEvent : IEvent
    {
        public float RecoilAmount { get; }

        public CameraRecoilEvent(float recoilAmount)
        {
            RecoilAmount = recoilAmount;
        }
    }
}
