using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 狼状態の移動、攻撃、死亡、人間への復帰処理を管理します。
/// </summary>
public class Wolf : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float speed = 2f;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 wolfScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 wolfScale2 = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private Vector3 humanChangeScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private float scaleDuration = 2f;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private timerGameSystem TimerGameSystem;

    private Rigidbody rb;
    private Animator anim;

    public bool IsRun { get; private set; } = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        StartWolf();
    }

    private void Update()
    {
        Move();

        if (transform.position.y < -10)
        {
            Die();
        }
    }

    private void Move()
    {
        if (!IsRun) return;

        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
    }

    // 現在のステージに応じた大きさへ変化した後、狼の移動を開始する
    public void StartWolf()
    {
        if (TimerGameSystem.count == 1)
        {
            transform.DOScale(wolfScale2, scaleDuration).OnComplete(() =>
            {
                IsRun = true;
            });
        }
        if (TimerGameSystem.count == 2 || TimerGameSystem.count == 3)
        {
            transform.DOScale(wolfScale, scaleDuration).OnComplete(() =>
        {
            IsRun = true;
        });
        }
    }

    public void StopWolf()
    {
        speed = 0f;
        IsRun = false;
    }

    public void Die()
    {
        StopWolf();
        anim.SetBool("isDie", true);

        Invoke(nameof(TitleScene), 3.5f);
    }

    public void Attack()
    {
        anim.SetBool("isAttack", true);
    }

    public void wallAttack()
    {
        anim.SetBool("isWallAttack", true);
    }

    public void ChangeToHuman()
    {
        transform.DOScale(humanChangeScale, scaleDuration).OnComplete(() =>
        {
            player.SetActive(true);
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            gameObject.SetActive(false);
        });
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            if (TimerGameSystem.count == 1)
            {
                wallAttack();
                playerController.WallBlowing();
            }
            if (TimerGameSystem.count == 3)
            {
                Attack();
            }
        }

        if (other.CompareTag("HumanChange"))
        {
            ChangeToHuman();
        }
    }
    public void TitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

}
