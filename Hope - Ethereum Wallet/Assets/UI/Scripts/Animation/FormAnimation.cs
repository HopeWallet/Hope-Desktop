using UnityEngine;

public abstract class FormAnimation : MonoBehaviour
{

	public GameObject blocker;
	private bool animating;

	public bool Animating
	{
		get { return animating; }
		protected set { ChangeAnimationState(value); }
	}

	protected virtual void InitializeElements() { }

	protected void DisableMenu()
	{
		ChangeAnimationState(true);
		AnimateOut();
	}

	private void OnEnable()
	{
		InitializeElements();
		ChangeAnimationState(true);
		AnimateIn();
	}

	protected abstract void AnimateIn();

	protected abstract void AnimateOut();

	protected void FinishedAnimatingIn() => ChangeAnimationState(false);

	protected void FinishedAnimatingOut()
	{
		ChangeAnimationState(false);
		gameObject.SetActive(false);
	}

	private void ChangeAnimationState(bool state)
	{
		blocker.SetActive(state);
		animating = state;
	}

}
