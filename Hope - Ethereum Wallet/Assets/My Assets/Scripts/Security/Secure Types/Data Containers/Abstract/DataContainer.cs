/// <summary>
/// Base class used for passing around data as a reference type without creating copies in memory.
/// </summary>
/// <typeparam name="T"> The type of the data to contain. </typeparam>
public abstract class DataContainer<T>
{
    private readonly T value;

    /// <summary>
    /// Initializes the DataContainer with the value.
    /// </summary>
    /// <param name="value"> The value to assign to the container. </param>
    protected DataContainer(T value)
    {
        this.value = value;
    }
}