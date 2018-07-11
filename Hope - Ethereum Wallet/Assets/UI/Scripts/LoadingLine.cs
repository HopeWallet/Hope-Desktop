using System.Collections;
using UnityEngine;

public class LoadingLine : MonoBehaviour
{

	[SerializeField] private GameObject[] circles;

	void Start() => StartCoroutine(MoveCircles());

	private IEnumerator MoveCircles()
	{
		yield return new WaitForSeconds(0.1f);

		for (int i = 0; i < circles.Length; i++)
		{
			Vector2 circleTransform = circles[i].transform.localPosition;
			circles[i].transform.localPosition = new Vector2(circleTransform.x >= 100 ? -100f : circleTransform.x + 25f, 0f);
		}

		StartCoroutine(MoveCircles());
	}
}
