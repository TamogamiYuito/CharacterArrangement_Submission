using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 選択された文字オブジェクトを中央揃えで配置し、単語完成後の回収演出を管理します。
/// </summary>
public class move : MonoBehaviour
{
    private List<GameObject> selectedObjects = new List<GameObject>();

    public float minSpacing = 0.5f;

    public float yPosition = 2.5f;

    public Materialize materialize;

    Vector3 zero1 = new Vector3(0f, 2.5f, -0.75f);

    public void AddObject(GameObject obj)
    {
        if (!selectedObjects.Contains(obj))
        {
            selectedObjects.Add(obj);

            AlignObjects();
        }
    }

    public void RemoveObject(GameObject obj)
    {
        if (selectedObjects.Contains(obj))
        {
            selectedObjects.Remove(obj);

            AlignObjects();
        }
    }

    void AlignObjects()
    {
        int objectCount = selectedObjects.Count;

        if (objectCount == 0)
        {
            return;
        }

        // 最も幅の大きいオブジェクトを基準に間隔を決め、文字同士の重なりを防ぐ
        float spacing = Mathf.Max(GetMaxObjectWidth() + minSpacing, minSpacing);

        // オブジェクト全体が中央に並ぶよう、先頭のX座標を計算する
        float startX = -(spacing * (objectCount - 1)) / 2;

        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = selectedObjects[i];

            float xPosition = startX + spacing * i;

            Vector3 targetPosition = new Vector3(xPosition, yPosition, -0.75f);

            obj.transform
                .DOMove(targetPosition, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    if (materialize.Word)
                    {
                        foreach (GameObject targetObj in selectedObjects.ToArray())
                        {
                            targetObj.transform
                                .DOMove(zero1, 1f)
                                .SetDelay(0.8f)
                                .SetEase(Ease.OutQuad)
                                .OnComplete(() =>
                                {
                                    if (targetObj != null)
                                    {
                                        Destroy(targetObj);

                                        RemoveObject(targetObj);

                                        // 全部消えたら字幕表示
                                        if (selectedObjects.Count == 0)
                                        {
                                            materialize.ShowCompletedWord();
                                        }
                                    }
                                });
                        }
                    }
                });
        }
    }

    float GetMaxObjectWidth()
    {
        float maxWidth = 0f;

        foreach (GameObject obj in selectedObjects)
        {
            if (obj.TryGetComponent<Renderer>(out Renderer renderer))
            {
                maxWidth = Mathf.Max(maxWidth, renderer.bounds.size.x);
            }
        }

        return maxWidth > 0 ? maxWidth : 1.0f;
    }
}
