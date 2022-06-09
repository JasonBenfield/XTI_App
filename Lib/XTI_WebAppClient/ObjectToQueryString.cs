namespace XTI_WebAppClient;

public sealed class ObjectToQueryString
{
    public ObjectToQueryString(object? obj)
    {
        var nameValues = new List<string>();
        addNameValues(nameValues, "", obj);
        var value = string.Join("&", nameValues);
        Value = string.IsNullOrWhiteSpace(value) ? "" : $"?{value}";
    }

    private void addNameValues(List<string> nameValues, string prefix, object? obj)
    {
        if(obj != null)
        {
            foreach(var property in obj.GetType().GetProperties())
            {
                var value = property.GetValue(obj);
                if(value != null)
                {
                    if(property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                    {
                        nameValues.Add($"{prefix}{property.Name}={value}");
                    }
                    else
                    {
                        addNameValues(nameValues, $"{prefix}{property.Name}.", value);
                    }
                }
            }
        }
    }

    public string Value { get; }
}
