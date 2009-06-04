using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using System.Web;
using MyLife.Configuration;

namespace MyLife
{
    public class MyLifeContext
    {
        public static readonly MyLifeSection Settings = (MyLifeSection) ConfigurationManager.GetSection("myLife");

        public static readonly string WorkingFolder = HttpContext.Current.Server.MapPath("~");

        public string RequestIP
        {
            get { return HttpContext.Current.Request.UserHostAddress; }
        }

        public CultureInfo CultureInfo
        {
            get
            {
                var culture = HttpContext.Current.Items["Culture"] as CultureInfo;
                if (culture == null)
                {
                    if (HttpContext.Current.Request.UserLanguages != null &&
                        HttpContext.Current.Request.UserLanguages.Length > 0)
                    {
                        var lang = HttpContext.Current.Request.UserLanguages[0];
                        lang = lang.Split(';')[0];

                        try
                        {
                            culture = CultureInfo.CreateSpecificCulture(lang);
                        }
                        catch
                        {
                            culture = CultureInfo.InvariantCulture;
                        }
                    }
                    else
                    {
                        culture = CultureInfo.InvariantCulture;
                    }
                    HttpContext.Current.Items["Culture"] = culture;
                }
                return culture;
            }
        }

        public IPrincipal User
        {
            get { return HttpContext.Current.User; }
            set { HttpContext.Current.User = value; }
        }

        public static MyLifeContext Current
        {
            get
            {
                var current = HttpContext.Current.Items["MyLifeContext"] as MyLifeContext;
                if (current == null)
                {
                    current = new MyLifeContext();
                    HttpContext.Current.Items["MyLifeContext"] = current;
                }
                return current;
            }
        }

        public static Uri AbsoluteWebRoot
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null)
                {
                    throw new WebException("The current HttpContext is null");
                }
                if (context.Items["absoluteUrl"] == null)
                {
                    context.Items["absoluteUrl"] = new Uri(context.Request.Url.GetLeftPart(UriPartial.Authority));
                }
                return (context.Items["absoluteUrl"] as Uri);
            }
        }
    }
}