using UnityEngine;

public class DropdownFixer : MonoBehaviour
{
	[SerializeField] private bool twoItems;

	private void OnEnable()
	{
		CoroutineUtils.ExecuteAfterWait(0.1f, () =>
		{
			RectTransform rectTransform = GetComponent<RectTransform>();
			rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, twoItems ? -52f : - 77f);
		});
	}
}
