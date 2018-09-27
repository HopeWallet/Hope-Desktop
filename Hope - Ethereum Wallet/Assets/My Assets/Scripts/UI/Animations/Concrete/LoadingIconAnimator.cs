using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

/// <summary>
/// Class which animates the loading icon.
/// </summary>
public class LoadingIconAnimator : MonoBehaviour, IUpdater
{
	[SerializeField] private bool pulsateOnly;

	private Image image;

    private GameObject iconObj;
    private Transform iconTransform;

    private readonly Color LIGHT_COLOR = new Color(1f, 1f, 1f);
    private readonly Color GRAY_COLOR = new Color(0.7f, 0.7f, 0.7f);

    private readonly Vector3 ICON_ROTATION = new Vector3(0f, 0f, -1.2f);

	private bool isEnabled = true;

    [Inject]
    private UpdateManager updateManager;

    /// <summary>
    /// Sets randomized variables
    /// </summary>
    private void Awake()
    {
        iconObj = gameObject;
        iconTransform = transform;
        image = transform.GetComponent<Image>();
    }

    /// <summary>
    /// Starts the animations.
    /// </summary>
    private void OnEnable()
    {
		updateManager.AddUpdater(this);
		image.color = GRAY_COLOR;
		AnimateColor(true);
	}

	/// <summary>
	/// Removes the updater.
	/// </summary>
	private void OnDisable() => updateManager.RemoveUpdater(this);

	/// <summary>
	/// Animates the image's color to a white or a gray
	/// </summary>
	/// <param name="isGray"> Checks what the current color state is </param>
	private void AnimateColor(bool isGray)
    {
        if (this.enabled)
            image.DOColor(isGray ? LIGHT_COLOR : GRAY_COLOR, 1f).OnComplete(() => AnimateColor(!isGray));
    }

    /// <summary>
    /// Updates the rotation of the loading icon.
    /// </summary>
    public void UpdaterUpdate()
    {
        if (this.enabled && !pulsateOnly)
            iconTransform.DOLocalRotate(ICON_ROTATION, 0.01f, RotateMode.LocalAxisAdd);
    }
}
