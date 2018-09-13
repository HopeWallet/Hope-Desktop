using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class IdleTimeoutManager : MonoBehaviour
{
	public static Action IdleTimeoutEnabled;

	private Vector3 previousMousePosition;

	private int currentIdleTime, maxIdleTime;

	private UIManager uiManager;

	[Inject]
	public void Construct(UIManager uiManager) => this.uiManager = uiManager;

	private void Start()
	{
		IdleTimeoutEnabled = () => CheckIfIdle().StartCoroutine();

		if (SecurePlayerPrefs.GetBool("idle timeout"))
		{
			maxIdleTime = SecurePlayerPrefs.GetInt("idle time");

			previousMousePosition = Input.mousePosition;
			CheckIfIdle().StartCoroutine();
		}
	}

	private IEnumerator CheckIfIdle()
	{
		yield return new WaitForSeconds(1);

		if (!SecurePlayerPrefs.GetBool("idle timeout"))
			yield break;

		currentIdleTime.Log();

		if (previousMousePosition == Input.mousePosition)
		{
			if ((currentIdleTime / 60) == maxIdleTime)
			{
				uiManager.OpenMenu<ReEnterPasswordMenu>();
				yield break;
			}
			else
				currentIdleTime++;
		}
		else
		{
			currentIdleTime = 0;
		}

		previousMousePosition = Input.mousePosition;
		CheckIfIdle().StartCoroutine();
	}
}
