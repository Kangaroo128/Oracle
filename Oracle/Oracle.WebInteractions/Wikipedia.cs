using System;
using System.IO;
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
            try
            {
                string encodedTitle = WebUtility.HtmlEncode(title);
                string address = string.Format(wikiAddress, encodedTitle);
                string resultString = WebMethods.ExecuteWebRequest(address);

                string parsedArticle = ParseWikipediaArticle(resultString);
                return parsedArticle;
            }
            catch (Exception)
            {
                return null;
            }
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
            if (firstParagraph != null)
            {
                htmlText = firstParagraph.InnerText;

                htmlText = WebUtility.HtmlDecode(htmlText);

                //Remove all subscripts and superscripts from text.
                //Example match: [1]
                Regex scriptRegex = new Regex("\\[(.*?)\\]");
                htmlText = scriptRegex.Replace(htmlText, string.Empty);
            }

            return htmlText;
        }
    }
}
