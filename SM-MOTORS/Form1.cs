using Bike18;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Формирование_ЧПУ;

namespace SM_MOTORS
{
    public partial class Form1 : Form
    {
        web.WebRequest webRequest = new web.WebRequest();
        nethouse nethouse = new nethouse();
        CHPU chpu = new CHPU();

        int addCount = 0;
        int countEditProduct = 0;
        double discountPrice = 0.02;

        public Form1()
        {
            InitializeComponent();
            tbLogin.Text = Properties.Settings.Default.login;
            tbPassword.Text = Properties.Settings.Default.password;

            if (!File.Exists("miniText.txt"))
            {
                File.Create("miniText.txt");
            }
            if (!File.Exists("fullText.txt"))
            {
                File.Create("fullText.txt");
            }
            if (!File.Exists("title.txt"))
            {
                File.Create("title.txt");
            }
            if (!File.Exists("description.txt"))
            {
                File.Create("description.txt");
            }
            if (!File.Exists("keywords.txt"))
            {
                File.Create("keywords.txt");
            }

            StreamReader altText = new StreamReader("miniText.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                richTextBox1.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("fullText.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                richTextBox2.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("title.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                textBox1.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("description.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                textBox2.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("keywords.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                textBox3.AppendText(str + "\n");
            }
            altText.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.login = tbLogin.Text;
            Properties.Settings.Default.password = tbPassword.Text;
            Properties.Settings.Default.Save();

            File.Delete("naSite.csv");
            newFileNaSite();

            string otv = null;
            string login = tbLogin.Text;
            string password = tbPassword.Text;
            CookieContainer cookieBike18 = webRequest.webCookieBike18(login, password);
            CookieContainer cookie = webRequest.webCookie("https://www.sm-motors.ru/");

            otv = webRequest.getRequest("https://www.sm-motors.ru/");
            MatchCollection urls = new Regex("(?<=<li><a href=\")/catalog.*?(?=\">)").Matches(otv);
            for (int i = 0; urls.Count > i; i++)
            {
                string urlsCategory = urls[i].ToString();
                if (urlsCategory == "/catalog/zapchasti/" || urls[i].ToString() == "/catalog/velogibridy/" || urls[i].ToString() == "/catalog/zapchasti-dlya-lodochnykh-motorov/" || urls[i].ToString() == "/catalog/akkumulyatory/" || urls[i].ToString() == "/catalog/tyuning-dlya-skuterov/" || urls[i].ToString() == "/catalog/pokryshki-kamery/" || urls[i].ToString() == "/catalog/gsm/")
                {
                    string pages = "";
                    otv = webRequest.getRequest("https://www.sm-motors.ru" + urlsCategory + "?count=60" + pages);
                    int maxVal = countPagesSM(otv);

                    for (int x = 0; maxVal >= x; x++)
                    {
                        if (x == 1)
                        {
                            pages = "";
                        }
                        else
                        {
                            pages = "&PAGEN_1=" + x;
                        }
                        if (maxVal == 0)
                        {
                            pages = "";
                        }
                        otv = webRequest.getRequest("https://www.sm-motors.ru" + urlsCategory + "?count=60" + pages);
                        MatchCollection tovars = new Regex("(?<=<a class=\"image-container\" href=\").*?(?=\" title=\")").Matches(otv);
                        for (int m = 0; tovars.Count > m; m++)
                        {
                            string urlTovar = "https://www.sm-motors.ru" + tovars[m].ToString();
                            AddTovarInCSV(cookieBike18, urlTovar, discountPrice, urlsCategory);
                        }
                        if (x == 0)
                        {
                            x++;
                        }
                    }
                    MessageBox.Show("Изменено товаров " + countEditProduct);
                    System.Threading.Thread.Sleep(20000);

                    string trueOtv = null;
                    string[] naSite1 = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
                    if (naSite1.Length > 1)
                    {
                        do
                        {
                            string otvimg = DownloadNaSite(cookieBike18);
                            string check = "{\"success\":true,\"imports\":{\"state\":1,\"errorCode\":0,\"errorLine\":0}}";
                            do
                            {
                                System.Threading.Thread.Sleep(2000);
                                otvimg = ChekedLoading(cookieBike18);
                            }
                            while (otvimg == check);

                            trueOtv = new Regex("(?<=\":{\"state\":).*?(?=,\")").Match(otvimg).ToString();
                            string error = new Regex("(?<=errorCode\":).*?(?=,\")").Match(otvimg).ToString();
                            if (error == "13")
                            {
                                ErrorDownloadInSite13(otvimg);
                            }
                            if (error == "37")
                            {
                                ErrorDownloadInSite37(otvimg);
                            }
                            if (error == "27")
                            {
                                string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otvimg).ToString();
                                string[] naSite = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
                                int u = Convert.ToInt32(errstr) - 1;
                                string[] s = naSite[u].ToString().Split(';');
                            }
                            if (error == "11")
                            {
                                string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otvimg).ToString();

                            }
                        }
                        while (trueOtv != "2");

                        //System.Threading.Thread.Sleep(50000);
                        //string[] newProductCSV = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
                        //cookie = webRequest.webCookieBike18();
                        //for (int f = 1; newProductCSV.Length > f; f++)
                        //{
                        //    string artProd = newProductCSV[f].Split(';')[1].ToString();
                        //    artProd = artProd.Replace("\"", "");
                        //    string nameProd = newProductCSV[f].Split(';')[2].ToString();
                        //    nameProd = nameProd.Replace("\"", "");

                        //    if (System.IO.File.Exists("Pic\\" + artProd + ".jpg"))
                        //    {
                        //        otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + artProd);

                        //        MatchCollection strUrlProd = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                        //        for (int t = 0; strUrlProd.Count > t; t++)
                        //        {
                        //            string nameProduct = new Regex("(?<=<a href=\\\"" + strUrlProd[t].ToString() + "\" >).*?(?=</a>)").Match(otv).ToString();

                        //            if (nameProd == nameProduct)
                        //            {
                        //                string url = strUrlProd[t].ToString();
                        //                url = url.Replace("http://bike18.ru", "http://bike18.nethouse.ru");

                        //                otv = webRequest.PostRequest(cookie, url);

                        //                MatchCollection prId = new Regex("(?<=data-id=\").*?(?=\")").Matches(otv);
                        //                int prodId = Convert.ToInt32(prId[0].ToString());

                        //                Image newImg = Image.FromFile("Pic\\" + artProd + ".jpg");
                        //                double widthImg = newImg.Width;
                        //                double heigthImg = newImg.Height;
                        //                if (widthImg > heigthImg)
                        //                {
                        //                    double dblx = widthImg * 0.9;
                        //                    if (dblx < heigthImg)
                        //                    {
                        //                        heigthImg = heigthImg * 0.9;
                        //                    }
                        //                    else
                        //                        widthImg = widthImg * 0.9;
                        //                }
                        //                else
                        //                {
                        //                    double dblx = heigthImg * 0.9;
                        //                    if (dblx < widthImg)
                        //                    {
                        //                        widthImg = widthImg * 0.9;
                        //                    }
                        //                    else
                        //                        heigthImg = heigthImg * 0.9;
                        //                }
                        //                string otvimg = DownloadImages(artProd);
                        //                string urlSaveImg = new Regex("(?<=url\":\").*?(?=\")").Match(otvimg).Value.Replace("\\/", "%2F");
                        //                string otvSave = SaveImages(urlSaveImg, prodId, widthImg, heigthImg);
                        //                List<string> listProd = webRequest.arraySaveimage(webRequest, cookie, url);
                        //                listProd[3] = "10833347";
                        //                webRequest.saveImage(cookie, listProd);
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        StreamWriter newfile1 = new StreamWriter("errr.csv", true, Encoding.GetEncoding("windows-1251"));
                        //        newfile1.WriteLine(artProd + ";");
                        //        newfile1.Close();
                        //    }
                        //}
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StreamWriter writers = new StreamWriter("miniText.txt", false, Encoding.GetEncoding(1251));
            for (int i = 0; richTextBox1.Lines.Length > i; i++)
            {
                writers.WriteLine(richTextBox1.Lines[i].ToString());
            }
            writers.Close();

            writers = new StreamWriter("fullText.txt", false, Encoding.GetEncoding(1251));
            for (int i = 0; richTextBox2.Lines.Length > i; i++)
            {
                writers.WriteLine(richTextBox2.Lines[i].ToString());
            }
            writers.Close();

            writers = new StreamWriter("title.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(textBox1.Lines[0]);
            writers.Close();

            writers = new StreamWriter("description.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(textBox2.Lines[0]);
            writers.Close();

            writers = new StreamWriter("keywords.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(textBox3.Lines[0]);
            writers.Close();

            MessageBox.Show("Сохранено");
        } // сохранить шаблон

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.login = tbLogin.Text;
            Properties.Settings.Default.password = tbPassword.Text;
            Properties.Settings.Default.Save();
            string login = tbLogin.Text;
            string password = tbPassword.Text;
            CookieContainer cookieBike18 = webRequest.webCookieBike18(login, password);

            string otv = null;
            otv = webRequest.getRequest("http://bike18.ru/products/category/1689456");
            MatchCollection razdel = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);
            for (int i = 0; razdel.Count > i; i++)
            {
                otv = webRequest.getRequest(razdel[i].ToString() + "/page/all");
                MatchCollection product = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                for (int n = 0; product.Count > n; n++)
                {
                    otv = webRequest.getRequest(product[n].ToString());
                    string artProd = new Regex("(?<=Артикул:)[\\w\\W]*?(?=</title><)").Match(otv).ToString().Trim();
                    if (System.IO.File.Exists("Pic\\" + artProd + ".jpg"))
                    {

                        otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + artProd);

                        string url = product[n].ToString();
                        url = url.Replace("http://bike18.ru", "http://bike18.nethouse.ru");
                        otv = webRequest.PostRequest(cookieBike18, url);

                        MatchCollection prId = new Regex("(?<=data-id=\").*?(?=\")").Matches(otv);

                        Image newImg = Image.FromFile("Pic\\" + artProd + ".jpg");
                        double widthImg = newImg.Width;
                        double heigthImg = newImg.Height;
                        if (widthImg > heigthImg)
                        {
                            double dblx = widthImg * 0.9;
                            if (dblx < heigthImg)
                            {
                                heigthImg = heigthImg * 0.9;
                            }
                            else
                                widthImg = widthImg * 0.9;
                        }
                        else
                        {
                            double dblx = heigthImg * 0.9;
                            if (dblx < widthImg)
                            {
                                widthImg = widthImg * 0.9;
                            }
                            else
                                heigthImg = heigthImg * 0.9;
                        }

                        string epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(",", "");
                        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/putimg?fileapi" + epoch);
                        req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
                        req.Method = "POST";
                        req.ContentType = "multipart/form-data; boundary=---------------------------12709277337355";
                        req.CookieContainer = cookieBike18;
                        req.Headers.Add("X-Requested-With", "XMLHttpRequest");
                        byte[] pic = File.ReadAllBytes("Pic\\" + artProd + ".jpg");
                        byte[] end = Encoding.ASCII.GetBytes("\r\n-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"_files\"\r\n\r\n" + artProd + ".jpg\r\n-----------------------------12709277337355--\r\n");
                        byte[] ms1 = Encoding.ASCII.GetBytes("-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"files\"; filename=\"" + artProd + ".jpg\"\r\nContent-Type: image/jpeg\r\n\r\n");
                        req.ContentLength = ms1.Length + pic.Length + end.Length;
                        Stream stre1 = req.GetRequestStream();
                        stre1.Write(ms1, 0, ms1.Length);
                        stre1.Write(pic, 0, pic.Length);
                        stre1.Write(end, 0, end.Length);
                        stre1.Close();
                        HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
                        StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
                        string otvimg = ressrImg.ReadToEnd();

                        string urlSaveImg = new Regex("(?<=url\":\").*?(?=\")").Match(otvimg).Value.Replace("\\/", "%2F");

                        req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/save-image");
                        req.Accept = "application/json, text/plain, */*";
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
                        req.Method = "POST";
                        req.ContentType = "application/x-www-form-urlencoded";
                        req.CookieContainer = cookieBike18;
                        byte[] saveImg = Encoding.ASCII.GetBytes("url=" + urlSaveImg + "&id=0&type=4&objectId=" + prId[0] + "&imgCrop[x]=0&imgCrop[y]=0&imgCrop[width]=" + widthImg + "&imgCrop[height]=" + heigthImg + "&imageId=0&iObjectId=" + prId[0] + "&iImageType=4&replacePhoto=0");
                        req.ContentLength = saveImg.Length;
                        Stream srSave = req.GetRequestStream();
                        srSave.Write(saveImg, 0, saveImg.Length);
                        srSave.Close();
                        HttpWebResponse resSave = (HttpWebResponse)req.GetResponse();
                        StreamReader ressrSave = new StreamReader(resSave.GetResponseStream());
                        String otvSave = ressrSave.ReadToEnd();

                        List<string> listProd = webRequest.arraySaveimage(webRequest, cookieBike18, url);
                        listProd[3] = "10833347";

                        webRequest.saveImage(cookieBike18, listProd);
                    }
                }
            }
        } //загрузка картинок

        public void newFileNaSite()
        {
            List<string> newProduct = new List<string>();
            newProduct.Add("id");
            newProduct.Add("Артикул *");
            newProduct.Add("Название товара *");
            newProduct.Add("Стоимость товара *");
            newProduct.Add("Стоимость со скидкой");
            newProduct.Add("Раздел товара *");
            newProduct.Add("Товар в наличии *");
            newProduct.Add("Поставка под заказ *");
            newProduct.Add("Срок поставки (дни) *");
            newProduct.Add("Краткий текст");
            newProduct.Add("Текст полностью");
            newProduct.Add("Заголовок страницы (title)");
            newProduct.Add("Описание страницы (description)");
            newProduct.Add("Ключевые слова страницы (keywords)");
            newProduct.Add("ЧПУ страницы (slug)");
            newProduct.Add("С этим товаром покупают");
            newProduct.Add("Рекламные метки");
            newProduct.Add("Показывать на сайте *");
            newProduct.Add("Удалить *");
            webRequest.fileWriterCSV(newProduct, "naSite");
        }

        public int countPagesSM(string otv)
        {
            int max = 0;
            MatchCollection pagins = new Regex("(?<=count=60&amp;PAGEN_1=).*?(?=\">)").Matches(otv);
            List<int> array = new List<int>();
            for (int z = 0; pagins.Count > z; z++)
            {
                if (pagins[z].ToString().Length < 5)
                {
                    array.Add(Convert.ToInt32(pagins[z].ToString()));
                }
            }
            if (array.Count > 0)
            {
                max = array.Max();
            }
            else max = 0;
            return max;
        }

        private int ReturnCountAdd()
        {
            if (addCount == 99)
                addCount = 0;
            addCount++;
            return addCount;
        }

        public string descriptionsTovar(string otv)
        {
            string descriptionTovar = new Regex("(?<=<div class=\"tab-content text-container\">)[\\w\\W]*?(?=</div>)").Match(otv).ToString().Trim().Replace(Environment.NewLine, "<br />");
            File.Delete("1.txt");
            File.WriteAllText("1.txt", descriptionTovar);
            string[] descriptionTovar1 = File.ReadAllLines("1.txt");
            descriptionTovar = "";
            for (int d = 0; descriptionTovar1.Length > d; d++)
            {
                descriptionTovar += descriptionTovar1[d].ToString();
            }
            return descriptionTovar;
        }

        public string podrazdel(string otv)
        {
            string podRazdel = new Regex("(?<=Аккумуляторы</a></span><span><a href=\").*?(?=\">)").Match(otv).ToString();
            if (podRazdel != "")
            {
                podRazdel = podRazdel.Substring(podRazdel.IndexOf("\"") + 2).Replace("title=\"", "");
            }
            return podRazdel;
        }

        public void EditSizeImages(string urlImage, string article)
        {
            try
            {
                new WebClient().DownloadFile(urlImage, "Pic\\" + article + ".jpg");
                Bitmap bmp = (Bitmap)Bitmap.FromFile("Pic\\" + article + ".jpg");
                Rectangle rec = new Rectangle(0, 0, bmp.Width, bmp.Height - 27);
                Size s = bmp.Size;
                int xx = s.Height;
                int yy = s.Width;
                Bitmap cropBmp = bmp.Clone(rec, bmp.PixelFormat);
                bmp.Dispose();
                cropBmp.Save("Pic\\" + article + ".jpg");
            }
            catch
            {

            }
        }

        public int Price(string otv, double discountPrice)
        {
            int price = 0;
            string strPrice = new Regex("(?<=<span class=\"price\">).*(?=</span>)").Match(otv).ToString();
            if (strPrice != "")
            {
                double priceD = Convert.ToDouble(strPrice);
                priceD = priceD - (priceD * discountPrice);
                priceD = Math.Round(priceD);
                price = Convert.ToInt32(priceD);
                price = (price / 10) * 10;
            }
            return price;
        }

        public string MiniText()
        {
            string minitext = null;
            for (int z = 0; richTextBox1.Lines.Length > z; z++)
            {
                if (richTextBox1.Lines[z].ToString() == "")
                {
                    minitext += "<p><br /></p>";
                }
                else
                {
                    minitext += "<p>" + richTextBox1.Lines[z].ToString() + "</p>";
                }
            }
            return minitext;
        }

        public string FullText()
        {
            string fullText = null;
            for (int y = 0; richTextBox2.Lines.Length > y; y++)
            {
                if (richTextBox2.Lines[y].ToString() == "")
                {
                    fullText += "<p><br /></p>";
                }
                else
                {
                    fullText += "<p>" + richTextBox2.Lines[y].ToString() + "</p>";
                }
            }
            return fullText;
        }

        public string Discount()
        {
            string discount = "<p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> Сделай ТРОЙНОЙ удар по нашим ценам! </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 1. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Скидки за отзывы о товарах!</a> </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 2. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Друзьям скидки и подарки!</a> </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 3. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Нашли дешевле!? 110% разницы Ваши!</a></span></p>";
            return discount;
        }

        public string DownloadNaSite(CookieContainer cookie)
        {
            string epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(",", "");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/export-import/import-from-csv?fileapi" + epoch);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=---------------------------12709277337355";
            req.CookieContainer = cookie;
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            byte[] csv = File.ReadAllBytes("naSite.csv");
            byte[] end = Encoding.ASCII.GetBytes("\r\n-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"_catalog_file\"\r\n\r\nnaSite.csv\r\n-----------------------------12709277337355--\r\n");
            byte[] ms1 = Encoding.ASCII.GetBytes("-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"catalog_file\"; filename=\"naSite.csv\"\r\nContent-Type: text/csv\r\n\r\n");
            req.ContentLength = ms1.Length + csv.Length + end.Length;
            Stream stre1 = req.GetRequestStream();
            stre1.Write(ms1, 0, ms1.Length);
            stre1.Write(csv, 0, csv.Length);
            stre1.Write(end, 0, end.Length);
            stre1.Close();
            HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
            StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
            string otvimg = ressrImg.ReadToEnd();
            return otvimg;
        }

        public string ChekedLoading(CookieContainer cookie)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/export-import/check-import");
            req.Accept = "application/json, text/plain, */*";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentLength = 0;
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            Stream stre1 = req.GetRequestStream();
            stre1.Close();
            HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
            StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
            string otvimg = ressrImg.ReadToEnd();
            return otvimg;
        }

        public string DownloadImages(CookieContainer cookie, string artProd)
        {
            string epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(",", "");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/putimg?fileapi" + epoch);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=---------------------------12709277337355";
            req.CookieContainer = cookie;
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            byte[] pic = File.ReadAllBytes("Pic\\" + artProd + ".jpg");
            byte[] end = Encoding.ASCII.GetBytes("\r\n-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"_files\"\r\n\r\n" + artProd + ".jpg\r\n-----------------------------12709277337355--\r\n");
            byte[] ms1 = Encoding.ASCII.GetBytes("-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"files\"; filename=\"" + artProd + ".jpg\"\r\nContent-Type: image/jpeg\r\n\r\n");
            req.ContentLength = ms1.Length + pic.Length + end.Length;
            Stream stre1 = req.GetRequestStream();
            stre1.Write(ms1, 0, ms1.Length);
            stre1.Write(pic, 0, pic.Length);
            stre1.Write(end, 0, end.Length);
            stre1.Close();
            HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
            StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
            string otvimg = ressrImg.ReadToEnd();
            return otvimg;
        }

        public string SaveImages(CookieContainer cookie, string urlSaveImg, int prodId, double widthImg, double heigthImg)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/save-image");
            req.Accept = "application/json, text/plain, */*";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] saveImg = Encoding.ASCII.GetBytes("url=" + urlSaveImg + "&id=0&type=4&objectId=" + prodId + "&imgCrop[x]=0&imgCrop[y]=0&imgCrop[width]=" + widthImg + "&imgCrop[height]=" + heigthImg + "&imageId=0&iObjectId=" + prodId + "&iImageType=4&replacePhoto=0");
            req.ContentLength = saveImg.Length;
            Stream srSave = req.GetRequestStream();
            srSave.Write(saveImg, 0, saveImg.Length);
            srSave.Close();
            HttpWebResponse resSave = (HttpWebResponse)req.GetResponse();
            StreamReader ressrSave = new StreamReader(resSave.GetResponseStream());
            string otvSave = ressrSave.ReadToEnd();
            return otvSave;
        }

        public void AddTovarInCSV(CookieContainer cookieBike18, string urlTovar, double discountPrice, string urlsCategory)
        {
            List<string> newProduct = new List<string>();
            string urlTovarBike = null;
            CookieContainer cookie = new CookieContainer();
            string otv = null;
            string discount = Discount();
            string dblProduct = "НАЗВАНИЕ также подходит для аналогичных моделей.";

            List<string> tovarSMMotors = getTovarSMMotors(urlTovar, urlsCategory);

            string name = tovarSMMotors[0].ToString();
            string article = tovarSMMotors[1].ToString();
            string availability = tovarSMMotors[2].ToString();
            string newMetka = tovarSMMotors[3].ToString();
            string saleMEtka = tovarSMMotors[4].ToString();
            string descriptionTovar = tovarSMMotors[5].ToString();
            string characteristics = tovarSMMotors[6].ToString();
            string urlImage = tovarSMMotors[7].ToString();
            string podRazdelSeo = tovarSMMotors[8].ToString();
            string priceNoProd = tovarSMMotors[9].ToString();
            string chpuNoProd = tovarSMMotors[10].ToString();
            string metka = tovarSMMotors[11].ToString();
            string razdel = tovarSMMotors[12].ToString();
            string razdelSeo = tovarSMMotors[13].ToString();



            //MatchCollection section = new Regex("(?<=\" title=\").*?(?=\">)").Matches(otv);
            if (!File.Exists("Pic\\" + article + ".jpg"))
            {
                EditSizeImages(urlImage, article);
            }

            if (availability == "1")
            {
                bool b = false;

                //поиск по артикулу
                otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + article);
                MatchCollection strUrlProd1 = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                for (int t = 0; strUrlProd1.Count > t; t++)
                {
                    string nameProduct1 = new Regex("(?<=<a href=\\\"" + strUrlProd1[t].ToString() + "\" >).*?(?=</a>)").Match(otv).ToString().Replace("&amp;quot;", "").Replace("&#039;", "'").Replace("&amp;gt;", ">").Trim();
                    if (name == nameProduct1)
                    {
                        b = true;
                        urlTovarBike = strUrlProd1[t].ToString();
                        break;
                    }
                }

                //Поиск по названию товара
                if (!b)
                {
                    otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + name);
                    strUrlProd1 = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                    for (int t = 0; strUrlProd1.Count > t; t++)
                    {
                        string nameProduct1 = new Regex("(?<=<a href=\\\"" + strUrlProd1[t].ToString() + "\" >).*?(?=</a>)").Match(otv).ToString().Replace("&amp;quot;", "").Replace("&#039;", "'").Replace("&amp;gt;", ">").Trim();
                        if (name == nameProduct1)
                        {
                            b = true;
                            urlTovarBike = strUrlProd1[t].ToString();
                            break;
                        }
                    }
                }

                if (!b)
                {
                    string minitext = null;
                    string titleText = null;
                    string descriptionText = null;
                    string keywordsText = null;
                    string fullText = null;
                    string dblProdSEO = null;
                    string artNoProd = article;
                    string razdelmini = null;
                    string podRazdel = null;
                    string boldOpen = "<span style=\"\"font-weight: bold; font-weight: bold;\"\">";
                    string boldClose = "</span>";

                    minitext = MiniText();
                    fullText = FullText();
                    titleText = textBox1.Lines[0].ToString();
                    descriptionText = textBox2.Lines[0].ToString() + " " + dblProdSEO;
                    keywordsText = textBox3.Lines[0].ToString();

                    podRazdel = boldOpen + podRazdelSeo + boldClose;
                    razdelmini = boldOpen + razdelSeo + boldClose;
                    string nameText = boldOpen + name + boldClose;

                    minitext = minitext.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", podRazdel).Replace("РАЗДЕЛ", razdelmini).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", nameText).Replace("АРТИКУЛ", artNoProd).Replace("ОПИСАНИЕ", descriptionTovar).Replace("ХАРАКТЕРИСТИКА", characteristics).Replace("<p><br /></p><p></p><p><br /></p>", "<p><br /></p>").Replace("<p><br /></p><p><br /></p><p><br /></p>", "");
                    minitext = specChar(minitext);
                    if (minitext.Contains('&'))
                    {
                        minitext = fullText.Replace("&nbsp;", " ");
                    }

                    fullText = fullText.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", podRazdel).Replace("РАЗДЕЛ", razdelmini).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", nameText).Replace("АРТИКУЛ", artNoProd).Replace("ОПИСАНИЕ", descriptionTovar).Replace("ХАРАКТЕРИСТИКА", characteristics).Replace("<p><br /></p>", "");
                    fullText = specChar(fullText);
                    if (fullText.Contains('&'))
                    {
                        
                    }

                    titleText = titleText.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", podRazdel).Replace("РАЗДЕЛ", razdelSeo).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", name).Replace("АРТИКУЛ", artNoProd).Replace("ОПИСАНИЕ", descriptionTovar).Replace("ХАРАКТЕРИСТИКА", characteristics);

                    descriptionText = descriptionText.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", podRazdelSeo).Replace("РАЗДЕЛ", razdelSeo).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", name).Replace("АРТИКУЛ", artNoProd).Replace("ОПИСАНИЕ", descriptionTovar).Replace("ХАРАКТЕРИСТИКА", characteristics);

                    keywordsText = keywordsText.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", podRazdel).Replace("РАЗДЕЛ", razdelSeo).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", name).Replace("АРТИКУЛ", artNoProd).Replace("ОПИСАНИЕ", descriptionTovar).Replace("ХАРАКТЕРИСТИКА", characteristics);

                    if (name.Length > 255)
                    {
                        name = name.Remove(255);
                        name = name.Remove(name.LastIndexOf(" "));
                    }
                    if (titleText.Length > 255)
                    {
                        titleText = titleText.Remove(255);
                        titleText = titleText.Remove(titleText.LastIndexOf(" "));
                    }
                    if (descriptionText.Length > 200)
                    {
                        descriptionText = descriptionText.Remove(200);
                        descriptionText = descriptionText.Remove(descriptionText.LastIndexOf(" "));
                    }
                    if (keywordsText.Length > 100)
                    {
                        keywordsText = keywordsText.Remove(100);
                        keywordsText = keywordsText.Remove(keywordsText.LastIndexOf(" "));
                    }
                    if (chpuNoProd.Length > 64)
                    {
                        chpuNoProd = chpuNoProd.Remove(64);
                        chpuNoProd = chpuNoProd.Remove(chpuNoProd.LastIndexOf(" "));
                    }

                    newProduct = new List<string>();
                    newProduct.Add("");                                   //id
                    newProduct.Add("\"" + artNoProd + "\"");                                    //артикул
                    newProduct.Add("\"" + name + "\"");                               //название
                    newProduct.Add("\"" + priceNoProd + "\"");                                 //стоимость
                    newProduct.Add("\"" + "" + "\"");                              //со скидкой
                    newProduct.Add("\"" + razdel + "\"");                                     //раздел товара
                    newProduct.Add("\"" + "100" + "\"");                                  //в наличии
                    newProduct.Add("\"" + "0" + "\"");                                    //поставка
                    newProduct.Add("\"" + "1" + "\"");                                 //срок поставки
                    newProduct.Add("\"" + minitext + "\"");                                       //краткий текст
                    newProduct.Add("\"" + fullText + "\"");                               //полностью текст
                    newProduct.Add("\"" + titleText + "\"");                                 //заголовок страницы
                    newProduct.Add("\"" + descriptionText + "\"");                              //описание
                    newProduct.Add("\"" + keywordsText + "\"");                               //ключевые слова
                    newProduct.Add("\"" + chpuNoProd + "\"");                                //ЧПУ
                    newProduct.Add("");                                                      //с этим товаром покупают
                    newProduct.Add("\"" + metka + "\"");                                            //рекламные метки
                    newProduct.Add("\"" + "1" + "\"");                                      //показывать
                    newProduct.Add("\"" + "0" + "\"");                                     //удалить

                    if (priceNoProd != "0")
                        webRequest.fileWriterCSV(newProduct, "naSite");

                }
                else
                {
                    string fullText = null;
                    string razdelmini = null;
                    string podRazdel = null;
                    fullText = FullText();
                    string boldOpen = "<span style=\"font-weight: bold; font-weight: bold;\">";
                    string boldClose = "</span>";

                    podRazdel = boldOpen + podRazdelSeo + boldClose;
                    razdelmini = boldOpen + razdelSeo + boldClose;
                    string nameText = boldOpen + name + boldClose;
                    string nameNoProd = name;

                    fullText = fullText.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", podRazdel).Replace("РАЗДЕЛ", razdelmini).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", nameText).Replace("АРТИКУЛ", article).Replace("ОПИСАНИЕ", descriptionTovar).Replace("ХАРАКТЕРИСТИКА", characteristics).Replace("<p><br /></p>", "");
                    fullText = specChar(fullText);

                    if (fullText.Contains('&'))
                    {
                    }

                    //обновить цену
                    List<string> listProduct = nethouse.getProductList(cookieBike18, urlTovarBike);
                    string priceBike = listProduct[9];
                    otv = webRequest.PostRequest(cookie, urlTovar);
                    int price = Price(otv, discountPrice);
                    if (Convert.ToInt32(priceBike) != price)
                    {
                        listProduct[9] = price.ToString();
                        listProduct[8] = fullText;
                        nethouse.saveTovar(cookieBike18, listProduct);
                        countEditProduct++;
                    }
                    else
                    {
                        //Обновление поля "Полное описание" товара
                        listProduct[8] = fullText;
                        nethouse.saveTovar(cookieBike18, listProduct);
                    }
                }
            }
            else
            {
                //тут надо реализовать если на сайте товара нет в наличии и проверить есть ли на байк18

                bool b = false;

                //поиск по артикулу
                otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + article);
                MatchCollection strUrlProd1 = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                for (int t = 0; strUrlProd1.Count > t; t++)
                {
                    string nameProduct1 = new Regex("(?<=<a href=\\\"" + strUrlProd1[t].ToString() + "\" >).*?(?=</a>)").Match(otv).ToString().Trim();
                    nameProduct1 = specChar(nameProduct1);
                    if (name == nameProduct1)
                    {
                        b = true;
                        urlTovarBike = strUrlProd1[t].ToString();
                        break;
                    }
                }

                //Поиск по названию товара
                if (!b)
                {
                    otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + name);
                    strUrlProd1 = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                    for (int t = 0; strUrlProd1.Count > t; t++)
                    {
                        string nameProduct1 = new Regex("(?<=<a href=\\\"" + strUrlProd1[t].ToString() + "\" >).*?(?=</a>)").Match(otv).ToString().Trim();
                        nameProduct1 = specChar(nameProduct1);
                        if (name == nameProduct1)
                        {
                            b = true;
                            urlTovarBike = strUrlProd1[t].ToString();
                            break;
                        }
                    }
                }

                if (b)
                {
                    List<string> listProduct = nethouse.getProductList(cookieBike18, urlTovarBike);
                    otv = webRequest.PostRequest(cookie, urlTovar);
                    int price = Price(otv, discountPrice);
                    listProduct[9] = price.ToString();
                    listProduct[43] = "0";

                    nethouse.saveTovar(cookieBike18, listProduct);
                    countEditProduct++;
                }
            }
        }

        private string specChar(string text)
        {
            text = text.Replace("&quot;", "\"").Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&laquo;", "«").Replace("&raquo;", "»").Replace("&ndash;", "-").Replace("&mdash;", "-").Replace("&lsquo;", "‘").Replace("&rsquo;", "’").Replace("&sbquo;", "‚").Replace("&ldquo;", "\"").Replace("&rdquo;", "”").Replace("&bdquo;", "„").Replace("&#43;", "+").Replace("&#40;", "(").Replace("&nbsp;", " ").Replace("&#41;", ")").Replace("&amp;quot;", "").Replace("&#039;", "'").Replace("&amp;gt;", ">").Replace("&#43;", "+").Replace("&#40;", "(").Replace("&nbsp;", " ").Replace("&#41;", ")").Replace("&#39;", "'");

            return text;
        }

        private List<string> getTovarSMMotors(string urlTovar, string urlsCategory)
        {
            CookieContainer cookie = new CookieContainer();
            List<string> getTovar = new List<string>();
            string otv = webRequest.getRequest(urlTovar);

            string razdelmini = null;
            string razdelSeo = null;
            string podRazdel = null;
            string podrazdel1 = null;
            string razdel = "Запчасти и расходники => Каталог запчастей SM-MOTORS => ";
            string metka = "";

            string availability = new Regex("(?<=<input type=\"text\" maxlength=\"3\" value=\").*(?=\" data-max)").Match(otv).ToString();
            string article = new Regex("(?<=<span>Артикул:</span>).*?(?=</div>)").Match(otv).ToString();
            string name = new Regex("(?<=<h1>).*(?=</h1>)").Match(otv).ToString();
            name = name.Replace("&quot;", "").Replace("&gt;", ">").Replace("&#039;", "'").Trim();
            string newMetka = new Regex("(?<=<div class=\"icons-container\">)[\\w\\W]*?(?=<div class=\"ico\"></div></div>)").Match(otv).ToString().Trim();
            string saleMEtka = new Regex("(?<=<div class=\"old-price\">).*?(?=</div>)").Match(otv).ToString();
            string descriptionTovar = descriptionsTovar(otv);
            string characteristics = new Regex("(?<=<div class=\"tab-content columns-content\">)[\\w\\W]*?(?=</ul>)").Match(otv).ToString().Replace("<ul>", "").Replace("<li>", "").Replace("</li>", "").Trim();
            string urlImage = new Regex("(?<=<div class=\"gallery\">)[\\w\\W]*?(?=data-large=)").Match(otv).ToString().Replace("<a href=\"", "").Replace("\"", "").Trim();
            urlImage = "https://www.sm-motors.ru" + urlImage;
            //string podRazdel = podrazdel(otv);
            int price = Price(otv, discountPrice);
            string priceNoProd = price.ToString();
            string chpuNoProd = chpu.vozvr(name);
            string objProduct = urlsCategory;

            if (newMetka != "")
            {
                metka = "новинка";
            }
            if (saleMEtka != "")
            {
                metka = "акция";
            }

            if (urlsCategory == "/catalog/zapchasti/")
            {
                podrazdel1 = new Regex("(?<=\" title=\"Запчасти\">Запчасти</a></span><span><a href=\").*?(?=\" title=\")").Match(otv).ToString();
                objProduct = new Regex("(?<=/catalog/zapchasti/).*(?=/)").Match(podrazdel1).ToString();
            }

            switch (objProduct)
            {
                case ("zapchasti-dlya-pitbaykov-i-kitayskikh-mototsiklov"):
                    razdel = razdel + "Запчасти => Запчасти для питбайков и китайских мотоциклов";
                    razdelmini = "Запчасти для питбайков и китайских мотоциклов";
                    razdelSeo = "Запчасти для питбайков и китайских мотоциклов";
                    break;
                case ("zapchasti-originalnye"):
                    razdel = razdel + "Запчасти => Запчасти оригинальные";
                    razdelmini = "Запчасти оригинальные";
                    razdelSeo = "Запчасти оригинальные";
                    break;
                case ("dvigateli"):
                    razdel = razdel + "Запчасти => Двигатели";
                    razdelmini = "Двигатели";
                    razdelSeo = "Двигатели";
                    break;
                case ("zapchasti-dlya-mototsiklov"):
                    razdel = razdel + "Запчасти => Запчасти для мотоциклов";
                    razdelmini = "Запчасти для мотоциклов";
                    razdelSeo = "Запчасти для мотоциклов";
                    break;
                case ("zapchasti-dlya-kvadrotsiklov"):
                    razdel = razdel + "Запчасти => Запчасти для квадроциклов";
                    razdelmini = "Запчасти для квадроциклов";
                    razdelSeo = "Запчасти для квадроциклов";
                    break;
                case ("zapchasti-dlya-skuterov"):
                    razdel = razdel + "Запчасти => Запчасти для скутеров";
                    razdelmini = "Запчасти для скутеров";
                    razdelSeo = "Запчасти для скутеров";
                    break;
                case ("zapchasti-snegokhody"):
                    razdel = razdel + "Запчасти => Запчасти снегоходы";
                    razdelmini = "Запчасти снегоходы";
                    razdelSeo = "Запчасти снегоходы";
                    break;
                case ("/catalog/akkumulyatory/"):
                    razdel = razdel + "Аккумуляторы";
                    razdelmini = "Аккумуляторы";
                    razdelSeo = "Аккумуляторы";
                    break;
                case ("/catalog/tyuning-dlya-skuterov/"):
                    razdel = razdel + "Тюнинг для скутеров";
                    razdelmini = "Тюнинг для скутеров";
                    razdelSeo = "Тюнинг для скутеров";
                    break;
                case ("/catalog/velogibridy/"):
                    razdel = razdel + "Велогибриды";
                    razdelmini = "Велогибриды";
                    razdelSeo = "Велогибриды";
                    break;
                case ("/catalog/zapchasti-dlya-lodochnykh-motorov/"):
                    razdel = razdel + "Запчасти для лодочных моторов";
                    razdelmini = "Запчасти для лодочных моторов";
                    razdelSeo = "Запчасти для лодочных моторов";
                    break;
                default:
                    break;
            }

            podRazdel = new Regex("(?<=\">" + razdelmini + "</a></span><span><a href=\").*?(?=>)").Match(otv).ToString();
            podRazdel = new Regex("(?<=title=\").*?(?=\")").Match(podRazdel).ToString();
            //string str = new Regex("(?<=<div class=\"breadcrumbs-container\"><span><a href=\"/\" title=\").*?(?=</span></div></div></section>)").Match(otv).ToString();
            //MatchCollection mc = new Regex("(?<=\" title=\").*?(?=\">)").Matches(str);
            //int mcCOunt = mc.Count - 1;
            string podrazdelSeo = podRazdel;
            //podRazdel = podRazdel + " - " + mc[mcCOunt].ToString();

            getTovar.Add(name);
            getTovar.Add(article);
            getTovar.Add(availability);
            getTovar.Add(newMetka);
            getTovar.Add(saleMEtka);
            getTovar.Add(descriptionTovar);
            getTovar.Add(characteristics);
            getTovar.Add(urlImage);
            getTovar.Add(podRazdel);
            getTovar.Add(priceNoProd);
            getTovar.Add(chpuNoProd);
            getTovar.Add(metka);
            getTovar.Add(razdel);
            getTovar.Add(razdelSeo);
            getTovar.Add(podrazdelSeo);

            return getTovar;
        }

        private void ErrorDownloadInSite37(string otvimg)
        {
            string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otvimg).ToString();
            string[] naSite = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
            int u = Convert.ToInt32(errstr) - 1;
            string[] strslug3 = naSite[u].ToString().Split(';');
            string strslug = strslug3[strslug3.Length - 5];
            int slug = strslug.Length;
            int countAdd = ReturnCountAdd();
            int countDel = countAdd.ToString().Length;
            if (strslug.Contains("\""))
            {
                countDel = countDel + 2;
            }
            string strslug2 = strslug.Remove(slug - countDel);
            strslug2 += countAdd;
            strslug2 = strslug2.Replace("”", "").Replace("~", "").Replace("#", "").Replace("?", "");
            if (strslug2.Contains("\""))
            {
                strslug2 = strslug2 + "\"";
                countDel = countDel - 2;
            }
            naSite[u] = naSite[u].Replace(strslug, strslug2);
            File.WriteAllLines("naSite.csv", naSite, Encoding.GetEncoding(1251));
        }

        private void ErrorDownloadInSite13(string otvimg)
        {
            string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otvimg).ToString();
            string[] naSite = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
            int u = Convert.ToInt32(errstr) - 1;
            string[] strslug3 = naSite[u].ToString().Split(';');
            string strslug = strslug3[strslug3.Length - 5];
            int slug = strslug.Length;
            int countAdd = ReturnCountAdd();
            int countDel = countAdd.ToString().Length;
            if (strslug.Contains("\""))
                countDel = countDel + 1;
            string strslug2 = strslug.Remove(slug - countDel);
            if (strslug.Contains("\""))
                strslug2 += countAdd + "\"";
            else
                strslug2 += countAdd;
            naSite[u] = naSite[u].Replace(strslug, strslug2);
            File.WriteAllLines("naSite.csv", naSite, Encoding.GetEncoding(1251));
        }
    }
}
