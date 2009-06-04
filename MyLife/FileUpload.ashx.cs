using System;
using System.IO;
using System.Web;

namespace MyLife
{
    public class FileUpload : IHttpHandler
    {
        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!MyLifeContext.Current.User.Identity.IsAuthenticated)
            {
                context.Response.Write("error");
                return;
            }
            
            try
            {
                var extension = Path.GetExtension(context.Request.Files[0].FileName);
                var key = context.Request.Files.AllKeys[0];

                switch (key)
                {
                    case "avatar":
                        var path = Path.Combine(MyLifeContext.WorkingFolder, "Uploads\\Avatars");
                        context.Request.Files[0].SaveAs(Path.Combine(path,
                                                                     string.Format("{0}{1}",
                                                                                   MyLifeContext.Current.User.Identity.
                                                                                       Name, extension)));
                        break;
                }
                context.Response.Write("success");
            }
            catch (Exception)
            {
                context.Response.Write("error");
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion
    }
}