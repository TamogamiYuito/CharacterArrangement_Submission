using System;
using TMPro;
using UnityEngine;

public enum GameLanguage
{
    Japanese,
    English,
    Chinese,
    Korean
}

/// <summary>
/// ゲーム全体の言語設定を保持し、言語変更をイベントで各UIへ通知します。
/// </summary>
public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    public GameLanguage CurrentLanguage { get; private set; }
    public event Action<GameLanguage> OnLanguageChanged;

    [Header("Font Assets")]
    [SerializeField] private TMP_FontAsset japaneseFont;
    [SerializeField] private TMP_FontAsset englishFont;
    [SerializeField] private TMP_FontAsset chineseFont;
    [SerializeField] private TMP_FontAsset koreanFont;

    private const string LANGUAGE_KEY = "Language";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 前回保存した言語を読み込み、未保存の場合は日本語を使用する
            CurrentLanguage = (GameLanguage)PlayerPrefs.GetInt(LANGUAGE_KEY, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public TMP_FontAsset GetCurrentFont()
    {
        return CurrentLanguage switch
        {
            GameLanguage.Japanese => japaneseFont,
            GameLanguage.English => englishFont,
            GameLanguage.Chinese => chineseFont,
            GameLanguage.Korean => koreanFont,
            _ => japaneseFont
        };
    }

    public void SetLanguage(GameLanguage language)
    {
        Debug.Log($"SetLanguage : {language}");

        CurrentLanguage = language;

        PlayerPrefs.SetInt(LANGUAGE_KEY, (int)language);
        PlayerPrefs.Save();

        Debug.Log($"Invoke Event : {CurrentLanguage}");

        // 購読中のUIへ言語変更を通知する
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }

    public void SetJapanese() => SetLanguage(GameLanguage.Japanese);
    public void SetEnglish() => SetLanguage(GameLanguage.English);
    public void SetChinese() => SetLanguage(GameLanguage.Chinese);
    public void SetKorean() => SetLanguage(GameLanguage.Korean);
}
