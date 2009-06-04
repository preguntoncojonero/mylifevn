using System;
using System.Web.Mvc;
using ValidateAntiForgeryTokenAttribute=Microsoft.Web.Mvc.ValidateAntiForgeryTokenAttribute;

namespace MyLife.Web.Mvc
{
    public class MyLifeValidateAntiForgeryTokenAttribute : ValidateAntiForgeryTokenAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.HttpMethod == "POST")
            {
                base.OnAuthorization(filterContext);
            }
        }
    }
}