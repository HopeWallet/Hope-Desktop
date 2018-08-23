using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransactionTabsManager : MonoBehaviour
{
	public static event Action<TabType> OnTabChanged;

	private Button[] transactionTabs;
	private TextMeshProUGUI[] buttonTextElements;

	private int previouslyActiveTab;

	/// <summary>
	/// Sets the variables and button listeners
	/// </summary>
	private void Awake()
	{
		transactionTabs = new Button[transform.childCount];
		buttonTextElements = new TextMeshProUGUI[transform.childCount];

		for (int i = 0; i < transform.childCount; i++)
		{
			transactionTabs[i] = transform.GetChild(i).GetComponent<Button>();
			buttonTextElements[i] = transactionTabs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			SetButtonListener(i);
		}
	}

	/// <summary>
	/// Sets the onClick listener for the given button
	/// </summary>
	/// <param name="index"> The index of the tab being pressed </param>
	private void SetButtonListener(int index) => transactionTabs[index].onClick.AddListener(() => TabClicked(index));

	/// <summary>
	/// A tab has been clicked
	/// </summary>
	/// <param name="activeIndex"> The tab that has been clicked </param>
	public void TabClicked(int activeIndex)
	{
		ChangeTabLook(previouslyActiveTab, false);
		ChangeTabLook(activeIndex, true);

		previouslyActiveTab = activeIndex;

		OnTabChanged?.Invoke((TabType)activeIndex);
	}

	/// <summary>
	/// Changes the button look
	/// </summary>
	/// <param name="tab"> The index of the tab being changed </param>
	/// <param name="activeTab"> Whether the tab is currently active or not </param>
	private void ChangeTabLook(int tab, bool activeTab)
	{
		transactionTabs[tab].interactable = !activeTab;
		buttonTextElements[tab].color = activeTab ? UIColors.White : UIColors.LightGrey;
	}

	/// <summary>
	/// The tab types
	/// </summary>
	public enum TabType { All, Sent, Received }
}
