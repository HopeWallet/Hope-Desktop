using Zenject;

/// <summary>
/// Class which handles the factory for ui popups.
/// </summary>
/// <typeparam name="T"> The type of the FactoryPopup. </typeparam>
public abstract class FactoryPopup<T> : PopupBase where T : FactoryPopup<T>
{
	protected PopupManager popupManager;

	/// <summary>
	/// Injects the PopupManager dependencies into this popup.
	/// </summary>
	/// <param name="popupManager"> The active PopupManager to use. </param>
	[Inject]
	public void Construct(PopupManager popupManager) => this.popupManager = popupManager;

	/// <summary>
	/// Class which represents the factory for popups.
	/// </summary>
	public class Factory : Factory<T>
    {
    }
}