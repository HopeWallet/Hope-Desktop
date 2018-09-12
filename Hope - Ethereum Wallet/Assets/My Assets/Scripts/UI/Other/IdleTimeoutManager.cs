using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class IdleTimeoutManager : MonoBehaviour
{
	public static Action IdleTimeoutEnabled;

	private Vector3 previousMousePosition;

	private int currentIdleTime, maxIdleTime;

	private PopupManager popupManager;

	[Inject]
	public void Construct(PopupManager popupManager)
	{
		this.popupManager = popupManager;
	}

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

		if (!SecurePlayerPrefs.GetBool("idle timeout") || popupManager.ActivePopupType == typeof(UnlockWalletPopup))
			yield break;

		currentIdleTime.Log();

		if (previousMousePosition == Input.mousePosition)
		{
			if ((currentIdleTime / 60) == maxIdleTime)
			{
				popupManager.GetPopup<UnlockWalletPopup>().SetPopupDetails(() => CheckIfIdle().StartCoroutine(), false);
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
