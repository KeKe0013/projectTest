using System.ComponentModel.DataAnnotations;

namespace projectTest.Attributes
{
    public class NotNoneAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            // 使用 nullable object 檢查 value 是否為 null
            if (value is string stringValue && stringValue == "none")
            {
                return new ValidationResult("請選擇計畫名稱");
            }


            return ValidationResult.Success;
        }
    }
}
