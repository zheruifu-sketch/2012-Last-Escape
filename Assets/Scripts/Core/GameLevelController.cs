using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameLevelController : MonoBehaviour
{
    [Serializable]
    private class LevelConfig
    {
        [SerializeField] private string levelName = "Level";
        [SerializeField] private List<PlayerFormType> unlockedForms = new List<PlayerFormType>();
        [SerializeField] private List<ZoneType> allowedZones = new List<ZoneType>();

        public LevelConfig()
        {
        }

        public LevelConfig(string levelName, List<PlayerFormType> unlockedForms, List<ZoneType> allowedZones)
        {
            this.levelName = levelName;
            this.unlockedForms = unlockedForms;
            this.allowedZones = allowedZones;
        }

        public string LevelName => levelName;
        public List<PlayerFormType> UnlockedForms => unlockedForms;
        public List<ZoneType> AllowedZones => allowedZones;
    }

    [Header("Levels")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private List<LevelConfig> levels = new List<LevelConfig>();

    [Header("Debug")]
    [SerializeField] private bool enableDebugHotkeys = true;
    [SerializeField] private GameSessionController sessionController;

    public static GameLevelController Instance { get; private set; }

    public int LevelCount => levels.Count;
    public int CurrentLevelIndex { get; private set; }
    public int CurrentLevelNumber => CurrentLevelIndex + 1;
    public string CurrentLevelName
    {
        get
        {
            LevelConfig config = GetCurrentLevelConfig();
            return config != null && !string.IsNullOrWhiteSpace(config.LevelName)
                ? config.LevelName
                : $"Level {CurrentLevelNumber}";
        }
    }

    public event Action<int> LevelChanged;

    public static GameLevelController GetOrCreateInstance()
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameLevelController existing = FindObjectOfType<GameLevelController>();
        if (existing != null)
        {
            Instance = existing;
            return existing;
        }

        GameObject controllerObject = new GameObject("GameLevelController");
        return controllerObject.AddComponent<GameLevelController>();
    }

    private void Reset()
    {
        EnsureDefaultLevels();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameLevelController found. Destroying the new instance.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureDefaultLevels();
        GameFlowController.GetOrCreateInstance();
        sessionController = sessionController != null ? sessionController : GameSessionController.GetOrCreate();
        int initialLevel = sessionController != null && sessionController.HasActiveRun
            ? sessionController.CurrentLevelNumber
            : startingLevel;
        SetLevel(initialLevel - 1, false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!enableDebugHotkeys)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetLevel(0);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SetLevel(1);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            SetLevel(2);
        }
    }

    public void SetLevel(int levelIndex, bool notifyListeners = true)
    {
        if (levels.Count == 0)
        {
            CurrentLevelIndex = 0;
            return;
        }

        int clampedIndex = Mathf.Clamp(levelIndex, 0, levels.Count - 1);
        bool changed = clampedIndex != CurrentLevelIndex;
        CurrentLevelIndex = clampedIndex;
        if (sessionController != null && sessionController.HasActiveRun)
        {
            sessionController.ResumeLevel(CurrentLevelNumber);
        }

        if (notifyListeners && changed)
        {
            LevelChanged?.Invoke(CurrentLevelIndex);
        }
    }

    public void NextLevel()
    {
        SetLevel(CurrentLevelIndex + 1);
    }

    public void PreviousLevel()
    {
        SetLevel(CurrentLevelIndex - 1);
    }

    public bool IsFormUnlocked(PlayerFormType formType)
    {
        LevelConfig config = GetCurrentLevelConfig();
        if (config == null || config.UnlockedForms == null || config.UnlockedForms.Count == 0)
        {
            return true;
        }

        return config.UnlockedForms.Contains(formType);
    }

    public bool IsZoneAllowed(ZoneType zoneType)
    {
        LevelConfig config = GetCurrentLevelConfig();
        if (config == null || config.AllowedZones == null || config.AllowedZones.Count == 0)
        {
            return true;
        }

        return config.AllowedZones.Contains(zoneType);
    }

    public PlayerFormType GetFallbackUnlockedForm(PlayerFormType preferredForm = PlayerFormType.Human)
    {
        LevelConfig config = GetCurrentLevelConfig();
        if (config == null || config.UnlockedForms == null || config.UnlockedForms.Count == 0)
        {
            return preferredForm;
        }

        if (config.UnlockedForms.Contains(preferredForm))
        {
            return preferredForm;
        }

        return config.UnlockedForms[0];
    }

    private LevelConfig GetCurrentLevelConfig()
    {
        if (levels.Count == 0)
        {
            return null;
        }

        int safeIndex = Mathf.Clamp(CurrentLevelIndex, 0, levels.Count - 1);
        return levels[safeIndex];
    }

    private void EnsureDefaultLevels()
    {
        if (levels.Count > 0)
        {
            return;
        }

        levels.Add(CreateLevel(
            "Level 1",
            new List<PlayerFormType> { PlayerFormType.Human, PlayerFormType.Car },
            new List<ZoneType> { ZoneType.Road, ZoneType.Blizzard }));

        levels.Add(CreateLevel(
            "Level 2",
            new List<PlayerFormType> { PlayerFormType.Human, PlayerFormType.Car, PlayerFormType.Boat },
            new List<ZoneType> { ZoneType.Road, ZoneType.Blizzard, ZoneType.Water }));

        levels.Add(CreateLevel(
            "Level 3",
            new List<PlayerFormType> { PlayerFormType.Human, PlayerFormType.Car, PlayerFormType.Boat, PlayerFormType.Plane },
            new List<ZoneType> { ZoneType.Road, ZoneType.Blizzard, ZoneType.Water, ZoneType.Cliff }));
    }

    private static LevelConfig CreateLevel(string levelName, List<PlayerFormType> unlockedForms, List<ZoneType> allowedZones)
    {
        return new LevelConfig(levelName, unlockedForms, allowedZones);
    }
}
