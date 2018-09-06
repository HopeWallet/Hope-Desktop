using UnityEngine;

public class SettingsPopupAnimator : UIAnimator
{
	[SerializeField] private GameObject settingsCategoriesParent;
	[SerializeField] private GameObject line;
	[SerializeField] private GameObject[] sections;

	private CategoryButtons settingsCategories;

	private void Awake()
	{
		settingsCategories = settingsCategoriesParent.GetComponent<CategoryButtons>();
		settingsCategories.OnButtonChanged += CategoryChanged;
	}

	protected override void AnimateUniqueElementsIn()
	{
		settingsCategoriesParent.AnimateScaleY(1f, 0.25f);
		line.AnimateScaleX(1f, 0.25f);
		sections[0].AnimateScale(1f, 0.3f, FinishedAnimating);
	}

	private void CategoryChanged(int categoryNum)
	{
		sections[settingsCategories.previouslySelectedButton].AnimateScale(0f, 0.15f, () => sections[categoryNum].AnimateScale(1f, 0.15f));
	}
}
