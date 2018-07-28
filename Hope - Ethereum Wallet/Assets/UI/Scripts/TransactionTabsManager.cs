using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransactionTabsManager : MonoBehaviour
{

	[SerializeField] private Button[] transactionTabs;

	private TextMeshProUGUI[] buttonTextElements;

	private readonly Color ACTIVE_TAB_COLOR = new Color(1f, 1f, 1f, 0.847f);
	private readonly Color INACTIVE_TAB_COLOR = new Color(1f, 1f, 1f, 0.647f);

	private int previouslyActiveTab;

	private void Awake()
	{
		buttonTextElements = new TextMeshProUGUI[transactionTabs.Length];

		for (int i = 0; i < transactionTabs.Length; i++)
			buttonTextElements[i] = transactionTabs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	public void TabClicked(int activeIndex)
	{
		transactionTabs[previouslyActiveTab].interactable = true;
		buttonTextElements[previouslyActiveTab].color = INACTIVE_TAB_COLOR;

		transactionTabs[activeIndex].interactable = false;
		buttonTextElements[activeIndex].color = ACTIVE_TAB_COLOR;

		previouslyActiveTab = activeIndex;
	}
}
