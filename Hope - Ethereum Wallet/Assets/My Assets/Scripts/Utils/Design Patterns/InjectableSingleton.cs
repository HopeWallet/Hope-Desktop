/// <summary>
/// Class which can be used as a singleton, while can also be injected in other classes and have other classes injected in this constructor initialization.
/// </summary>
/// <typeparam name="T"> The type of the InjectableSingleton. </typeparam>
public abstract class InjectableSingleton<T> where T : InjectableSingleton<T>
{

    /// <summary>
    /// The instance of this singleton.
    /// </summary>
    public static T Instance { get; private set; }

    /// <summary>
    /// Initializes this singleton by setting the instance.
    /// </summary>
    protected InjectableSingleton()
    {
        if (Instance != null)
            return;

        Instance = this as T;
    }

}
