using FluentValidation;
using LiftNet.Domain.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.SharedKenel.Extensions
{
    public static class ValidationExtension
    {
        public static async Task ValidateAndThrowAsync<T>(this IValidator<T> validator, T instance)
        {
            var result = await validator.ValidateAsync(instance);
            if (!result.IsValid)
            { 
                throw new BadRequestException(result.Errors.Select(x => x.), "Validate fail");
            }
        }
    }
}
