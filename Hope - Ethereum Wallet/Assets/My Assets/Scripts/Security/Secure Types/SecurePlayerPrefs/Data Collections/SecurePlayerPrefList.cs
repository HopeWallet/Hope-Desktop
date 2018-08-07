using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class SecurePlayerPrefList<T> : IList<T>
{
    private readonly List<T> itemList = new List<T>();
    private readonly List<string> serializedItemList = new List<string>();

    private readonly string keyName;

    private string jsonString;

    public SecurePlayerPrefList(string keyName)
    {
        this.keyName = keyName;

        InitializeList();
    }

    private void InitializeList()
    {
        if (!SecurePlayerPrefs.HasKey(keyName))
            return;

        jsonString = SecurePlayerPrefs.GetString(keyName);

        var items = JsonUtils.Deserialize<ItemArray>(jsonString).items;
        itemList.AddItems(items);
        serializedItemList.AddItems(items.Select(item => JsonUtils.Serialize(item)).ToArray());
    }

    private void UpdatePlayerPrefs()
    {
        ItemArray array = new ItemArray(itemList.ToArray());
        jsonString = JsonUtils.Serialize(array);

        SecurePlayerPrefs.SetString(keyName, jsonString);
    }

    public T this[int index]
    {
        get
        {
            if (itemList.Count > index)
                return itemList[index];

            throw new IndexOutOfRangeException("Index out of the bounds of SecurePlayerPrefList!");
        }
        set
        {
            if (itemList.Count > index)
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

    public int Count => itemList.Count;

    public bool IsReadOnly => false;

    public void Clear()
    {
        itemList.Clear();
        serializedItemList.Clear();

        jsonString = string.Empty;
        SecurePlayerPrefs.DeleteKey(keyName);
    }

    public void Add(T item)
    {
        string serializedItem = JsonUtils.Serialize(item);

        if (jsonString.Contains(serializedItem))
            return;

        itemList.Add(item);
        serializedItemList.Add(serializedItem);

        UpdatePlayerPrefs();
    }

    public void Insert(int index, T item)
    {
        string serializedItem = JsonUtils.Serialize(item);

        if (jsonString.Contains(serializedItem))
            return;

        itemList.Insert(index, item);
        serializedItemList.Insert(index, serializedItem);

        UpdatePlayerPrefs();
    }

    public bool Remove(T item)
    {
        string serializedItem = JsonUtils.Serialize(item);

        if (!jsonString.Contains(serializedItem))
            return false;

        int index = IndexOf(item);

        itemList.RemoveAt(index);
        serializedItemList.RemoveAt(index);

        UpdatePlayerPrefs();

        return true;
    }

    public void RemoveAt(int index)
    {
        if (itemList.Count <= index)
            return;

        itemList.RemoveAt(index);
        serializedItemList.RemoveAt(index);

        UpdatePlayerPrefs();
    }

    public bool Contains(T item) => jsonString.Contains(JsonUtils.Serialize(item));

    public int IndexOf(T item) => serializedItemList.IndexOf(serializedItemList.First(i => i.Equals(JsonUtils.Serialize(item))));

    public void CopyTo(T[] array, int arrayIndex) => itemList.ToArray().CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => itemList.GetEnumerator();

    [Serializable]
    private class ItemArray
    {
        public T[] items;

        public ItemArray(T[] items)
        {
            this.items = items;
        }
    }
}