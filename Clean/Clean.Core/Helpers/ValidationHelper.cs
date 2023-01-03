using System.ComponentModel.DataAnnotations;

namespace Clean.Core.Helpers;

public class ValidationHelper
{
    public static void ModelValidation(object obj)
    {
        var context = new ValidationContext(obj);
        var resultList = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(obj, context, resultList, true);

        if (isValid == false)
            throw new ArgumentException(resultList.FirstOrDefault()?.ErrorMessage);
    }
}
