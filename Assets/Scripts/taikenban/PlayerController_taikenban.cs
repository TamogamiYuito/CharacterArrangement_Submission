using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlayerController_taikenban : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float normalSpeed = 2f;
    [SerializeField] private float rushSpeed = 6f;

    [Header("Stage Objects")]
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject gareki;
    [SerializeField] private GameObject Stage2Bridge;
    [SerializeField] private GameObject Wood;
    [SerializeField] private GameObject Stage;


    [Header("UI")]
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
    [SerializeField] private timerGameSystem_taikenban TimerGameSystem;
    [SerializeField] private woodBridge WoodBridge;

    public bool nxstStage = false;
    public bool GameClear = false;

    private float speed;
    private Rigidbody rb;
    private Animator anim;
    private BoxCollider boxCollider;


    private bool count_Wood = false;
    private bool count_Wood2 = false;


    private void Start()
    {
        speed = normalSpeed;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
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

    private void PlayerMove()
    {
        if (speed == normalSpeed)
        {
            anim.SetBool("isAttack", false);
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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

    public void CreateBridge()
    {
            Stage2Bridge.SetActive(true);
            BridgeTimeline.Play();
    }

    public void CreateWood()
    {
        Wood.SetActive(true);
    }

  


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (rb != null && CharacterArrangement.text == "突進")
            {
                WallBlowing();
            }
            else if (CharacterArrangement.text == "橋")
            {
                Invoke(nameof(Jump), 1.5f);
                speed = 0f;
                anim.SetBool("isIdle1", true);
            }
            else
            {
                Die();
            }
        }

        if (collision.gameObject.CompareTag("Wood") && !count_Wood2)
        {
            count_Wood2 = true;

            speed = 0f;
            anim.SetBool("isUp", true);

            rb.DOMove(new Vector3(-0.8f, 1.24f, 0f), 1f)
                .OnComplete(() =>
                {
                    speed = normalSpeed;
                    anim.SetBool("isUp", false);
                });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Next"))
        {
            MoveNextStage();
        }

        if (other.CompareTag("Wood") && !count_Wood)
        {
            count_Wood = true;

            anim.SetBool("isAttack", true);

            speed = 0f;

            WoodBridge.fall(() =>
            {
                isRun();
            });

        }

        if (other.CompareTag("Goal"))
        {
            GoalText.SetActive(true);
            GameClear = true;
        }
    }

    private void MoveNextStage()
    {
        if (GameClear) return;

        nxstStage = true;

        anim.SetBool("isRush", false);
        speed = normalSpeed;

        CharacterArrangement.text = "";

        transform.Translate(new Vector3(0, 0.2f, 0));

        Vector3 currentPosition = Stage.transform.position;
        currentPosition.x -= 15f;

        Stage.transform.position = currentPosition;
        transform.position = new Vector3(-3.65f, 0.2f, 0f);
    }

    // =========================
    // Other
    // =========================

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

    private void Jump()
    {
        anim.SetBool("isJump", true);

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 jumpDirection = transform.forward * 2f + Vector3.up * 3f;
        rb.AddForce(jumpDirection, ForceMode.Impulse);

        Invoke(nameof(TitleScene), 3.5f);
    }

    public void isRun()
    {
        speed = normalSpeed;
        anim.SetBool("isUp", false);
    }

    private void Die()
    {
        speed = 0f;
        anim.SetBool("isDown", true);

        Invoke(nameof(TitleScene), 3.5f);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}