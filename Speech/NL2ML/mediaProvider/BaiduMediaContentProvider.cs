using log4net;
using NL2ML.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace NL2ML.mediaProvider
{
    public class BaiduMediaContentProvider :　IMediaContentProvider
    {
        private static ILog logger = LogManager.GetLogger("common");

        private const string SearchPattern = "http://box.zhangmen.baidu.com/x?op=12&count=1&title={0}$${1}$$$$";
        public MediaData GetMusic(string name, string artist)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(string.Format(SearchPattern,
                HttpUtility.UrlEncode(name, Encoding.UTF8), HttpUtility.UrlEncode(artist, Encoding.UTF8)));
            // Get the response.
            using(WebResponse response = request.GetResponse())
            {
                // Display the status.
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                using(StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
                {
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd();

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(responseFromServer);
                    XmlNode root = doc.DocumentElement;
                    XmlNode countNode = root.SelectSingleNode("//count");
                    logger.Debug("search count: " + countNode.InnerText);
                    int count = int.Parse(countNode.InnerText);
                    if (count <= 0)
                    {
                        return null;
                    }

                    MediaData mdata = new MediaData();
                    mdata.Name = name;
                    mdata.Artist = artist;

                    XmlNode urlNode = root.SelectSingleNode("//url");
                    string url = urlNode.SelectSingleNode("//encode").InnerText;
                    string dec = urlNode.SelectSingleNode("//decode").InnerText;
                    if (dec.LastIndexOf("&") != -1)
                    {
                        dec = dec.Substring(0, dec.LastIndexOf("&"));
                    }
                    url = url.Substring(0, url.LastIndexOf("/") + 1) + dec;
                    mdata.Url = url;
                    logger.Debug("url: " + url);

                    XmlNode durlNode = root.SelectSingleNode("//durl");
                    url = urlNode.SelectSingleNode("//encode").InnerText;
                    dec = urlNode.SelectSingleNode("//decode").InnerText;
                    if (dec.LastIndexOf("&") != -1)
                    {
                        dec = dec.Substring(0, dec.LastIndexOf("&"));
                    }
                    url = url.Substring(0, url.LastIndexOf("/") + 1) + dec;
                    mdata.Durl = url;
                    logger.Debug("durl: " + url);

                    return mdata;
                }
                
            }
            
        }

        public MediaData GetMusicByGenre(string genre)
        {
            throw new NotImplementedException();
        }
    }
}
