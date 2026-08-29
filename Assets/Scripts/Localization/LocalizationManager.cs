using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VContainer;
using YG;
using Random = UnityEngine.Random;

namespace Localization
{
    public class LocalizationManager : IDisposable
    {
        private string _currentLanguage;
    
        private Dictionary<string, Dictionary<string, string>> translations;
        private Dictionary<string, Dictionary<string, string>> _rawTranslations;
        private Dictionary<string, SortedDictionary<int, Dictionary<string, string>>> _variantTranslations;
        
        private List<LanguageSwitcher> languageSwitchers;
        public string[] AvailableLanguages { get; private set; }

        [Inject] private SaveService _saveService;
        [Inject] private ITranslationLoaderService _translationLoaderService;
        public void Initialize()
        {
            translations = new Dictionary<string, Dictionary<string, string>>();
            _variantTranslations = new Dictionary<string, SortedDictionary<int, Dictionary<string, string>>>();
            _rawTranslations = new Dictionary<string, Dictionary<string, string>>();
            languageSwitchers = new List<LanguageSwitcher>();
        
            LoadTranslations();
            YG2.onSwitchLang += SetLanguage;
            SetLanguage(_saveService.Data.currentLang);
        }
    
        /// <summary>
        /// Загрузка переводов из CSV файла
        /// </summary>
        private void LoadTranslations()
        {
            if (_translationLoaderService.IsLoaded && !string.IsNullOrEmpty(_translationLoaderService.CSV))
                ParseCSV(_translationLoaderService.CSV);
            else
                Debug.LogWarning("Translation CSV not loaded yet!");
        }
    
        /// <summary>
        /// Парсинг CSV данных
        /// </summary>
        private void ParseCSV(string csvText)
        {
            using (StringReader reader = new StringReader(csvText))
            {
                string line;
                bool isFirstLine = true;
                List<string> headers = new List<string>();
            
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                
                    // Простой парсер CSV (для сложных случаев используйте готовые библиотеки)
                    string[] values = ParseCSVLine(line);
                
                    if (isFirstLine)
                    {
                        // Первая строка - заголовки (key, ru, en, etc.)
                        headers = values.ToList();
                        AvailableLanguages = headers.Skip(1).ToArray(); // Все языки кроме "key"
                        isFirstLine = false;
                        continue;
                    }
                
                    if (values.Length < 2)
                        continue;
                
                    string key = values[0];
                    
                    if (TryGetVariantKey(key, out string baseKey, out int variantNumber))
                    {
                        // Это вариант – сохраняем в отдельный словарь
                        if (!_variantTranslations.ContainsKey(baseKey))
                        {
                            _variantTranslations[baseKey] = new SortedDictionary<int, Dictionary<string, string>>();
                        }
                    
                        var variantDict = new Dictionary<string, string>();
                        for (int i = 1; i < Math.Min(values.Length, headers.Count); i++)
                        {
                            variantDict[headers[i]] = values[i].Replace("\\n", "\n");
                        }
                        _variantTranslations[baseKey][variantNumber] = variantDict;
                    }
                    else
                    {
                        // Обычный ключ
                        var translationsForLanguage = new Dictionary<string, string>();
                        for (int i = 1; i < Math.Min(values.Length, headers.Count); i++)
                        {
                            translationsForLanguage[headers[i]] = values[i].Replace("\\n", "\n");
                        }
                        _rawTranslations[key] = translationsForLanguage;
                        translations[key] = translationsForLanguage;
                    }
                }
            }
            
            Debug.Log($"Loaded {translations.Count} regular keys, {_variantTranslations.Count} variant keys");
        }
        
        /// <summary>
        /// Форматирует шаблон перевода по ключу, подставляя аргументы, и обновляет все связанные тексты.
        /// </summary>
        /// <param name="key">Ключ перевода (шаблон с {0}, один параметр)</param>
        /// <param name="args">Аргументы для подстановки</param>
        /// <returns>Отформатированная строка для текущего языка или null при ошибке</returns>
        public void FormatText(string key, string args)
        {
            if (!_rawTranslations.TryGetValue(key, out var templates))
            {
                Debug.LogWarning($"No template for key '{key}'");
                return;
            }

            var formatted = new Dictionary<string, string>();
            foreach (var kvp in templates)
            {
                string lang = kvp.Key;
                string template = kvp.Value;
                try
                {
                    formatted[lang] = string.Format(template, args);
                }
                catch (FormatException e)
                {
                    Debug.LogError($"Error formatting key '{key}' for language '{lang}': {e.Message}");
                    formatted[lang] = template;
                }
            }

            translations[key] = formatted;
            UpdateTextsForKey(key);
        }
    
        /// <summary>
        /// Пытается распознать вариантный ключ вида baseKey_N.
        /// </summary>
        private bool TryGetVariantKey(string key, out string baseKey, out int variantNumber)
        {
            baseKey = null;
            variantNumber = -1;
        
            int lastUnderscore = key.LastIndexOf('_');
            if (lastUnderscore > 0 && lastUnderscore < key.Length - 1)
            {
                string suffix = key.Substring(lastUnderscore + 1);
                if (int.TryParse(suffix, out int num))
                {
                    baseKey = key.Substring(0, lastUnderscore);
                    variantNumber = num;
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Простой парсер CSV строки (учитывает кавычки)
        /// </summary>
        private string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string currentValue = "";
        
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
            
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ';' && !inQuotes)
                {
                    result.Add(currentValue);
                    currentValue = "";
                }
                else
                {
                    currentValue += c;
                }
            }
        
            result.Add(currentValue);
            return result.ToArray();
        }
    
        /// <summary>
        /// Установка языка
        /// </summary>
        private void SetLanguage(string language)
        {
            if (AvailableLanguages == null || !AvailableLanguages.Contains(language))
            {
                Debug.LogWarning($"Language '{language}' is not available!");
                language = "en";
            }
        
            _currentLanguage = language;
            UpdateAllTexts();
        
            Debug.Log($"Language changed to: {_currentLanguage}");
        }
    
        /// <summary>
        /// Получение перевода по ключу
        /// </summary>
        private string GetTranslation(string key)
        {
            if (translations.TryGetValue(key, out var languageTranslations))
            {
                if (languageTranslations.TryGetValue(_currentLanguage, out string translation))
                {
                    return translation;
                }
                else
                {
                    Debug.LogWarning($"Translation for key '{key}' not found in language '{_currentLanguage}'");
                }
            }
            else
            {
                Debug.LogWarning($"Key '{key}' not found in translations");
            }
        
            return key; // Возвращаем ключ как fallback
        }
        
        /// <summary>
        /// Выбирает случайный вариант по базовому ключу, форматирует его с аргументом
        /// и сохраняет результат в основной словарь translations.
        /// </summary>
        public void RandomFormatTextByKey(string baseKey, string argument, int variantNumber = 0)
        {
            if (!_variantTranslations.TryGetValue(baseKey, out var sortedVariants))
            {
                Debug.LogWarning($"No variants for key '{baseKey}'");
                return;
            }
        
            if (sortedVariants.Count == 0)
            {
                Debug.LogWarning($"Empty variants for key '{baseKey}'");
                return;
            }
        
            Dictionary<string, string> selectedVariant;

            if (variantNumber != 0)
            {
                // Пытаемся найти указанный вариант
                if (!sortedVariants.TryGetValue(variantNumber, out selectedVariant))
                {
                    Debug.LogWarning($"Variant {variantNumber} not found for key '{baseKey}', using random");
                    selectedVariant = sortedVariants.Values.ElementAt(Random.Range(0, sortedVariants.Count));
                }
            }
            else
            {
                selectedVariant = sortedVariants.Values.ElementAt(Random.Range(0, sortedVariants.Count));
            }
        
            // Форматируем шаблоны для каждого языка и сохраняем в translations
            var formattedTranslations = new Dictionary<string, string>();
            foreach (var kvp in selectedVariant)
            {
                string language = kvp.Key;
                string template = kvp.Value;
                string formatted = string.Format(template, argument);
                formattedTranslations[language] = formatted;
            }
        
            translations[baseKey] = formattedTranslations;
        
            // Обновляем тексты у всех LanguageSwitcher с этим ключом
            UpdateTextsForKey(baseKey);
        }
        
        private void UpdateTextsForKey(string key)
        {
            foreach (var switcher in languageSwitchers)
            {
                if (switcher != null && switcher.Key == key)
                {
                    switcher.SetText(GetTranslation(key));
                }
            }
        }
    
        /// <summary>
        /// Обновление всех текстов
        /// </summary>
        private void UpdateAllTexts()
        {
            foreach (var switcher in languageSwitchers)
            {
                if (switcher != null)
                {
                    string translation = GetTranslation(switcher.Key);
                    switcher.SetText(translation);
                }
            }
        }
        
        /// <summary>
        /// Регистрация LanguageSwitcher (для динамически создаваемых объектов)
        /// </summary>
        public void RegisterLanguageSwitcher(LanguageSwitcher switcher)
        {
            if (!languageSwitchers.Contains(switcher))
            {
                languageSwitchers.Add(switcher);
            
                // Сразу обновляем текст для нового объекта
                if (switcher != null)
                {
                    switcher.SetText(GetTranslation(switcher.Key));
                }
            }
        }
    
        /// <summary>
        /// Удаление LanguageSwitcher из списка
        /// </summary>
        public void UnregisterLanguageSwitcher(LanguageSwitcher switcher) => languageSwitchers.Remove(switcher);
        
        public void Dispose() => YG2.onSwitchLang -= SetLanguage;
    }
}