using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class DropdownButton : ImageButton, IPopupButton
{

    public bool createDropdownOnClick = true;
    public Text text;
    public DropdownButtonInfo[] dropdownButtons;

    private List<Button> buttonList = new List<Button>();

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
        buttonList.Select(b => b.GetComponent<DropdownButton>()).ForEach(b => popupButtonObserver.UnsubscribeObservable(b));
        buttonList.Where(button => button.gameObject != null).ToList().SafeForEach(button => Destroy(button.gameObject));
        buttonList.Clear();
    }

    private void OpenButtonDropdown()
    {
        var buttonToInstantiate = gameObject;
        foreach (DropdownButtonInfo button in dropdownButtons)
        {
            var newButton = Instantiate(buttonToInstantiate, transform);
            var rectTransform = newButton.GetComponent<RectTransform>();
            var dropdownComponent = newButton.GetComponent<DropdownButton>();

            dropdownComponent.buttonImage.sprite = button.buttonImage;
            dropdownComponent.text.text = button.buttonText;
            dropdownComponent.createDropdownOnClick = false;

            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.localPosition = new Vector3(0f, (buttonList.Count + 1) * -rectTransform.rect.size.y, 0f);

            popupButtonObserver.SubscribeObservable(dropdownComponent);

            var buttonComponent = newButton.GetComponent<Button>();

            if (button.onClickAction != null)
                buttonComponent.onClick.AddListener(button.onClickAction);

            buttonComponent.onClick.AddListener(CloseButtonDropdown);

            buttonList.Add(buttonComponent);
            buttonToInstantiate = newButton;
        }

        popupButtonObserver.SetPopupCloseAction(CloseButtonDropdown);
    }

    public void OnPointerEnter(PointerEventData eventData) => PointerEntered = true;

    public void OnPointerExit(PointerEventData eventData) => PointerEntered = false;
}