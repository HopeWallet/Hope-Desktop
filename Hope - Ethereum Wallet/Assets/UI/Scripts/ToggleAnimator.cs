using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleAnimator : MonoBehaviour
{

	[SerializeField] private GameObject toggleBackground;
	[SerializeField] private GameObject toggleCircle;

	private readonly Color blueColor = new Color(0.388f, 0.694f, 1f);
	private readonly Color fadedColor = new Color(1f, 1f, 1f);

	private bool isToggledOn;

	private void Awake()
	{
		toggleBackground.GetComponent<Button>().onClick.AddListener(ToggleClicked);
		toggleCircle.GetComponent<Button>().onClick.AddListener(ToggleClicked);
	}

	private void ToggleClicked()
	{
		toggleCircle.AnimateTransformX(isToggledOn ? -15f : 15f, 0.2f);
		toggleCircle.GetComponent<Image>().DOColor(isToggledOn ? fadedColor : blueColor, 0.2f);
		toggleBackground.GetComponent<Image>().DOColor(isToggledOn ? fadedColor : blueColor, 0.2f);
		isToggledOn = !isToggledOn;
	}
}
