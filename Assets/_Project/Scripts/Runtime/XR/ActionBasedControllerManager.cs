using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Use this class to mediate the controllers and their associated interactors and input actions under different interaction states.
    /// </summary>
    [AddComponentMenu("XR/Action Based Controller Manager")]
    [DefaultExecutionOrder(UPDATE_ORDER)]
    public class ActionBasedControllerManager : MonoBehaviour
    {
        /// <summary>
        /// Order when instances of type <see cref="ActionBasedControllerManager"/> are updated.
        /// </summary>
        /// <remarks>
        /// Executes before controller components to ensure input processors can be attached
        /// to input actions and/or bindings before the controller component reads the current
        /// values of the input actions.
        /// </remarks>
        public const int UPDATE_ORDER = XRInteractionUpdateOrder.k_Controllers - 1;

        [Space]
        [Header("Interactors")]

        [SerializeField]
        [Tooltip("The GameObject containing the interaction group used for direct and distant manipulation.")]
        private Interactors.XRInteractionGroup _manipulationInteractionGroup;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for direct manipulation.")]
        private Interactors.XRDirectInteractor _directInteractor;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for distant/ray manipulation.")]
        private Interactors.XRRayInteractor _rayInteractor;

        [SerializeField]
        [Tooltip("The GameObject containing the interactor used for teleportation.")]
        private Interactors.XRRayInteractor _teleportInteractor;

        [Space]
        [Header("Controller Actions")]

        [SerializeField]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        private InputActionReference _teleportModeActivate;

        [SerializeField]
        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        private InputActionReference _teleportModeCancel;

        [SerializeField]
        [Tooltip("The reference to the action of continuous turning the XR Origin with this controller.")]
        private InputActionReference _turn;

        [SerializeField]
        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        private InputActionReference _snapTurn;

        [SerializeField]
        [Tooltip("The reference to the action of moving the XR Origin with this controller.")]
        private InputActionReference _move;

        [SerializeField]
        [Tooltip("The reference to the action of scrolling UI with this controller.")]
        private InputActionReference _uIScroll;

        [Space]
        [Header("Locomotion Settings")]

        [SerializeField]
        [Tooltip("If true, continuous movement will be enabled. If false, teleport will enabled.")]
        private bool _smoothMotionEnabled;
        
        [SerializeField]
        [Tooltip("If true, continuous turn will be enabled. If false, snap turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        private bool _smoothTurnEnabled;

        [Space]
        [Header("UI Settings")]

        [SerializeField]
        [Tooltip("If true, UI scrolling will be enabled.")]
        private bool _uIScrollingEnabled;

        [Space]
        [Header("Mediation Events")]
        [SerializeField]
        [Tooltip("Event fired when the active ray interactor changes between interaction and teleport.")]
        private UnityEvent<Interactors.IXRRayProvider> _rayInteractorChanged;

        public bool SmoothMotionEnabled
        {
            get => _smoothMotionEnabled;
            set
            {
                _smoothMotionEnabled = value;
                UpdateLocomotionActions();
            }
        }

        public bool SmoothTurnEnabled
        {
            get => _smoothTurnEnabled;
            set
            {
                _smoothTurnEnabled = value;
                UpdateLocomotionActions();
            }
        }

        public bool UiScrollingEnabled
        {
            get => _uIScrollingEnabled;
            set
            {
                _uIScrollingEnabled = value;
                UpdateUIActions();
            }
        }

        private bool _startCalled;
        private bool _postponedDeactivateTeleport;
        private bool _hoveringScrollableUI;

        private const int INTERACT_OR_NOT_IN_GROUP = -1;

        private IEnumerator _afterInteractionEventsRoutine;
        private readonly HashSet<InputAction> _locomotionUsers = new();

        /// <summary>
        /// Temporary scratch list to populate with the group members of the interaction group.
        /// </summary>
        private static readonly List<Interactors.IXRGroupMember> _groupMembers = new();

        // For our input mediation, we are enforcing a few rules between direct, ray, and teleportation interaction:
        // 1. If the Teleportation Ray is engaged, the Ray interactor is disabled
        // 2. The interaction group ensures that the Direct and Ray interactors cannot interact at the same time, with the Direct interactor taking priority
        // 3. If the Ray interactor is selecting, all locomotion controls are disabled (teleport ray, move, and turn controls) to prevent input collision
        private void SetupInteractorEvents()
        {
            if (_rayInteractor != null)
            {
                _rayInteractor.selectEntered.AddListener(OnRaySelectEntered);
                _rayInteractor.selectExited.AddListener(OnRaySelectExited);
                _rayInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
                _rayInteractor.uiHoverExited.AddListener(OnUIHoverExited);
            }

            var teleportModeActivateAction = GetInputAction(_teleportModeActivate);
            if (teleportModeActivateAction != null)
            {
                teleportModeActivateAction.performed += OnStartTeleport;
                teleportModeActivateAction.performed += OnStartLocomotion;
                teleportModeActivateAction.canceled += OnCancelTeleport;
                teleportModeActivateAction.canceled += OnStopLocomotion;
            }

            var teleportModeCancelAction = GetInputAction(_teleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed += OnCancelTeleport;
            }

            var moveAction = GetInputAction(_move);
            if (moveAction != null)
            {
                moveAction.started += OnStartLocomotion;
                moveAction.canceled += OnStopLocomotion;
            }

            var turnAction = GetInputAction(_turn);
            if (turnAction != null)
            {
                turnAction.started += OnStartLocomotion;
                turnAction.canceled += OnStopLocomotion;
            }

            var snapTurnAction = GetInputAction(_snapTurn);
            if (snapTurnAction != null)
            {
                snapTurnAction.started += OnStartLocomotion;
                snapTurnAction.canceled += OnStopLocomotion;
            }
        }

        private void TeardownInteractorEvents()
        {
            if (_rayInteractor != null)
            {
                _rayInteractor.selectEntered.RemoveListener(OnRaySelectEntered);
                _rayInteractor.selectExited.RemoveListener(OnRaySelectExited);
            }

            var teleportModeActivateAction = GetInputAction(_teleportModeActivate);
            if (teleportModeActivateAction != null)
            {
                teleportModeActivateAction.performed -= OnStartTeleport;
                teleportModeActivateAction.performed -= OnStartLocomotion;
                teleportModeActivateAction.canceled -= OnCancelTeleport;
                teleportModeActivateAction.canceled -= OnStopLocomotion;
            }

            var teleportModeCancelAction = GetInputAction(_teleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed -= OnCancelTeleport;
            }

            var moveAction = GetInputAction(_move);
            if (moveAction != null)
            {
                moveAction.started -= OnStartLocomotion;
                moveAction.canceled -= OnStopLocomotion;
            }

            var turnAction = GetInputAction(_turn);
            if (turnAction != null)
            {
                turnAction.started -= OnStartLocomotion;
                turnAction.canceled -= OnStopLocomotion;
            }

            var snapTurnAction = GetInputAction(_snapTurn);
            if (snapTurnAction != null)
            {
                snapTurnAction.started -= OnStartLocomotion;
                snapTurnAction.canceled -= OnStopLocomotion;
            }
        }

        private void OnStartTeleport(InputAction.CallbackContext context)
        {
            _postponedDeactivateTeleport = false;

            if (_teleportInteractor != null)
                _teleportInteractor.gameObject.SetActive(true);

            if (_rayInteractor != null)
                _rayInteractor.gameObject.SetActive(false);

            _rayInteractorChanged?.Invoke(_teleportInteractor);
        }

        private void OnCancelTeleport(InputAction.CallbackContext context)
        {
            // Do not deactivate the teleport interactor in this callback.
            // We delay turning off the teleport interactor in this callback so that
            // the teleport interactor has a chance to complete the teleport if needed.
            // OnAfterInteractionEvents will handle deactivating its GameObject.
            _postponedDeactivateTeleport = true;

            if (_rayInteractor != null)
                _rayInteractor.gameObject.SetActive(true);

            _rayInteractorChanged?.Invoke(_rayInteractor);

        }

        private void OnStartLocomotion(InputAction.CallbackContext context) => _locomotionUsers.Add(context.action);

        private void OnStopLocomotion(InputAction.CallbackContext context)
        {
            _ = _locomotionUsers.Remove(context.action);

            if (_locomotionUsers.Count == 0 && _hoveringScrollableUI)
            {
                DisableLocomotionActions();
                UpdateUIActions();
            }
        }

        private void OnRaySelectEntered(SelectEnterEventArgs args) =>
            // Disable locomotion and turn actions
            DisableLocomotionActions();

        private void OnRaySelectExited(SelectExitEventArgs args) =>
            // Re-enable the locomotion and turn actions
            UpdateLocomotionActions();

        private void OnUIHoverEntered(UIHoverEventArgs args)
        {
            _hoveringScrollableUI = _uIScrollingEnabled && args.deviceModel.isScrollable;
            UpdateUIActions();

            // If locomotion is occurring, wait
            if (_hoveringScrollableUI && _locomotionUsers.Count == 0)
            {
                // Disable locomotion and turn actions
                DisableLocomotionActions();
            }
        }

        private void OnUIHoverExited(UIHoverEventArgs args)
        {
            _hoveringScrollableUI = false;
            UpdateUIActions();

            // Re-enable the locomotion and turn actions
            UpdateLocomotionActions();
        }

        protected void Awake() => _afterInteractionEventsRoutine = OnAfterInteractionEvents();

        protected void OnEnable()
        {
            if (_teleportInteractor != null)
                _teleportInteractor.gameObject.SetActive(false);

            // Allow the locomotion actions to be refreshed when this is re-enabled.
            // See comments in Start for why we wait until Start to enable/disable locomotion actions.
            if (_startCalled)
                UpdateLocomotionActions();

            SetupInteractorEvents();

            // Start the coroutine that executes code after the Update phase (during yield null).
            // Since this behavior has an execution order that runs before the XRInteractionManager,
            // we use the coroutine to run after the selection events
            _ = StartCoroutine(_afterInteractionEventsRoutine);
        }

        protected void OnDisable()
        {
            TeardownInteractorEvents();

            StopCoroutine(_afterInteractionEventsRoutine);
        }

        protected void Start()
        {
            _startCalled = true;

            // Ensure the enabled state of locomotion and turn actions are properly set up.
            // Called in Start so it is done after the InputActionManager enables all input actions earlier in OnEnable.
            UpdateLocomotionActions();
            UpdateUIActions();

            if (_manipulationInteractionGroup == null)
            {
                Debug.LogError("Missing required Manipulation Interaction Group reference. Use the Inspector window to assign the XR Interaction Group component reference.", this);
                return;
            }

            // Ensure interactors are properly set up in the interaction group by adding
            // them if necessary and ordering Direct before Ray interactor.
            var directInteractorIndex = INTERACT_OR_NOT_IN_GROUP;
            var rayInteractorIndex = INTERACT_OR_NOT_IN_GROUP;
            _manipulationInteractionGroup.GetGroupMembers(_groupMembers);
            for (var i = 0; i < _groupMembers.Count; ++i)
            {
                var groupMember = _groupMembers[i];
                if (ReferenceEquals(groupMember, _directInteractor))
                    directInteractorIndex = i;
                else if (ReferenceEquals(groupMember, _rayInteractor))
                    rayInteractorIndex = i;
            }

            if (directInteractorIndex == INTERACT_OR_NOT_IN_GROUP)
            {
                // Must add Direct interactor to group, and make sure it is ordered before the Ray interactor
                if (rayInteractorIndex == INTERACT_OR_NOT_IN_GROUP)
                {
                    // Must add Ray interactor to group
                    if (_directInteractor != null)
                        _manipulationInteractionGroup.AddGroupMember(_directInteractor);

                    if (_rayInteractor != null)
                        _manipulationInteractionGroup.AddGroupMember(_rayInteractor);
                }
                else if (_directInteractor != null)
                {
                    _manipulationInteractionGroup.MoveGroupMemberTo(_directInteractor, rayInteractorIndex);
                }
            }
            else
            {
                if (rayInteractorIndex == INTERACT_OR_NOT_IN_GROUP)
                {
                    // Must add Ray interactor to group
                    if (_rayInteractor != null)
                        _manipulationInteractionGroup.AddGroupMember(_rayInteractor);
                }
                else
                {
                    // Must make sure Direct interactor is ordered before the Ray interactor
                    if (rayInteractorIndex < directInteractorIndex)
                    {
                        _manipulationInteractionGroup.MoveGroupMemberTo(_directInteractor, rayInteractorIndex);
                    }
                }
            }
        }

        private IEnumerator OnAfterInteractionEvents()
        {
            while (true)
            {
                // Yield so this coroutine is resumed after the teleport interactor
                // has a chance to process its select interaction event during Update.
                yield return null;

                if (_postponedDeactivateTeleport)
                {
                    if (_teleportInteractor != null)
                        _teleportInteractor.gameObject.SetActive(false);

                    _postponedDeactivateTeleport = false;
                }
            }
        }

        private void UpdateLocomotionActions()
        {
            // Disable/enable Teleport and Turn when Move is enabled/disabled.
            SetEnabled(_move, _smoothMotionEnabled);
            SetEnabled(_teleportModeActivate, !_smoothMotionEnabled);
            SetEnabled(_teleportModeCancel, !_smoothMotionEnabled);

            // Disable ability to turn when using continuous movement
            SetEnabled(_turn, !_smoothMotionEnabled && _smoothTurnEnabled);
            SetEnabled(_snapTurn, !_smoothMotionEnabled && !_smoothTurnEnabled);
        }

        private void DisableLocomotionActions()
        {
            DisableAction(_move);
            DisableAction(_teleportModeActivate);
            DisableAction(_teleportModeCancel);
            DisableAction(_turn);
            DisableAction(_snapTurn);
        }

        private void UpdateUIActions() => SetEnabled(_uIScroll, _uIScrollingEnabled && _hoveringScrollableUI && _locomotionUsers.Count == 0);

        private static void SetEnabled(InputActionReference actionReference, bool enabled)
        {
            if (enabled)
                EnableAction(actionReference);
            else
                DisableAction(actionReference);
        }

        private static void EnableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && !action.enabled)
                action.Enable();
        }

        private static void DisableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);
            if (action != null && action.enabled)
                action.Disable();
        }

        private static InputAction GetInputAction(InputActionReference actionReference) =>
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031

    }
}
