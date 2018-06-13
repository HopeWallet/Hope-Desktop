using Zenject;

/// <summary>
/// Class which represents a button that is created at runtime and requires its dependencies filled.
/// </summary>
/// <typeparam name="T"> The type of the FactoryButton. </typeparam>
public abstract class FactoryButton<T> : ButtonBase where T : ButtonBase
{

    /// <summary>
    /// Class which is used as the factory for this button.
    /// </summary>
    public class Factory : Factory<T>
    {
    }
}