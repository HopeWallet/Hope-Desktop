using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class DropdownButton : ImageButton, IObserveLeftClick, IObserveRightClick
{

    public Text text;
    public DropdownButtonInfo[] dropdownButtons;

    private List<Button> buttonList;
    private GameObject mainButtonObj;

    private MouseClickObserver mouseClickObserver;

    [Inject]
    public void Construct(MouseClickObserver mouseClickObserver)
    {
        this.mouseClickObserver = mouseClickObserver;
    }

    protected override void OnAwake()
    {
        mainButtonObj = gameObject;
        buttonList = new List<Button>();
        Button.onClick.AddListener(ChangeButtonDropdown);

    }

    private void OnDestroy()
    {
        Button.onClick.RemoveAllListeners();

    }

    private void ChangeButtonDropdown()
    {
        if (buttonList.Count > 0)
            CloseButtonDropdown();
        else
            OpenButtonDropdown();
    }

    private void CloseButtonDropdown()
    {
        UnityEngine.Debug.Log("DELETING");
        buttonList.SafeForEach(button => Destroy(button.gameObject));
        buttonList.Clear();

        mouseClickObserver.RemoveLeftClickObserver(this);
        mouseClickObserver.RemoveRightClickObserver(this);
    }

    private void OpenButtonDropdown()
    {
        foreach (DropdownButtonInfo button in dropdownButtons)
        {
            var newButton = Instantiate(mainButtonObj, transform);
            var rectTransform = newButton.GetComponent<RectTransform>();
            var dropdownComponent = newButton.GetComponent<DropdownButton>();
            dropdownComponent.buttonImage.sprite = button.buttonImage;
            dropdownComponent.text.text = button.buttonText;
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.localPosition = new Vector3(0f, (buttonList.Count + 1) * -rectTransform.rect.size.y, 0f);

            var childButtons = newButton.GetComponentsInChildren<DropdownButton>();
            for (int i = 1; i < childButtons.Length; i++)
                Destroy(childButtons[i].gameObject);

            Destroy(dropdownComponent);

            var buttonComponent = newButton.GetComponent<Button>();
            //buttonComponent.onClick.AddListener(button.onClickAction);
            buttonComponent.onClick.AddListener(CloseButtonDropdown);

            buttonList.Add(buttonComponent);
        }

        mouseClickObserver.AddLeftClickObserver(this);
        mouseClickObserver.AddRightClickObserver(this);
    }

    public void OnRightClick(ClickType clickType) => CheckBounds(clickType);

    public void OnLeftClick(ClickType clickType) => CheckBounds(clickType);

    private void CheckBounds(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        var mousePosition = Input.mousePosition;
        var inBounds = false;

        buttonList.ForEach(button =>
        {
            if (button.GetButtonBounds().Contains(mousePosition))
                inBounds = true;
        });

        if (!inBounds && !Button.GetButtonBounds().Contains(mousePosition))
            CloseButtonDropdown();
    }

}

[Serializable]
public class DropdownButtonInfo
{
    public UnityAction onClickAction;
    public Sprite buttonImage;
    public string buttonText;
}