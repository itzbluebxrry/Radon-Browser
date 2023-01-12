using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace Yttrium_browser
{
    public class DataTransfer
    {
        //the file name
        string filename = "settings.xml";

        //saves history
        public async void SaveSearchTerm(string searchterm, string title, string url)
        {
            //result from documentload method is stored in doc
            var doc = await DocumentLoad().AsAsyncOperation(); //load xml file

            var history = doc.GetElementsByTagName("history");

            XmlElement elsearchterm = doc.CreateElement("searchterm");
            XmlElement elsitename = doc.CreateElement("sitename");
            XmlElement elurl = doc.CreateElement("url");

            var historyitem = history[0].AppendChild(doc.CreateElement("historyitem"));

            historyitem.AppendChild(elsearchterm);
            historyitem.AppendChild(elsitename);
            historyitem.AppendChild(elurl);

            elsearchterm.InnerText = searchterm;
            elsitename.InnerText = title;
            elurl.InnerText = url;

            //saves history to settings.xml
            SaveDocument(doc);

        }

        private async Task<XmlDocument> DocumentLoad()
        {
            XmlDocument result = null;

            await Task.Run(async () =>
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                XmlDocument doc = await XmlDocument.LoadFromFileAsync(file);
                result = doc;
            });
            return result;
        }

        //saves history to settings.xml
        private async void SaveDocument(XmlDocument doc)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
            await doc.SaveToFileAsync(file);
        }
    }
}
