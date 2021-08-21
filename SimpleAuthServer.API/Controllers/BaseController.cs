using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleAuthServer.API.Models.Responses;
using SimpleAuthServer.API.Services;
using SimpleAuthServer.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly TokenService tokenService;

        public BaseController(TokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        public BadRequestObjectResult InvalidModelResponse(ModelStateDictionary modelState) 
        {
            return BadRequest( new {
                Success = false,
                ErrorMessages = modelState.Values.SelectMany(item => item.Errors.Select(err => err.ErrorMessage)).ToList()
            });
        }

        public BadRequestObjectResult BadRequestWithMessage(string message)
        {
            return BadRequest( new {
                Success = false,
                ErrorMessages = new List<string>() { message }
            });
        }

        protected Guid? GetAuthorizedUserID()
        {
            var idClaim = HttpContext.User.Claims.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier);
            return idClaim != null ? new Guid(idClaim.Value) : null;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerInfo = context.ActionDescriptor as ControllerActionDescriptor;
            if(controllerInfo != null && controllerInfo.MethodInfo.CustomAttributes.Any(item => item.AttributeType == typeof(AuthorizeAttribute)))
            {
                if (tokenService.IsDeactivated(this.Request))
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            
            base.OnActionExecuting(context);
        }
    }
}
