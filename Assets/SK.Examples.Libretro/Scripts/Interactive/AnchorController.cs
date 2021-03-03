using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SK.Examples
{
    public class AnchorController : MonoBehaviour
    {
        public Transform AnchorTransform;

        private Vector3 _startPos;

        private void Start()
        {
            _startPos = AnchorTransform.localPosition;
        }

        public void OnHoverEntered(HoverEnterEventArgs args)
        {
           if ((args.interactor as XRRayInteractor).GetCurrentRaycastHit(out RaycastHit hit))
           {
                _startPos = AnchorTransform.localPosition;
                AnchorTransform.localPosition = hit.point;
           }
        }

        public void OnHoverExited(HoverExitEventArgs args)
        {
            AnchorTransform.localPosition = _startPos;
        }
    }
}
