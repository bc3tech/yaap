namespace Yaap.TestCommon;

/// <summary>
/// Provides extension methods for accessing private fields and properties of objects.
/// </summary>
public static class TestExtensions
{
    /// <summary>
    /// Retrieves the value of a private field from the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the field value.</typeparam>
    /// <param name="obj">The object from which to retrieve the field value.</param>
    /// <param name="fieldName">The name of the private field.</param>
    /// <returns>The value of the private field, or <c>null</c> if the field value is <c>null</c>.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified field is not found in the object's type.</exception>
    public static T? GetPrivateFieldValue<T>(this object obj, string fieldName)
    {
        var type = obj.GetType();
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field is null)
        {
            throw new ArgumentException($"Field '{fieldName}' not found in type '{type.FullName}'.");
        }

        return (T?)field.GetValue(obj);
    }

    /// <summary>
    /// Retrieves the value of a private property from the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="obj">The object from which to retrieve the property value.</param>
    /// <param name="propertyName">The name of the private property.</param>
    /// <returns>The value of the private property, or <c>null</c> if the property value is <c>null</c>.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified property is not found in the object's type.</exception>
    public static T? GetPrivatePropertyValue<T>(this object obj, string propertyName)
    {
        var type = obj.GetType();
        var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (property is null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found in type '{type.FullName}'.");
        }

        return (T?)property.GetValue(obj);
    }
}
