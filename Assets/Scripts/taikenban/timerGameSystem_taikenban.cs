using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class timerGameSystem_taikenban : MonoBehaviour
{
    public float timeLimit = 30f;
    private float currentTime;

    public Text timerText;

    public bool boolTime = false;

    [SerializeField] private GameObject tutorialOverlay;

    public GameObject portal;
    [SerializeField] public ParticleSystem magicCircle;

    private Vector2 targetSize = new Vector2(0.9f, 0.9f);
    private Vector2 enlargedSize = new Vector2(1f, 1f);
    private Vector2 finalSize = new Vector2(0.8f, 0.8f);

    private float initialChangeDuration = 0.2f;
    private float enlargeDuration = 0.05f;
    private float shrinkDuration = 0.05f;

    private bool hasExecuted = false;

    public TextMeshProUGUI hintTxst;
    public GameObject hukidasi;

    public int count = 0;
    public Transform rectTransform;

    public PlayerController_taikenban playerController;
    public Materialize materialize;

    public GameObject CrystalHitJudgment;

    private bool hasExecutedReservedEvent = false;

    private readonly string[] japaneseHints =
    {
        "頑張れば\r\n壊れそうな壁",
        "渡れそうな\r\n物があれば",
    };

    private readonly string[] englishHints =
    {
        "A wall that\r\nlooks breakable",
        "If there was\r\nsomething to cross",
    };

    private readonly string[] chineseHints =
    {
        "看起来只要努力\r\n就能打破的墙",
        "如果有东西\r\n可以渡过去",
    };

    private readonly string[] koreanHints =
    {
        "노력하면\r\n부술 수 있을 것 같은 벽",
        "건널 수 있는\r\n무언가가 있다면",
    };

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    private void Start()
    {
        currentTime = timeLimit;

        UpdateTimerText();

        hintTxst.text = "";
        UpdateFontSize();

        tutorialOverlay.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Update()
    {

        if (playerController.GameClear)
        {
            timerText.text = "";
            magicCircle.Stop();
        }

        if (tutorialOverlay.activeSelf && Input.GetMouseButtonDown(0))
        {
            tutorialOverlay.SetActive(false);
            Time.timeScale = 1f;
        }

        if (materialize.Word && currentTime > 5)
        {
            magicCircle.Stop();
            currentTime = 5;
        }

        if (currentTime > 0)
        {
            boolTime = false;

            currentTime -= Time.deltaTime;

            if (currentTime < 0)
            {
                currentTime = 0;
            }

            UpdateTimerText();
        }
        else
        {
            TimeUp();
            boolTime = true;
        }

        if (currentTime < 15 && !hasExecuted)
        {
            ShowHintAnimation();

            hasExecuted = true;
        }
    }

    private void ShowHintAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScale(targetSize, initialChangeDuration).SetEase(Ease.Linear));
        sequence.Append(rectTransform.DOScale(enlargedSize, enlargeDuration).SetEase(Ease.Linear));
        sequence.Append(rectTransform.DOScale(finalSize, shrinkDuration).SetEase(Ease.Linear));

        sequence.OnComplete(() =>
        {
            if (count >= 0 && count < japaneseHints.Length)
            {
                hintTxst.text = GetHint(count);
            }

            count++;
        });
    }

    private string GetHint(int index)
    {
        GameLanguage language = GameLanguage.Japanese;

        if (LanguageManager.Instance != null)
        {
            language = LanguageManager.Instance.CurrentLanguage;
        }

        switch (language)
        {
            case GameLanguage.English:
                return englishHints[index];

            case GameLanguage.Chinese:
                return chineseHints[index];

            case GameLanguage.Korean:
                return koreanHints[index];

            case GameLanguage.Japanese:
            default:
                return japaneseHints[index];
        }
    }

    private void UpdateFontSize()
    {
        if (hintTxst == null)
        {
            return;
        }

        GameLanguage language = GameLanguage.Japanese;

        if (LanguageManager.Instance != null)
        {
            language = LanguageManager.Instance.CurrentLanguage;
        }

        switch (language)
        {
            case GameLanguage.Japanese:
                hintTxst.fontSize = 23.4f;
                break;

            case GameLanguage.English:
                hintTxst.fontSize = 12f;
                break;

            case GameLanguage.Chinese:
                hintTxst.fontSize = 23.4f;
                break;

            case GameLanguage.Korean:
                hintTxst.fontSize = 23.4f;
                break;
        }
    }

    private void OnLanguageChanged(GameLanguage language)
    {
        UpdateFontSize();

        int currentHintIndex = count - 1;

        if (currentHintIndex >= 0 && currentHintIndex < japaneseHints.Length)
        {
            hintTxst.text = GetHint(currentHintIndex);
            if (currentTime >= 15)
            {
                hintTxst.text = "";
            }
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void TimeUp()
    {
        magicCircle.Stop();
        CrystalHitJudgment.SetActive(false);

        

        if (materialize.Word && !hasExecutedReservedEvent)
        {
            hasExecutedReservedEvent = true;
            materialize.ExecuteReservedEvent();
        }

        if (playerController.nxstStage)
        {

            hintTxst.text = "";
            hasExecuted = false;
            hasExecutedReservedEvent = false;

            materialize.ResetWord();

            hukidasi.transform.localScale = Vector3.zero;

            playerController.nxstStage = false;

            CrystalHitJudgment.SetActive(true);


            if (playerController.GameClear)
            {
                timerText.text = "";
                magicCircle.Stop();
            }
            else
            {
                magicCircle.Play();
                currentTime = 30;
            }
        }
    }

    public IEnumerator MagicEffect()
    {
        yield return new WaitForSeconds(0.2f);

        magicCircle.Stop();

        StartCoroutine(di(0.5f));
    }

    private IEnumerator di(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!materialize.Word)
        {
            magicCircle.Play();
        }
    }
}