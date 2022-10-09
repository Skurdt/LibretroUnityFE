using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SK.Libretro.Examples
{
    public sealed class UI : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onStartGameClicked;
        [SerializeField] private UnityEvent _onStopGameClicked;

        private UIDocument _uiDocument;
        private VisualElement _centerArea;
        private VisualElement _toolbarGameButton;
        private VisualElement _toolbarGameMenu;
        private VisualElement _toolbarGameMenuStartButton;
        private VisualElement _toolbarGameMenuStopButton;

        private void Awake() => _uiDocument = GetComponent<UIDocument>();

        private void OnEnable()
        {
            VisualElement root = _uiDocument.rootVisualElement;

            VisualElement toolbar = root.Q("Toolbar");
            _toolbarGameButton = toolbar.Q("GameButton");
            _toolbarGameButton.RegisterCallback<MouseDownEvent>(ToolbarGameButtonMouseDownEventCallback);

            _centerArea = root.Q("Center");
            _centerArea.RegisterCallback<MouseDownEvent>(CenterAreaMouseDownEventCallback);

            _toolbarGameMenu = _centerArea.Q("GameMenu");

            _toolbarGameMenuStartButton = _toolbarGameMenu.Q("StartPauseResumeButton");
            _toolbarGameMenuStartButton.RegisterCallback<MouseDownEvent>(ToolbarGameMenuStartButtonMouseDownEventCallback);

            _toolbarGameMenuStopButton = _toolbarGameMenu.Q("StopButton");
            _toolbarGameMenuStopButton.RegisterCallback<MouseDownEvent>(ToolbarGameMenuStopButtonMouseDownEventCallback);
        }

        private void OnDisable()
        {
            _toolbarGameButton.UnregisterCallback<MouseDownEvent>(ToolbarGameButtonMouseDownEventCallback);

            _centerArea.UnregisterCallback<MouseDownEvent>(CenterAreaMouseDownEventCallback);

            _toolbarGameMenuStartButton.UnregisterCallback<MouseDownEvent>(ToolbarGameMenuStartButtonMouseDownEventCallback);
            _toolbarGameMenuStopButton.UnregisterCallback<MouseDownEvent>(ToolbarGameMenuStopButtonMouseDownEventCallback);
        }

        private void ToolbarGameButtonMouseDownEventCallback(MouseDownEvent evnt) =>
            _toolbarGameMenu.EnableInClassList("display-hidden", false);

        private void CenterAreaMouseDownEventCallback(MouseDownEvent evnt) =>
            _toolbarGameMenu.EnableInClassList("display-hidden", true);

        private void ToolbarGameMenuStartButtonMouseDownEventCallback(MouseDownEvent evnt) =>
            _onStartGameClicked.Invoke();

        private void ToolbarGameMenuStopButtonMouseDownEventCallback(MouseDownEvent evnt) =>
            _onStopGameClicked.Invoke();
    }
}
