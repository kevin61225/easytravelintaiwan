using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApplication1
{
    public class AgilityRequest
    {
        DatabaseController database = new DatabaseController();

        public struct Info
        {
            public string Tel;
            public string Address;
            public string IntroHref;
            public string CarMethod;
            public string BusMethod;
        }

        public void StartSpider()
        {
            // firstlayer
            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument doc = webClient.Load("http://www.tbroc.gov.tw/m1.aspx?sNo=0001016&key=");

            string hrefNodeXPath = "//*[@id=\"ctl00_ContentPlaceHolder1_Wuc_Cnt1_Wuc_Next_Tree1_pnl_next\"]/table/tbody/tr/td/div/a";

            HtmlNodeCollection hrefNodes = doc.DocumentNode.SelectNodes(hrefNodeXPath);

            foreach (HtmlNode node in hrefNodes)
            {
                string cityName = node.InnerHtml.Trim();
                string href = node.GetAttributeValue("href", null);
                if (CheckIsCity(cityName))
                {
                    System.Threading.Thread.Sleep(1000);
                    GetSecondLayerData(cityName, href);
                }
            }
            doc = null;
            hrefNodes = null;
            webClient = null;
        }

        // 取得各景點的類型及網址
        public void GetSecondLayerData(string cityName, string cityHref)
        {
            Console.WriteLine(cityName);
            string cityNumber = GetCityNumber(cityHref);
            string frontHref = "http://www.tbroc.gov.tw/";
            int currentTypeNumber = 0;
            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument doc = webClient.Load(frontHref + cityHref);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@class=\"sport_little\"]");

            city insertCity = new city();
            insertCity.Cityname = cityName;
            insertCity.Citynumber = cityNumber;
            database.InsertCity(insertCity);    // 記得改這

            int i = 0;

            foreach (HtmlNode node in nodes)
            {
                foreach (HtmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name == "span")
                    {
                        viewtype type = new viewtype();
                        type.Typename = childNode.InnerText;
                        database.InsertViewType(type);
                        currentTypeNumber = database.GetViewTypeNumber(childNode.InnerText);
                        Console.WriteLine(type.Typename);
                    }
                    else if (childNode.Name == "a")
                    {
                        i++;
                        Console.Write(i + ". ");
                        GetThirdLayerData(childNode.InnerText, currentTypeNumber, childNode.GetAttributeValue("href", "Error"));
                        System.Threading.Thread.Sleep(2000);
                        if ((i % 50) == 0)
                        {
                            System.Threading.Thread.Sleep(30000);
                        }
                    }
                }
            }
        }

        // 取得景點的詳細資料
        public void GetThirdLayerData(string placeName, int typeNumber, string placeHref)
        {
            string frontHref = "http://www.tbroc.gov.tw/";
            HtmlWeb webClient = new HtmlWeb();
            if (placeHref != "Error")
            {
                string placeId = GetPlaceId(placeHref);
                string cityNumber = GetCityNumber(placeHref);

                HtmlDocument doc = webClient.Load(frontHref + placeHref);

                string titleXPath = "//*[@id=\"ctl00_ContentPlaceHolder1_Wuc_Cnt1_Wuc_OneView1_lbl_mw_name\"]";
                HtmlNode titleNode = doc.DocumentNode.SelectSingleNode(titleXPath);
                if (titleNode == null)
                {
                    WriteLog("Error Page: " + placeHref);
                    return;
                }

                string placeImgSrc = GetPlaceImageUrl(doc);                 // top image
                string tourContentOuterText = GetPlaceDescription(doc);     // description
                Dictionary<string, string> pictureRow = GetPictureRow(doc); // picture row
                Info info = GetPlaceTourGuide(doc);                         // tour guide

                place insertPlace = new place();
                insertPlace.Id = placeId;
                insertPlace.Name = placeName;
                insertPlace.Citynumber = cityNumber;
                insertPlace.Typenumber = typeNumber;
                insertPlace.Description = tourContentOuterText;
                insertPlace.Telphone = info.Tel;
                insertPlace.Address = info.Address;
                insertPlace.Url = info.IntroHref;
                insertPlace.Carmethod = info.CarMethod;
                insertPlace.Busmethod = info.BusMethod;

                database.InsertPlace(insertPlace);

                if (placeImgSrc != null)
                {
                    placeimage insertImage = new placeimage();
                    insertImage.Image = RequestImage(frontHref + placeImgSrc);
                    insertImage.Imagetype = 0;
                    insertImage.Name = placeName;
                    insertImage.Id = placeId;
                    database.InsertPlaceImage(insertImage);
                }

                if (pictureRow.Count != 0)
                {
                    foreach (KeyValuePair<string, string> imgdata in pictureRow)
                    {
                        try
                        {
                            placeimage insertImage = new placeimage();
                            insertImage.Image = RequestImage(frontHref + imgdata.Key);
                            System.Threading.Thread.Sleep(500);      //hold
                            insertImage.Imagetype = 1;
                            insertImage.Name = (imgdata.Value != "") ? imgdata.Value : placeName;
                            insertImage.Id = placeId;

                            database.InsertPlaceImage(insertImage);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                            continue;
                        }
                    }
                }
                Console.WriteLine(placeName + " :Success");
            }
            else
                Console.WriteLine(placeName + " :Fail");
        }

        private Info GetPlaceTourGuide(HtmlDocument doc)
        {
            const int REGULAR_COUNT = 5;
            string placeInfoXPath = "//*[@class=\"live3_right\"]";
            string placeTitleXPath = "//*[@class=\"live3_left\"]";
            HtmlNodeCollection placeInfoCollection = doc.DocumentNode.SelectNodes(placeInfoXPath);
            Info placeInfo = new Info();
            if (placeInfoCollection.Count == REGULAR_COUNT)
            {
                // node 1 電話
                placeInfo.Tel = placeInfoCollection[0].InnerText;
                // node 2 地址
                placeInfo.Address = placeInfoCollection[1].ChildNodes[0].InnerText;
                // node 3 網站
                placeInfo.IntroHref = placeInfoCollection[2].ChildNodes["a"].GetAttributeValue("href", null);
                // node 4 自行開車
                placeInfo.CarMethod = placeInfoCollection[3].InnerHtml;
                // node 5 大眾運輸
                placeInfo.BusMethod = placeInfoCollection[4].InnerHtml;
            }
            else
            {
                HtmlNodeCollection placeTitleCollection = doc.DocumentNode.SelectNodes(placeTitleXPath);

                for (int i = 0; i < placeTitleCollection.Count; i++)
                {
                    switch (placeTitleCollection[i].InnerText)
                    {
                        case "電話":
                            placeInfo.Tel = placeInfoCollection[i].InnerText;
                            break;
                        case "地址":
                            placeInfo.Address = placeInfoCollection[i].ChildNodes[0].InnerText;
                            break;
                        case "網站":
                            placeInfo.IntroHref = placeInfoCollection[i].ChildNodes["a"].GetAttributeValue("href", null);
                            break;
                        case "自行開車":
                            placeInfo.CarMethod = placeInfoCollection[i].InnerHtml;
                            break;
                        case "大眾運輸":
                            placeInfo.BusMethod = placeInfoCollection[i].InnerHtml;
                            break;
                        default:
                            string titleXPath = "//*[@id=\"ctl00_ContentPlaceHolder1_Wuc_Cnt1_Wuc_OneView1_lbl_mw_name\"]";
                            string title = doc.DocumentNode.SelectSingleNode(titleXPath).InnerText;
                            string tour_live_left = placeTitleCollection[i].InnerText;
                            WriteLog("Unexpect description type: " + tour_live_left + " in " + title);
                            break;
                    }
                }
            }
            return placeInfo;
        }

        private void WriteLog(string log)
        {
            string path = "Log.txt";
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(log);
            }
        }

        private Dictionary<string, string> GetPictureRow(HtmlDocument doc)
        {   // description, image url
            string pictureRowXPath = "//*[@id=\"ctl00_ContentPlaceHolder1_Wuc_Cnt1_Wuc_OneView1_lbl_view_cntpic\"]/div[2]/a";
            HtmlNodeCollection pictureRowCollection = doc.DocumentNode.SelectNodes(pictureRowXPath);
            Dictionary<string, string> pictureRow = new Dictionary<string, string>();
            if (pictureRowCollection != null)
            {
                foreach (HtmlNode pictureItem in pictureRowCollection)
                {
                    string bigImgUrl = pictureItem.GetAttributeValue("href", null);
                    // 圖片描述
                    HtmlNode image = pictureItem.ChildNodes["img"];
                    string description = image.GetAttributeValue("alt", null);
                    description = description.Remove(0, description.IndexOf(' ') + 1);
                    pictureRow.Add(bigImgUrl, description);
                }
            }
            return pictureRow;
        }

        private string GetPlaceDescription(HtmlDocument doc)
        {
            string descriptonXPath = "//*[@class=\"tour_content\"]/p";
            string description = "";
            HtmlNodeCollection descriptionCollection = doc.DocumentNode.SelectNodes(descriptonXPath);
            if (descriptionCollection != null)
            {
                foreach (HtmlNode tourContentNode in descriptionCollection)
                {
                    description += tourContentNode.OuterHtml;
                }
            }
            return description;
        }

        public string GetPlaceImageUrl(HtmlDocument doc)
        {   // 景點可能會沒有圖片 => collection is null
            string placeImageXPath = "//*[@class=\"photoleft_line\"]";
            HtmlNode placeImage = doc.DocumentNode.SelectSingleNode(placeImageXPath);
            return (placeImage != null) ? placeImage.GetAttributeValue("src", null) : null;
        }

        public string GetCityNumber(string href)
        {
            string cityNumber = href.Remove(0, href.IndexOf("=") + 1);
            int frontIndex = cityNumber.IndexOf("&");
            int lastIndex = cityNumber.Length;
            cityNumber = cityNumber.Remove(frontIndex, lastIndex - frontIndex);
            return cityNumber;
        }

        public string GetPlaceId(string href)
        {
            string placeId = href.Remove(0, href.LastIndexOf("=") + 1);
            return placeId;
        }

        public bool CheckIsCity(string cityName)
        {
            if (cityName == null)
            {
                return false;
            }
            if (cityName.Contains("市") || cityName.Contains("縣"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Byte[] RequestImage(string urlImage)
        {
            try
            {
                WebRequest request = System.Net.WebRequest.Create(urlImage);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                Image image = Image.FromStream(stream);

                if (image.Height > 1000 || image.Width > 1000)
                {
                    image = ImageConverter.ResizeImage((Bitmap)image, 1000, 1000, 100);
                }

                byte[] imgByte = ImageConverter.ImageToBuffer(image, System.Drawing.Imaging.ImageFormat.Jpeg);
                return imgByte;
            }
            catch (WebException e)
            {
                WriteLog(urlImage + e.Message);
                throw new Exception("Request 逾時");
            }
        }
    }
}
