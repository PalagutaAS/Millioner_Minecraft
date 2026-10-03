using Localization;
using UnityEngine;
using VContainer;

namespace UI
{
    public class LocalizationUiService : MonoBehaviour
    {
        [SerializeField] private DropdownLanguage _dropdown;
        
        private SaveService _saveService;

        [Inject] 
        private void Constructor(SaveService saveService)
        {
            _saveService = saveService;
            _dropdown.OnValueChange += UpdateDropdown;
        }

        private void UpdateDropdown(string lang)
        {
            _saveService.SetLanguage(lang);
        }
        
        public void UpdateDropdown()
        {
            _dropdown?.SetValue(_saveService.Data.currentLang);
        }

        private void OnDestroy()
        {
            _dropdown.OnValueChange -= UpdateDropdown;
        }
    }
}