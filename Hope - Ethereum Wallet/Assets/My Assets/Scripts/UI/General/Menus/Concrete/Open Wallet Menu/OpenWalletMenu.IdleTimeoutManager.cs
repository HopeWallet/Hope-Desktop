using System;
using System.Collections;
using UnityEngine;

public sealed partial class OpenWalletMenu : Menu<OpenWalletMenu>
{
    public sealed class IdleTimeoutManager : MonoBehaviour
    {
        public static Action IdleTimeoutEnabled;

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
            yield return new WaitForSeconds(1);

            if (!SecurePlayerPrefs.GetBool("idle timeout"))
                yield break;

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
}