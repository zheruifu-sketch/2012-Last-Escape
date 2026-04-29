using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class TransformCooldownUI : HudUIBase
{
    [Header("References")]
    [LabelText("玩家运行时上下文")]
    [SerializeField] private PlayerRuntimeContext runtimeContext;
    [LabelText("形态控制器")]
    [SerializeField] private PlayerFormController formController;
    [LabelText("冷却根节点")]
    [SerializeField] private GameObject cooldownUiRoot;
    [LabelText("填充图片")]
    [SerializeField] private Image fillImage;

    protected override void Reset()
    {
        base.Reset();
        if (cooldownUiRoot == null)
        {
            cooldownUiRoot = gameObject;
        }

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

    public override void Initialize()
    {
        TryAutoBind();
    }

    private void Update()
    {
        if (formController == null || fillImage == null)
        {
            return;
        }

        bool onCooldown = formController.IsTransformOnCooldown;
        if (onCooldown)
        {
            fillImage.fillAmount = formController.TransformCooldownNormalizedRemaining;
        }

        SetCooldownVisible(onCooldown);
    }

    private void TryAutoBind()
    {
        runtimeContext = runtimeContext != null ? runtimeContext : PlayerRuntimeContext.FindInScene();
        if (runtimeContext != null)
        {
            runtimeContext.RefreshReferences();
            formController = formController != null ? formController : runtimeContext.FormController;
        }

        if (formController == null)
        {
            formController = FindObjectOfType<PlayerFormController>();
        }

        if (cooldownUiRoot == null)
        {
            cooldownUiRoot = gameObject;
        }

        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }

        SetCooldownVisible(formController != null && formController.IsTransformOnCooldown);
    }

    private void SetCooldownVisible(bool visible)
    {
        if (cooldownUiRoot != null && cooldownUiRoot != gameObject)
        {
            if (cooldownUiRoot.activeSelf != visible)
            {
                cooldownUiRoot.SetActive(visible);
            }

            return;
        }

        if (fillImage != null && fillImage.enabled != visible)
        {
            fillImage.enabled = visible;
        }
    }
}
