using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HealthBarUI : HudUIBase
{
    [Header("References")]
    [LabelText("玩家运行时上下文")]
    [SerializeField] private PlayerRuntimeContext runtimeContext;
    [LabelText("生命控制器")]
    [SerializeField] private PlayerHealthController healthController;
    [LabelText("填充图片")]
    [SerializeField] private Image fillImage;

    protected override void Reset()
    {
        base.Reset();
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }
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
    }

    private void OnDisable()
    {
        if (healthController != null)
        {
            healthController.HealthChanged -= HandleHealthChanged;
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
        }

        if (healthController == null)
        {
            healthController = FindObjectOfType<PlayerHealthController>();
        }

        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        if (fillImage == null)
        {
            return;
        }

        float normalized = maxHealth > 0f ? currentHealth / maxHealth : 0f;
        fillImage.fillAmount = Mathf.Clamp01(normalized);
    }
}
