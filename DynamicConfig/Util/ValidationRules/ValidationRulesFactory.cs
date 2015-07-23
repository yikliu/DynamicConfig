using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace DynamicConfig.ConfigTray.Util.ValidationRules
{
    internal static class ValidationRulesFactory
    {
        /// <summary>
        /// Use Lazy to do late initialization
        /// </summary>
        private static readonly Dictionary<Type, Lazy<PriminitiveValidationRule>> Dict = new Dictionary<Type, Lazy<PriminitiveValidationRule>>
        {
            {typeof(Int64), new Lazy<PriminitiveValidationRule>(()=> new PriminitiveValidationRule(typeof(Int64)))},
            {typeof(Double), new Lazy<PriminitiveValidationRule>(()=> new PriminitiveValidationRule(typeof(Double)))},
            {typeof(Boolean), new Lazy<PriminitiveValidationRule>(()=> new PriminitiveValidationRule(typeof(Boolean)))},
            {typeof(String), new Lazy<PriminitiveValidationRule>(()=> new PriminitiveValidationRule(typeof(String)))},
        };

        public static PriminitiveValidationRule GetValidationRule(Type type)
        {
            return Dict[type].Value;
        }
    }

    internal class PriminitiveValidationRule : ValidationRule
    {
        private Type supposedType;

        public PriminitiveValidationRule(Type type)
        {
            supposedType = type;
        }

        private string ErrorMsg
        {
            get { return "Please input a value of type " + supposedType.Name; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string inputString = (value ?? string.Empty).ToString();
            if (inputString.Length == 0)
            {
                return new ValidationResult(false, "New value cannot be empty.");
            }

            object converted;
            bool success = OmniStringConverter.ConvertStringToPrimitive(inputString, supposedType, out converted);
            if (success)
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, this.ErrorMsg);
        }
    }
}