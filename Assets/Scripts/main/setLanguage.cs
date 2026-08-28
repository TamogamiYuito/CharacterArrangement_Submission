using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIボタンからLanguageManagerの言語変更処理を呼び出します。
/// </summary>
public class setLanguage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetEnglish()
    {
        LanguageManager.Instance.SetEnglish();
    }

    public void SetJapanese()
    {
        LanguageManager.Instance.SetJapanese();
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
