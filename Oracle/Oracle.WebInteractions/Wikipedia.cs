using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Net;
using HtmlAgilityPack;

namespace Oracle.WebInteractions
{
    public static class Wikipedia
    {
        private static string wikiAddress = "http://en.wikipedia.org/wiki/{0}";

        public static string ReadArticle(string title)
        {
            string encodedTitle = WebUtility.HtmlEncode(title);
            string address = string.Format(wikiAddress, encodedTitle);
            string resultString = ExecuteWebRequest(address);

            string parsedArticle = ParseWikipediaArticle(resultString);
            return WebUtility.HtmlDecode(parsedArticle);
        }

        /// <summary>
        /// Currently returning the inner text of the first paragraph in a Wikipedia
        /// article.
        /// </summary>
        /// <param name="htmlStream"></param>
        /// <returns></returns>
        private static String ParseWikipediaArticle(string htmlString)
        {
            string htmlText = null;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlString);

            //Currently, xpath is set to load first child paragraph of content text div.
            HtmlNode firstParagraph = doc.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']/p");
            htmlText = firstParagraph.InnerText;

            //Remove all subscripts and superscripts from text.
            //Example match: [1]
            Regex scriptRegex = new Regex("\\[(.*?)\\]");
            htmlText = scriptRegex.Replace(htmlText, string.Empty);

            return htmlText;
        }

        #region Web Methods
        private static String ExecuteWebRequest(string address)
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
        #endregion
    }
}
