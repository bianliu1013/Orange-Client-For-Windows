using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;

namespace SmartUpdate
{
    public class SmartUpdateXml
    {
        private Version version;
        private Uri uri;
        private string fileName;
        private string md5;
        private string description;
        private string launchArgs;
        private List<string> fileList;

        public Version Version
        {
            get { return this.version; }
        }

        public Uri Uri
        {
            get { return this.uri; }
        }

        public string FileName
        {
            get { return this.fileName; }
        }

        public string MD5
        {
            get { return this.md5; }
        }

        public string Description
        {
            get { return this.description; }
        }

        public string LaunchArgs
        {
            get { return this.launchArgs; }
        }
        public List<string> FileList
        {
            get { return this.fileList; }
        }

        public SmartUpdateXml(Version version, Uri uri, string fileName, string md5, string description, string launchArgs, List<string> fileList)
        {
            this.version = version;
            this.uri = uri;
            this.fileName = fileName;
            this.md5 = md5;
            this.description = description;
            this.launchArgs = launchArgs;
            this.fileList = fileList;
        }

        public bool IsNewerThan(Version version)
        {
            return this.version > version;
        }
        public bool IsEqualsVer(Version version)
        {
            return this.version == version;
        }

        public static bool ExistsOnServer(Uri location)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(location.AbsoluteUri);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                resp.Close();

                return resp.StatusCode == HttpStatusCode.OK;
            }
            catch { return false; }
        }

        public static SmartUpdateXml Parse(Uri location, string appID)
        {
            Version version = null;
            string url = "", fileName = "", md5 = "", description = "", launchArgs = "";

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(location.AbsoluteUri);

                XmlNode node = doc.DocumentElement.SelectSingleNode("//update[@appId='" + appID + "']");

                if (node == null)
                    return null;

                version = Version.Parse(node["version"].InnerText);
                url = node["url"].InnerText;
                fileName = node["fileName"].InnerText;
                md5 = node["md5"].InnerText;
                description = node["description"].InnerText;
                launchArgs = node["launchArgs"].InnerText;
                XmlNodeList nodeList = node["Files"].ChildNodes;
                List<string> fileList = new List<string>();

                for (int i = 0; i < nodeList.Count; i++)
                {
                    string item = nodeList.Item(i).InnerText;

                    fileList.Add(item);
                }    
                return new SmartUpdateXml(version, new Uri(url), fileName, md5, description, launchArgs, fileList);
            }
            catch { return null; }
        }
    }
}
