using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 入力された文字列を判定し、対応する単語データとギミックイベントを管理します。
/// </summary>
public class Materialize : MonoBehaviour
{
    [Serializable]
    public class WordData
    {
        public string inputWord;

        public string completedWord; // 日本語固定

        [Header("字幕")]
        public string englishSubtitle;
        public string chineseSubtitle;
        public string koreanSubtitle;

        public UnityEvent onComplete;
    }

    [Header("日本語表示")]
    [SerializeField] private TextMeshProUGUI characterArrangement;

    [Header("字幕")]
    [SerializeField] private TextMeshProUGUI subtitleText;

    [SerializeField] private List<WordData> wordDatas = new();

    public bool Word { get; private set; }

    public WordData CurrentWordData { get; private set; }

    public UnityEvent ReservedEvent { get; private set; }

    private Dictionary<string, WordData> wordDictionary;

    private void Awake()
    {
        // 入力文字列から対応データを高速に取得できるよう、Dictionaryへ変換する
        wordDictionary = new Dictionary<string, WordData>();

        foreach (WordData data in wordDatas)
        {
            if (!wordDictionary.ContainsKey(data.inputWord))
            {
                wordDictionary.Add(data.inputWord, data);
            }
        }
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
        {
            // 言語変更時に字幕を更新する
            LanguageManager.Instance.OnLanguageChanged += UpdateLanguage;
        }
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.OnLanguageChanged -= UpdateLanguage;
        }
    }

    private void Update()
    {
        CheckWord();
    }

    // 現在入力されている文字列が登録済みの単語と一致するか判定する
    private void CheckWord()
    {
        if (characterArrangement == null)
        {
            return;
        }

        // 入力された文字列に対応する単語データが存在するか確認する
        if (!wordDictionary.TryGetValue(characterArrangement.text, out WordData data))
        {
            return;
        }

        CurrentWordData = data;

        CompleteWord(data);
    }

    public void ShowCompletedWord()
    {
        if (CurrentWordData == null)
        {
            return;
        }

        characterArrangement.text = CurrentWordData.completedWord;

        UpdateLanguage(LanguageManager.Instance.CurrentLanguage);
    }

    private void UpdateLanguage(GameLanguage language)
    {
        if (CurrentWordData == null || subtitleText == null)
        {
            return;
        }

        subtitleText.text = GetSubtitle(CurrentWordData, language);
    }

    private string GetSubtitle(WordData data, GameLanguage language)
    {
        switch (language)
        {
            case GameLanguage.English:
                return data.englishSubtitle;

            case GameLanguage.Chinese:
                return data.chineseSubtitle;

            case GameLanguage.Korean:
                return data.koreanSubtitle;

            case GameLanguage.Japanese:
            default:
                return "";
        }
    }

    // 単語完成状態へ移行し、対応するギミックイベントを予約する
    private void CompleteWord(WordData data)
    {
        Word = true;

        // 単語完成時のギミックを制限時間終了後に実行できるよう保持する
        ReservedEvent = data.onComplete;

        gameObject.SetActive(false);
    }

    public void ExecuteReservedEvent()
    {
        ReservedEvent?.Invoke();
    }

    public void ResetWord()
    {
        Word = false;

        ReservedEvent = null;

        CurrentWordData = null;

        characterArrangement.text = "";
        subtitleText.text = "";

        gameObject.SetActive(true);
    }

    public WordData GetWordData(string key)
    {
        if (wordDictionary.TryGetValue(key, out WordData data))
        {
            return data;
        }

        return null;
    }
}
