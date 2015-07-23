using System;
using System.Diagnostics;

namespace DynamicConfig.ConfigTray.Util
{
    internal static class PrimitiveTypeHelper
    {
        public static bool IsCongruence(Type type1, Type type2)
        {
            return PrimitiveTypeCatgorizer(type1) == PrimitiveTypeCatgorizer(type2);
        }

        public static bool IsPrimitiveOrWrapper(Type type)
        {
            try
            {
                PrimitiveTypeCatgorizer(type);
                return true;
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Categorize type into Int, Double or Boolean or String
        /// </summary>
        /// <param name="dataType">the unknown type</param>
        /// <returns>DataType Enum</returns>
        /// <exception cref="System.ArgumentException">Cannot recognize this priminitve type</exception>
        public static DataType PrimitiveTypeCatgorizer(Type dataType)
        {
            if (dataType == typeof(int)
                || dataType == typeof(Int16)
                || dataType == typeof(Int32)
                || dataType == typeof(Int64)
                || dataType == typeof(uint)
                || dataType == typeof(UInt16)
                || dataType == typeof(UInt32)
                || dataType == typeof(UInt64)
                )
            {
                return DataType.INT;
            }
            if (
                   dataType == typeof(double)
                || dataType == typeof(long)
                || dataType == typeof(short)
                || dataType == typeof(float)
                )
            {
                return DataType.DOUBLE;
            }

            if (dataType == typeof(bool) || dataType == typeof(Boolean))
                return DataType.BOOLEAN;
            if (dataType == typeof(string) || dataType == typeof(String))
                return DataType.STRING;

            throw new ArgumentException("Cannot recognize this priminitve type: " + dataType.Name);
        }
    }
}