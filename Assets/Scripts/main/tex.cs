using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テキストなどのオブジェクトを常にカメラ方向へ向けます。
/// </summary>
public class tex : MonoBehaviour
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
    }
}
