using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [Header("UI Base")]
    [LabelText("根节点")]
    [SerializeField] private GameObject root;

    public bool IsVisible => Root.activeSelf;
    protected GameObject Root => root != null ? root : gameObject;

    protected virtual void Reset()
    {
        root = gameObject;
    }

    protected virtual void Awake()
    {
        if (root == null)
        {
            root = gameObject;
        }
    }

    public virtual void Initialize()
    {
    }

    public virtual void Show()
    {
        SetVisible(true);
    }

    public virtual void Hide()
    {
        SetVisible(false);
    }

    protected void SetVisible(bool visible)
    {
        if (Root.activeSelf == visible)
        {
            return;
        }

        Root.SetActive(visible);
    }
}
