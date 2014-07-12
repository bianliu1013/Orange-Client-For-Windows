using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Json;
using System.IO;

namespace Orange
{
    public class JSONHelper
    {
        public static JsonArrayCollection getJSONArray(string query)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string result = streamReader.ReadToEnd();

            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(result);
            JsonArrayCollection items = (JsonArrayCollection)obj;

            return items;
        }

        public static JsonObjectCollection getJson(string query)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string result = streamReader.ReadToEnd();

            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(result);
            JsonObjectCollection col = (JsonObjectCollection)obj;

            return col;
        }
    }
}
