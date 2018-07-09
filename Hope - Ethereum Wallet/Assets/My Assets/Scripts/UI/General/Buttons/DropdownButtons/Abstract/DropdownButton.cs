using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    public TMP_Text text;
    public DropdownButtonInfo[] dropdownButtons;

    private List<DropdownButton> buttonList = new List<DropdownButton>();

    private PopupButtonObserver popupButtonObserver;

    /// <summary>
    /// Value changed when the mouse pointer is hovered above this button.
    /// </summary>
    public bool PointerEntered { get; private set; }

    /// <summary>
    /// The object of this dropdown button.
    /// </summary>
    public GameObject PopupObject => gameObject;

    /// <summary>
    /// Injects the required dependencies.
    /// </summary>
    /// <param name="popupButtonObserver"> The active PopupButtonObserver. </param>
    [Inject]
    public void Construct(PopupButtonObserver popupButtonObserver) => this.popupButtonObserver = popupButtonObserver;

    /// <summary>
    /// Adds the button listener.
    /// </summary>
    protected void OnEnable() => Button.onClick.AddListener(ChangeButtonDropdown);

    /// <summary>
    /// Removes all button listeners.
    /// </summary>
    private void OnDisable() => Button.onClick.RemoveAllListeners();

    /// <summary>
    /// Changes the button dropdown to active/inactive.
    /// </summary>
    private void ChangeButtonDropdown()
    {
        if (!createDropdownOnClick)
            return;

        if (buttonList.Count > 0)
            CloseButtonDropdown();
        else
            OpenButtonDropdown();
    }

    /// <summary>
    /// Closes the button dropdown.
    /// </summary>
    public void CloseButtonDropdown()
    {
        buttonList.ForEach(button => popupButtonObserver.UnsubscribeObservable(button));
        buttonList.Where(button => button.gameObject != null).ToList().SafeForEach(button => Destroy(button.gameObject));
        buttonList.Clear();
    }

    /// <summary>
    /// Opens the button dropdown.
    /// </summary>
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

    /// <summary>
    /// Fixes the values of the DropdownButton.
    /// </summary>
    /// <param name="buttonInfo"> The info of the button. </param>
    /// <param name="newButton"> The newly created gameobject. </param>
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

    /// <summary>
    /// Fixes the rect of the newly created button.
    /// </summary>
    /// <param name="newButton"> The new button gameobject. </param>
    private void FixNewButtonRect(GameObject newButton)
    {
        var rectTransform = newButton.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.localPosition = new Vector3(0f, (buttonList.Count + 1) * -rectTransform.rect.size.y, 0f);
    }

    /// <summary>
    /// Called when the mouse pointer enters the button vicinity.
    /// </summary>
    /// <param name="eventData"> The pointer event data. </param>
    public void OnPointerEnter(PointerEventData eventData) => PointerEntered = true;

    /// <summary>
    /// Called when the mouse pointer exits the button vicinity.
    /// </summary>
    /// <param name="eventData"> The pointer event data. </param>
    public void OnPointerExit(PointerEventData eventData) => PointerEntered = false;

}