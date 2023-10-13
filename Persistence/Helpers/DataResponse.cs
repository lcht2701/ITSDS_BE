using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Helpers;

public static class DataResponse
{
    public static string GetEnumDescription(Enum? value)
    {
        if (value == null)
        {
            return "Not Assigned";
        }

        FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo == null)
        {
            // Handle the case where the field info is null (e.g., throw an exception).
            return "Not Assigned";
        }

        DescriptionAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (attributes != null && attributes.Length > 0)
        {
            return attributes[0].Description;
        }
        else
        {
            return value.ToString();
        }
    }

    public static DateTime? CleanNullableDateTime(DateTime? dateTime)
    {
        return dateTime == DateTime.MinValue ? null : dateTime;
    }
}
