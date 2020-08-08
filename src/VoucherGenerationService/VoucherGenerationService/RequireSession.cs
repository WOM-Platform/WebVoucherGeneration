using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WomPlatform.Web.Generator {
    
    public class RequireSession : ActionFilterAttribute {

        private readonly string _sessionKey;

        public RequireSession(string sessionKey) {
            _sessionKey = sessionKey;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            if(!context.HttpContext.Session.TryGetValue(_sessionKey, out byte[] _)) {
                context.Result = new RedirectResult("/");
                return;
            }

            await next();
        }

    }

}
