using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransactionTabsManager : MonoBehaviour
{

	public static event Action<TabType> OnTabChanged;

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
		ChangeTabLook(previouslyActiveTab, false);
		ChangeTabLook(activeIndex, true);

		previouslyActiveTab = activeIndex;

		OnTabChanged?.Invoke((TabType)activeIndex);
	}

	private void ChangeTabLook(int tab, bool activeTab)
	{
		transactionTabs[tab].interactable = !activeTab;
		buttonTextElements[tab].color = activeTab ? ACTIVE_TAB_COLOR : INACTIVE_TAB_COLOR;
		transactionTabs[tab].transform.localScale = new Vector2(transactionTabs[tab].transform.localScale.x, activeTab ? 1.25f : 1f);
		buttonTextElements[tab].transform.localScale = new Vector2(buttonTextElements[tab].transform.localScale.x, activeTab ? 0.85f : 1f);
	}

	public enum TabType { All, Sent, Received, Pending };
}
