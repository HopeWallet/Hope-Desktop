using DG.Tweening;
using TMPro;
using UnityEngine;

public class InfoPopup : FactoryPopup<InfoPopup>
{

	[SerializeField] private TextMeshProUGUI title;
	[SerializeField] private TextMeshProUGUI body;
	[SerializeField] private GameObject infoIcon;
	[SerializeField] private GameObject errorIcon;

	public void AnimateForm(bool animateIn) => transform.DOScaleX(animateIn ? 1f : 0f, 0.1f);

	public void SetUIElements(string titleText, string bodyText, bool infoMessage, Vector2 iconPosition)
	{
		title.text = titleText;
		body.text = bodyText;
		infoIcon.SetActive(infoMessage);
		errorIcon.SetActive(!infoMessage);
		transform.localPosition = iconPosition;
	}
}
