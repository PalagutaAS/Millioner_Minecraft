using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Localization
{
    public class LanguageSwitcher : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private TMP_Text _textMeshProComponent;
        [SerializeField] private Text _textComponent;
        [Inject] private LocalizationManager _localizationManager;
        
        public string Key => _key;

        private void Awake()
        {
            if (String.IsNullOrEmpty(_key))
            {
                throw new NullReferenceException("_key is null");
            }
        }
        
        private void Start()
        {
            if (_localizationManager != null)
            {
                _localizationManager.RegisterLanguageSwitcher(this);
            }
        }

        public void SetText(string translation)
        {
            if (_textComponent != null)
                _textComponent.text = translation;
            else
            if (_textMeshProComponent != null)
                _textMeshProComponent.text = translation;
        }
    }
}