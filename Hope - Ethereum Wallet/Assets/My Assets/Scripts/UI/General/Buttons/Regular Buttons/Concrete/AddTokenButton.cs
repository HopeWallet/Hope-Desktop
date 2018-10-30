using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class AddTokenButton : MonoBehaviour
{
	private Button button;

	private PopupManager popupManager;

	/// <summary>
	/// Adds the dependencies to the TokenListButton.
	/// </summary>
	/// <param name="popupManager"> The active PopupManager. </param>
	[Inject]
	public void Construct(PopupManager popupManager) => this.popupManager = popupManager;

	/// <summary>
	/// Sets the button listener
	/// </summary>
	private void Awake()
	{
		button = transform.GetComponent<Button>();
		button.onClick.AddListener(ButtonClicked);
	}

	/// <summary>
	/// Opens the ModifyTokensPopup.
	/// </summary>
	private void ButtonClicked()
	{
		popupManager.GetPopup<AddTokenPopup>().OnPopupClose(() => button.interactable = true);
		button.interactable = false;
	}
}
