using System.ComponentModel.DataAnnotations;

namespace ECommerce528.Validations
{
    //[AttributeUsage(AttributeTargets.Property)]
    public class CustomLengthAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public CustomLengthAttribute(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public override bool IsValid(object? value)
        {
            if(value is string result)
            {
                if(result.Length >= _minLength && result.Length < _maxLength)
                {
                    return true;
                }
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The length of field {name} must be greater than or equal to {_minLength} and less than {_maxLength}.";
        }
    }
}
