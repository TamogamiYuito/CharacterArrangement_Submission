using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// DOTweenを使用して橋が倒れる演出を再生し、完了時にコールバックを通知します。
/// </summary>
public class woodBridge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fall(System.Action onComplete)
    {
        transform.DORotate(new Vector3(transform.rotation.x, transform.rotation.y, -20f), 3f).SetEase(Ease.OutBounce).SetDelay(1f).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
