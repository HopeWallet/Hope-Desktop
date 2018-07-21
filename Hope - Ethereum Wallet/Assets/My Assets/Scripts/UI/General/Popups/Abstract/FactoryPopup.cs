using Zenject;

/// <summary>
/// Class which handles the factory for ui popups.
/// </summary>
/// <typeparam name="T"> The type of the FactoryPopup. </typeparam>
public abstract class FactoryPopup<T> : PopupBase where T : FactoryPopup<T>
{

    /// <summary>
    /// Class which represents the factory for popups.
    /// </summary>
    public class Factory : Factory<T>
    {
    }
}