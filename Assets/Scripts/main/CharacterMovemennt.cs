using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 文字オブジェクトのクリック、ドラッグ、文字列への追加と削除を管理します。
/// </summary>
public class CharacterMovement : MonoBehaviour
{
    private Vector3 initial;
    private Vector3 offset;
    private float zPosition; // 固定したいZ座標
    private float MoziZPosition = -0.75f;

    public TextMeshPro Crystal; // クリスタルに対応する文字
    public TextMeshProUGUI embodiment; // 表示中のテキスト

    string crystalTextWithoutNewline;

    bool crystalMoji = false;

    private move aligner;
    public Materialize materialize;
    public timerGameSystem TimerGameSystem;

    public move Move;

    public AudioClip sound1;
    private AudioSource audioSource;


    private void Start()
    {
        aligner = FindObjectOfType<move>();
        initial = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(materialize.Word && transform.position.z != -0.75f && TimerGameSystem.boolTime)
        {
            transform.position = initial;
        }
    }


    // オブジェクトがクリックされたときに呼ばれる
    void OnMouseDown()
    {
        if (materialize.Word == false)
        {
            
            zPosition = -1.89f;

            if (crystalMoji) crystalMoji = false;

            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);

            // マウスのスクリーン座標をワールド座標に変換し、マウスの位置とオブジェクトの差分を記録
            // ドラッグ開始時にオブジェクトが飛ばないよう、マウス位置との差分を保持する
            offset = transform.position - GetMouseWorldPos();

            crystalTextWithoutNewline = Crystal.text.Replace("\n", "").Replace("\r", "");
            // テキストから文字を削除
            embodiment.text = embodiment.text.Replace(crystalTextWithoutNewline, "");

            aligner.RemoveObject(gameObject);
        }

    }

    // オブジェクトをドラッグしている間に呼ばれる
    void OnMouseDrag()
    {
        if (materialize.Word == false)
        {
            // マウスの位置に合わせてXY座標のみを移動させ、Z座標は固定
            Vector3 mousePosition = GetMouseWorldPos() + offset;
            transform.position = new Vector3(mousePosition.x, mousePosition.y, zPosition);
        }
    }

    // マウスのスクリーン座標をワールド座標に変換
    private Vector3 GetMouseWorldPos()
    {
        // マウスのスクリーン座標を取得
        Vector3 mousePoint = Input.mousePosition;

        // オブジェクトの現在のZ位置を維持（固定）
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }


    private void OnTriggerStay(Collider other)
    {
        if (!Input.GetMouseButton(0) && crystalMoji == false)
        {

            audioSource.PlayOneShot(sound1);

            crystalTextWithoutNewline = Crystal.text.Replace("\n", "").Replace("\r", "");

            // テキストに文字を追加
            embodiment.text += crystalTextWithoutNewline;

            aligner.AddObject(gameObject);

            crystalMoji = true;

            StartCoroutine(TimerGameSystem.MagicEffect());
        }
    }
}
