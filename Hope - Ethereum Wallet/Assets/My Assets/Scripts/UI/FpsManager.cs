using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FpsManager : MonoBehaviour
{
    public static int DesiredFPS;

    private const int ANIMATING_FPS = 60;
    private const int IDLE_FPS = 25;
    private const int UNFOCUS_FPS = 3;

    private void Awake()
    {
        UpdateFpsAndAnimations(ANIMATING_FPS);
    }

    private void OnApplicationFocus(bool focus)
    {
        Application.targetFrameRate = focus ? DesiredFPS : UNFOCUS_FPS;
    }

    private void Update()
    {
        if (DesiredFPS == ANIMATING_FPS && !UIAnimator.IsAnimatingUI)
        {
            UpdateFpsAndAnimations(IDLE_FPS);
        }
        else if (DesiredFPS == IDLE_FPS && UIAnimator.IsAnimatingUI)
        {
            UpdateFpsAndAnimations(ANIMATING_FPS);
        }
    }

    private void UpdateFpsAndAnimations(int targetFps)
    {
        DesiredFPS = targetFps;
        Application.targetFrameRate = targetFps;
    }
}
