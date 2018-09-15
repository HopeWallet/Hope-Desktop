using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralOkCancelPopup : OkCancelPopupComponent<GeneralOkCancelPopup>
{
	[SerializeField] private TextMeshProUGUI subText;

	private Action onFinish;
	
	public GeneralOkCancelPopup SetSubText(string subText)
	{
		this.subText.text = subText;
		return this;
	}

	public GeneralOkCancelPopup OnOkClicked(Action onOkClicked)
	{
		okButton.onClick.AddListener(() => onOkClicked());
		return this;
	}

	public void OnFinish(Action onFinish) => this.onFinish = onFinish;

	private void OnDestroy() => onFinish?.Invoke();
}
