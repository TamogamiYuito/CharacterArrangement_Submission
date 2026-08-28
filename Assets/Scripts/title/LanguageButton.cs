using UnityEngine;

/// <summary>
/// 言語選択ボタンからLanguageManagerへ変更要求を送ります。
/// </summary>
public class LanguageButton : MonoBehaviour
{
    public void SetJapanese()
    {
        LanguageManager.Instance.SetJapanese();
    }

    public void SetEnglish()
    {
        LanguageManager.Instance.SetEnglish();
    }

    public void SetChinese()
    {
        LanguageManager.Instance.SetChinese();
    }

    public void SetKorean()
    {
        LanguageManager.Instance.SetKorean();
    }
}
