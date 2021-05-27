using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.DTO;
using FluentValidation;

namespace BoomaEcommerce.Services
{
    public static class ServiceUtilities
    {
        public static void ValidateDto<T, TValidator>(T dto)  
            where TValidator : AbstractValidator<T>, new()
        {
            var validator = new TValidator();
            var validationResult = validator.Validate(dto);
            if (validationResult.IsValid)
            {
                return;
            }

            throw new ValidationException(validationResult.Errors);
        }
    }
}
