using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameProgressionConfig", menuName = "JumpGame/Game Progression Config")]
public class GameProgressionConfig : ScriptableObject
{
    public enum HazardSpawnPositionMode
    {
        World = 0,
        RelativeToPlayer = 1
    }

    [Serializable]
    public class BoulderChaseSettings
    {
        [SerializeField] private float startBehindDistance = 12f;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float acceleration = 0f;
        [SerializeField] private bool instantKillOnTouch = true;

        public float StartBehindDistance => startBehindDistance;
        public float MoveSpeed => moveSpeed;
        public float Acceleration => acceleration;
        public bool InstantKillOnTouch => instantKillOnTouch;
    }

    [Serializable]
    public class RisingWaterSettings
    {
        [SerializeField] private float startY = -4f;
        [SerializeField] private float riseSpeed = 0.75f;
        [SerializeField] private float maxY = 8f;
        [SerializeField] private bool instantKillBelowWaterLine = false;

        public float StartY => startY;
        public float RiseSpeed => riseSpeed;
        public float MaxY => maxY;
        public bool InstantKillBelowWaterLine => instantKillBelowWaterLine;
    }

    [Serializable]
    public class FallingRocksSettings
    {
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private int rocksPerWave = 1;
        [SerializeField] private float warningDuration = 0.75f;
        [SerializeField] private float spawnHeight = 10f;
        [SerializeField] private float minSpawnAheadDistance = 6f;
        [SerializeField] private float maxSpawnAheadDistance = 14f;
        [SerializeField] private float fallSpeed = 12f;
        [SerializeField] private bool instantKillOnHit = true;

        public float SpawnInterval => spawnInterval;
        public int RocksPerWave => Mathf.Max(1, rocksPerWave);
        public float WarningDuration => warningDuration;
        public float SpawnHeight => spawnHeight;
        public float MinSpawnAheadDistance => minSpawnAheadDistance;
        public float MaxSpawnAheadDistance => Mathf.Max(minSpawnAheadDistance, maxSpawnAheadDistance);
        public float FallSpeed => fallSpeed;
        public bool InstantKillOnHit => instantKillOnHit;
    }

    [Serializable]
    public class HazardDefinition
    {
        [SerializeField] private bool enabled = true;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private GameHazardType hazardType = GameHazardType.None;
        [SerializeField] private GameObject hazardPrefab;
        [SerializeField] private HazardSpawnPositionMode spawnPositionMode = HazardSpawnPositionMode.RelativeToPlayer;
        [SerializeField] private Vector3 spawnOffset = Vector3.zero;
        [SerializeField] private BoulderChaseSettings boulderChase = new BoulderChaseSettings();
        [SerializeField] private RisingWaterSettings risingWater = new RisingWaterSettings();
        [SerializeField] private FallingRocksSettings fallingRocks = new FallingRocksSettings();

        public bool Enabled => enabled;
        public string DisplayName => displayName;
        public GameHazardType HazardType => hazardType;
        public GameObject HazardPrefab => hazardPrefab;
        public HazardSpawnPositionMode SpawnPositionMode => spawnPositionMode;
        public Vector3 SpawnOffset => spawnOffset;
        public BoulderChaseSettings BoulderChase => boulderChase;
        public RisingWaterSettings RisingWater => risingWater;
        public FallingRocksSettings FallingRocks => fallingRocks;
    }

    [Serializable]
    public class ZoneGenerationRule
    {
        [SerializeField] private ZoneType zoneType = ZoneType.None;
        [SerializeField] private float weight = 1f;
        [SerializeField] private int minConsecutiveCount = 1;
        [SerializeField] private int maxConsecutiveCount = 2;
        [SerializeField] private bool canBeFirstRandomSegment = true;
        [SerializeField] private List<ZoneType> allowedPreviousZones = new List<ZoneType>();

        public ZoneType ZoneType => zoneType;
        public float Weight => Mathf.Max(0.01f, weight);
        public int MinConsecutiveCount => Mathf.Max(1, minConsecutiveCount);
        public int MaxConsecutiveCount => Mathf.Max(MinConsecutiveCount, maxConsecutiveCount);
        public bool CanBeFirstRandomSegment => canBeFirstRandomSegment;
        public List<ZoneType> AllowedPreviousZones => allowedPreviousZones;
    }

    [Serializable]
    public class LevelDefinition
    {
        [SerializeField] private string levelName = "Level";
        [SerializeField] private string description = string.Empty;
        [SerializeField] private float targetDistance = 45f;
        [SerializeField] private string startHint = string.Empty;
        [SerializeField] private string clearHint = string.Empty;
        [SerializeField] private int openingRoadRepeatCount = 3;
        [SerializeField] private List<PlayerFormType> unlockedForms = new List<PlayerFormType>();
        [SerializeField] private List<ZoneType> allowedZones = new List<ZoneType>();
        [SerializeField] private List<ZoneGenerationRule> zoneGenerationRules = new List<ZoneGenerationRule>();
        [SerializeField] private List<HazardDefinition> hazards = new List<HazardDefinition>();

        public string LevelName => string.IsNullOrWhiteSpace(levelName) ? "Level" : levelName;
        public string Description => description;
        public float TargetDistance => Mathf.Max(1f, targetDistance);
        public string StartHint => startHint;
        public string ClearHint => clearHint;
        public int OpeningRoadRepeatCount => Mathf.Max(1, openingRoadRepeatCount);
        public List<PlayerFormType> UnlockedForms => unlockedForms;
        public List<ZoneType> AllowedZones => allowedZones;
        public List<ZoneGenerationRule> ZoneGenerationRules => zoneGenerationRules;
        public List<HazardDefinition> Hazards => hazards;
    }

    [Header("Flow")]
    [SerializeField] private int defaultStartingLevel = 1;
    [SerializeField] private float transitionDelay = 1.25f;
    [SerializeField] private bool enableDebugHotkeys = true;

    [Header("Levels")]
    [SerializeField] private List<LevelDefinition> levels = new List<LevelDefinition>();

    private static GameProgressionConfig cachedConfig;

    public int DefaultStartingLevel => Mathf.Max(1, defaultStartingLevel);
    public float TransitionDelay => Mathf.Max(0f, transitionDelay);
    public bool EnableDebugHotkeys => enableDebugHotkeys;
    public int LevelCount => levels != null ? levels.Count : 0;

    public static GameProgressionConfig Load()
    {
        if (cachedConfig != null)
        {
            return cachedConfig;
        }

        cachedConfig = Resources.Load<GameProgressionConfig>("GameConfig/GameProgressionConfig");
        return cachedConfig;
    }

    public LevelDefinition GetLevel(int levelIndex)
    {
        if (levels == null || levels.Count == 0)
        {
            return null;
        }

        int clampedIndex = Mathf.Clamp(levelIndex, 0, levels.Count - 1);
        return levels[clampedIndex];
    }

    public bool IsFormUnlocked(int levelIndex, PlayerFormType formType)
    {
        LevelDefinition level = GetLevel(levelIndex);
        if (level == null || level.UnlockedForms == null || level.UnlockedForms.Count == 0)
        {
            return true;
        }

        return level.UnlockedForms.Contains(formType);
    }

    public bool IsZoneAllowed(int levelIndex, ZoneType zoneType)
    {
        LevelDefinition level = GetLevel(levelIndex);
        if (level == null || level.AllowedZones == null || level.AllowedZones.Count == 0)
        {
            return true;
        }

        return level.AllowedZones.Contains(zoneType);
    }

    public PlayerFormType GetFallbackUnlockedForm(int levelIndex, PlayerFormType preferredForm = PlayerFormType.Human)
    {
        LevelDefinition level = GetLevel(levelIndex);
        if (level == null || level.UnlockedForms == null || level.UnlockedForms.Count == 0)
        {
            return preferredForm;
        }

        if (level.UnlockedForms.Contains(preferredForm))
        {
            return preferredForm;
        }

        return level.UnlockedForms[0];
    }
}
