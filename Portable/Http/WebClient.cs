using System;
using System.IO;
using System.Net;
using System.Text;

namespace Portable.Http
{
    public class DownloadStringCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the data that is downloaded by a <see cref="Overload:System.Net.WebClient.DownloadStringAsync"/> method.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.String"/> that contains the downloaded data.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The asynchronous request was cancelled. </exception>
        public string Result { get; set; }

        public DownloadStringCompletedEventArgs(string data)
        {
            Result = data;
        }
    }

    public delegate void DownloadStringCompletedEventHandler(object sender, DownloadStringCompletedEventArgs e);
    
    public class WebClient: IWebClient
    {
        public class WebDownloadResult
        {
            public HttpStatusCode StatusCode { get; set; }
            public int StatusCodeNumber { get; set; }
            public bool ErrorOccured { get; set; }
            public string ResultString { get; set; }
        }

        public event DownloadStringCompletedEventHandler DownloadStringCompleted;

        public void DownloadStringAsync(Uri uri)
        {
            if (DownloadStringCompleted == null)
                throw new InvalidOperationException("DownloadStringCompleted event cannot be null");

            DownloadAsync(uri.AbsoluteUri, (a) =>
                {
                    DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(a.ResultString));
                });
        }

        public void DownloadAsync(string url, Action<WebDownloadResult> resultAction)
        {
            WebDownloadResult response = new WebDownloadResult();
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                IAsyncResult result = (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(delegate(IAsyncResult tempResult)
                {
                    HttpWebResponse webResponse = (HttpWebResponse)myHttpWebRequest.EndGetResponse(tempResult);
                    Stream responseStream = webResponse.GetResponseStream();

                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        response.ResultString = reader.ReadToEnd();
                        response.StatusCode = webResponse.StatusCode;
                        response.StatusCodeNumber = (int)webResponse.StatusCode;

                        if (resultAction != null) resultAction(response);
                    }
                }), null);


            }
            catch(Exception ex)
            {
                response.ErrorOccured = true;
                if (resultAction != null) resultAction(response);
            }
        }
    }
}
