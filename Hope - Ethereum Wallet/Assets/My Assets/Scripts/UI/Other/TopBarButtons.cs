using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TopBarButtons : MonoBehaviour
{
	public static Action popupClosed;

	private Button[] buttons = new Button[5];

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
		{
			if (i == 0)
				buttons[0] = transform.GetChild(0).GetChild(0).GetComponent<Button>();
			else
				buttons[i] = transform.GetChild(i).GetComponent<Button>();

			SetButtonListener(i);
		}
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
		switch (index)
		{
			case 0:
				if (lockedPRPSManager.UnfulfilledItems?.Count > 0)
					popupManager.GetPopup<LockedPRPSPopup>();
				else
					popupManager.GetPopup<LockPRPSPopup>();
				break;
			case 1:
				//Open market 
				break;
			case 2:
				popupManager.GetPopup<SendAssetPopup>();
				break;
			case 3:
				popupManager.GetPopup<ReceiveAssetPopup>();
				break;
			case 4:
				//Open more popup
				break;
		}

		buttons[index].interactable = false;
		popupClosed = () => buttons[index].interactable = true;
	}
}
