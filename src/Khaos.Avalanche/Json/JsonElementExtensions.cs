using System.Text.Json;

namespace Khaos.Avalanche.Json;

public static class JsonElementExtensions
{
    public static JsonElement GetPropertyByNameVariants(this JsonElement element, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            if (element.TryGetProperty(propertyName, out var result))
            {
                return result;
            }
        }

        throw new KeyNotFoundException();
    }

    public static JsonElement TryWalkPropertiesWithNameVariants(
        this JsonElement element,
        params string[][] propertiesWithVariants)
    {
        foreach (var propertyNames in propertiesWithVariants)
        {
            var found = false;
            
            foreach (var propertyName in propertyNames)
            {
                if (element.TryGetProperty(propertyName, out var result))
                {
                    element = result;
                    found = true;

                    break;
                }
            }

            if (!found)
            {
                return new JsonElement();
            }
        }

        return element;
    }
}