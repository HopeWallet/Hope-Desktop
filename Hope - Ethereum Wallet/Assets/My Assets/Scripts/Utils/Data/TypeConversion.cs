using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class TypeConversion
{

    public static T ChangeType<T>(object value) => (T)ChangeType(typeof(T), value);

    private static object ChangeType(Type t, object value)
    {
        TypeConverter tc = TypeDescriptor.GetConverter(t);
        return tc.ConvertFrom(value);
    }
}