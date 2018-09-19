using System.Collections;
using UnityEngine;

public sealed partial class OpenWalletMenu : Menu<OpenWalletMenu>
{
    public sealed class IdleTimeoutManager : MonoBehaviour
    {
        private readonly WaitForSeconds waiter = new WaitForSeconds(1f);

        private readonly UIManager uiManager;

        private Vector3 previousMousePosition;
        private int currentIdleTime;

        public IdleTimeoutManager(UIManager uiManager)
        {
            this.uiManager = uiManager;

            CheckIfIdle().StartCoroutine();
        }

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