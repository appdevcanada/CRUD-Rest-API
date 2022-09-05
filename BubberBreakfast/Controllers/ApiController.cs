using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BubberBreakfast.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        protected IActionResult Problem(List<Error> errors)
        {
            if (errors.Any(errors => errors.Type == ErrorType.Unexpected))
            {
                return Problem();
            }

            if (errors.All(er => er.Type == ErrorType.Validation))
            {
                var modelStateDic = new ModelStateDictionary();
                foreach (var error in errors)
                {
                    modelStateDic.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem(modelStateDic);
            }

            var firstError = errors[0];

            var statusCode = firstError.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(statusCode: statusCode, title: firstError.Description);
        }
    }
}

