using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class used for displaying a series of buttons below another button.
/// </summary>
public class DropdownButton : ImageButton, IPopupButton
{

    public bool createDropdownOnClick = true;

    public Text text;
    public DropdownButtonInfo[] dropdownButtons;

    private List<DropdownButton> buttonList = new List<DropdownButton>();

    private PopupButtonObserver popupButtonObserver;

    public bool PointerEntered { get; private set; }

    public GameObject PopupObject => gameObject;

    [Inject]
    public void Construct(PopupButtonObserver popupButtonObserver) => this.popupButtonObserver = popupButtonObserver;

    protected void OnEnable() => Button.onClick.AddListener(ChangeButtonDropdown);

    private void OnDisable() => Button.onClick.RemoveAllListeners();

    private void ChangeButtonDropdown()
    {
        if (!createDropdownOnClick)
            return;

        if (buttonList.Count > 0)
            CloseButtonDropdown();
        else
            OpenButtonDropdown();
    }

    public void CloseButtonDropdown()
    {
        buttonList.ForEach(button => popupButtonObserver.UnsubscribeObservable(button));
        buttonList.Where(button => button.gameObject != null).ToList().SafeForEach(button => Destroy(button.gameObject));
        buttonList.Clear();
    }

    private void OpenButtonDropdown()
    {
        var buttonToInstantiate = gameObject;
        foreach (DropdownButtonInfo buttonInfo in dropdownButtons)
        {
            var newButton = Instantiate(buttonToInstantiate, transform);

            FixNewButtonRect(newButton);
            FixNewButtonDropdownValues(buttonInfo, newButton);

            buttonToInstantiate = newButton;
        }

        popupButtonObserver.SetPopupCloseAction(CloseButtonDropdown);
    }

    private void FixNewButtonDropdownValues(DropdownButtonInfo buttonInfo, GameObject newButton)
    {
        var dropdownComponent = newButton.GetComponent<DropdownButton>();
        dropdownComponent.buttonImage.sprite = buttonInfo.buttonImage;
        dropdownComponent.text.text = buttonInfo.buttonText;
        dropdownComponent.createDropdownOnClick = false;

        if (buttonInfo.onClickAction != null)
            dropdownComponent.Button.onClick.AddListener(buttonInfo.onClickAction);

        dropdownComponent.Button.onClick.AddListener(CloseButtonDropdown);

        buttonList.Add(dropdownComponent);
        popupButtonObserver.SubscribeObservable(dropdownComponent);
    }

    private void FixNewButtonRect(GameObject newButton)
    {
        var rectTransform = newButton.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.localPosition = new Vector3(0f, (buttonList.Count + 1) * -rectTransform.rect.size.y, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData) => PointerEntered = true;

    public void OnPointerExit(PointerEventData eventData) => PointerEntered = false;

}