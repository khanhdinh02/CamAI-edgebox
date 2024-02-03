namespace CamAI.EdgeBox.Services.Utils;

public static class DictionaryExtension
{
    public static void SetOrIncrease<TKey>(this Dictionary<TKey, int> dict, TKey key, int amount) where TKey : notnull
    {
        if (!dict.TryAdd(key, amount))
            dict[key] += amount;
    }
}