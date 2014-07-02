//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
#if !WINDOWS_PHONE && !PCL
using System.Windows.Browser;
#endif

namespace SharpMap.Services
{
    internal class HttpHelper
    {
        private HttpWebRequest Request { get; set; }
        internal Dictionary<string, string> PostValues { get; private set; }

        internal event HttpResponseCompleteEventHandler ResponseComplete;
        private void OnResponseComplete(HttpResponseCompleteEventArgs e)
        {
            if (this.ResponseComplete != null)
            {
                this.ResponseComplete(e);
            }
        }

        internal HttpHelper(Uri requestUri, string method, params KeyValuePair<string, string>[] postValues)
        {
            this.Request = (HttpWebRequest)WebRequest.Create(requestUri);
            this.Request.ContentType = "application/x-www-form-urlencoded";
            this.Request.Method = method;
            this.PostValues = new Dictionary<string, string>();
            if (postValues != null && postValues.Length > 0)
            {
                foreach (var item in postValues)
                {
                    this.PostValues.Add(item.Key, item.Value);
                }
            }
        }

        internal void Execute()
        {
            this.Request.BeginGetRequestStream(new AsyncCallback(HttpHelper.BeginRequest), this);
        }

        private static void BeginRequest(IAsyncResult ar)
        {
            HttpHelper helper = ar.AsyncState as HttpHelper;
            if (helper != null)
            {
                if (helper.PostValues.Count > 0)
                {
                    using (StreamWriter writer = new StreamWriter(helper.Request.EndGetRequestStream(ar)))
                    {
                        foreach (var item in helper.PostValues)
                        {
#if PCL
                            writer.Write("{0}={1}&", item.Key, Uri.EscapeDataString(item.Value));
#else
                            writer.Write("{0}={1}&", item.Key, HttpUtility.UrlEncode(item.Value));
#endif
                        }
                    }
                }
                helper.Request.BeginGetResponse(new AsyncCallback(HttpHelper.BeginResponse), helper);
            }
        }

        private static void BeginResponse(IAsyncResult ar)
        {
            HttpHelper helper = ar.AsyncState as HttpHelper;
            if (helper != null)
            {
                HttpWebResponse response = (HttpWebResponse)helper.Request.EndGetResponse(ar);
                if (response != null)
                {
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            helper.OnResponseComplete(new HttpResponseCompleteEventArgs(reader.ReadToEnd()));
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Delegate for http asyncrouns operations.
    /// </summary>
    /// <param name="e"></param>
    internal delegate void HttpResponseCompleteEventHandler(HttpResponseCompleteEventArgs e);

    /// <summary>
    /// This class manages responses from asyncronous operations.
    /// </summary>
    internal class HttpResponseCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Response.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="HttpResponseCompleteEventArgs"/>
        /// </summary>
        public HttpResponseCompleteEventArgs(string response)
        {
            this.Response = response;
        }
    }
}
