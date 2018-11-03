using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Manages the top bar buttons and button listenerss
/// </summary>
public sealed class TopBarButtons : MonoBehaviour
{
	[SerializeField] private Button[] buttons = new Button[5];

	private PopupManager popupManager;
	private LockedPRPSManager lockedPRPSManager;

	/// <summary>
	/// Adds the dependencies to the LockPRPSButton.
	/// </summary>
	/// <param name="popupManager"> The active PopupManager. </param>
	/// <param name="lockedPRPSManager"> The active LockedPRPSManager. </param>
	[Inject]
	public void Construct(PopupManager popupManager, LockedPRPSManager lockedPRPSManager)
	{
		this.popupManager = popupManager;
		this.lockedPRPSManager = lockedPRPSManager;
	}

	/// <summary>
	/// Sets the button array values
	/// </summary>
	private void Awake()
	{
		for (int i = 0; i < buttons.Length; i++)
			SetButtonListener(i);
	}

	/// <summary>
	/// Sets the button onClick listeners
	/// </summary>
	/// <param name="index"> The index of the button in the array </param>
	private void SetButtonListener(int index) => buttons[index].onClick.AddListener(() => ButtonClicked(index));

	/// <summary>
	/// Opens the corresponding popup
	/// </summary>
	/// <param name="index"> The index of the button being pressed </param>
	private void ButtonClicked(int index)
	{
        PopupBase popup = null;

		switch (index)
		{
			case 0:
				if (lockedPRPSManager.UnfulfilledItems?.Count > 0)
					popup = popupManager.GetPopup<LockedPRPSPopup>();
				else
					popup = popupManager.GetPopup<LockPRPSPopup>();
				break;
			case 1:
				    popup = popupManager.GetPopup<SendAssetPopup>();
				break;
			case 2:
				    popup = popupManager.GetPopup<ReceiveAssetPopup>();
				break;
		}

		buttons[index].interactable = false;
        popup.OnPopupClose(() => buttons[index].interactable = true);
	}
}
