using System;
using System.Collections.Generic;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using UnityEngine;

[DisallowMultipleComponent]
public class UIManager : MonoBehaviour
{
    [LabelText("运行时自动扫描子节点")]
    [SerializeField] private bool autoRegisterChildren = true;

    private readonly Dictionary<Type, UIBase> uiLookup = new Dictionary<Type, UIBase>();

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        RegisterSceneUis();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterSceneUis()
    {
        uiLookup.Clear();

        if (!autoRegisterChildren)
        {
            return;
        }

        UIBase[] sceneUis = FindObjectsOfType<UIBase>(true);
        for (int i = 0; i < sceneUis.Length; i++)
        {
            Register(sceneUis[i]);
        }
    }

    public void Register(UIBase ui)
    {
        if (ui == null)
        {
            return;
        }

        uiLookup[ui.GetType()] = ui;
        ui.Initialize();
    }

    public T Get<T>() where T : UIBase
    {
        uiLookup.TryGetValue(typeof(T), out UIBase ui);
        return ui as T;
    }

    public T Show<T>() where T : UIBase
    {
        T ui = Get<T>();
        if (ui != null)
        {
            ui.Show();
        }

        return ui;
    }

    public void Hide<T>() where T : UIBase
    {
        T ui = Get<T>();
        if (ui != null)
        {
            ui.Hide();
        }
    }

    public void HideAllPanels()
    {
        foreach (UIBase ui in uiLookup.Values)
        {
            if (ui is PanelUIBase panelUi)
            {
                panelUi.Hide();
            }
        }
    }

    public T ShowOnly<T>() where T : PanelUIBase
    {
        HideAllPanels();
        return Show<T>();
    }
}
