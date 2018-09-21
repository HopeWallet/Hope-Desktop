using System.Collections;
using UnityEngine;

public sealed partial class OpenWalletMenu : Menu<OpenWalletMenu>
{
	/// <summary>
	/// The class that manages the user's idle time
	/// </summary>
    public sealed class IdleTimeoutManager : MonoBehaviour
    {
        private readonly WaitForSeconds waiter = new WaitForSeconds(1f);

        private readonly UIManager uiManager;

        private Vector3 previousMousePosition;
        private int currentIdleTime;

		/// <summary>
		/// Sets the UIManager and starts the idle time couroutine
		/// </summary>
		/// <param name="uiManager"></param>
        public IdleTimeoutManager(UIManager uiManager)
        {
            this.uiManager = uiManager;

            CheckIfIdle().StartCoroutine();
        }

		/// <summary>
		/// Waits one second, and then checks if the idle time has been met, if not, it continues
		/// </summary>
		/// <returns> Returns one WaitForSeconds </returns>
        private IEnumerator CheckIfIdle()
        {
            yield return waiter;

            if (SecurePlayerPrefs.GetBool("idle timeout") && uiManager.ActiveMenuType != typeof(ReEnterPasswordMenu))
            {
                if (previousMousePosition == Input.mousePosition)
                {
                    if ((currentIdleTime / 60) == SecurePlayerPrefs.GetInt("idle time"))
                    {
                        uiManager.OpenMenu<ReEnterPasswordMenu>();
                        currentIdleTime = 0;
                    }
                    else
                    {
                        currentIdleTime++;
                    }
                }
                else
                {
                    currentIdleTime = 0;
                }
            }

            previousMousePosition = Input.mousePosition;
            CheckIfIdle().StartCoroutine();
        }
    }
}