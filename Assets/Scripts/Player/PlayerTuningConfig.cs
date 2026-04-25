using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTuningConfig", menuName = "JumpGame/Player Tuning Config")]
public class PlayerTuningConfig : ScriptableObject
{
    [Serializable]
    public class MovementSettings
    {
        [SerializeField] private float humanMoveSpeed = 4f;
        [SerializeField] private float carMoveSpeed = 7f;
        [SerializeField] private float planeMoveSpeed = 6f;
        [SerializeField] private float planeVerticalSpeed = 5f;
        [SerializeField] private float boatMoveSpeed = 4.5f;
        [SerializeField] private float boatFloatHeightOffset = 0.85f;
        [SerializeField] private float boatFloatVerticalSpeed = 8f;
        [SerializeField] private float boatFloatSnapDeadZone = 0.08f;
        [SerializeField] private float boatFloatActivationMargin = 0.35f;
        [SerializeField] private float sprintMultiplier = 1.6f;
        [SerializeField] private float humanJumpForce = 9f;
        [SerializeField] private float humanGravityScale = 3f;
        [SerializeField] private float carGravityScale = 4f;
        [SerializeField] private float planeGravityScale = 0f;
        [SerializeField] private float boatGravityScale = 3f;
        [SerializeField] private float jumpBufferTime = GameConstants.DefaultJumpBufferTime;
        [SerializeField] private float coyoteTime = GameConstants.DefaultCoyoteTime;
        [SerializeField] private int maxHumanJumpCount = 2;
        [SerializeField] private float groundedVelocityThreshold = 0.1f;

        public float HumanMoveSpeed => humanMoveSpeed;
        public float CarMoveSpeed => carMoveSpeed;
        public float PlaneMoveSpeed => planeMoveSpeed;
        public float PlaneVerticalSpeed => planeVerticalSpeed;
        public float BoatMoveSpeed => boatMoveSpeed;
        public float BoatFloatHeightOffset => boatFloatHeightOffset;
        public float BoatFloatVerticalSpeed => boatFloatVerticalSpeed;
        public float BoatFloatSnapDeadZone => boatFloatSnapDeadZone;
        public float BoatFloatActivationMargin => boatFloatActivationMargin;
        public float SprintMultiplier => sprintMultiplier;
        public float HumanJumpForce => humanJumpForce;
        public float HumanGravityScale => humanGravityScale;
        public float CarGravityScale => carGravityScale;
        public float PlaneGravityScale => planeGravityScale;
        public float BoatGravityScale => boatGravityScale;
        public float JumpBufferTime => jumpBufferTime;
        public float CoyoteTime => coyoteTime;
        public int MaxHumanJumpCount => Mathf.Max(1, maxHumanJumpCount);
        public float GroundedVelocityThreshold => groundedVelocityThreshold;
    }

    [Serializable]
    public class FormSettings
    {
        [SerializeField] private float transformCooldown = GameConstants.DefaultTransformCooldown;
        [SerializeField] private bool forceHumanWhenPlaneBlocked = true;

        public float TransformCooldown => transformCooldown;
        public bool ForceHumanWhenPlaneBlocked => forceHumanWhenPlaneBlocked;
    }

    [Serializable]
    public class EnvironmentRuleSettings
    {
        [SerializeField] private float blizzardHumanSpeedMultiplier = 0.3f;
        [SerializeField] private Vector2 boatSwitchCheckOffset = GameConstants.DefaultBoatSwitchCheckOffset;
        [SerializeField] private float boatSwitchCheckRadius = GameConstants.DefaultBoatSwitchCheckRadius;
        [SerializeField] private float floodBoatSupportHeight = 1.5f;

        public float BlizzardHumanSpeedMultiplier => blizzardHumanSpeedMultiplier;
        public Vector2 BoatSwitchCheckOffset => boatSwitchCheckOffset;
        public float BoatSwitchCheckRadius => boatSwitchCheckRadius;
        public float FloodBoatSupportHeight => floodBoatSupportHeight;
    }

    [Serializable]
    public class SurvivalSettings
    {
        [SerializeField] private float maxHealth = GameConstants.DefaultMaxHealth;
        [SerializeField] private float hazardDamagePerSecond = GameConstants.DefaultHazardDamagePerSecond;
        [SerializeField] private float maxEnergy = GameConstants.DefaultMaxEnergy;
        [SerializeField] private float carEnergyCostPerSecond = GameConstants.DefaultCarEnergyCostPerSecond;
        [SerializeField] private float planeEnergyCostPerSecond = GameConstants.DefaultPlaneEnergyCostPerSecond;
        [SerializeField] private float boatEnergyCostPerSecond = GameConstants.DefaultBoatEnergyCostPerSecond;

        public float MaxHealth => Mathf.Max(1f, maxHealth);
        public float HazardDamagePerSecond => Mathf.Max(0f, hazardDamagePerSecond);
        public float MaxEnergy => Mathf.Max(1f, maxEnergy);
        public float CarEnergyCostPerSecond => Mathf.Max(0f, carEnergyCostPerSecond);
        public float PlaneEnergyCostPerSecond => Mathf.Max(0f, planeEnergyCostPerSecond);
        public float BoatEnergyCostPerSecond => Mathf.Max(0f, boatEnergyCostPerSecond);
    }

    [SerializeField] private MovementSettings movement = new MovementSettings();
    [SerializeField] private FormSettings form = new FormSettings();
    [SerializeField] private EnvironmentRuleSettings environmentRules = new EnvironmentRuleSettings();
    [SerializeField] private SurvivalSettings survival = new SurvivalSettings();

    private static PlayerTuningConfig cachedConfig;

    public MovementSettings Movement => movement;
    public FormSettings Form => form;
    public EnvironmentRuleSettings EnvironmentRules => environmentRules;
    public SurvivalSettings Survival => survival;

    public static PlayerTuningConfig Load()
    {
        if (cachedConfig != null)
        {
            return cachedConfig;
        }

        cachedConfig = Resources.Load<PlayerTuningConfig>("GameConfig/PlayerTuningConfig");
        return cachedConfig;
    }
}
