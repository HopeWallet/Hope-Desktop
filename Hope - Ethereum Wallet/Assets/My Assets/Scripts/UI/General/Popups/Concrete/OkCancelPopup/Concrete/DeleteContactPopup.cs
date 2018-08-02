using TMPro;
using UnityEngine;

public sealed class DeleteContactPopup : OkCancelPopupComponent<DeleteContactPopup>
{

	public TextMeshProUGUI contactName;
	public TextMeshProUGUI contactAddress;

	public override void OkButton()
	{
	}

	public void SetContactDetails(string name, string address)
	{
		contactName.text = name;
		contactAddress.text = address;
	}
}
