using UnityEngine;
using UnityEngine.UI;

namespace SK.Examples
{
    [DisallowMultipleComponent]
    public sealed class UIMainMenuOpenCoreOptionsPanelButton : MonoBehaviour
    {
        [SerializeField] private GameObject _coreOptionsPanel;

        private Button _button;

        private void Awake() => _button = GetComponent<Button>();

        private void OnEnable() => _button.onClick.AddListener(ButtonCallback);

        private void OnDisable() => _button.onClick.RemoveListener(ButtonCallback);

        private void ButtonCallback() => _coreOptionsPanel.SetActive(true);
    }
}
