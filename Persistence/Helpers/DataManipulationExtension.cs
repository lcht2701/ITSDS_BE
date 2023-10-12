using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Helpers;

public static class DataManipulationExtension
{
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> data, int page, int pageSize)
    {
        var totalCount = data.Count();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

        var paginatedData = data
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return paginatedData;
    }

    public static IEnumerable<T> Sort<T>(this IEnumerable<T> data, string key, string orderBy)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(orderBy))
            return data; // Return the original collection if key or orderBy is not specified.

        var propertyInfo = typeof(T).GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo == null)
            throw new ArgumentException($"Property with name {key} not found in type {typeof(T).Name}");

        bool ascending = string.Equals(orderBy, "asc", StringComparison.OrdinalIgnoreCase);

        return ascending ? data.OrderBy(item => propertyInfo.GetValue(item, null)) : data.OrderByDescending(item => propertyInfo.GetValue(item, null));
    }

    public static IEnumerable<T> Filter<T>(this IEnumerable<T> data, string key, string condition)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(condition))
        {
            throw new ArgumentException("Both key and condition must be specified.");
        }

        var property = typeof(T).GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
        {
            throw new ArgumentException($"Property with name {key} not found in type {typeof(T).Name}");
        }

        var parts = condition.Split(' ');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid condition format. Use 'operator value' (e.g., '== 20' or '== HCM').");
        }

        var operatorStr = parts[0];
        var valueStr = parts[1];

        object value = null;

        try
        {
            var propertyType = property.PropertyType;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            if (propertyType == typeof(string))
            {
                value = valueStr;
            }
            else
            {
                // Attempt to convert valueStr to the appropriate data type
                if (propertyType == typeof(int))
                {
                    value = int.Parse(valueStr);
                }
                else if (propertyType == typeof(decimal))
                {
                    value = decimal.Parse(valueStr);
                }
                // Add more cases for other data types as needed
            }
        }
        catch (FormatException)
        {
            throw new ArgumentException("Invalid value format for the property type.");
        }

        // Build a lambda expression for the filter
        var parameter = Expression.Parameter(typeof(T), "item");
        var propertyExpression = Expression.Property(parameter, property);
        var constantExpression = Expression.Constant(value, property.PropertyType);

        BinaryExpression filterExpression = null;

        switch (operatorStr)
        {
            case "=":
            case "==":
                filterExpression = Expression.Equal(propertyExpression, constantExpression);
                break;
            case "!=":
            case "<>":
                filterExpression = Expression.NotEqual(propertyExpression, constantExpression);
                break;
            case ">":
                filterExpression = Expression.GreaterThan(propertyExpression, constantExpression);
                break;
            case "<":
                filterExpression = Expression.LessThan(propertyExpression, constantExpression);
                break;
            case ">=":
                filterExpression = Expression.GreaterThanOrEqual(propertyExpression, constantExpression);
                break;
            case "<=":
                filterExpression = Expression.LessThanOrEqual(propertyExpression, constantExpression);
                break;
            default:
                throw new ArgumentException($"Invalid operator: {operatorStr}");
        }

        var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);

        // Use a case-insensitive string comparison for string properties
        if (property.PropertyType == typeof(string))
        {
            return data.Where(item => lambda.Compile()(item) || property.GetValue(item)?.ToString().ToLower().Contains(valueStr.ToLower()) == true);
        }

        return data.Where(lambda.Compile());
    }


}
