using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameFlowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameLevelController levelController;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHintUI hintUI;

    [Header("Progress")]
    [SerializeField] private float[] levelTargetDistances = { 45f, 70f, 95f };
    [SerializeField] private float transitionDelay = 1.25f;

    [Header("UI")]
    [SerializeField] private string titleText = "Jump Game";
    [SerializeField] private string startButtonText = "Start";

    private Canvas rootCanvas;
    private GameObject startPanel;
    private TMP_Text startDescriptionText;
    private TMP_Text headerText;
    private TMP_Text progressText;
    private Button startButton;
    private bool isTransitioning;
    private float levelStartX;

    public static GameFlowController Instance { get; private set; }

    public static GameFlowController GetOrCreateInstance()
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameFlowController existing = FindObjectOfType<GameFlowController>();
        if (existing != null)
        {
            Instance = existing;
            return existing;
        }

        GameObject flowObject = new GameObject("GameFlowController");
        return flowObject.AddComponent<GameFlowController>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        levelController = levelController != null ? levelController : GameLevelController.GetOrCreateInstance();
        player = player != null ? player : FindPlayerTransform();
        hintUI = hintUI != null ? hintUI : FindObjectOfType<PlayerHintUI>(true);
        EnsureUi();
    }

    private void Start()
    {
        levelStartX = GetPlayerX();
        RefreshHeader();

        if (GameSessionState.HasActiveRun)
        {
            ResumeGameplay();
            ShowLevelHint();
            return;
        }

        PauseForStartScreen();
    }

    private void OnEnable()
    {
        if (levelController == null)
        {
            levelController = GameLevelController.GetOrCreateInstance();
        }

        if (levelController != null)
        {
            levelController.LevelChanged += HandleLevelChanged;
        }
    }

    private void Update()
    {
        RefreshProgressText();

        if (!GameSessionState.HasActiveRun || isTransitioning)
        {
            return;
        }

        if (GetDistanceTravelled() < GetCurrentTargetDistance())
        {
            return;
        }

        StartCoroutine(HandleLevelCompleted());
    }

    private void OnDisable()
    {
        if (levelController != null)
        {
            levelController.LevelChanged -= HandleLevelChanged;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void PauseForStartScreen()
    {
        Time.timeScale = 0f;
        isTransitioning = false;
        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        if (startDescriptionText != null)
        {
            startDescriptionText.text = BuildStartDescription();
        }
    }

    private void ResumeGameplay()
    {
        Time.timeScale = 1f;
        isTransitioning = false;
        levelStartX = GetPlayerX();
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }
    }

    private void BeginNewRun()
    {
        GameSessionState.StartNewRun();
        if (levelController != null)
        {
            levelController.SetLevel(0);
        }

        ResumeGameplay();
        ShowLevelHint();
    }

    private IEnumerator HandleLevelCompleted()
    {
        isTransitioning = true;
        Time.timeScale = 1f;

        int currentLevel = levelController != null ? levelController.CurrentLevelNumber : 1;
        int levelCount = levelController != null ? Mathf.Max(1, levelController.LevelCount) : 3;

        if (currentLevel < levelCount)
        {
            ShowHint($"Level {currentLevel} Clear", transitionDelay);
            yield return new WaitForSeconds(transitionDelay);
            GameSessionState.AdvanceLevel(levelCount);
            ReloadActiveScene();
            yield break;
        }

        ShowHint("All Levels Clear - Restarting", transitionDelay);
        yield return new WaitForSeconds(transitionDelay);
        GameSessionState.ResetRun();
        ReloadActiveScene();
    }

    private void RefreshHeader()
    {
        if (headerText == null || levelController == null)
        {
            return;
        }

        headerText.text = $"Level {levelController.CurrentLevelNumber} / {Mathf.Max(1, levelController.LevelCount)}";
    }

    private void RefreshProgressText()
    {
        if (progressText == null || levelController == null)
        {
            return;
        }

        float targetDistance = GetCurrentTargetDistance();
        float currentDistance = Mathf.Clamp(GetDistanceTravelled(), 0f, targetDistance);
        progressText.text = $"Goal {currentDistance:0}/{targetDistance:0}m";
    }

    private float GetCurrentTargetDistance()
    {
        if (levelController == null || levelTargetDistances == null || levelTargetDistances.Length == 0)
        {
            return 60f;
        }

        int index = Mathf.Clamp(levelController.CurrentLevelIndex, 0, levelTargetDistances.Length - 1);
        return Mathf.Max(1f, levelTargetDistances[index]);
    }

    private float GetDistanceTravelled()
    {
        return Mathf.Max(0f, GetPlayerX() - levelStartX);
    }

    private float GetPlayerX()
    {
        return player != null ? player.position.x : 0f;
    }

    private void ReloadActiveScene()
    {
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private void ShowLevelHint()
    {
        if (levelController == null)
        {
            return;
        }

        ShowHint($"{levelController.CurrentLevelName} Start", 1.6f);
    }

    private void ShowHint(string message, float duration)
    {
        if (hintUI != null)
        {
            hintUI.ShowHint(message, duration);
        }
    }

    private string BuildStartDescription()
    {
        float targetDistance = GetCurrentTargetDistance();
        return $"Level 1 starts with Human and Car.\nReach {targetDistance:0}m to clear the level.";
    }

    private void EnsureUi()
    {
        rootCanvas = FindObjectOfType<Canvas>();
        if (rootCanvas == null)
        {
            return;
        }

        CreateHeaderUi(rootCanvas.transform);
        CreateStartPanel(rootCanvas.transform);
    }

    private void CreateHeaderUi(Transform parent)
    {
        GameObject headerRoot = CreateUiObject("GameFlowHUD", parent);
        RectTransform rect = headerRoot.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -28f);
        rect.sizeDelta = new Vector2(420f, 80f);

        Image background = headerRoot.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.35f);

        headerText = CreateText("LevelText", headerRoot.transform, 30, FontStyles.Bold);
        SetStretch(headerText.rectTransform, new Vector2(18f, -8f), new Vector2(-18f, -36f));
        headerText.alignment = TextAlignmentOptions.Center;

        progressText = CreateText("ProgressText", headerRoot.transform, 24, FontStyles.Normal);
        SetStretch(progressText.rectTransform, new Vector2(18f, -38f), new Vector2(-18f, -8f));
        progressText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateStartPanel(Transform parent)
    {
        startPanel = CreateUiObject("GameStartPanel", parent);
        RectTransform rect = startPanel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image background = startPanel.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.72f);

        GameObject card = CreateUiObject("Card", startPanel.transform);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(720f, 360f);
        cardRect.anchoredPosition = Vector2.zero;

        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.08f, 0.11f, 0.16f, 0.96f);

        TMP_Text title = CreateText("Title", card.transform, 48, FontStyles.Bold);
        title.text = titleText;
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -38f), new Vector2(620f, 70f));
        title.alignment = TextAlignmentOptions.Center;

        startDescriptionText = CreateText("Description", card.transform, 28, FontStyles.Normal);
        SetRect(startDescriptionText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 14f), new Vector2(620f, 120f));
        startDescriptionText.alignment = TextAlignmentOptions.Center;

        GameObject buttonObject = CreateUiObject("StartButton", card.transform);
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.anchoredPosition = new Vector2(0f, 34f);
        buttonRect.sizeDelta = new Vector2(240f, 72f);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.12f, 0.62f, 0.98f, 1f);

        startButton = buttonObject.AddComponent<Button>();
        ColorBlock colors = startButton.colors;
        colors.normalColor = buttonImage.color;
        colors.highlightedColor = new Color(0.18f, 0.7f, 1f, 1f);
        colors.pressedColor = new Color(0.08f, 0.5f, 0.86f, 1f);
        colors.selectedColor = colors.highlightedColor;
        startButton.colors = colors;
        startButton.targetGraphic = buttonImage;
        startButton.onClick.AddListener(BeginNewRun);

        TMP_Text buttonLabel = CreateText("Label", buttonObject.transform, 30, FontStyles.Bold);
        buttonLabel.text = startButtonText;
        SetStretch(buttonLabel.rectTransform, Vector2.zero, Vector2.zero);
        buttonLabel.alignment = TextAlignmentOptions.Center;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
    }

    private static void SetStretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private static TMP_Text CreateText(string objectName, Transform parent, float fontSize, FontStyles fontStyle)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = Color.white;
        text.enableWordWrapping = true;
        return text;
    }

    private static Transform FindPlayerTransform()
    {
        PlayerFormRoot formRoot = FindObjectOfType<PlayerFormRoot>();
        return formRoot != null ? formRoot.transform : null;
    }

    private void HandleLevelChanged(int _)
    {
        levelStartX = GetPlayerX();
        RefreshHeader();
        RefreshProgressText();
    }
}
