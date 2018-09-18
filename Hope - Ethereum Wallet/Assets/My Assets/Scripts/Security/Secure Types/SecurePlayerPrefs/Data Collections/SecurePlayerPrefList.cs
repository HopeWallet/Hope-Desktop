using Hope.Security.HashGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class used to represent a collection of data usable and updatable through the <see cref="SecurePlayerPrefs"/>.
/// </summary>
/// <typeparam name="T"> The type of the data to create a list out of in the <see cref="SecurePlayerPrefs"/>. Must be readable and writable to json format. </typeparam>
public sealed class SecurePlayerPrefList<T> : IList<T>
{
    private readonly List<T> itemList = new List<T>();
    private readonly List<string> serializedItemList = new List<string>();

    private readonly string keyName;
    private readonly object listId;

    /// <summary>
    /// The <see langword="string"/> which holds all serialized list data.
    /// </summary>
    public string SerializedList { get; private set; } = string.Empty;

    /// <summary>
    /// The readonly List of this SecurePlayerPrefList.
    /// </summary>
    public List<T> ReadonlyList => itemList.ToList();

    /// <summary>
    /// Initializes the <see cref="SecurePlayerPrefList"/> given the key to use to access the list in the <see cref="SecurePlayerPrefs"/>.
    /// </summary>
    /// <param name="keyName"> The key of the <see cref="SecurePlayerPrefs"/> containing the list data. </param>
    /// <param name="listId"> The id of this list. Can be used to have multiple versions of the same list. </param>
    public SecurePlayerPrefList(string keyName, object listId)
    {
        this.keyName = keyName;
        this.listId = listId;

        InitializeList();
    }

    /// <summary>
    /// Initializes the <see cref="SecurePlayerPrefList"/> given the key to use to access the list in the <see cref="SecurePlayerPrefs"/>.
    /// </summary>
    /// <param name="keyName"> The key of the <see cref="SecurePlayerPrefs"/> containing the list data. </param>
    public SecurePlayerPrefList(string keyName) : this(keyName, null)
    {
    }

    /// <summary>
    /// Gets the full key name based on the keyName string and unique listId.
    /// </summary>
    /// <returns> The full key name which can be used to set/get the prefs. </returns>
    private string GetFullKeyName() => listId != null ? string.Concat(keyName, listId.ToString()).Keccak_128() : keyName;

    /// <summary>
    /// Initializes all values in the <see cref="SecurePlayerPrefList"/>.
    /// </summary>
    private void InitializeList()
    {
        if (!SecurePlayerPrefs.HasKey(GetFullKeyName()))
            return;

        SerializedList = SecurePlayerPrefs.GetString(GetFullKeyName());

        var items = JsonUtils.Deserialize<ListJson>(SerializedList).items;
        itemList.AddItems(items);
        serializedItemList.AddItems(items.Select(item => JsonUtils.Serialize(item)).ToArray());
    }

    /// <summary>
    /// Updates the <see cref="SecurePlayerPrefs"/> with the new values of the <see cref="SecurePlayerPrefList"/>.
    /// </summary>
    private void UpdatePlayerPrefs()
    {
        ListJson array = new ListJson(itemList.ToArray());
        SerializedList = JsonUtils.Serialize(array);

        SecurePlayerPrefs.SetString(GetFullKeyName(), SerializedList);
    }

    /// <summary>
    /// Gets or sets an item at a given index.
    /// </summary>
    /// <param name="index"> The index to get or set a new item at. </param>
    /// <returns> The item returned from the <see cref="SecurePlayerPrefList"/> at the given index. </returns>
    public T this[int index]
    {
        get
        {
            if (itemList.Count > index && index >= 0)
                return itemList[index];

            throw new IndexOutOfRangeException("Index out of the bounds of SecurePlayerPrefList!");
        }
        set
        {
            if (itemList.Count > index && index >= 0)
            {
                itemList[index] = value;
                serializedItemList[index] = JsonUtils.Serialize(value);
                UpdatePlayerPrefs();
            }
            else
            {
                throw new IndexOutOfRangeException("Index out of the bounds of SecurePlayerPrefList!");
            }
        }
    }

    /// <summary>
    /// Gets or sets an item that was found that contains a certain string text.
    /// </summary>
    /// <param name="text"> The text to search for. </param>
    /// <returns> The item found which contains a certain <see langword="string"/> text. </returns>
    public T this[string text]
    {
        get { return this[IndexOf(text)]; }
        set { this[IndexOf(text)] = value; }
    }

    /// <summary>
    /// The number of elements in this <see cref="SecurePlayerPrefList"/>.
    /// </summary>
    public int Count => itemList.Count;

    /// <summary>
    /// Whether this list is read only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Clears all elements from the <see cref="SecurePlayerPrefList"/> and removes all traces in the <see cref="SecurePlayerPrefs"/>.
    /// </summary>
    public void Clear()
    {
        itemList.Clear();
        serializedItemList.Clear();

        SerializedList = string.Empty;
        SecurePlayerPrefs.DeleteKey(GetFullKeyName());
    }

    /// <summary>
    /// Adds an item to the <see cref="SecurePlayerPrefList"/>.
    /// </summary>
    /// <param name="item"> The item to add. </param>
    public void Add(T item)
    {
        string serializedItem = JsonUtils.Serialize(item);

        if (SerializedList.Contains(serializedItem))
            return;

        itemList.Add(item);
        serializedItemList.Add(serializedItem);

        UpdatePlayerPrefs();
    }

    /// <summary>
    /// Inserts an item into the <see cref="SecurePlayerPrefList"/> at a given index.
    /// </summary>
    /// <param name="index"> The index to insert the item at. </param>
    /// <param name="item"> The item to insert into the <see cref="SecurePlayerPrefList"/>. </param>
    public void Insert(int index, T item)
    {
        string serializedItem = JsonUtils.Serialize(item);

        if (itemList.Count <= index || SerializedList.Contains(serializedItem))
            return;

        itemList.Insert(index, item);
        serializedItemList.Insert(index, serializedItem);

        UpdatePlayerPrefs();
    }

    /// <summary>
    /// Removes an item found which contained <see langword="string"/> value.
    /// </summary>
    /// <param name="textToSearch"> The <see langword="string"/> text to search for in the <see cref="SecurePlayerPrefList"/>. </param>
    /// <returns> True if the item which contained the string value was removed successfully, false otherwise. </returns>
    public bool Remove(string textToSearch)
    {
		if (string.IsNullOrEmpty(textToSearch.Trim()))
			return false;

        string[] items = serializedItemList.Where(item => item.Contains(textToSearch)).ToArray();

        if (items.Length == 0)
            return false;

        RemoveAt(serializedItemList.IndexOf(items[0]));

        return true;
    }

    /// <summary>
    /// Removes an item from the <see cref="SecurePlayerPrefList"/> at a given index.
    /// </summary>
    /// <param name="index"> The index to remove the item at. </param>
    public void RemoveAt(int index)
    {
        if (itemList.Count <= index)
            return;

        itemList.RemoveAt(index);
        serializedItemList.RemoveAt(index);

        UpdatePlayerPrefs();
    }

    /// <summary>
    /// Gets the index of an item found to contain a <see langword="string"/> value.
    /// </summary>
    /// <param name="textToSearch"> The <see langword="string"/> text to search for in the <see cref="SecurePlayerPrefList"/>. </param>
    /// <returns> Returns -1 if the text was not found, otherwise returns the index of the item which contained the <see langword="string"/> value. </returns>
    public int IndexOf(string textToSearch)
    {
		if (string.IsNullOrEmpty(textToSearch.Trim()))
			return -1;

        string[] items = serializedItemList.Where(item => item.Contains(textToSearch)).ToArray();

        if (items.Length == 0)
            return -1;

        return serializedItemList.IndexOf(items[0]);
    }

    /// <summary>
    /// Checks if the <see cref="SecurePlayerPrefList"/> contains a <see langword="string"/> value.
    /// </summary>
    /// <param name="textToSearch"> The <see langword="string"/> to search for in the <see cref="SecurePlayerPrefList"/>. </param>
    /// <returns> True if this <see cref="SecurePlayerPrefList"/> contains the item. </returns>
    public bool Contains(string textToSearch) => string.IsNullOrEmpty(textToSearch.Trim()) ? false : SerializedList.Contains(textToSearch);

    /// <summary>
    /// Checks if the <see cref="SecurePlayerPrefList"/> contains an item.
    /// </summary>
    /// <param name="item"> The item to search for. </param>
    /// <returns> True if this <see cref="SecurePlayerPrefList"/> contains the item. </returns>
    public bool Contains(T item) => Contains(JsonUtils.Serialize(item));

    /// <summary>
    /// Removes an item from this <see cref="SecurePlayerPrefList"/>.
    /// </summary>
    /// <param name="item"> The item to remove. </param>
    /// <returns> True if the item was successfully removed, false otherwise. </returns>
    public bool Remove(T item) => Remove(JsonUtils.Serialize(item));

    /// <summary>
    /// Gets the index of an item.
    /// </summary>
    /// <param name="item"> The item to get the index of. </param>
    /// <returns> Returns -1 if item was not found, otherwise returns the index. </returns>
    public int IndexOf(T item) => IndexOf(JsonUtils.Serialize(item));

    /// <summary>
    /// Copies the items from this <see cref="SecurePlayerPrefList"/> to another array.
    /// </summary>
    /// <param name="array"> The array to copy the list to. </param>
    /// <param name="arrayIndex"> The index of the array to start copying the values at. </param>
    public void CopyTo(T[] array, int arrayIndex) => itemList.ToArray().CopyTo(array, arrayIndex);

    /// <summary>
    /// Gets an <see cref="IEnumerator"/> of type <see cref="T"/> from the list of items.
    /// </summary>
    /// <returns> The <see cref="IEnumerator"/> of this list of items of type <see cref="T"/>. </returns>
    public IEnumerator<T> GetEnumerator() => itemList.GetEnumerator();

    /// <summary>
    /// Gets an <see cref="IEnumerator"/> of the list of items.
    /// </summary>
    /// <returns> The <see cref="IEnumerator"/> of this list of items. </returns>
    IEnumerator IEnumerable.GetEnumerator() => itemList.GetEnumerator();

    /// <summary>
    /// Class used to serialize/deserialize the list of data.
    /// </summary>
    [Serializable]
    private sealed class ListJson
    {
        public T[] items;

        /// <summary>
        /// Initializes the <see cref="ListJson"/> with the items.
        /// </summary>
        /// <param name="items"> The items to serialize. </param>
        public ListJson(T[] items)
        {
            this.items = items;
        }
    }
}