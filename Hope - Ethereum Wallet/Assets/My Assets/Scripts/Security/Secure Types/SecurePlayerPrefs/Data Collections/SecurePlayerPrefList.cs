using System;
using System.Collections;
using System.Collections.Generic;

public sealed class SecurePlayerPrefList<T> : IList<T>
{
    private readonly List<T> list = new List<T>();
    private readonly string keyName;

    private string jsonString;

    public SecurePlayerPrefList(string keyName)
    {
        this.keyName = keyName;

        GetPrefJson();
    }

    private void GetPrefJson()
    {
        if (!SecurePlayerPrefs.HasKey(keyName))
            return;

        jsonString = SecurePlayerPrefs.GetString(keyName);
        list.AddItems(JsonUtils.Deserialize<ItemArray>(jsonString).items);
    }

    private void SetPrefJson()
    {
        ItemArray array = new ItemArray(list.ToArray());
        jsonString = JsonUtils.Serialize(array);

        SecurePlayerPrefs.SetString(keyName, jsonString);
    }

    public T this[int index]
    {
        get
        {
            if (list.Count > index)
                return list[index];

            throw new IndexOutOfRangeException("Index out of the bounds of SecurePlayerPrefList!");
        }
        set
        {
            if (list.Count > index)
                list[index] = value;
            else
                throw new IndexOutOfRangeException("Index out of the bounds of SecurePlayerPrefList!");
        }
    }

    public int Count => 0;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        list.Add(item);
        SetPrefJson();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public int IndexOf(T item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

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