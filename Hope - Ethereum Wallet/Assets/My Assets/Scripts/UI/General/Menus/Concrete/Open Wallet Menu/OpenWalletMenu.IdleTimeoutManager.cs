using System;
using System.Collections;
using UnityEngine;

public sealed partial class OpenWalletMenu : Menu<OpenWalletMenu>
{
    public sealed class IdleTimeoutManager : MonoBehaviour
    {
        public static Action IdleTimeoutEnabled;

        private readonly WaitForSeconds waiter = new WaitForSeconds(1f);

        private readonly UIManager uiManager;
        private readonly int maxIdleTime;

        private Vector3 previousMousePosition;
        private int currentIdleTime;

        public IdleTimeoutManager(UIManager uiManager)
        {
            this.uiManager = uiManager;

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
            yield return waiter;

            if (!SecurePlayerPrefs.GetBool("idle timeout"))
                yield break;

            if (uiManager.ActiveMenuType != typeof(ReEnterPasswordMenu))
            {
                if (previousMousePosition == Input.mousePosition)
                {
                    if ((currentIdleTime / 60) == maxIdleTime)
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