using System;
using System.Web;

namespace MyLife.Web.DynamicData
{
    public class DynamicForm : IDisposable
    {
        private readonly HttpResponseBase _httpResponse;
        private bool _disposed;

        public DynamicForm(HttpResponseBase httpResponse)
        {
            if (httpResponse == null)
            {
                throw new ArgumentNullException("httpResponse");
            }
            _httpResponse = httpResponse;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                _httpResponse.Write("</form>");
            }
        }

        public void EndForm()
        {
            Dispose(true);
        }
    }
}