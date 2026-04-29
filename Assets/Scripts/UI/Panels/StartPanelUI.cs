using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Nenn.InspectorEnhancements.Runtime.Attributes;

[DisallowMultipleComponent]
public class StartPanelUI : PanelUIBase
{
    [Header("References")]
    [LabelText("标题文本")]
    [SerializeField] private TMP_Text titleText;
    [LabelText("说明文本")]
    [SerializeField] private TMP_Text descriptionText;
    [LabelText("开始按钮")]
    [SerializeField] private Button startButton;
    [LabelText("按钮文本")]
    [SerializeField] private TMP_Text startButtonText;

    public event Action StartRequested;

    protected override void Reset()
    {
        base.Reset();
        AutoBind();
    }

    protected override void Awake()
    {
        base.Awake();
        AutoBind();
        BindButton();
    }

    public override void Initialize()
    {
        AutoBind();
        BindButton();
    }

    public void SetContent(string title, string description, string buttonLabel)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = description;
        }

        if (startButtonText != null)
        {
            startButtonText.text = buttonLabel;
        }
    }

    private void BindButton()
    {
        if (startButton == null)
        {
            return;
        }

        startButton.onClick.RemoveListener(HandleStartClicked);
        startButton.onClick.AddListener(HandleStartClicked);
    }

    private void HandleStartClicked()
    {
        StartRequested?.Invoke();
    }

    private void AutoBind()
    {
        titleText = titleText != null ? titleText : FindText("Card/Title");
        descriptionText = descriptionText != null ? descriptionText : FindText("Card/Description");
        startButton = startButton != null ? startButton : FindButton("Card/StartButton");
        startButtonText = startButtonText != null ? startButtonText : FindText("Card/StartButton/Label");
    }

    private TMP_Text FindText(string path)
    {
        Transform child = transform.Find(path);
        return child != null ? child.GetComponent<TMP_Text>() : null;
    }

    private Button FindButton(string path)
    {
        Transform child = transform.Find(path);
        return child != null ? child.GetComponent<Button>() : null;
    }
}
