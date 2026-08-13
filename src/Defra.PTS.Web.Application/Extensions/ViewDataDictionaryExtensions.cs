using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Web.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ViewDataDictionaryExtensions
{
    public static T Get<T>(this ViewDataDictionary viewData, string key = null)
    {
        ArgumentNullException.ThrowIfNull(viewData);

        key ??= typeof(T).FullName;
        
        return !viewData.TryGetValue(key, out var result)
            ? throw new KeyNotFoundException($"ViewData does not contain a value for {JsonConvert.SerializeObject(key)}")
            : (T)result;
    }

    public static void Set<T>(this ViewDataDictionary viewData, T value) => Set(viewData, typeof(T).FullName, value);

    public static void Set<T>(this ViewDataDictionary viewData, string key, T value)
    {
        ArgumentNullException.ThrowIfNull(viewData);
        viewData[key] = value;
    }
}