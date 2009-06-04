using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MyLife.Models;
using MyLife.Web.Profile;

namespace MyLife.Controllers
{
    public abstract class BaseController : Controller
    {
        protected virtual bool IsAjaxRequest
        {
            get
            {
                var value = Request.Headers["x-requested-with"];
                return !string.IsNullOrEmpty(value) &&
                       value.Equals("XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        protected virtual bool IsJsonRequest
        {
            get { return Request.ContentType.Contains("application/json"); }
        }

        protected virtual bool IsPostRequest
        {
            get { return Request.HttpMethod == "POST"; }
        }

        protected MyLifeProfile GetProfile()
        {
            return MyLifeProfile.GetProfile(User.Identity.Name);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (IsPostRequest)
            {
                if (IsJsonRequest)
                {
                    var obj = new AjaxModel {Message = filterContext.Exception.Message};
                    if (filterContext.Exception.InnerException != null)
                    {
                        obj.Message += Environment.NewLine + filterContext.Exception.InnerException.Message;
                    }

                    filterContext.Result = Json(obj);
                }
                else
                {
                    filterContext.Result = View("ErrorMessage", filterContext.Exception);
                }

                filterContext.ExceptionHandled = true;

                var message = filterContext.Exception.Message;
                if (filterContext.Exception.InnerException != null)
                {
                    message += Environment.NewLine + filterContext.Exception.InnerException.Message;
                }
                message += Environment.NewLine + string.Format("Raw url: {0}", Request.RawUrl);
                Logger.Write(message);
            }
            base.OnException(filterContext);
        }

        protected static void ThrowHttpException(int httpCode, string message, params object[] args)
        {
            throw new HttpException(httpCode, string.Format(message, args));
        }

        protected static void ThrowNullReferenceException(string message, params object[] args)
        {
            throw new NullReferenceException(string.Format(message, args));
        }

        protected static void ThrowArgumentException(string message, params object[] args)
        {
            throw new ArgumentException(string.Format(message, args));
        }
    }
}