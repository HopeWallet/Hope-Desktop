using UnityEngine;

public class SettingsPopupAnimator : UIAnimator {

	[SerializeField] private CategoryButtons settingsCategories;
	[SerializeField] private GameObject[] sections;

	private void Awake() => settingsCategories.OnButtonChanged += CategoryChanged;

	protected override void AnimateUniqueElementsIn()
	{
		FinishedAnimating();
	}

	private void CategoryChanged(int categoryNum)
	{
		sections[settingsCategories.previouslySelectedButton].AnimateScale(0f, 0.15f);
		sections[categoryNum].AnimateScale(1f, 0.15f);
	}
}
