using System;
using System.Collections.Generic;

namespace DynamicConfig.ConfigTray.Util
{
    internal enum DataType
    {
        INT,
        DOUBLE,
        BOOLEAN,
        STRING
    };

    /// <summary>
    /// convert string value to its supposed type.
    /// </summary>
    internal interface IConverter
    {
        bool TryConvert(string obj, out object result);
    }

    internal static class OmniStringConverter
    {
        /// <summary>
        /// Easy factory creation
        /// </summary>
        private static readonly Dictionary<DataType, IConverter> ConvertersDict;

        static OmniStringConverter()
        {
            ConvertersDict = new Dictionary<DataType, IConverter>
            {
                {DataType.INT, new IntConverter()},
                {DataType.BOOLEAN, new BoolConverter()},
                {DataType.DOUBLE, new DoubleConverter()},
                {DataType.STRING, new StringConverter()},
            };
        }

        public static bool ConvertStringToPrimitive(string toConvert, Type t, out object res)
        {
            object obj;
            var con = ConvertersDict[PrimitiveTypeHelper.PrimitiveTypeCatgorizer(t)];
            bool success = con.TryConvert(toConvert, out obj);
            res = obj;
            return success;
        }
    }

    internal class IntConverter : IConverter
    {
        public bool TryConvert(string obj, out object result)
        {
            int num;
            if (int.TryParse(obj, out num))
            {
                result = num;
                return true;
            }
            result = 0;
            return false;
        }
    }

    internal class BoolConverter : IConverter
    {
        public bool TryConvert(string obj, out object result)
        {
            bool num;
            if (bool.TryParse(obj, out num))
            {
                result = num;
                return true;
            }
            result = false;
            return false;
        }
    }

    internal class DoubleConverter : IConverter
    {
        public bool TryConvert(string obj, out object result)
        {
            double num;
            if (double.TryParse(obj, out num))
            {
                result = num;
                return true;
            }
            result = 0.0;
            return false;
        }
    }

    internal class StringConverter : IConverter
    {
        public bool TryConvert(string obj, out object result)
        {
            result = obj;
            return true;
        }
    }
}