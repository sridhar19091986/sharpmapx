using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Portable.Http
{
    public static class HttpExtensions
    {
        public static Task<HttpWebResponse> GetResponseStreamAsync(this HttpWebRequest context, object state)
        {
            // this will be our sentry that will know when our async operation is completed
            var tcs = new TaskCompletionSource<HttpWebResponse>();

            try
            {
                context.BeginGetResponse((iar) =>
                {
                    try
                    {
                        var result = context.EndGetResponse(iar as IAsyncResult);
                        tcs.TrySetResult(result as HttpWebResponse);
                    }
                    catch (OperationCanceledException ex)
                    {
                        // if the inner operation was canceled, this task is cancelled too
                        tcs.TrySetCanceled();
                    }
                    catch (Exception ex)
                    {
                        // general exception has been set
                        tcs.TrySetException(ex);
                    }
                }, state);
            }
            catch
            {
                tcs.TrySetResult(default(HttpWebResponse));
                // propagate exceptions to the outside
                throw;
            }

            return tcs.Task;
        }


        public static Task<Stream> GetStreamAsync(this HttpWebRequest context, object state)
        {
            // this will be our sentry that will know when our async operation is completed
            var tcs = new TaskCompletionSource<Stream>();

            try
            {
                context.BeginGetRequestStream((iar) =>
                {
                    try
                    {
                        var result = context.EndGetRequestStream(iar as IAsyncResult);
                        tcs.TrySetResult(result);
                    }
                    catch (OperationCanceledException ex)
                    {
                        // if the inner operation was canceled, this task is cancelled too
                        tcs.TrySetCanceled();
                    }
                    catch (Exception ex)
                    {
                        // general exception has been set
                        tcs.TrySetException(ex);
                    }
                }, state);
            }
            catch
            {
                tcs.TrySetResult(default(Stream));
                // propagate exceptions to the outside
                throw;
            }

            return tcs.Task;
        }

    }
}
