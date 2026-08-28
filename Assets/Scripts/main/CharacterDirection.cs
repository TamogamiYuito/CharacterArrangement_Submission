using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトを常にカメラ方向へ向け、デバッグ用の時間加速入力も処理します。
/// </summary>
public class CharacterDirection : MonoBehaviour
{
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        // メインカメラを自動的に取得
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // テキストがカメラに向くように回転させる
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Time.timeScale = 5.0f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1.0f;
        }
    }
}
