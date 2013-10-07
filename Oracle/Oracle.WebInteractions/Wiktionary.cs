using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using HtmlAgilityPack;

namespace Oracle.WebInteractions
{
    public static class Wiktionary
    {
        private static string wikiAddress = "http://en.wiktionary.org/wiki/{0}";

        public static string ReadArticle(string title)
        {
            try
            {
                string encodedTitle = WebUtility.HtmlEncode(title);
                string address = string.Format(wikiAddress, encodedTitle);
                string resultString = WebMethods.ExecuteWebRequest(address);

                string parsedArticle = ParseWiktionaryArticle(resultString);
                return parsedArticle;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Reads the word type, all definitions, and all example uses listed in
        /// Wiktionary article.
        /// </summary>
        /// <param name="htmlStream"></param>
        /// <returns></returns>
        private static String ParseWiktionaryArticle(string htmlString)
        {
            string wordType = null;
            string definition = null;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlString);

            //Read word type from document. Found as third header element.
            HtmlNode wordTypeParagraph = doc.DocumentNode.SelectSingleNode("//div[@id='mw-content-text']/h3[3]");
            if (wordTypeParagraph != null)
            {
                wordType = ParsePartOfSpeech(wordTypeParagraph);
            }

            HtmlNodeCollection definitionListItems = doc.DocumentNode.SelectNodes("//div[@id='mw-content-text']/ol/li");
            if (definitionListItems != null)
            {
                definition = ParseDefinitionList(definitionListItems);   
            }
            
            return String.Format("Part of speech, {0}; {1}", wordType, definition);
        }

        private static string ParsePartOfSpeech(HtmlNode node)
        {
            string wordType = null;
            wordType = node.InnerText;
            wordType = WebUtility.HtmlDecode(wordType);

            //Remove all subscripts and superscripts from text.
            //Example match: [1]
            Regex scriptRegex = new Regex("\\[(.*?)\\]");
            wordType = scriptRegex.Replace(wordType, string.Empty);
            return wordType;
        }

        private static string ParseDefinitionList(HtmlNodeCollection nodeCollection)
        {
            string definition = null;

            for (var i = 0; i < nodeCollection.Count; i++)
            {
                HtmlNode listItemNode = nodeCollection[i];
                HtmlNode listItemExampleUseList = listItemNode.SelectSingleNode("dl");

                if (listItemExampleUseList != null)
                {
                    //Remove 'dl' element to prevent example use items being picked up with InnerText of current node.
                    listItemNode.RemoveChild(listItemExampleUseList);
                }

                //Read definiton and the first example usage sentence for each list item.
                definition += String.Format("Definition {0}, {1};  ",
                                            i + 1,
                                            listItemNode.InnerText);

                if (listItemExampleUseList != null)
                {
                    //Add dl child node back to parent after processing inner text.
                    listItemNode.AppendChild(listItemExampleUseList);

                    //Add example usage sections for each definition added.
                    HtmlNodeCollection exampleUseListItems = listItemExampleUseList.SelectNodes("dd");
                    if (exampleUseListItems != null)
                    {
                        for (var j = 0; j < exampleUseListItems.Count; j++)
                        {
                            definition += String.Format("Example use {0}, {1};  ",
                                                        j + 1,
                                                        exampleUseListItems[j].InnerText);
                        }
                    }
                }
            }

            return definition;
        }
    }
}
