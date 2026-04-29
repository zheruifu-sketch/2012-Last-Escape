using System.Collections;
using TMPro;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HintBarUI : HudUIBase
{
    [Header("References")]
    [LabelText("提示根节点")]
    [SerializeField] private GameObject hintRoot;
    [LabelText("TMP 文本")]
    [SerializeField] private TMP_Text tmpText;
    [LabelText("旧版文本")]
    [SerializeField] private Text legacyText;

    private Coroutine hideCoroutine;
    private CanvasGroup canvasGroup;

    protected override void Reset()
    {
        base.Reset();
        if (hintRoot == null)
        {
            hintRoot = gameObject;
        }

        if (tmpText == null)
        {
            tmpText = GetComponentInChildren<TMP_Text>(true);
        }

        if (legacyText == null)
        {
            legacyText = GetComponentInChildren<Text>(true);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (hintRoot == null)
        {
            hintRoot = gameObject;
        }

        canvasGroup = hintRoot != null ? hintRoot.GetComponent<CanvasGroup>() : null;
        HideImmediate();
    }

    public override void Initialize()
    {
        if (hintRoot == null)
        {
            hintRoot = gameObject;
        }

        canvasGroup = hintRoot.GetComponent<CanvasGroup>();
    }

    public void ShowHint(string message, float duration)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        SetText(message);
        SetVisible(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        if (duration > 0f)
        {
            hideCoroutine = StartCoroutine(HideAfterDelay(duration));
        }
    }

    public void HideImmediate()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        SetVisible(false);
    }

    private void SetText(string message)
    {
        if (tmpText != null)
        {
            tmpText.text = message;
        }

        if (legacyText != null)
        {
            legacyText.text = message;
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hideCoroutine = null;
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (hintRoot == null)
        {
            return;
        }

        if (hintRoot == gameObject)
        {
            if (canvasGroup == null)
            {
                canvasGroup = hintRoot.GetComponent<CanvasGroup>();
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
                return;
            }
        }

        if (hintRoot.activeSelf != visible)
        {
            hintRoot.SetActive(visible);
        }
    }
}
