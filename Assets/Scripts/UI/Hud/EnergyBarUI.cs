using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EnergyBarUI : HudUIBase
{
    [Header("References")]
    [LabelText("玩家运行时上下文")]
    [SerializeField] private PlayerRuntimeContext runtimeContext;
    [LabelText("能量控制器")]
    [SerializeField] private PlayerEnergyController energyController;
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
        if (energyController != null)
        {
            energyController.EnergyChanged += HandleEnergyChanged;
            HandleEnergyChanged(energyController.CurrentEnergy, energyController.MaxEnergy);
        }
    }

    private void OnDisable()
    {
        if (energyController != null)
        {
            energyController.EnergyChanged -= HandleEnergyChanged;
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
            energyController = energyController != null ? energyController : runtimeContext.EnergyController;
        }

        if (energyController == null)
        {
            energyController = FindObjectOfType<PlayerEnergyController>();
        }

        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }
    }

    private void HandleEnergyChanged(float currentEnergy, float maxEnergy)
    {
        if (fillImage == null)
        {
            return;
        }

        float normalized = maxEnergy > 0f ? currentEnergy / maxEnergy : 0f;
        fillImage.fillAmount = Mathf.Clamp01(normalized);
    }
}
