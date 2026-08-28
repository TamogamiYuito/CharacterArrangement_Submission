using UnityEditor;
using UnityEngine;

/// <summary>
/// タイトル画面のメインメニューと設定画面の表示切替を管理します。
/// </summary>
public class MenuUIController : MonoBehaviour
{
    [Header("メインメニュー")]
    [SerializeField] private GameObject mainMenuPanel;

    [Header("設定画面")]
    [SerializeField] private GameObject settingPanel;

    private void Start()
    {
        // 最初はメインメニューだけ表示
        mainMenuPanel.SetActive(true);
        settingPanel.SetActive(false);
    }

    // 設定ボタンを押したとき
    public void OpenSetting()
    {
        mainMenuPanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    // 閉じるボタンを押したとき
    public void CloseSetting()
    {
        settingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }


    public void ExitGame()
    {
#if UNITY_EDITOR
        // Unityエディタ上で再生を止める
        EditorApplication.isPlaying = false;
#else
        // ビルド後のアプリを終了する
        Application.Quit();
#endif
    }
}
