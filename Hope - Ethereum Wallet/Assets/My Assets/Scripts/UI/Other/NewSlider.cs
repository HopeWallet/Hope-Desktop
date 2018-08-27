using UnityEngine;
using UnityEngine.UI;

public class NewSlider : MonoBehaviour
{
	[SerializeField] private GameObject slowText, fastText;

	private void Awake() => transform.GetComponent<Slider>().onValueChanged.AddListener(SliderChanged);

	private void SliderChanged(float value)
	{
		ModifyText(slowText, ((1 - value) * 0.3f) + 0.6f);
		ModifyText(fastText, (value * 0.3f) + 0.6f);
	}

	private void ModifyText(GameObject text, float value) => text.AnimateColor(new Color(value, value, value), 0.05f);
}
