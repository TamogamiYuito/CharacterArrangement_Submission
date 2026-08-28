using TMPro;
using UnityEngine;

/// <summary>
/// ステージ進行条件に応じてゴーレムを移動させ、衝突時の死亡処理を行います。
/// </summary>
public class golemController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float speed = 2f;

    [Header("References")]
    [SerializeField] private timerGameSystem timerGameSystem;
    [SerializeField] private TextMeshProUGUI characterArrangement;
    [SerializeField] private Wolf wolf;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        TryMove();
    }

    private void TryMove()
    {
        if (!CanMove()) return;

        Run();
    }

    private bool CanMove()
    {
        if (timerGameSystem.count != 3) return false;
        if (!timerGameSystem.boolTime) return false;

        if (characterArrangement.text == "月")
        {
            return wolf.IsRun;
        }

        return true;
    }

    private void Run()
    {
        anim.SetBool("isRun", true);
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
    }

    public void Die()
    {
        speed = 0f;
        anim.SetBool("isDie", true);
        capsuleCollider.isTrigger = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wolf"))
        {
            Die();
        }
    }
}
