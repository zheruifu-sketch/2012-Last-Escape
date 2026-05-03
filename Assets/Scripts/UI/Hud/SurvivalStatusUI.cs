using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SurvivalStatusUI : HudUIBase
{
    [Header("References")]
    [LabelText("玩家运行时上下文")]
    [SerializeField] private PlayerRuntimeContext runtimeContext;
    [LabelText("生命控制器")]
    [SerializeField] private PlayerHealthController healthController;
    [LabelText("燃油控制器")]
    [SerializeField] private PlayerFuelController fuelController;
    [LabelText("生命滑条")]
    [SerializeField] private Slider healthSlider;
    [LabelText("燃油滑条")]
    [SerializeField] private Slider fuelSlider;

    protected override void Reset()
    {
        base.Reset();
        TryAutoBind();
    }

    protected override void Awake()
    {
        base.Awake();
        TryAutoBind();
    }

    private void OnEnable()
    {
        if (healthController != null)
        {
            healthController.HealthChanged += HandleHealthChanged;
            HandleHealthChanged(healthController.CurrentHealth, healthController.MaxHealth);
        }

        if (fuelController != null)
        {
            fuelController.FuelChanged += HandleFuelChanged;
            HandleFuelChanged(fuelController.CurrentFuel, fuelController.MaxFuel);
        }
    }

    private void OnDisable()
    {
        if (healthController != null)
        {
            healthController.HealthChanged -= HandleHealthChanged;
        }

        if (fuelController != null)
        {
            fuelController.FuelChanged -= HandleFuelChanged;
        }
    }

    public override void Initialize()
    {
        TryAutoBind();
    }

    private void TryAutoBind()
    {
        runtimeContext = runtimeContext != null ? runtimeContext : PlayerRuntimeContext.FindInScene();
        if (runtimeContext != null)
        {
            runtimeContext.RefreshReferences();
            healthController = healthController != null ? healthController : runtimeContext.HealthController;
            fuelController = fuelController != null ? fuelController : runtimeContext.FuelController;
        }

        if (healthController == null)
        {
            healthController = FindObjectOfType<PlayerHealthController>();
        }

        if (fuelController == null)
        {
            fuelController = FindObjectOfType<PlayerFuelController>();
        }
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        if (healthSlider == null)
        {
            return;
        }

        float normalized = maxHealth > 0f ? currentHealth / maxHealth : 0f;
        healthSlider.normalizedValue = Mathf.Clamp01(normalized);
    }

    private void HandleFuelChanged(float currentFuel, float maxFuel)
    {
        if (fuelSlider == null)
        {
            return;
        }

        float normalized = maxFuel > 0f ? currentFuel / maxFuel : 0f;
        fuelSlider.normalizedValue = Mathf.Clamp01(normalized);
    }
}
