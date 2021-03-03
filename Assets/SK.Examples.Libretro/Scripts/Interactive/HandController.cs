using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SK.Examples
{
    [RequireComponent(typeof(ActionBasedController))]
    public sealed class HandController : MonoBehaviour
    {
        [SerializeField] private Hand _hand;

        private ActionBasedController _controller;

        private void Awake() => _controller = GetComponent<ActionBasedController>();

        private void Update()
        {
            _hand.SetFlex(_controller.selectAction.action.ReadValue<float>());
            _hand.SetPinch(_controller.activateAction.action.ReadValue<float>());
        }
    }
}
