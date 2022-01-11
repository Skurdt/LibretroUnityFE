using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent]
    public sealed class UIMenuEntryDiskIndex : MonoBehaviour
    {
        [SerializeField] private Button _decreaseButton;
        [SerializeField] private Button _increaseButton;
        [SerializeField] private TMP_InputField _diskIndexInputField;
        [SerializeField] private TMP_Text _replaceDiskButtonText;

        public event System.Action<int> OnDiskIndexChanged;

        public int NumImages { get; set; } = 1;
        public int CurrentImageIndex { get; private set; } = MIN_INDEX;

        private const int MIN_INDEX                  = 0;
        private const int INPUTFIELD_CHARACTER_LIMIT = 4;

        private void Awake()
        {
            _diskIndexInputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
            _diskIndexInputField.characterLimit      = INPUTFIELD_CHARACTER_LIMIT;
        }

        private void OnEnable()
        {
            _diskIndexInputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            _decreaseButton.onClick.AddListener(OnDecreaseButtonClicked);
            _increaseButton.onClick.AddListener(OnIncreaseButtonClicked);
        }

        private void OnDisable()
        {
            _diskIndexInputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
            _decreaseButton.onClick.RemoveListener(OnDecreaseButtonClicked);
            _increaseButton.onClick.RemoveListener(OnIncreaseButtonClicked);
        }

        private void OnInputFieldValueChanged(string value)
        {
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out int intValue))
                intValue = MIN_INDEX;

            CurrentImageIndex = intValue;
            CurrentImageIndex = Mathf.Clamp(CurrentImageIndex, MIN_INDEX, NumImages);
            _replaceDiskButtonText.SetText($"Replace Disk ({CurrentImageIndex})");
            OnDiskIndexChanged?.Invoke(CurrentImageIndex);
        }

        private void OnDecreaseButtonClicked()
        {
            --CurrentImageIndex;
            CurrentImageIndex = Mathf.Max(MIN_INDEX, CurrentImageIndex);
            _diskIndexInputField.text = CurrentImageIndex.ToString();
        }

        private void OnIncreaseButtonClicked()
        {
            ++CurrentImageIndex;
            CurrentImageIndex = Mathf.Min(CurrentImageIndex, NumImages);
            _diskIndexInputField.text = CurrentImageIndex.ToString();
        }
    }
}
