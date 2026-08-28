using UnityEngine;
using DG.Tweening;

/// <summary>
/// DOTweenのSequenceを使用して、トンカチで釘を段階的に打ち込む演出を再生します。
/// </summary>
public class HammerNailAnimation : MonoBehaviour
{
    [SerializeField] private Transform hammer; // トンカチのTransform
    [SerializeField] private Transform nail;   // 釘のTransform
    [SerializeField] private float hammerMoveDistance = 0.5f; // トンカチの上下移動距離
    [SerializeField] private float hammerAnimationTime = 0.2f; // トンカチアニメーションの時間
    [SerializeField] private float hammerRotateAngle = 45f; // トンカチの回転角度
    [SerializeField] private float nailMovePerHit = 0.1f; // 1回の打ち込み距離

    private void Start()
    {
        Invoke("AnimateHammerAndNail", 1f);
    }

    // 必要な打撃回数を計算し、トンカチと釘の連続アニメーションを組み立てる
    private void AnimateHammerAndNail()
    {
        float nailInitialY = nail.localPosition.y;
        float nailTargetY = -0.23f; // Y=0まで打ち込む
        int hitCount = Mathf.CeilToInt((nailInitialY - nailTargetY) / nailMovePerHit); // 必要な打ち込み回数

        float initialHammerY = hammer.position.y; // トンカチ初期Y位置

        Sequence animationSequence = DOTween.Sequence();

        for (int i = 0; i < hitCount; i++)
        {
            float targetNailY = Mathf.Max(nail.localPosition.y - (i + 1) * nailMovePerHit, nailTargetY); // 次の釘位置
            float hammerTargetY = initialHammerY - (i + 1) * nailMovePerHit; // トンカチ基準位置調整

            // トンカチが上に上がる動作と回転（X軸回転のみ）
            animationSequence.Append(hammer.DOMoveY(hammerTargetY + hammerMoveDistance, hammerAnimationTime)
                .SetEase(Ease.OutCubic));
            animationSequence.Join(hammer.DORotate(new Vector3(-90f + hammerRotateAngle, -90f, 90f), hammerAnimationTime)
                .SetEase(Ease.OutCubic));

            // トンカチが下に降りる動作 + 回転復帰 + 釘を打ち込む動作
            animationSequence.Append(hammer.DOMoveY(hammerTargetY, hammerAnimationTime)
                .SetEase(Ease.InCubic));
            animationSequence.Join(hammer.DORotate(new Vector3(-90f, -90f, 90f), hammerAnimationTime)
                .SetEase(Ease.InCubic));

            // 釘の動きを動的に更新
            animationSequence.Join(nail.DOLocalMoveY(targetNailY, hammerAnimationTime)
                .SetEase(Ease.Linear));
        }

        animationSequence.Play();
    }
}
