using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NoteMapper.Services.Web.ViewModels.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        public RequiredIfAttribute(string otherProperty)
            : base("")
        {
            OtherProperty = otherProperty;
        }

        public string OtherProperty { get; }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {            
            PropertyInfo? otherProperty = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherProperty == null)
            {
                return null;
            }

            object? otherValue = otherProperty.GetValue(validationContext.ObjectInstance);
            if (!Equals(otherValue, true))
            {
                return null;
            }

            RequiredAttribute required = new RequiredAttribute
            { 
                ErrorMessage = "This field is required."
            };
            return required.GetValidationResult(value, validationContext);
        }
    }
}
