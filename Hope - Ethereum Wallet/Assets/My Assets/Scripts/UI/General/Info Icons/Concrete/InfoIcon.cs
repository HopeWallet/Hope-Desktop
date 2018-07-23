using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine;

public sealed class InfoIcon : InfoIconBase
{
	public string titleText;
	public string bodyText;
	public bool infoIcon;

	public override void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("ENTERED");
		Debug.Log(popupManager);

		popupManager.GetPopup<InfoPopup>().SetUIElements(titleText, bodyText, infoIcon, transform.localPosition);
		popupManager.GetPopup<InfoPopup>().AnimateForm(true);
		AnimateIconScale(true);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		UnityEngine.Debug.Log("EXITED");

		popupManager.GetPopup<InfoPopup>().AnimateForm(false);
		AnimateIconScale(false);
	}

	private void AnimateIconScale(bool animateIn)
	{
		transform.DOScaleX(animateIn ? 1.3f : 1f, 0.1f);
		transform.DOScaleY(animateIn ? 1.3f : 1f, 0.1f);
	}
}