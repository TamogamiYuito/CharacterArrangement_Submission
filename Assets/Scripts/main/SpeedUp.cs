using UnityEngine;

/// <summary>
/// 左Shiftキー入力中のみゲーム全体の時間速度を変更します。
/// </summary>
public class SpeedUp : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float normalTimeScale = 1f;

    private bool isSpeedingUp = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isSpeedingUp)
        {
            Time.timeScale = normalTimeScale * speedMultiplier;
            isSpeedingUp = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && isSpeedingUp)
        {
            Time.timeScale = normalTimeScale;
            isSpeedingUp = false;
        }
    }
}
