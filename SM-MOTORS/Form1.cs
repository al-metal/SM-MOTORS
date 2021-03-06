﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Формирование_ЧПУ;
using NehouseLibrary;
using System.Drawing;
using xNet.Net;
using OfficeOpenXml;

namespace SM_MOTORS
{
    public partial class Form1 : Form
    {
        nethouse nethouse = new nethouse();

        CHPU chpu = new CHPU();

        int addCount = 0;
        int editTovar = 0;
        int countEditProduct = 0;
        int delTovar = 0;
        int q = 0;
        double discountPrice = 0.02;
        string boldOpen;
        string boldOpenCSV = "<span style=\"\"font-weight: bold; font-weight: bold;\"\">";
        string boldOpenSite = "<span style=\"font-weight: bold; font-weight: bold;\">";
        string boldClose = "</span>";
        bool chekedSEO;
        bool chekedFullText;
        bool chekedMiniText;
        ExcelWorksheet w;

        public Form1()
        {
            InitializeComponent();

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
            Properties.Settings.Default.login = tbLoginBike.Text;
            Properties.Settings.Default.password = tbPasswordBike.Text;
            Properties.Settings.Default.loginSM = tbLoginSM.Text;
            Properties.Settings.Default.passwordSM = tbPasswordSM.Text;
            Properties.Settings.Default.Save();

            chekedSEO = cbSEO.Checked;
            chekedFullText = cbFullText.Checked;
            chekedMiniText = cbMiniText.Checked;

            string otv = null;
            string loginBike = tbLoginBike.Text;
            string passwordBike = tbPasswordBike.Text;
            string loginSM = tbLoginSM.Text;
            string passwordSM = tbPasswordSM.Text;
            countEditProduct = 0;

            CookieDictionary cookieBike18 = nethouse.CookieNethouse(loginBike, passwordBike);
            CookieContainer cookieSM = LoginSMMOTORS(loginSM, passwordSM);

            if (cookieBike18.Count != 4)
            {
                MessageBox.Show("Логин/пароль для сайта BIKE18.RU введен не верно!");
                return;
            }
            if (cookieSM.Count != 4)
            {
                MessageBox.Show("Логин/пароль для сайта SM-MOTORS введен не верно!");
                return;
            }

            File.Delete("naSite.csv");
            File.Delete("allTovars");
            nethouse.NewListUploadinBike18("naSite");

            FileInfo file = new FileInfo("bike18.xlsx");
            ExcelPackage ep = new ExcelPackage(file);
            w = ep.Workbook.Worksheets[1];
            q = w.Dimension.Rows;

            otv = nethouse.getRequest("https://www.sm-motors.ru/");
            MatchCollection urls = new Regex("(?<=<li><a href=\")/catalog.*?(?=\">)").Matches(otv);
            for (int i = 0; urls.Count > i; i++)
            {
                string urlsCategory = urls[i].ToString();

                #region Запчасти
                if (urlsCategory == "/catalog/zapchasti/")
                {
                    for (int t = i; urls.Count > t; t++)
                    {
                        string urlsZapchasti = urls[t].ToString();
                        if (urlsZapchasti == "/catalog/zapchasti/zapchasti-dlya-pitbaykov-i-kitayskikh-mototsiklov/" ||
                            urlsZapchasti == "/catalog/zapchasti/zapchasti-dlya-skuterov/" ||
                            urlsZapchasti == "/catalog/zapchasti/zapchasti-dlya-kvadrotsiklov/" ||
                            urlsZapchasti == "/catalog/zapchasti/zapchasti-dlya-mototsiklov/" ||
                            urlsZapchasti == "/catalog/zapchasti/zapchasti-dlya-snegokhodov/")
                        {
                            for (int tt = ++t; urls.Count > tt; tt++)
                            {
                                string zapchastiUrls = urls[tt].ToString();
                                if (zapchastiUrls.Contains(urlsZapchasti))
                                {
                                    string pages = "";
                                    otv = nethouse.getRequest("https://www.sm-motors.ru" + zapchastiUrls + "?count=60" + pages);
                                    int maxVal = countPagesSM(otv);

                                    for (int x = 0; maxVal >= x; x++)
                                    {
                                        if (x == 1)
                                            pages = "";
                                        else
                                            pages = "&PAGEN_1=" + x;

                                        if (maxVal == 0)
                                            pages = "";

                                        otv = nethouse.getRequest("https://www.sm-motors.ru" + zapchastiUrls + "?count=60" + pages);
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
                                }
                            }

                            //uploadNewTovar(cookieBike18);
                        }
                    }
                }
                #endregion
            }

            #region Удаление товаров с сайта байк18 если его нет на сайте см-моторс
            //string[] allTovars = File.ReadAllLines("allTovars");
            //if (allTovars.Length > 1)
            //{
            //    otv = null;
            //    otv = nethouse.getRequest("http://bike18.ru/products/category/1689456");
            //    MatchCollection razdel = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);
            //    for (int i = 0; razdel.Count > i; i++)
            //    {
            //        List<string> tovar = new List<string>();
            //        string urlRazdel = razdel[i].ToString();
            //        if (urlRazdel == "/products/category/sm-motors-zapchasti")
            //            urlRazdel = "http://bike18.ru/products/category/sm-motors-zapchasti";
            //        else if (urlRazdel == "/products/category/tuning-dly-skuterov")
            //            urlRazdel = "https://bike18.ru/products/category/2078597/page/all";
            //        else if (urlRazdel == "/products/category/sm-motors-pokrishki-kameri")
            //            urlRazdel = "https://bike18.ru/products/category/sm-motors-pokrishki-kameri";
            //        else if (urlRazdel == "/products/category/2331797")
            //            urlRazdel = "https://bike18.ru/products/category/2331797";
            //        else if (urlRazdel == "/products/category/sm-zapchasti-velogibridi")
            //            urlRazdel = "https://bike18.ru/products/category/2325564/page/all";
            //        else if (urlRazdel == "/products/category/zapchasti-dly-lodochnih-motorov")
            //            urlRazdel = "https://bike18.ru/products/category/2326229/page/all";
            //        else
            //            urlRazdel = "https://bike18.ru" + urlRazdel + "/page/all";

            //        otv = nethouse.getRequest(urlRazdel);
            //        MatchCollection product = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
            //        MatchCollection podRazdel = new Regex("(?<=center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);

            //        if (podRazdel.Count != 0)
            //        {
            //            for (int p = 0; podRazdel.Count > p; p++)
            //            {
            //                string urlPodRazdel = podRazdel[p].ToString();
            //                if (urlPodRazdel == "/products/category/sm-zapchasti-dvigatel")
            //                    urlPodRazdel = "http://bike18.ru/products/category/2328540/page/all";
            //                else if (urlPodRazdel == "/products/category/zapchasti")
            //                    urlPodRazdel = "https://bike18.ru/products/category/2328541/page/all";
            //                else
            //                    urlPodRazdel = "https://bike18.ru" + urlPodRazdel + "/page/all";

            //                otv = nethouse.getRequest(urlPodRazdel);
            //                MatchCollection product2 = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
            //                MatchCollection podRazdel2 = new Regex("(?<=center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);

            //                if (podRazdel2.Count != 0)
            //                {
            //                    for (int d = 0; podRazdel2.Count > d; d++)
            //                    {
            //                        string urlPodRazdel2 = podRazdel2[d].ToString();
            //                        otv = nethouse.getRequest("http://bike18.ru" + urlPodRazdel2 + "/page/all");
            //                        MatchCollection product3 = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
            //                        MatchCollection podRazdel3 = new Regex("(?<=center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);

            //                        if (product3.Count != 0)
            //                        {
            //                            for (int m = 0; product3.Count > m; m++)
            //                            {
            //                                tovar = nethouse.GetProductList(cookieBike18, product3[m].ToString());
            //                                string article = tovar[6].ToString();
            //                                string[] all = File.ReadAllLines("allTovars", Encoding.GetEncoding(1251));
            //                                bool b = false;
            //                                foreach (string s in all)
            //                                {
            //                                    if (s == article)
            //                                        b = true;
            //                                }
            //                                if (!b)
            //                                {
            //                                    nethouse.DeleteProduct(cookieBike18, tovar);
            //                                    delTovar++;
            //                                }
            //                            }
            //                        }

            //                        if (podRazdel3.Count != 0)
            //                        {

            //                        }
            //                    }
            //                }

            //                if (product2.Count != 0)
            //                {
            //                    for (int m = 0; product2.Count > m; m++)
            //                    {
            //                        tovar = nethouse.GetProductList(cookieBike18, product2[m].ToString());
            //                        string article = tovar[6].ToString();
            //                        string[] all = File.ReadAllLines("allTovars", Encoding.GetEncoding(1251));
            //                        bool b = false;
            //                        foreach (string s in all)
            //                        {
            //                            if (s == article)
            //                                b = true;
            //                        }
            //                        if (!b)
            //                        {
            //                            nethouse.DeleteProduct(cookieBike18, tovar);
            //                            delTovar++;
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        if (product.Count != 0)
            //        {
            //            for (int m = 0; product.Count > m; m++)
            //            {
            //                tovar = nethouse.GetProductList(cookieBike18, product[m].ToString());
            //                string article = tovar[6].ToString();
            //                string[] all = File.ReadAllLines("allTovars", Encoding.GetEncoding(1251));
            //                bool b = false;
            //                foreach (string s in all)
            //                {
            //                    if (s == article)
            //                        b = true;
            //                }
            //                if (!b)
            //                {
            //                    nethouse.DeleteProduct(cookieBike18, tovar);
            //                    delTovar++;
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            MessageBox.Show("Измененные и новые товары находятся в файле nasite.csv в папке с программой\nНеобходимо загрузить данный файл на сайт");
        }

        private void uploadNewTovar(CookieDictionary cookie)
        {
            //System.Threading.Thread.Sleep(20000);
            string[] naSite1 = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
            if (naSite1.Length > 1)
            {
                //   nethouse.UploadCSVNethouse(cookie, "naSite.csv", tbLoginBike.Text, tbPasswordBike.Text);
            }
            // File.Delete("naSite.csv");
            //nethouse.NewListUploadinBike18("naSite");
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
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.login = tbLoginBike.Text;
            Properties.Settings.Default.password = tbPasswordBike.Text;
            Properties.Settings.Default.Save();
            string login = tbLoginBike.Text;
            string password = tbPasswordBike.Text;
            CookieDictionary cookieBike18 = nethouse.CookieNethouse(login, password);
            editTovar = 0;

            string otv = null;
            otv = nethouse.getRequest("http://bike18.ru/products/category/1689456");
            MatchCollection razdel = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);
            for (int i = 0; razdel.Count > i; i++)
            {
                string urlRazdel = razdel[i].ToString();
                if (urlRazdel == "/products/category/sm-motors-zapchasti")
                    urlRazdel = "http://bike18.ru/products/category/sm-motors-zapchasti";
                else if (urlRazdel == "/products/category/tuning-dly-skuterov")
                    urlRazdel = "https://bike18.ru/products/category/2078597/page/all";
                else if (urlRazdel == "/products/category/sm-motors-pokrishki-kameri")
                    urlRazdel = "https://bike18.ru/products/category/sm-motors-pokrishki-kameri";
                else if (urlRazdel == "/products/category/2331797")
                    urlRazdel = "https://bike18.ru/products/category/2331797";
                else if (urlRazdel == "/products/category/sm-zapchasti-velogibridi")
                    urlRazdel = "https://bike18.ru/products/category/2325564/page/all";
                else if (urlRazdel == "/products/category/zapchasti-dly-lodochnih-motorov")
                    urlRazdel = "https://bike18.ru/products/category/2326229/page/all";
                else
                    urlRazdel = "https://bike18.ru" + urlRazdel + "?page=all";

                otv = nethouse.getRequest(urlRazdel);
                MatchCollection product = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                MatchCollection podRazdel = new Regex("(?<=center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);

                if (podRazdel.Count != 0)
                {
                    for (int p = 0; podRazdel.Count > p; p++)
                    {
                        string urlPodRazdel = podRazdel[p].ToString();
                        if (urlPodRazdel == "/products/category/sm-zapchasti-dvigatel")
                            urlPodRazdel = "http://bike18.ru/products/category/2328540/page/all";
                        else if (urlPodRazdel == "/products/category/zapchasti")
                            urlPodRazdel = "https://bike18.ru/products/category/2328541/page/all";
                        else
                            urlPodRazdel = "https://bike18.ru" + urlPodRazdel + "?page=all";

                        otv = nethouse.getRequest(urlPodRazdel);
                        MatchCollection product2 = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                        MatchCollection podRazdel2 = new Regex("(?<=center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);
                        if (podRazdel2.Count != 0)
                        {
                            for (int o = 0; podRazdel2.Count > o; o++)
                            {
                                string urlPodRazdel2 = podRazdel2[o].ToString();
                                otv = nethouse.getRequest("http://bike18.ru" + urlPodRazdel2 + "?page=all");
                                MatchCollection product3 = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                                MatchCollection podRazdel3 = new Regex("(?<=center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);

                                for (int b = 0; product3.Count > b; b++)
                                {
                                    string urlProduct = product3[b].ToString();

                                    UpdateTovar(cookieBike18, urlProduct);
                                }
                            }
                        }
                        if (product2.Count != 0)
                        {
                            for (int m = 0; product2.Count > m; m++)
                            {
                                string urlProduct = product2[m].ToString();

                                UpdateTovar(cookieBike18, urlProduct);
                            }
                        }
                    }
                }

                if (product.Count != 0)
                {
                    for (int n = 0; product.Count > n; n++)
                    {
                        string urlProduct = product[n].ToString();

                        UpdateTovar(cookieBike18, urlProduct);
                    }
                }
            }
            MessageBox.Show("Обновлено товаров " + editTovar);
        } //загрузка картинок

        private void UpdateTovar(CookieDictionary cookieBike18, string urlProduct)
        {
            bool b = false;
            List<string> listProduct = nethouse.GetProductList(cookieBike18, urlProduct);

            string article = listProduct[6].ToString();
            string images = listProduct[32];
            string alsoby = listProduct[42];
            string productGroupe = listProduct[3];

            if (!article.Contains("SM_"))
            {
                listProduct[6] = "SM_" + article;
                b = true;
            }

            /*
            if (images == "")
            {
                if (File.Exists("Pic\\" + article + ".jpg"))
                {
                    nethouse.UploadImage(cookieBike18, urlProduct);
                    b = true;
                    editTovar++;
                }
            }

            if (alsoby == "&alsoBuy[0]=0")
            {
                listProduct[42] = nethouse.alsoBuyTovars(listProduct);
                b = true;
                editTovar++;
            }

            if (productGroupe != "10833347")
            {
                listProduct[3] = "10833347";
                b = true;
                editTovar++;
            }*/

            if (b)
                nethouse.SaveTovar(cookieBike18, listProduct);
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

        public void AddTovarInCSV(CookieDictionary cookieBike18, string urlTovar, double discountPrice, string urlsCategory)
        {
            List<string> newProduct = new List<string>();
            CookieContainer cookie = new CookieContainer();

            string dblProduct = "НАЗВАНИЕ также подходит для аналогичных моделей.";
            string name = "";
            string article = "";
            string availability = "";
            string descriptionTovar = "";
            string characteristics = "";
            string urlImage = "";
            string podRazdelSeo = "";
            string priceNoProd = "";
            string chpuNoProd = "";
            string metka = "";
            string razdel = "";
            string razdelSeo = "";

            string discount = nethouse.Discount();

            List<string> tovarSMMotors = getTovarSMMotors(urlTovar, urlsCategory);
            if (tovarSMMotors == null)
                return;

            try
            {
                name = tovarSMMotors[0].ToString();
                article = tovarSMMotors[1].ToString();
                availability = tovarSMMotors[2].ToString();
                descriptionTovar = tovarSMMotors[3].ToString();
                characteristics = tovarSMMotors[4].ToString();
                urlImage = tovarSMMotors[5].ToString();
                podRazdelSeo = tovarSMMotors[6].ToString();
                priceNoProd = tovarSMMotors[7].ToString();
                chpuNoProd = tovarSMMotors[8].ToString();
                metka = tovarSMMotors[9].ToString();
                razdel = tovarSMMotors[10].ToString();
                razdelSeo = tovarSMMotors[11].ToString();
            }
            catch
            {
                return;
            }

            bool malossi = ReturnMalossi(name, ref priceNoProd);

            WrireArticleTovar(article);

            if (!File.Exists("Pic\\" + article + ".jpg"))
            {
                EditSizeImages(urlImage, article);
            }

            List<string> tovarB18;
            bool b18;
            GetTovarB18(article.Replace("-", "_"), out tovarB18, out b18);

            if (availability == "1")
            {
                if (b18)
                {
                    string priceB18 = tovarB18[3];

                    if (priceNoProd == "0" && malossi == false)
                    {
                        string urlTovarBike = "https://bike18.ru/products/" + tovarB18[0].ToString();
                        nethouse.DeleteProduct(cookieBike18, urlTovarBike);
                        delTovar++;
                        return;
                    }

                    if (priceB18 != priceNoProd)
                    {
                        newProduct = new List<string>();
                        newProduct.Add("\"" + tovarB18[0] + "\"");                                   //id
                        newProduct.Add("\"" + tovarB18[1] + "\"");                                    //артикул
                        newProduct.Add("\"" + tovarB18[2] + "\"");                               //название
                        newProduct.Add("\"" + priceNoProd + "\"");                                 //стоимость
                        newProduct.Add("\"" + tovarB18[4] + "\"");                              //со скидкой
                        newProduct.Add("\"" + tovarB18[5] + "\"");                                     //раздел товара
                        newProduct.Add("\"" + tovarB18[6] + "\"");                                  //в наличии
                        newProduct.Add("\"" + tovarB18[7] + "\"");                                    //поставка
                        newProduct.Add("\"" + tovarB18[8] + "\"");                                 //срок поставки
                        newProduct.Add("\"" + tovarB18[9].Replace("\"", "\"\"") + "\"");                                       //краткий текст
                        newProduct.Add("\"" + tovarB18[10].Replace("\"", "\"\"") + "\"");                               //полностью текст
                        newProduct.Add("\"" + tovarB18[11].Replace("\"", "\"\"") + "\"");                                 //заголовок страницы
                        newProduct.Add("\"" + tovarB18[12].Replace("\"", "\"\"") + "\"");                              //описание
                        newProduct.Add("\"" + tovarB18[13].Replace("\"", "\"\"") + "\"");                               //ключевые слова
                        newProduct.Add("\"" + tovarB18[14] + "\"");                                //ЧПУ
                        newProduct.Add("\"" + tovarB18[15] + "\"");                                                      //с этим товаром покупают
                        newProduct.Add("\"" + tovarB18[16] + "\"");                                            //рекламные метки
                        newProduct.Add("\"" + tovarB18[17] + "\"");                                      //показывать
                        newProduct.Add("\"" + tovarB18[18] + "\"");

                        if (priceNoProd != "0")
                            nethouse.WriteFileInCSV(newProduct, "naSite");
                        else if (priceNoProd == "0" && malossi)
                            nethouse.WriteFileInCSV(newProduct, "naSite");
                    }
                }
                else
                {
                    boldOpen = boldOpenCSV;

                    string minitext = null;
                    string titleText = null;
                    string descriptionText = null;
                    string keywordsText = null;
                    string fullText = null;
                    string dblProdSEO = null;
                    string artNoProd = article;
                    string razdelmini = null;
                    string podRazdel = null;

                    minitext = MiniText();
                    fullText = FullText();
                    titleText = textBox1.Lines[0].ToString();
                    descriptionText = textBox2.Lines[0].ToString() + " " + dblProdSEO;
                    keywordsText = textBox3.Lines[0].ToString();
                    discount = discount.Replace("\"", "\"\"");

                    podRazdel = boldOpen + podRazdelSeo + boldClose;
                    razdelmini = boldOpen + razdelSeo + boldClose;
                    string nameText = boldOpen + name + boldClose;

                    minitext = minitext.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", podRazdel).Replace("РАЗДЕЛ", razdelmini).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", nameText).Replace("АРТИКУЛ", artNoProd).Replace("ОПИСАНИЕ", descriptionTovar).Replace("ХАРАКТЕРИСТИКА", characteristics).Replace("<p><br /></p><p></p><p><br /></p>", "<p><br /></p>").Replace("<p><br /></p><p><br /></p><p><br /></p>", "");
                    minitext = nethouse.ReplaceAmpersandsChar(minitext);

                    fullText = fullText.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", podRazdel).Replace("РАЗДЕЛ", razdelmini).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", nameText).Replace("АРТИКУЛ", artNoProd).Replace("ОПИСАНИЕ", descriptionTovar).Replace("ХАРАКТЕРИСТИКА", characteristics).Replace("<p><br /></p>", "");
                    fullText = nethouse.ReplaceAmpersandsChar(fullText);

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
                        nethouse.WriteFileInCSV(newProduct, "naSite");
                }
            }
            else
            {
                // Товар есть на сайте но нет в наличии, удаляем его у нас с сайта
                if (b18)
                {
                    string urlTovarBike = "https://bike18.ru/products/" + tovarB18[0].ToString();
                    nethouse.DeleteProduct(cookieBike18, urlTovarBike);
                    delTovar++;
                }
            }
        }

        private void GetTovarB18(string article, out List<string> tovarB18, out bool b18)
        {
            tovarB18 = new List<string>();
            b18 = false;
            for (int i = 1; q > i; i++)
            {
                string articleB18 = (string)w.Cells[i, 2].Value.ToString();
                if (article == "020233_803_2892")
                {

                }

                if (article == articleB18)
                {
                    string oldPriceB18 = "";
                    string alsoByB18 = "";
                    string reklamaB18 = "";
                    string slugB18 = "";

                    string idB18 = (string)w.Cells[i, 1].Value.ToString();
                    string nameB18 = (string)w.Cells[i, 3].Value.ToString();
                    string priceB18 = (string)w.Cells[i, 4].Value.ToString();
                    try
                    {
                        oldPriceB18 = (string)w.Cells[i, 5].Value.ToString();

                    }
                    catch
                    {
                        oldPriceB18 = "";
                    }

                    string categoryB18 = (string)w.Cells[i, 6].Value.ToString();
                    string availabilityB18 = (string)w.Cells[i, 7].Value.ToString();
                    string postavkaB18 = (string)w.Cells[i, 8].Value.ToString();
                    string postavkaDayB18 = (string)w.Cells[i, 9].Value.ToString();
                    string MiniTextB18 = (string)w.Cells[i, 10].Value.ToString();
                    string fullTextB18 = (string)w.Cells[i, 11].Value.ToString();
                    string titleB18 = (string)w.Cells[i, 12].Value.ToString();
                    string descriptionB18 = (string)w.Cells[i, 13].Value.ToString();
                    string keywordsB18 = (string)w.Cells[i, 14].Value.ToString();
                    try
                    {
                        slugB18 = (string)w.Cells[i, 15].Value.ToString();
                    }
                    catch
                    {
                        slugB18 = "";
                    }
                    try
                    {
                        alsoByB18 = (string)w.Cells[i, 16].Value.ToString();
                    }
                    catch
                    {
                        alsoByB18 = "";
                    }
                    try
                    {
                        reklamaB18 = (string)w.Cells[i, 17].Value.ToString();
                    }
                    catch
                    {
                        reklamaB18 = "";
                    }
                    string showB18 = (string)w.Cells[i, 18].Value.ToString();
                    string deleteB18 = (string)w.Cells[i, 19].Value.ToString();

                    tovarB18.Add(idB18);
                    tovarB18.Add(articleB18);
                    tovarB18.Add(nameB18);
                    tovarB18.Add(priceB18);
                    tovarB18.Add(oldPriceB18);
                    tovarB18.Add(categoryB18);
                    tovarB18.Add(availabilityB18);
                    tovarB18.Add(postavkaB18);
                    tovarB18.Add(postavkaDayB18);
                    tovarB18.Add(MiniTextB18);
                    tovarB18.Add(fullTextB18);
                    tovarB18.Add(titleB18);
                    tovarB18.Add(descriptionB18);
                    tovarB18.Add(keywordsB18);
                    tovarB18.Add(slugB18);
                    tovarB18.Add(alsoByB18);
                    tovarB18.Add(reklamaB18);
                    tovarB18.Add(showB18);
                    tovarB18.Add(deleteB18);

                    b18 = true;
                    break;
                }

            }
        }

        private static bool ReturnMalossi(string name, ref string priceNoProd)
        {
            bool malossi = false;

            if (name.Contains("Malossi") || name.Contains("malossi"))
            {
                malossi = true;
                priceNoProd = "0";
            }

            return malossi;
        }

        private void WrireArticleTovar(string article)
        {
            StreamWriter sw = new StreamWriter("allTovars", true);
            sw.WriteLine(article);
            sw.Close();
        }

        private List<string> getTovarSMMotors(string urlTovar, string urlsCategory)
        {
            CookieContainer cookie = new CookieContainer();
            List<string> getTovar = new List<string>();
            string otv;
            double priceD = 0;

            otv = nethouse.getRequest(urlTovar);

            if (otv == "err" || otv == null || otv == "")
            {
                otv = nethouse.getRequest(urlTovar);

                if (otv == "err" || otv == null || otv == "")
                    return getTovar = null;
            }

            string otvPrice;
            try
            {
                otvPrice = nethouse.getRequest(urlTovar + "?bxrand =1500970347631");
            }
            catch
            {
                return getTovar = null;
            }

            string razdelmini = null;
            string razdelSeo = null;
            string podRazdel = null;
            string podrazdel1 = null;
            string razdel = "Запчасти и расходники => Каталог запчастей SM-MOTORS => ";
            string metka = "";

            string availability = new Regex("(?<=<input type=\"text\" maxlength=\"3\" value=\").*(?=\" data-max)").Match(otv).ToString();

            string article = new Regex("(?<=<span>Артикул:</span>).*?(?=</div>)").Match(otv).ToString();
            article = "SM_" + article.Replace("-", "_");

            string name = new Regex("(?<=<h1>).*(?=</h1>)").Match(otv).ToString();
            name = name.Replace("&quot;", "").Replace("&gt;", ">").Replace("&#039;", "'").Replace("+", "").Replace("  ", " ").Trim();

            string saleMEtka = new Regex("(?<=<div class=\"old-price\">).*?(?=</div>)").Match(otv).ToString();

            string newMetka = new Regex("(?<=<div class=\"icons-container\">)[\\w\\W]*?(?=<div class=\"ico\"></div></div>)").Match(otv).ToString().Trim();
            if (newMetka != "")
                metka = "новинка";

            if (saleMEtka != "")
                metka = "акция";

            string descriptionTovar = descriptionsTovar(otv);
            descriptionTovar = nethouse.ReplaceAmpersandsChar(descriptionTovar).Replace("\"", "");

            string characteristics = new Regex("(?<=<div class=\"tab-content columns-content\">)[\\w\\W]*?(?=</ul>)").Match(otv).ToString().Replace("<ul>", "").Replace("<li>", "").Replace("</li>", "").Replace("\n", " ").Replace("\"", "\"\"").Trim();
            characteristics = nethouse.ReplaceAmpersandsChar(characteristics).Replace("\"", "");

            string urlImage = new Regex("(?<=<div class=\"gallery\">)[\\w\\W]*?(?=data-large=)").Match(otv).ToString().Replace("<a href=\"", "").Replace("\"", "").Trim();
            urlImage = "https:" + urlImage;

            string strPrice = new Regex("(?<=<span class=\"price\">).*(?=</span>)").Match(otvPrice).ToString();
            if (strPrice != "")
            {
                priceD = Convert.ToDouble(strPrice);
                if (priceD > 200)
                    priceD = nethouse.ReturnPrice(priceD, discountPrice);
            }

            string chpuNoProd = chpu.vozvr(name);

            string objProduct = urlsCategory;

            string namesLinkMenu = new Regex("(?<=<a href=\"/\" title=\"Главная\">Главная</a></span><span><a).*(?=</a></span>)").Match(otv).ToString();
            MatchCollection titlesMenu = new Regex("(?<=/\" title=\").*?(?=\">)").Matches(namesLinkMenu);

            if (urlsCategory == "/catalog/zapchasti/")
            {
                podrazdel1 = new Regex("(?<=\" title=\"Запчасти\">Запчасти</a></span><span><a href=\").*?(?=\" title=\")").Match(otv).ToString();
                objProduct = new Regex("(?<=/catalog/zapchasti/).*(?=/)").Match(podrazdel1).ToString();
            }

            if (objProduct == "/catalog/gsm/")
            {
                objProduct = new Regex("(?<=href=\"/catalog/gsm/\" title=\"ГСМ\">ГСМ</a></span><span><a href=\").*(?=\" title=\")").Match(otv).ToString();
                if (objProduct == "")
                    objProduct = "/catalog/gsm/";
            }

            objProduct = ReturnNameRazdel(objProduct);

            ReturnRazdel(ref razdelmini, ref razdelSeo, ref razdel, name, objProduct, titlesMenu);

            podRazdel = new Regex("(?<=\">" + razdelmini + "</a></span><span><a href=\").*?(?=>)").Match(otv).ToString();
            podRazdel = new Regex("(?<=title=\").*?(?=\")").Match(podRazdel).ToString();
            string str = new Regex("(?<=<div class=\"breadcrumbs-container\"><span><a href=\"/\" title=\").*?(?=</span></div></div></section>)").Match(otv).ToString();

            string podrazdelSeo = podRazdel;

            getTovar.Add(name);
            getTovar.Add(article);
            getTovar.Add(availability);
            getTovar.Add(descriptionTovar);
            getTovar.Add(characteristics);
            getTovar.Add(urlImage);
            getTovar.Add(podRazdel);
            getTovar.Add(priceD.ToString());
            getTovar.Add(chpuNoProd);
            getTovar.Add(metka);
            getTovar.Add(razdel);
            getTovar.Add(razdelSeo);
            getTovar.Add(podrazdelSeo);

            return getTovar;
        }

        private static string ReturnNameRazdel(string objProduct)
        {
            if (objProduct.Contains("/catalog/zapchasti-lodochnykh-motorov/anodnaya-zashchita/"))
                objProduct = "/catalog/zapchasti-lodochnykh-motorov/anodnaya-zashchita/";
            else if (objProduct.Contains("/catalog/zapchasti-lodochnykh-motorov/zapchasti-dlya-podvesnykh-lodochnykh-motorov/"))
                objProduct = "/catalog/zapchasti-lodochnykh-motorov/zapchasti-dlya-podvesnykh-lodochnykh-motorov/";
            else if (objProduct.Contains("/catalog/zapchasti-lodochnykh-motorov/zapchasti-dlya-statsionarnykh-dvigateley/"))
                objProduct = "/catalog/zapchasti-lodochnykh-motorov/zapchasti-dlya-statsionarnykh-dvigateley/";
            else if (objProduct.Contains("/catalog/zapchasti-lodochnykh-motorov/grebnye-vinty/"))
                objProduct = "/catalog/zapchasti-lodochnykh-motorov/grebnye-vinty/";
            else if (objProduct.Contains("/catalog/zapchasti-lodochnykh-motorov/distantsionnoe-upravlenie/"))
                objProduct = "/catalog/zapchasti-lodochnykh-motorov/distantsionnoe-upravlenie/";
            else if (objProduct.Contains("/catalog/zapchasti-lodochnykh-motorov/filtry_1/"))
                objProduct = "/catalog/zapchasti-lodochnykh-motorov/filtry_1/";
            else if (objProduct.Contains("/catalog/zapchasti-lodochnykh-motorov/toplivnye-sistema/"))
                objProduct = "/catalog/zapchasti-lodochnykh-motorov/toplivnye-sistema/";
            else if (objProduct.Contains("/catalog/zapchasti-lodochnykh-motorov/elektrooborudovanie-i-prinadlezhnosti-/"))
                objProduct = "/catalog/zapchasti-lodochnykh-motorov/elektrooborudovanie-i-prinadlezhnosti-/";
            return objProduct;
        }

        private static void ReturnRazdel(ref string razdelmini, ref string razdelSeo, ref string razdel, string name, string objProduct, MatchCollection titlesMenu)
        {
            switch (objProduct)
            {
                case ("/catalog/zapchasti/zapchasti-snegokhody/buran-rys-tayga/"):
                    if (name.Contains("Тайга"))
                    {
                        razdel = "Запчасти и расходники => Запчасти для снегоходов и мотобуксировщиков => Запчасти на снегоходы Тайга";
                        razdelmini = "Снегоход Тайга";
                        razdelSeo = "Снегоход Тайга";
                    }
                    else if (name.Contains("Рысь"))
                    {
                        razdel = "Запчасти и расходники => Запчасти для снегоходов и мотобуксировщиков => Запчасти на снегоходы Рысь";
                        razdelmini = "Снегоход Рысь";
                        razdelSeo = "Снегоход Рысь";
                    }
                    else if (name.Contains("Буран"))
                    {
                        razdel = "Запчасти и расходники => Запчасти для снегоходов и мотобуксировщиков => Запчасти на снегоходы Буран";
                        razdelmini = "Снегоход Буран";
                        razdelSeo = "Снегоход Буран";
                    }
                    else
                    {
                        razdel = "Запчасти и расходники => Запчасти для снегоходов и мотобуксировщиков => Различные запчасти на снегоходы";
                        razdelmini = "Различные запчасти на снегоходы";
                        razdelSeo = "Различные запчасти на снегоходы";
                    }
                    break;
                case ("/catalog/zapchasti/zapchasti-snegokhody/snowmax-t-200/"):
                    razdel = "Запчасти и расходники => Запчасти для снегоходов и мотобуксировщиков => SNOWMAX T-200";
                    razdelmini = "Снегоход SNOWMAX T-200";
                    razdelSeo = "Снегоход SNOWMAX T-200";
                    break;
                case ("/catalog/zapchasti/zapchasti-snegokhody/remni-variatora_1/"):
                    razdel = "Запчасти и расходники => Запчасти для снегоходов и мотобуксировщиков => Ремни вариатора";
                    razdelmini = "Ремни вариатора";
                    razdelSeo = "Ремни вариатора";
                    break;
                case ("zapchasti-dlya-pitbaykov-i-kitayskikh-mototsiklov"):
                    razdel = razdel + "Запчасти => Запчасти для питбайков и китайских мотоциклов => " + titlesMenu[2].ToString();
                    razdelmini = titlesMenu[2].ToString();
                    razdelSeo = titlesMenu[2].ToString();
                    break;
                case ("/catalog/pokryshki-kamery/aksessuary-dlya-pokryshek/"):
                    razdel = "Запчасти и расходники => Расходники для мототехники => Аксессуары => Аксессуары для покрышек";
                    razdelmini = "Аксессуары для покрышек";
                    razdelSeo = "Аксессуары для покрышек";
                    break;
                case ("/catalog/pokryshki-kamery/kamery/"):
                    razdel = "Запчасти и расходники => Расходники для мототехники => Моторезина";
                    razdelmini = "Камеры";
                    razdelSeo = "Камеры";
                    break;
                case ("/catalog/pokryshki-kamery/pokryshki-dlya-atv/"):
                    razdel = "Запчасти и расходники => Расходники для мототехники => Моторезина";
                    razdelmini = "Покрышки для ATV";
                    razdelSeo = "Покрышки для ATV";
                    break;
                case ("/catalog/pokryshki-kamery/pokryshki-dlya-mototsiklov/"):
                    razdel = "Запчасти и расходники => Расходники для мототехники => Моторезина";
                    razdelmini = "Покрышки для мотоциклов";
                    razdelSeo = "Покрышки для мотоциклов";
                    break;
                case ("/catalog/pokryshki-kamery/pokryshki-dlya-skuterov/"):
                    razdel = "Запчасти и расходники => Расходники для мототехники => Моторезина";
                    razdelmini = "Покрышки для скутеров";
                    razdelSeo = "Покрышки для скутеров";
                    break;
                case ("/catalog/zapchasti/zapchasti-originalnye/"):
                    razdel = razdel + "Запчасти => Запчасти оригинальные";
                    razdelmini = "Запчасти оригинальные";
                    razdelSeo = "Запчасти оригинальные";
                    break;
                case ("/catalog/zapchasti/dvigateli/"):
                    razdel = "Запчасти и расходники => Двигатели";
                    razdelmini = "Двигатели";
                    razdelSeo = "Двигатели";
                    break;
                case ("zapchasti-dlya-mototsiklov"):
                    if (titlesMenu[2].ToString() == "Запчасти для Кроссовых мотоциклов")
                    {
                        razdel = razdel + "Запчасти => Запчасти для кроссовых и эндуро мотоциклов => " + titlesMenu[3].ToString();
                        razdelmini = titlesMenu[3].ToString();
                        razdelSeo = titlesMenu[3].ToString();
                    }
                    else
                    {
                        razdel = razdel + "Запчасти => Запчасти для японских, европейских, американских мотоциклов => " + titlesMenu[2].ToString();
                        razdelmini = titlesMenu[2].ToString();
                        razdelSeo = titlesMenu[2].ToString();
                    }
                    break;
                case ("zapchasti-dlya-kvadrotsiklov"):
                    razdel = razdel + "Запчасти => Запчасти для квадроциклов => " + titlesMenu[2].ToString();
                    razdelmini = titlesMenu[2].ToString();
                    razdelSeo = titlesMenu[2].ToString();
                    break;
                case ("zapchasti-dlya-skuterov"):
                    razdel = "Запчасти и расходники => Запчасти для скутеров => " + titlesMenu[2].ToString();
                    razdelmini = titlesMenu[2].ToString();
                    razdelSeo = titlesMenu[2].ToString();
                    break;
                case ("/catalog/akkumulyatory/"):
                    razdel = "Запчасти и расходники => Расходники для мототехники => Аккумуляторы";
                    razdelmini = "Аккумуляторы";
                    razdelSeo = "Аккумуляторы";
                    break;
                case ("/catalog/tyuning-dlya-skuterov/"):
                    razdel = "Запчасти и расходники => Запчасти для скутеров => Тюнинг для скутеров";
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

                case ("/catalog/gsm/maslo-agip-eni/"):
                    razdel = "Запчасти и расходники => ГСМ => Масло AGIP-ENI";
                    razdelmini = "ГСМ";
                    razdelSeo = "ГСМ";
                    break;
                case ("/catalog/gsm/maslo-liqui-moly/"):
                    razdel = "Запчасти и расходники => ГСМ => Масло LIQUI MOLY";
                    razdelmini = "ГСМ";
                    razdelSeo = "ГСМ";
                    break;
                case ("/catalog/gsm/maslo-motul/"):
                    razdel = "Запчасти и расходники => ГСМ => Масло MOTUL";
                    razdelmini = "ГСМ";
                    razdelSeo = "ГСМ";
                    break;
                case ("/catalog/gsm/maslo-repsol/"):
                    razdel = "Запчасти и расходники => ГСМ => Масло REPSOL";
                    razdelmini = "ГСМ";
                    razdelSeo = "ГСМ";
                    break;
                case ("/catalog/gsm/maslo-ipone/"):
                    razdel = "Запчасти и расходники => ГСМ => IPONE";
                    razdelmini = "IPONE";
                    razdelSeo = "IPONE";
                    break;
                case ("/catalog/gsm/maslo-maxima/"):
                    razdel = "Запчасти и расходники => ГСМ => Maxima";
                    razdelmini = "Maxima";
                    razdelSeo = "Maxima";
                    break;
                case ("/catalog/gsm/"):
                    razdel = "Запчасти и расходники => ГСМ => Разное";
                    razdelmini = "ГСМ";
                    razdelSeo = "ГСМ";
                    break;
                case ("/catalog/kofry-sumki/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Кофры / Сумки";
                    razdelmini = "Кофры / Сумки";
                    razdelSeo = "Кофры / Сумки";
                    break;
                case ("/catalog/gsm/maslo-bel-ray/"):
                    razdel = "Запчасти и расходники => ГСМ => Масло BEL-RAY";
                    razdelmini = "Масло BEL-RAY";
                    razdelSeo = "Масло BEL-RAY";
                    break;
                case ("/catalog/zapchasti-lodochnykh-motorov/anodnaya-zashchita/"):
                    razdel = "Запчасти и расходники => Запчасти лодочных моторов => Анодная защита";
                    razdelmini = "Анодная защита";
                    razdelSeo = "Анодная защита";
                    break;
                case ("/catalog/zapchasti-lodochnykh-motorov/zapchasti-dlya-podvesnykh-lodochnykh-motorov/"):
                    razdel = "Запчасти и расходники => Запчасти лодочных моторов => Запчасти для подвесных лодочных моторов";
                    razdelmini = "Запчасти для подвесных лодочных моторов";
                    razdelSeo = "Запчасти для подвесных лодочных моторов";
                    break;
                case ("/catalog/zapchasti-lodochnykh-motorov/zapchasti-dlya-statsionarnykh-dvigateley/"):
                    razdel = "Запчасти и расходники => Запчасти лодочных моторов => Запчастидля стационарных двигателей";
                    razdelmini = "Запчастидля стационарных двигателей";
                    razdelSeo = "Запчастидля стационарных двигателей";
                    break;
                case ("/catalog/zapchasti-lodochnykh-motorov/grebnye-vinty/"):
                    razdel = "Запчасти и расходники => Запчасти лодочных моторов => Гребные винты";
                    razdelmini = "Гребные винты";
                    razdelSeo = "Гребные винты";
                    break;
                case ("/catalog/zapchasti-lodochnykh-motorov/distantsionnoe-upravlenie/"):
                    razdel = "Запчасти и расходники => Запчасти лодочных моторов => Дистанционное управление";
                    razdelmini = "Дистанционное управление";
                    razdelSeo = "Дистанционное управление";
                    break;
                case ("/catalog/zapchasti-lodochnykh-motorov/toplivnye-sistema/"):
                    razdel = "Запчасти и расходники => Запчасти лодочных моторов => Топливные система";
                    razdelmini = "Топливные система";
                    razdelSeo = "Топливные система";
                    break;
                case ("/catalog/zapchasti-lodochnykh-motorov/filtry_1/"):
                    razdel = "Запчасти и расходники => Запчасти лодочных моторов => Фильтры";
                    razdelmini = "Фильтры";
                    razdelSeo = "Фильтры";
                    break;
                case ("/catalog/zapchasti-lodochnykh-motorov/elektrooborudovanie-i-prinadlezhnosti-/"):
                    razdel = "Запчасти и расходники => Запчасти лодочных моторов => Электрооборудование и принадлежности";
                    razdelmini = "Электрооборудование и принадлежности";
                    razdelSeo = "Электрооборудование и принадлежности";
                    break;
                case ("/catalog/gsm/maslo-ipone_1/"):
                    razdel = "Запчасти и расходники => ГСМ => IPONE";
                    razdelmini = "IPONE";
                    razdelSeo = "IPONE";
                    break;
                case ("/catalog/gsm/maslo-bel-ray_1/"):
                    razdel = "Запчасти и расходники => ГСМ => Масло BEL-RAY";
                    razdelmini = "Масло BEL-RAY";
                    razdelSeo = "Масло BEL-RAY";
                    break;
                case ("/catalog/gsm/maslo-maxima_1/"):
                    razdel = "Запчасти и расходники => ГСМ => Maxima";
                    razdelmini = "Maxima";
                    razdelSeo = "Maxima";
                    break;
                case ("/catalog/aksessuary/servis-i-perevozka/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Сервис и перевозка";
                    razdelmini = "Сервис и перевозка";
                    razdelSeo = "Сервис и перевозка";
                    break;
                case ("/catalog/aksessuary/zashchita-ruk/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Защита рук";
                    razdelmini = "Защита рук";
                    razdelSeo = "Защита рук";
                    break;
                case ("/catalog/aksessuary/schetchiki-motochasov/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Счетчики моточасов";
                    razdelmini = "Счетчики моточасов";
                    razdelSeo = "Счетчики моточасов";
                    break;
                case ("/catalog/aksessuary/aksessuary-dlya-kolesnykh-diskov/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Аксессуары для колесных дисков";
                    razdelmini = "Аксессуары для колесных дисков";
                    razdelSeo = "Аксессуары для колесных дисков";
                    break;
                case ("/catalog/aksessuary/bolty-krepyezh/"):
                    break;
                case ("/catalog/aksessuary/brelki/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Брелки";
                    razdelmini = "Брелки";
                    razdelSeo = "Брелки";
                    break;
                case ("/catalog/aksessuary/gruziki-rulya/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Грузики руля";
                    razdelmini = "Грузики руля";
                    razdelSeo = "Грузики руля";
                    break;
                case ("/catalog/aksessuary/zamki-protivougonnye/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Замки противоугонные";
                    razdelmini = "Замки противоугонные";
                    razdelSeo = "Замки противоугонные";
                    break;
                case ("/catalog/aksessuary/zashchitnye-chekhly/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Защитные чехлы";
                    razdelmini = "Защитные чехлы";
                    razdelSeo = "Защитные чехлы";
                    break;
                case ("/catalog/aksessuary/ramki-dlya-nomera/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Рамки для номера";
                    razdelmini = "Рамки для номера";
                    razdelSeo = "Рамки для номера";
                    break;
                case ("/catalog/aksessuary/setki-bagazhnye/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Сетки багажные";
                    razdelmini = "Сетки багажные";
                    razdelSeo = "Сетки багажные";
                    break;
                case ("/catalog/aksessuary/stekla-vetrovye/"):
                    razdel = "Аксессуары и инструменты => Аксессуары SM-MOTORS => Стекла ветровые";
                    razdelmini = "Стекла ветровые";
                    razdelSeo = "Стекла ветровые";
                    break;
                case ("/catalog/gsm/maslo-twin-air/"):
                    break;
                default:
                    {
                        break;
                    }
                    
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbLoginBike.Text = Properties.Settings.Default.login;
            tbPasswordBike.Text = Properties.Settings.Default.password;
            tbLoginSM.Text = Properties.Settings.Default.loginSM;
            tbPasswordSM.Text = Properties.Settings.Default.passwordSM;
        }

        public CookieContainer LoginSMMOTORS(string login, string password)
        {
            CookieContainer cookie = new CookieContainer();
            login = login.Replace("@", "%40");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://www.sm-motors.ru/bitrix/templates/sm_motors/ajax/connector.php?act=validate&rule=auth");
            req.Accept = "application/json, text/javascript, */*; q=0.01";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36";
            req.Method = "POST";
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] ms = Encoding.ASCII.GetBytes("backurl=%2F&AUTH_FORM=Y&TYPE=AUTH&USER_REMEMBER=Y&USER_LOGIN=" + login + "&USER_PASSWORD=" + password);
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            return cookie;
        }

        private void DeleteTovarsInBike18(CookieDictionary cookie, string url)
        {
            string[] allTovars = File.ReadAllLines("allTovars", Encoding.GetEncoding(1251));
            if (allTovars.Length > 1)
            {
                string otv = null;
                otv = nethouse.getRequest(url);
                MatchCollection product = new Regex("(?<=<div class=\"product-item__content\"><a href=\").*?(?=\")").Matches(otv);
                MatchCollection razdel = new Regex("(?<=<div class=\"category-item__link\"><a href=\").*?(?=\">)").Matches(otv);
                if (razdel.Count == 0)
                {
                    List<string> tovar = new List<string>();
                    if (product.Count != 0)
                    {
                        for (int m = 0; product.Count > m; m++)
                        {
                            tovar = nethouse.GetProductList(cookie, product[m].ToString());
                            if (tovar == null)
                                continue;
                            string article = tovar[6].ToString();

                            if (!article.Contains("SM_"))
                                continue;
                            bool b = false;
                            foreach (string s in allTovars)
                            {
                                if (s == article)
                                    b = true;
                            }
                            if (!b)
                            {
                                nethouse.DeleteProduct(cookie, tovar);
                                delTovar++;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; razdel.Count > i; i++)
                    {
                        url = razdel[i].ToString();
                        otv = nethouse.getRequest(url);
                        product = new Regex("(?<=<a href=\").*(?=\"><div class=\"-relative item-image\")").Matches(otv);
                        List<string> tovar = new List<string>();
                        if (product.Count != 0)
                        {
                            for (int m = 0; product.Count > m; m++)
                            {
                                tovar = nethouse.GetProductList(cookie, product[m].ToString());
                                string article = tovar[6].ToString();

                                if (!article.Contains("SM_"))
                                    continue;
                                bool b = false;
                                foreach (string s in allTovars)
                                {
                                    if (s == article)
                                        b = true;
                                }
                                if (!b)
                                {
                                    nethouse.DeleteProduct(cookie, tovar);
                                    delTovar++;
                                }
                            }
                        }
                    }
                }

            }
        }

        private void btnRazdels_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.login = tbLoginBike.Text;
            Properties.Settings.Default.password = tbPasswordBike.Text;
            Properties.Settings.Default.loginSM = tbLoginSM.Text;
            Properties.Settings.Default.passwordSM = tbPasswordSM.Text;
            Properties.Settings.Default.Save();

            chekedSEO = cbSEO.Checked;
            chekedFullText = cbFullText.Checked;
            chekedMiniText = cbMiniText.Checked;

            string otv = null;
            string loginBike = tbLoginBike.Text;
            string passwordBike = tbPasswordBike.Text;
            string loginSM = tbLoginSM.Text;
            string passwordSM = tbPasswordSM.Text;
            countEditProduct = 0;

            CookieDictionary cookieBike18 = nethouse.CookieNethouse(loginBike, passwordBike);
            CookieContainer cookieSM = LoginSMMOTORS(loginSM, passwordSM);

            if (cookieBike18.Count != 4)
            {
                MessageBox.Show("Логин/пароль для сайта BIKE18.RU введен не верно!");
                return;
            }
            if (cookieSM.Count != 4)
            {
                MessageBox.Show("Логин/пароль для сайта SM-MOTORS введен не верно!");
                return;
            }

            File.Delete("naSite.csv");
            File.Delete("allTovars");
            nethouse.NewListUploadinBike18("naSite");

            FileInfo file = new FileInfo("bike18.xlsx");
            ExcelPackage ep = new ExcelPackage(file);
            w = ep.Workbook.Worksheets[1];
            q = w.Dimension.Rows;

            otv = nethouse.getRequest("https://www.sm-motors.ru/");

            if (otv == "err")
                return;

            MatchCollection urls = new Regex("(?<=<li><a href=\")/catalog.*?(?=\">)").Matches(otv);
            for (int i = 0; urls.Count > i; i++)
            {
                string urlsCategory = urls[i].ToString();

                if (urlsCategory == "/catalog/tyuning-dlya-skuterov/" ||
                    urlsCategory == "/catalog/gsm/" ||
                    urlsCategory == "/catalog/kofry-sumki/" ||
                    urlsCategory == "/catalog/zapchasti/dvigateli/" ||
                    urlsCategory == "/catalog/zapchasti/zapchasti-snegokhody/snowmax-t-200/" ||
                    urlsCategory == "/catalog/zapchasti/zapchasti-snegokhody/buran-rys-tayga/" ||
                    urlsCategory == "/catalog/zapchasti/zapchasti-snegokhody/remni-variatora_1/" ||
                    urlsCategory == "/catalog/pokryshki-kamery/kamery/" ||
                    urlsCategory == "/catalog/pokryshki-kamery/pokryshki-dlya-atv/" ||
                    urlsCategory == "/catalog/pokryshki-kamery/pokryshki-dlya-mototsiklov/" ||
                    urlsCategory == "/catalog/pokryshki-kamery/pokryshki-dlya-skuterov/" ||
                    urlsCategory == "/catalog/akkumulyatory/" ||
                    urlsCategory == "/catalog/zapchasti-lodochnykh-motorov/anodnaya-zashchita/" ||
                    urlsCategory == "/catalog/zapchasti-lodochnykh-motorov/zapchasti-dlya-podvesnykh-lodochnykh-motorov/" ||
                    urlsCategory == "/catalog/zapchasti-lodochnykh-motorov/zapchasti-dlya-statsionarnykh-dvigateley/" ||
                    urlsCategory == "/catalog/zapchasti-lodochnykh-motorov/grebnye-vinty/" ||
                    urlsCategory == "/catalog/zapchasti-lodochnykh-motorov/distantsionnoe-upravlenie/" ||
                    urlsCategory == "/catalog/zapchasti-lodochnykh-motorov/toplivnye-sistema/" ||
                    urlsCategory == "/catalog/zapchasti-lodochnykh-motorov/filtry_1/" ||
                    urlsCategory == "/catalog/zapchasti-lodochnykh-motorov/elektrooborudovanie-i-prinadlezhnosti-/" ||
                    urlsCategory == "/catalog/aksessuary/zashchita-ruk/" ||
                    urlsCategory == "/catalog/aksessuary/schetchiki-motochasov/" ||
                    urlsCategory == "/catalog/aksessuary/aksessuary-dlya-kolesnykh-diskov/" ||
                    urlsCategory == "/catalog/aksessuary/bolty-krepyezh/" ||
                    urlsCategory == "/catalog/aksessuary/brelki/" ||
                    urlsCategory == "/catalog/aksessuary/gruziki-rulya/" ||
                    urlsCategory == "/catalog/aksessuary/zamki-protivougonnye/" ||
                    urlsCategory == "/catalog/aksessuary/zashchitnye-chekhly/" ||
                    urlsCategory == "/catalog/aksessuary/ramki-dlya-nomera/" ||
                    urlsCategory == "/catalog/aksessuary/servis-i-perevozka/" ||
                    urlsCategory == "/catalog/aksessuary/setki-bagazhnye/" ||
                    urlsCategory == "/catalog/aksessuary/stekla-vetrovye/")
                {
                    string pages = "";

                    try
                    {
                        otv = nethouse.getRequest("https://www.sm-motors.ru" + urlsCategory + "?count=60" + pages);
                    }
                    catch
                    {
                        continue;
                    }

                    int maxVal = countPagesSM(otv);

                    for (int x = 0; maxVal >= x; x++)
                    {
                        if (x == 1)
                            pages = "";
                        else
                            pages = "&PAGEN_1=" + x;

                        if (maxVal == 0)
                            pages = "";

                        try
                        {
                            otv = nethouse.getRequest("https://www.sm-motors.ru" + urlsCategory + "?count=60" + pages);
                        }
                        catch
                        {
                            continue;
                        }

                        MatchCollection tovars = new Regex("(?<=<a class=\"image-container\" href=\").*?(?=\" title=\")").Matches(otv);
                        for (int m = 0; tovars.Count > m; m++)
                        {
                            string urlTovar = "https://www.sm-motors.ru" + tovars[m].ToString();
                            if(urlTovar == "https://www.sm-motors.ru/catalog/aksessuary/servis-i-perevozka/podkat-pod-zadniy-mayatnik-sm-parts/")
                            {

                            }
                            AddTovarInCSV(cookieBike18, urlTovar, discountPrice, urlsCategory);
                        }
                        if (x == 0)
                        {
                            x++;
                        }
                    }
                    uploadNewTovar(cookieBike18);
                }
            }

            #region Удаление товаров с сайта байк18 если его нет на сайте см-моторс
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/tuning-dly-skuterov?page=all");    //  Тюнинг для скутеров
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/3022582?page=all");    //  ГСМ
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/2078659?page=all");    //  Кофры и сумки
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/dvigateli?page=all");  //  Двигатели
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/zapchasti-na-snegohody-raznoe?page=all");  //  Запчасти на снегоходы
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/zapchasti-na-snegohod-ris?page=all");  //  Запчасти на снегоходы
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/zapchasti-na-snegohody-buran?page=all");   //    Запчасти на снегоходы
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/snowmax-t200?page=all");   //  Запчасти на снегоход Т200
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/zapchasti-na-snegohody-tayga?page=all");   //  Запчасти на снегоход тайга
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/2514653?page=all");    //  Запчасти для снегоходов - ремни вариатора
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/moto-rezina?page=all"); //  Покрышки и камеры
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/akkumulyatory?page=all");   //  аккумуляторы
            //DeleteTovarsInBike18(cookieBike18, "https://bike18.ru/products/category/2941045");  //  Лодочные моторы с подкаталогами
            #endregion

            MessageBox.Show("Измененные и новые товары находятся в файле nasite.csv в папке с программой\nНеобходимо загрузить данный файл на сайт");
        }
    }
}
//1693
