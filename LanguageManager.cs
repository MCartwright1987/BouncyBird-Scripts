using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;

    // Array of GameObjects, each containing a Text component for a different language
    private void Awake()
    {
        instance = this;
    }

    public void SetLanguageToChinese()
    {
        SetLanguage("zh-Hans"); // Simplified Chinese locale code
    }

    public void SetLanguageToEnglish()
    {
        SetLanguage("en"); // English locale code
    }

    public void SetLanguageToFrench()
    {
        SetLanguage("fr"); // French locale code
    }

    public void SetLanguageToJapanese()
    {
        SetLanguage("ja"); // Japanese locale code
    }

    public void SetLanguageToKorean()
    {
        SetLanguage("ko"); // Korean locale code
    }

    public void SetLanguageToPortuguese()
    {
        SetLanguage("pt"); // Portuguese locale code
    }

    public void SetLanguageToRussian()
    {
        SetLanguage("ru"); // Russian locale code
    }

    public void SetLanguageToSpanish()
    {
        SetLanguage("es"); // Spanish locale code
    }

    public void SetLanguage(string localeCode)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetString("Language", localeCode);
            Debug.Log("Language set to: " + locale.LocaleName);
        }
        else
        {
            Debug.LogError("Locale not found: " + localeCode);
        }
    }

    public void StartLanguageCycle()
    {
        StartCoroutine(CycleThroughLanguages());
    }
    
    private IEnumerator CycleThroughLanguages()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        int index = 0;
    
        while (true)
        {
            // Set the current locale
            LocalizationSettings.SelectedLocale = locales[index];
            Debug.Log("Cycling language: " + locales[index].LocaleName);
    
            // Wait for 0.5 seconds
            yield return new WaitForSeconds(0.5f);
    
            // Increment the index and wrap around if necessary
            index = (index + 1) % locales.Count;
        }
    }
}

