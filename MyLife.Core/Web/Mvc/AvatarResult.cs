using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace MyLife.Web.Mvc
{
    public class AvatarResult : ActionResult
    {
        private static readonly string avatarsPath = Path.Combine(MyLifeContext.WorkingFolder, "Uploads\\Avatars");

        public AvatarResult(string user)
        {
            User = user;
        }

        public string User { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Cache.SetExpires(DateTime.Now.AddDays(1));
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetValidUntilExpires(false);

            var file = Path.Combine(avatarsPath, User + ".jpg");
            if (File.Exists(file))
            {
                response.ContentType = "image/jpg";
                goto Return;
            }

            file = Path.Combine(avatarsPath, User + ".png");
            if (File.Exists(file))
            {
                response.ContentType = "image/png";
                goto Return;
            }

            file = Path.Combine(avatarsPath, User + ".gif");
            if (File.Exists(file))
            {
                response.ContentType = "image/gif";
                goto Return;
            }

            response.ContentType = "image/png";
            file = Path.Combine(avatarsPath, "default.png");

            Return:
            response.TransmitFile(file);
        }
    }
}