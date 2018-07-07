using System;
using UnityEngine;

public abstract class MenuAnimator : MonoBehaviour
{

    [SerializeField]
	private GameObject blocker;

    private Action onAnimationFinished;

	private bool animating;

	public bool Animating
	{
		get { return animating; }
		protected set { ChangeAnimationState(value); }
	}

    public void AnimateEnable(Action onAnimationFinished = null)
    {
        this.onAnimationFinished = onAnimationFinished;

        ChangeAnimationState(true);
        AnimateIn();
    }

    public void AnimateDisable(Action onAnimationFinished = null)
    {
        this.onAnimationFinished = onAnimationFinished;

        ChangeAnimationState(true);
        AnimateOut();
    }

	protected abstract void AnimateIn();

	protected abstract void AnimateOut();

    protected void FinishedAnimating()
    {
        ChangeAnimationState(false);
        onAnimationFinished?.Invoke();
    }

	private void ChangeAnimationState(bool state)
	{
		blocker.SetActive(state);
		animating = state;
	}

}
