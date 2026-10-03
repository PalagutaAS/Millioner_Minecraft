using System;
using TMPro;
using UnityEngine;

namespace Localization
{
    public class DropdownLanguage : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        
        public event Action<string> OnValueChange;
        
        public void SetValue(string lang)
        {
            _dropdown.onValueChanged.AddListener(OnDropdownChanged);
            _dropdown.value = lang switch
            {
                "ru" => 0,
                "en" => 1,
                "tr" => 2,
                _ => 1
            };
            _dropdown.RefreshShownValue();
        }

        private void OnDestroy() => _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);

        private void OnDropdownChanged(int index)
        {
            string code = index switch
            {
                0 => "ru",
                1 => "en",
                2 => "tr",
                _ => "en"
            };
            OnValueChange?.Invoke(code); 
        }
    }
}