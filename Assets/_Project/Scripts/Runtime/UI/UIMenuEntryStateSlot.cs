using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent]
    public sealed class UIMenuEntryStateSlot : MonoBehaviour
    {
        [SerializeField] private Button _decreaseButton;
        [SerializeField] private Button _increaseButton;
        [SerializeField] private TMP_InputField _stateSlotInputField;
        [SerializeField] private TMP_Text _saveStateButtonText;
        [SerializeField] private TMP_Text _loadStateButtonText;

        public event System.Action<int> OnStateSlotChanged;

        private const int MIN_SLOT = 0;
        private const int MAX_SLOT = 9999;

        private int _currentSlot = MIN_SLOT;

        private void Awake()
        {
            _stateSlotInputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
            _stateSlotInputField.characterLimit      = MAX_SLOT.ToString().Length;
        }

        private void OnEnable()
        {
            _stateSlotInputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            _decreaseButton.onClick.AddListener(OnDecreaseButtonClicked);
            _increaseButton.onClick.AddListener(OnIncreaseButtonClicked);
        }

        private void OnDisable()
        {
            _stateSlotInputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
            _decreaseButton.onClick.RemoveListener(OnDecreaseButtonClicked);
            _increaseButton.onClick.RemoveListener(OnIncreaseButtonClicked);
        }

        private void OnInputFieldValueChanged(string value)
        {
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out int intValue))
                intValue = MIN_SLOT;

            _currentSlot = intValue;
            _currentSlot = Mathf.Clamp(_currentSlot, MIN_SLOT, MAX_SLOT);
            _saveStateButtonText.SetText($"Save State ({_currentSlot})");
            _loadStateButtonText.SetText($"Load State ({_currentSlot})");
            OnStateSlotChanged?.Invoke(_currentSlot);
        }

        private void OnDecreaseButtonClicked()
        {
            --_currentSlot;
            _currentSlot = Mathf.Max(MIN_SLOT, _currentSlot);
            _stateSlotInputField.text = _currentSlot.ToString();
        }

        private void OnIncreaseButtonClicked()
        {
            ++_currentSlot;
            _currentSlot = Mathf.Min(_currentSlot, MAX_SLOT);
            _stateSlotInputField.text = _currentSlot.ToString();
        }
    }
}
