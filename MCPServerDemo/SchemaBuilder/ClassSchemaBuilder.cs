using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MCPServerDemo.SchemaBuilder
{
    /// <summary>
    /// A utility class that generates a JSON Schema-like representation of a given C# type, including support for
    /// complex types and validation attributes.
    /// </summary>
    public static class ClassSchemaBuilder
    {
        /// <summary>
        /// Asynchronously builds a schema representation for the specified .NET type, including its properties and
        /// validation attributes.
        /// </summary>
        public static Dictionary<string, object?> Build(Type type, HashSet<Type>? visited = null)
        {
            visited ??= new HashSet<Type>();

            if (visited.Contains(type))
                return new Dictionary<string, object?>();

            visited.Add(type);

            var properties = new Dictionary<string, object?>();
            var requiredList = new List<string>();

            object? instance = null;
            try { instance = Activator.CreateInstance(type); } catch { }

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propType = prop.PropertyType;
                var entry = GetSchemaForType(propType, visited);

                if (entry is Dictionary<string, object?> dict)
                {
                    AddValidationAttributesToSchema(prop, dict);

                    if (IsRequired(prop))
                        requiredList.Add(prop.Name);

                    // Default values (only primitives)
                    if (instance != null && prop.GetIndexParameters().Length == 0)
                    {
                        var defaultValue = prop.GetValue(instance);
                        if (defaultValue != null && IsPrimitiveOrString(propType))
                        {
                            dict["default"] = defaultValue;
                        }
                    }
                }

                properties[prop.Name] = entry;
            }

            return new Dictionary<string, object?>
            {
                ["type"] = "object",
                ["properties"] = properties,
                ["required"] = requiredList,
                ["additionalProperties"] = false
            };
        }

        /// <summary>
        /// Adds validation-related metadata from the specified property to the provided schema dictionary based on data
        /// annotation attributes.
        /// </summary>
        /// <remarks>This method inspects the property for standard validation attributes such as
        /// RangeAttribute and RegularExpressionAttribute, as well as other ValidationAttribute-derived attributes. The
        /// extracted metadata is added to the dictionary using keys commonly used in schema definitions (e.g.,
        /// 'minimum', 'maximum', 'pattern', 'errorMessage'). Existing entries in the dictionary with the same keys may
        /// be overwritten.</remarks>
        private static void AddValidationAttributesToSchema(PropertyInfo prop, Dictionary<string, object?> dict)
        {
            var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                dict["minimum"] = rangeAttr.Minimum;
                dict["maximum"] = rangeAttr.Maximum;
                if (!string.IsNullOrWhiteSpace(rangeAttr.ErrorMessage))
                    dict["errorMessage"] = rangeAttr.ErrorMessage;
            }

            var regexAttr = prop.GetCustomAttribute<RegularExpressionAttribute>();
            if (regexAttr != null)
            {
                dict["pattern"] = regexAttr.Pattern;
                if (!string.IsNullOrWhiteSpace(regexAttr.ErrorMessage))
                    dict["errorMessage"] = regexAttr.ErrorMessage;
            }

            var validationAttrs = prop.GetCustomAttributes<ValidationAttribute>();
            foreach (var attr in validationAttrs)
            {
                if (!string.IsNullOrWhiteSpace(attr.ErrorMessage))
                {
                    dict["errorMessage"] = attr.ErrorMessage;
                }
            }
        }

        /// <summary>
        /// Generates a schema representation for the specified .NET type, supporting primitives, arrays, collections,
        /// enums, dictionaries, and complex objects.
        /// </summary>
        /// <remarks>This method supports a variety of .NET types, including nullable types, primitive
        /// types, arrays, generic collections, enums, and complex objects. For interface or abstract types, the schema
        /// includes all concrete implementations found in loaded assemblies. Circular references are handled using the
        /// visited set to avoid infinite recursion.</remarks>
        private static object? GetSchemaForType(Type type, HashSet<Type> visited)
        {
            // Nullable types
            if (Nullable.GetUnderlyingType(type) is Type underlyingNullable)
                return GetSchemaForType(underlyingNullable, visited);

            // Primitive types
            if (IsPrimitiveOrString(type))
            {
                return new Dictionary<string, object?>
                {
                    ["type"] = MapPrimitiveToString(type)
                };
            }

            // Arrays
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return new Dictionary<string, object?>
                {
                    ["type"] = "array",
                    ["items"] = GetSchemaForType(elementType!, visited)
                };
            }

            // IEnumerable / List<T>
            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            {
                Type elementType = type.IsGenericType
                    ? type.GetGenericArguments()[0]
                    : typeof(object);

                return new Dictionary<string, object?>
                {
                    ["type"] = "array",
                    ["items"] = GetSchemaForType(elementType, visited)
                };
            }

            // Enum
            if (type.IsEnum)
            {
                return new Dictionary<string, object?>
                {
                    ["type"] = "string",
                    ["enum"] = Enum.GetNames(type)
                };
            }

            // Dictionary<string, T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = type.GetGenericArguments()[0];
                var valueType = type.GetGenericArguments()[1];

                if (keyType != typeof(string))
                    throw new NotSupportedException("Only Dictionary<string, T> is supported.");

                return new Dictionary<string, object?>
                {
                    ["type"] = "object",
                    ["additionalProperties"] = GetSchemaForType(valueType, visited)
                };
            }

            // Polymorphic (interface / abstract)
            if (type.IsInterface || type.IsAbstract)
            {
                var implementations = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); }
                        catch { return Array.Empty<Type>(); }
                    })
                    .Where(t => type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    .ToList();

                return new Dictionary<string, object?>
                {
                    ["oneOf"] = implementations.Select(impl => new Dictionary<string, object?>
                    {
                        ["type"] = "object",
                        ["properties"] = Build(impl, visited)
                    }).ToList()
                };
            }

            // Complex object
            if (type.IsClass)
            {
                return Build(type, visited);
            }

            return new Dictionary<string, object?>
            {
                ["type"] = "object"
            };
        }

        /// <summary>
        /// Determines whether the specified type is a primitive type or a string.
        /// </summary>
        /// <param name="t"></param>
        /// <returns>A boolean indicating whether the specified type is a primitive type or a string.</returns>
        private static bool IsPrimitiveOrString(Type t)
        {
            return t.IsPrimitive ||
                   t == typeof(string) ||
                   t == typeof(decimal);
        }

        private static string MapPrimitiveToString(Type t)
        {
            if (t == typeof(string)) return "string";
            if (t == typeof(int) || t == typeof(long)) return "integer";
            if (t == typeof(bool)) return "boolean";
            if (t == typeof(float) || t == typeof(double) || t == typeof(decimal)) return "number";

            return "string";
        }

        private static bool IsRequired(PropertyInfo prop)
        {
            if (prop.GetCustomAttribute<RequiredAttribute>() != null)
                return true;

            var t = prop.PropertyType;
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return true;

            return false;
        }
    }
}