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

        public int CurrentSlot { get; private set; } = MIN_SLOT;

        private const int MIN_SLOT = 0;
        private const int MAX_SLOT = 9999;

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

            CurrentSlot = intValue;
            CurrentSlot = Mathf.Clamp(CurrentSlot, MIN_SLOT, MAX_SLOT);
            _saveStateButtonText.SetText($"Save State ({CurrentSlot})");
            _loadStateButtonText.SetText($"Load State ({CurrentSlot})");
        }

        private void OnDecreaseButtonClicked()
        {
            --CurrentSlot;
            CurrentSlot = Mathf.Max(MIN_SLOT, CurrentSlot);
            _stateSlotInputField.text = CurrentSlot.ToString();
        }

        private void OnIncreaseButtonClicked()
        {
            ++CurrentSlot;
            CurrentSlot = Mathf.Min(CurrentSlot, MAX_SLOT);
            _stateSlotInputField.text = CurrentSlot.ToString();
        }
    }
}
