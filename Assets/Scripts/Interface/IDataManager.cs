using System;
using System.Collections.Generic;

public interface IDataManager
{
    public abstract string GetDefaultFilePath();
    public abstract void LoadDataFromJson();
}

public interface IDataManager<TKey, TValue>
{
    public abstract Dictionary<TKey, TValue> GetDataDictionry();
    public abstract TValue GetData(TKey key);
}