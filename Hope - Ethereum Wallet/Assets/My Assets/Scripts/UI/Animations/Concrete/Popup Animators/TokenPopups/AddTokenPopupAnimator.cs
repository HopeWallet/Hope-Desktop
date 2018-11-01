using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The animator class of the AddTokenPopup
/// </summary>
public sealed class AddTokenPopupAnimator : PopupAnimator
{
	[SerializeField] private GameObject addressInputField;
	[SerializeField] private GameObject loadingIcon;
	[SerializeField] private GameObject noTokenFoundSection;
	[SerializeField] private GameObject invalidTokenSection;
	[SerializeField] private GameObject validTokenSection;
	[SerializeField] private GameObject addTokenButton;
	[SerializeField] private GameObject tokenListSection;
	[SerializeField] private GameObject tooManyResultsSection;

	private AddTokenPopup.Status previousStatus;

	/// <summary>
	/// Initializes the button listeners
	/// </summary>
	private void Awake()
	{
        GetComponent<AddTokenPopup>().OnStatusChanged += OnStatusChanged;
		previousStatus = AddTokenPopup.Status.NoTokenFound;
	}

	/// <summary>
	/// Animates the unique elements of this form into view
	/// </summary>
	protected override void AnimateUniqueElementsIn()
	{
		addressInputField.GetComponent<HopeInputField>().InputFieldBase.ActivateInputField();
		addressInputField.AnimateScaleX(1f, 0.2f);
		noTokenFoundSection.AnimateGraphicAndScale(1f, 1f, 0.25f);
		addTokenButton.AnimateGraphicAndScale(1f, 1f, 0.3f, FinishedAnimating);
	}

	/// <summary>
	/// Gets the current status of the AddTokenPopup and sets the proper sections to visible or not
	/// </summary>
	/// <param name="tokenPopupStatus"> The AddTokenPopup status </param>
	private void OnStatusChanged(AddTokenPopup.Status tokenPopupStatus)
    {
		if (tokenPopupStatus == previousStatus)
			return;

		switch (tokenPopupStatus)
		{
			case AddTokenPopup.Status.Loading:
				ChangeStatus(loading: true);
				break;
			case AddTokenPopup.Status.NoTokenFound:
				ChangeStatus(noTokenFound: true);
				break;
			case AddTokenPopup.Status.InvalidToken:
				ChangeStatus(invalidToken: true);
				break;
			case AddTokenPopup.Status.ValidToken:
				ChangeStatus(validToken: true);
				break;
			case AddTokenPopup.Status.MultipleTokensFound:
				ChangeStatus(tokenList: true);
				break;
			case AddTokenPopup.Status.TooManyResults:
				ChangeStatus(tooManyResults: true);
				break;
		}

		previousStatus = tokenPopupStatus;
	}

	/// <summary>
	/// Changes the sections according to their booleans
	/// </summary>
	/// <param name="loading"> Whether the loading icon should appear </param>
	/// <param name="noTokenFound"> Whether the noTokenFoundSection should appear </param>
	/// <param name="invalidToken"> Whether the invalidTokenSection should appear </param>
	/// <param name="validToken"> Whether the validTokenSection should appear </param>
	/// <param name="tokenList"> Whether the tokenList should appear </param>
	/// <param name="tooManyResults"> Whether there are too many token results </param>
	private void ChangeStatus(bool loading = false, bool noTokenFound = false, bool invalidToken = false, bool validToken = false, bool tokenList = false, bool tooManyResults = false)
	{
		AnimateSection(loadingIcon, loading);
		AnimateSection(noTokenFoundSection, noTokenFound);
		AnimateSection(invalidTokenSection, invalidToken);
		AnimateSection(validTokenSection, validToken);
		AnimateSection(tokenListSection, tokenList);
		AnimateSection(tooManyResultsSection, tooManyResults);
	}

	/// <summary>
	/// Animates a section in if the boolean states, or makes the section disappear otherwise
	/// </summary>
	/// <param name="section"> The section being modified </param>
	/// <param name="animateIn"> Whether animating section in or not </param>
	private void AnimateSection(GameObject section, bool animateIn)
	{
		if (animateIn)
		{
			if (section == loadingIcon) loadingIcon.SetActive(true);
			section.AnimateGraphicAndScale(1f, 1f, 0.1f);
		}
		else
		{
			if (section == loadingIcon)
			{
				loadingIcon.SetActive(false);
				loadingIcon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
			}
			section.transform.localScale = Vector2.zero;
		}
	}
}
