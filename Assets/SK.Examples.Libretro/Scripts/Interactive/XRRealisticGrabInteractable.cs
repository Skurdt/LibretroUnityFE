using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SK.Examples
{
    public class XRRealisticGrabInteractable : XRGrabInteractable
    {
        private Vector3 _interactionPosition    = Vector3.zero;
        private Quaternion _interactionRotation = Quaternion.identity;

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            StoreInteractor(args.interactor);
            MatchAttachmentPoint(args.interactor);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            ResetAttachmentPoint(args.interactor);
            ClearInteractor();
        }

        private void StoreInteractor(XRBaseInteractor interactor)
        {
            _interactionPosition = interactor.attachTransform.localPosition;
            _interactionRotation = interactor.attachTransform.localRotation;
        }

        private void MatchAttachmentPoint(XRBaseInteractor interactor)
        {
            bool hasAttach = attachTransform != null;
            interactor.attachTransform.position = hasAttach ? attachTransform.position : transform.position;
            interactor.attachTransform.rotation = hasAttach ? attachTransform.rotation : transform.rotation;

        }

        private void ResetAttachmentPoint(XRBaseInteractor interactor)
        {
            interactor.attachTransform.localPosition = _interactionPosition;
            interactor.attachTransform.localRotation = _interactionRotation;
        }

        private void ClearInteractor()
        {
            _interactionPosition = Vector3.zero;
            _interactionRotation = Quaternion.identity;
        }
    }
}
