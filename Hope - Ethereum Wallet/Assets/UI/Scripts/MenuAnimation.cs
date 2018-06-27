using UnityEngine;

public abstract class MenuAnimation : MonoBehaviour
{

	public GameObject blocker;

	public bool Animating { get; private set; }

	protected void DisableMenu()
	{
		ChangeAnimationState(true);
		AnimateOut();
	}

	private void OnEnable()
	{
		ChangeAnimationState(true);
		AnimateIn();
	}

	protected abstract void AnimateIn();

	protected abstract void AnimateOut();

	protected void FinishedAnimatingIn()
	{
		ChangeAnimationState(false);
	}

	protected void FinishedAnimatingOut()
	{
		ChangeAnimationState(false);
		//Destroy(this.gameObject);
	}

	private void ChangeAnimationState(bool state)
	{
		blocker.SetActive(state);
		Animating = state;
	}

}
