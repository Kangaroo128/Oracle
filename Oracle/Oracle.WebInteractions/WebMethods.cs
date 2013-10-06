using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Oracle.WebInteractions
{
    public static class WebMethods
    {
        public static String ExecuteWebRequest(string address)
        {
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(address);
            webReq.Method = WebRequestMethods.Http.Get;
            webReq.KeepAlive = true;
            webReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webReq.Headers.Add("Accept-Encoding", "gzip,deflate");
            HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
            string result = null;

            using (Stream respStream = GetStreamForResponse(webResp, 30))
            {
                using (var responseReader = new StreamReader(respStream))
                {
                    result = responseReader.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves a response stream from the provided web response instance.
        /// Handles compressed streams of type gzip and deflate.
        /// </summary>
        /// <param name="webResponse">HttpWebResponse instance to retrieve stream from.</param>
        /// <param name="readTimeOut">(Milliseconds)How long the stream will attempt to read before a timeout.</param>
        /// <returns>New instance of Stream retrieved from web response.</returns>
        /// <see href="http://stackoverflow.com/questions/839888/httpwebrequest-native-gzip-compression"/>
        private static Stream GetStreamForResponse(HttpWebResponse webResponse, int readTimeOut)
        {
            Stream stream;
            switch (webResponse.ContentEncoding.ToUpperInvariant())
            {
                case "GZIP":
                    stream = new GZipStream(webResponse.GetResponseStream(), CompressionMode.Decompress);
                    break;
                case "DEFLATE":
                    stream = new DeflateStream(webResponse.GetResponseStream(), CompressionMode.Decompress);
                    break;

                default:
                    stream = webResponse.GetResponseStream();
                    stream.ReadTimeout = readTimeOut;
                    break;
            }
            return stream;
        }
    }
}