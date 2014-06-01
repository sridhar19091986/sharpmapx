using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portable.Http
{
    public interface IWebClient
    {
        event DownloadStringCompletedEventHandler DownloadStringCompleted;
        void DownloadStringAsync(Uri uri);
    }
}
