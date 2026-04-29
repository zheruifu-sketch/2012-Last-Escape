using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Nenn.InspectorEnhancements.Runtime.Attributes;

[DisallowMultipleComponent]
public class ResultPanelUI : PanelUIBase
{
    [Header("References")]
    [LabelText("标题文本")]
    [SerializeField] private TMP_Text titleText;
    [LabelText("说明文本")]
    [SerializeField] private TMP_Text descriptionText;
    [LabelText("确认按钮")]
    [SerializeField] private Button confirmButton;
    [LabelText("按钮文本")]
    [SerializeField] private TMP_Text confirmButtonText;

    public event Action ConfirmRequested;

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

        if (confirmButtonText != null)
        {
            confirmButtonText.text = buttonLabel;
        }
    }

    private void BindButton()
    {
        if (confirmButton == null)
        {
            return;
        }

        confirmButton.onClick.RemoveListener(HandleConfirmClicked);
        confirmButton.onClick.AddListener(HandleConfirmClicked);
    }

    private void HandleConfirmClicked()
    {
        ConfirmRequested?.Invoke();
    }

    private void AutoBind()
    {
        titleText = titleText != null ? titleText : FindText("Card/Title");
        descriptionText = descriptionText != null ? descriptionText : FindText("Card/Description");
        confirmButton = confirmButton != null ? confirmButton : FindButton("Card/StartButton");
        confirmButtonText = confirmButtonText != null ? confirmButtonText : FindText("Card/StartButton/Label");
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
