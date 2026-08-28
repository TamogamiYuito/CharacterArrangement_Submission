using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤー移動、単語に対応したギミック、衝突処理、ステージ進行を管理します。
/// </summary>
public class PlayerController : MonoBehaviour
{
    // =========================
    // Const
    // =========================

    private const string WordRush = "突進";
    private const string WordBridge = "橋";
    private const string WordHammer = "トンカチ";

    private const string TagWall = "Wall";
    private const string TagEnemy = "Enemy";
    private const string TagWood = "Wood";
    private const string TagNail = "nail";
    private const string TagFallingGround = "FallingGround";
    private const string TagNext = "Next";
    private const string TagGoal = "Goal";

    private const string TitleSceneName = "TitleScene";

    // =========================
    // Inspector
    // =========================

    [Header("Move Settings")]
    [SerializeField] private float normalSpeed = 2f;
    [SerializeField] private float rushSpeed = 6f;

    [Header("Stage Objects")]
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject gareki;
    [SerializeField] private GameObject Moon;
    [SerializeField] private GameObject Stage1Bridge;
    [SerializeField] private GameObject Stage2Bridge;
    [SerializeField] private GameObject Wood;
    [SerializeField] private GameObject Stage;
    [SerializeField] private GameObject Wolf;
    [SerializeField] private GameObject obstacle;
    [SerializeField] private GameObject Hammer;
    [SerializeField] private GameObject Nail;

    [Header("Basement Objects")]
    [SerializeField] private GameObject ground1;
    [SerializeField] private GameObject ground2;
    [SerializeField] private GameObject ground3;
    [SerializeField] private GameObject ground4;
    [SerializeField] private GameObject basement;

    [Header("UI")]
    [SerializeField] private GameObject RetryButton;
    [SerializeField] private GameObject GoalText;
    [SerializeField] private TextMeshProUGUI CharacterArrangement;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector BridgeTimeline;

    [Header("Wall Blowing Settings")]
    [SerializeField] private string targetName = "wall";
    [SerializeField] private float moveDistanceX = 10f;
    [SerializeField] private float randomRangeY = 5f;
    [SerializeField] private float randomRangeZ = 5f;
    [SerializeField] private float duration = 1f;

    [Header("References")]
    [SerializeField] private timerGameSystem TimerGameSystem;
    [SerializeField] private woodBridge WoodBridge;

    // =========================
    // Public
    // =========================

    public bool nxstStage = false;
    public bool GameClear = false;

    // =========================
    // Private
    // =========================

    private float speed;
    private Rigidbody rb;
    private Animator anim;
    private BoxCollider nailBoxCollider;

    private bool count_tuki = false;
    private bool count_hasi = false;
    private bool count_nail = false;
    private bool count_Wood = false;
    private bool count_Wood2 = false;

    // =========================
    // Unity Event
    // =========================

    private void Start()
    {
        speed = normalSpeed;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        if (Nail != null)
        {
            nailBoxCollider = Nail.GetComponent<BoxCollider>();
        }
    }

    private void Update()
    {
        if (TimerGameSystem.boolTime || GameClear)
        {
            PlayerMove();
        }

        if (transform.position.y < -10)
        {
            Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 衝突対象のタグごとに処理を分け、各ギミック用メソッドへ委譲する
        switch (collision.gameObject.tag)
        {
            case TagWall:
                HitWall();
                break;

            case TagEnemy:
                Die();
                break;

            case TagWood:
                HitWoodCollision();
                break;

            case TagNail:
                HitNail();
                break;

            case TagFallingGround:
                HitFallingGround();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case TagNext:
                MoveNextStage();
                break;

            case TagWood:
                HitWoodTrigger();
                break;

            case TagGoal:
                HitGoal();
                break;
        }
    }

    // =========================
    // Move
    // =========================

    private void PlayerMove()
    {
        if (speed == normalSpeed)
        {
            anim.SetBool("isAttack", false);
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void StopPlayer()
    {
        speed = 0f;
    }

    private void MovePlayerTo(Vector3 targetPosition, TweenCallback onComplete)
    {
        StopPlayer();
        anim.SetBool("isUp", true);

        rb.DOMove(targetPosition, 1f)
            .OnComplete(onComplete);
    }

    public void isRun()
    {
        speed = normalSpeed;
        anim.SetBool("isUp", false);
    }

    // =========================
    // 単語完成時に呼ばれる関数
    // =========================

    public void StartRush()
    {
        speed = rushSpeed;
        anim.SetBool("isRush", true);
    }

    public void OpenHole()
    {
        wall.SetActive(false);
        gareki.SetActive(false);
    }

    public void OpenBasement()
    {
        SetBasementState(true);
    }

    public void ChangeToWolf()
    {
        if (count_tuki) return;

        count_tuki = true;

        Moon.SetActive(true);
        Moon.transform.DOMove(new Vector3(-3.5f, 1f, 2.2f), 8f);

        StopPlayer();

        rb.DORotate(Vector3.zero, 1f)
            .SetDelay(1f)
            .OnComplete(() =>
            {
                anim.SetBool("isIdle", true);
            });

        Invoke(nameof(WolfMode), 6f);
    }

    private void WolfMode()
    {
        speed = normalSpeed;
        anim.SetBool("isIdle", false);

        Wolf.SetActive(true);
        gameObject.SetActive(false);
    }

    public void CreateBridge()
    {
        if (TimerGameSystem.count == 1)
        {
            Stage1Bridge.SetActive(true);
            return;
        }

        if (TimerGameSystem.count == 2 && !count_hasi)
        {
            count_hasi = true;

            Stage2Bridge.SetActive(true);
            BridgeTimeline.Play();
        }
    }

    public void CreateWood()
    {
        Wood.SetActive(true);
    }

    public void CreateHammer()
    {
        Hammer.SetActive(true);

        count_nail = true;

        if (nailBoxCollider != null)
        {
            nailBoxCollider.isTrigger = true;
        }
    }

    // =========================
    // Hit Collision
    // =========================

    private void HitWall()
    {
        if (CharacterArrangement.text == WordRush)
        {
            WallBlowing();
            return;
        }

        if (CharacterArrangement.text == WordBridge)
        {
            Invoke(nameof(Jump), 1.5f);
            StopPlayer();
            anim.SetBool("isIdle1", true);
            return;
        }
        Die();
    }

    private void HitWoodCollision()
    {
        if (count_Wood2) return;

        count_Wood2 = true;

        MovePlayerTo(new Vector3(-0.8f, 1.24f, 0f), () =>
        {
            speed = normalSpeed;
            anim.SetBool("isUp", false);
        });
    }

    private void HitNail()
    {
        if (count_nail) return;

        count_nail = true;

        MovePlayerTo(new Vector3(2.32f, 1.54f, 0f), () =>
        {
            Jump();
        });
    }

    private void HitFallingGround()
    {
        if (CharacterArrangement.text == WordHammer) return;

        Destroy(obstacle);
    }

    // =========================
    // Hit Trigger
    // =========================

    private void HitWoodTrigger()
    {
        // 同じ橋ギミックが複数回実行されることを防ぐ
        if (count_Wood) return;

        count_Wood = true;

        anim.SetBool("isAttack", true);
        StopPlayer();

        WoodBridge.fall(() =>
        {
            isRun();
        });
    }

    private void HitGoal()
    {
        GoalText.SetActive(true);
        GameClear = true;
    }

    // =========================
    // Stage
    // =========================

    private void MoveNextStage()
    {
        if (GameClear) return;

        nxstStage = true;

        anim.SetBool("isRush", false);
        speed = normalSpeed;

        CharacterArrangement.text = "";

        SetBasementState(false);

        transform.Translate(new Vector3(0f, 0.2f, 0f));

        Vector3 stagePosition = Stage.transform.position;
        stagePosition.x -= 15f;
        Stage.transform.position = stagePosition;

        transform.position = new Vector3(-3.65f, 0.2f, 0f);

        if (count_tuki)
        {
            Moon.SetActive(false);
        }
    }

    private void SetBasementState(bool isOpen)
    {
        ground1.SetActive(!isOpen);
        ground2.SetActive(!isOpen);
        ground3.SetActive(!isOpen);
        ground4.SetActive(!isOpen);

        basement.SetActive(isOpen);
    }

    // =========================
    // Wall Blowing
    // =========================

    // 対象となる壁パーツへRigidbodyを付与し、吹き飛ぶ演出を再生する
    public void WallBlowing()
    {
        wall.SetActive(false);

        Transform[] allTransforms = GameObject.FindObjectsOfType<Transform>();
        List<GameObject> targetObjects = new List<GameObject>();

        foreach (Transform objTransform in allTransforms)
        {
            if (objTransform.name == targetName)
            {
                targetObjects.Add(objTransform.gameObject);
            }
        }

        foreach (GameObject targetObject in targetObjects)
        {
            Rigidbody targetRb = targetObject.GetComponent<Rigidbody>();

            if (targetRb == null)
            {
                targetRb = targetObject.AddComponent<Rigidbody>();
                targetRb.useGravity = true;
            }

            float randomY = Random.Range(-randomRangeY, randomRangeY);
            float randomZ = Random.Range(-randomRangeZ, randomRangeZ);

            Vector3 targetPosition =
                targetRb.position + new Vector3(moveDistanceX, randomY, randomZ);

            targetRb.DOMove(targetPosition, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    Destroy(targetRb.gameObject, 5.0f);
                });
        }

        if (targetObjects.Count == 0)
        {
            Debug.LogWarning($"{targetName} という名前のオブジェクトが見つかりませんでした。");
        }
    }

    // =========================
    // Action
    // =========================

    private void Jump()
    {
        anim.SetBool("isJump", true);

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 jumpDirection = transform.forward * 2f + Vector3.up * 3f;
        rb.AddForce(jumpDirection, ForceMode.Impulse);

        Invoke(nameof(TitleScene), 3.5f);
    }

    private void Die()
    {
        StopPlayer();
        anim.SetBool("isDown", true);

        Invoke(nameof(TitleScene), 3.5f);
    }

    // =========================
    // Scene
    // =========================

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TitleScene()
    {
        SceneManager.LoadScene(TitleSceneName);
    }
}
