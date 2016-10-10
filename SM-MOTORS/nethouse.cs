using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bike18
{
    class nethouse
    {
        public string PostRequest(CookieContainer cookie, string url)
        {
            string otv = null;
            HttpWebResponse res = null;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            try
            {
                res = (HttpWebResponse)req.GetResponse();
                StreamReader ressr = new StreamReader(res.GetResponseStream());
                otv = ressr.ReadToEnd();
            }
            catch (WebException e)
            {
                otv = "";
            }

            return otv;
        }

        public CookieContainer cookieNethouse(string login, string password)
        {
            CookieContainer cookie = new CookieContainer();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://nethouse.ru/signin");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] ms = Encoding.ASCII.GetBytes("login=" + login + "&password=" + password + "&quick_expire=0&submit=%D0%92%D0%BE%D0%B9%D1%82%D0%B8");
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            return cookie;
        }

        internal List<string> getProductList(CookieContainer cookie, string urlTovar)
        {
            List<string> listTovar = new List<string>();
            string otv = PostRequest(cookie, urlTovar);
            if (otv != null)
            {
                string productId = new Regex("(?<=<section class=\"comment\" id=\").*?(?=\">)").Match(otv).ToString();
                String article = new Regex("(?<=Артикул:)[\\w\\W]*(?=</div><div><div class)").Match(otv).Value.Trim();
                if (article.Length > 11)
                {
                    article = new Regex("(?<=Артикул:)[\\w\\W]*(?=</title>)").Match(otv).ToString().Trim();
                }
                String prodName = new Regex("(?<=<h1>).*(?=</h1>)").Match(otv).Value;
                String price = new Regex("(?<=<span class=\"product-price-data\" data-cost=\").*?(?=\">)").Match(otv).Value;
                String imgId = new Regex("(?<=<div id=\"avatar-).*(?=\")").Match(otv).Value;
                String desc = new Regex("(?<=<div class=\"user-inner\">).*?(?=</div>)").Match(otv).Value;
                String fulldesc = new Regex("(?<=<div id=\"product-full-desc\" data-ng-non-bindable class=\"user-inner\">).*?(?=</div>)").Match(otv).Value.Replace("&nbsp;&nbsp;", " ").Replace("&deg;", "°");
                String seometa = new Regex("(?<=<meta name=\"description\" content=\").*?(?=\" >)").Match(otv).Value;
                String keywords = new Regex("(?<=<meta name=\"keywords\" content=\").*?(?=\" >)").Match(otv).Value;
                String title = new Regex("(?<=<title>).*?(?=</title>)").Match(otv).Value;
                String visible = new Regex("(?<=,\"balance\":).*?(?=,\")").Match(otv).Value;
                string reklama = new Regex("(?<=<div class=\"marker-icon size-big type-4\"><div class=\"left\"></div><div class=\"center\"><div class=\"text\">).*?(?=</div></div>)").Match(otv).ToString();
                if (reklama == "акция")
                {
                    reklama = "&markers[3]=1";
                }
                if (reklama == "новинка")
                {
                    reklama = "&markers[1]=1";
                }

                otv = PostRequest(cookie, "http://bike18.nethouse.ru/api/catalog/getproduct?id=" + productId);
                string slug = new Regex("(?<=\",\"slug\":\").*?(?=\")").Match(otv).ToString();
                string balance = new Regex("(?<=,\"balance\":\").*?(?=\",\")").Match(otv).ToString();
                string productCastomGroup = new Regex("(?<=productCustomGroup\":).*?(?=,\")").Match(otv).ToString();
                String discountCoast = new Regex("(?<=discountCost\":\").*?(?=\")").Match(otv).Value;
                String serial = new Regex("(?<=serial\":\").*?(?=\")").Match(otv).Value;
                String categoryId = new Regex("(?<=\",\"categoryId\":\").*?(?=\")").Match(otv).Value;
                String productGroup = new Regex("(?<=productGroup\":).*?(?=,\")").Match(otv).Value;
                String havenDetail = new Regex("(?<=haveDetail\".).*?(?=,\")").Match(otv).Value;
                String canMakeOrder = new Regex("(?<=canMakeOrder\".).*?(?=,\")").Match(otv).Value;
                canMakeOrder = canMakeOrder.Replace("false", "0");
                canMakeOrder = canMakeOrder.Replace("true", "1");
                String showOnMain = new Regex("(?<=showOnMain\".).*?(?=,\")").Match(otv).Value;
                String customDays = new Regex("(?<=,\"customDays\":\").*?(?=\")").Match(otv).Value;
                String isCustom = new Regex("(?<=\",\"isCustom\":).*?(?=,)").Match(otv).Value;
                string atribut = "";
                string atributes = new Regex("(?<=attributes\":{\").*?(?=,\"customDays)").Match(otv).Value;
                MatchCollection stringAtributes = new Regex("(?<=\":{\").*?(?=])").Matches(atributes);
                for (int i = 0; stringAtributes.Count > i; i++)
                {
                    string id = new Regex("(?<=primaryKey\":).*?(?=,\")").Match(stringAtributes[i].ToString()).Value;
                    string valueId = new Regex("(?<=\"valueId\":\").*?(?=\")").Match(stringAtributes[i].ToString()).Value;
                    string valueText = new Regex("(?<=valueText\":).*?(?=})").Match(stringAtributes[i].ToString()).Value;
                    string text = new Regex("(?<=\"text\":).*?(?=})").Match(stringAtributes[i].ToString()).Value;
                    string checkBox = new Regex("(?<=checkbox\":).*?(?=})").Match(stringAtributes[i].ToString()).Value;

                    if (valueId != "")
                    {
                        atribut = atribut + "&attributes[" + i + "][primaryKey]=" + id + "&attributes[" + i + "][attributeId]=" + id + "&attributes[" + i + "][values][0][empty]=0&attributes[" + i + "][values][0][valueId]=" + valueId;
                    }
                    else
                    {
                        if (text != "")
                            atribut = atribut + "&attributes[" + i + "][primaryKey]=" + id + "&attributes[" + i + "][attributeId]=" + id + "&attributes[" + i + "][values][0][empty]=0&attributes[" + i + "][values][0][text]=" + text;
                        if (checkBox != "")
                            atribut = atribut + "&attributes[" + i + "][primaryKey]=" + id + "&attributes[" + i + "][attributeId]=" + id + "&attributes[" + i + "][values][0][empty]=0&attributes[" + i + "][values][0][checkbox]=" + checkBox;
                    }
                }
                atribut = atribut.Replace("true", "1");
                string alsoBuy = new Regex("(?<=alsoBuy\":).*?(?=,\"markers)").Match(otv).ToString();
                alsoBuy = alsoBuy.Remove(alsoBuy.Length - 1, 1).Remove(0, 1);
                string[] alsoBuyArray = alsoBuy.Split(',');
                string alsoBuyStr = "";

                if (alsoBuyArray.Length > 0)
                {
                    for (int i = 0; alsoBuyArray.Length > i; i++)
                    {
                        alsoBuyStr += "&alsoBuy[" + i + "]=" + alsoBuyArray[i].ToString();
                    }
                }

                otv = PostRequest(cookie, "http://bike18.nethouse.ru/api/catalog/productmedia?id=" + productId);
                string avatarId = new Regex("(?<=\"id\":\").*?(?=\")").Match(otv).Value;
                string objektId = new Regex("(?<=\"objectId\":\").*?(?=\")").Match(otv).Value;
                string timestamp = new Regex("(?<=\"timestamp\":\").*?(?=\")").Match(otv).Value;
                string type = new Regex("(?<=\"type\":\").*?(?=\")").Match(otv).Value;
                string name = new Regex("(?<=\",\"name\":\").*?(?=\")").Match(otv).Value;
                string descimg = new Regex("(?<=\"desc\":\").*?(?=\")").Match(otv).Value;
                string ext = new Regex("(?<=\"ext\":\").*?(?=\")").Match(otv).Value;
                string raw = new Regex("(?<=\"raw\":\").*?(?=\")").Match(otv).Value;
                string W215 = new Regex("(?<=\"W215\":\").*?(?=\")").Match(otv).Value;
                string srimg = new Regex("(?<=\"150x120\":\").*?(?=\")").Match(otv).Value;
                string minimg = new Regex("(?<=\"104x82\":\").*?(?=\")").Match(otv).Value;
                string filesize = new Regex("(?<=\"fileSize\":).*?(?=})").Match(otv).Value;
                string alt = new Regex("(?<=\"alt\":\").*?(?=\")").Match(otv).Value;
                string isvisibleonmain = new Regex("(?<=\"isVisibleOnMain\".).*?(?=,)").Match(otv).Value;
                string prioriti = new Regex("(?<=\"priority\":\").*?(?=\")").Match(otv).Value;
                string avatarurl = new Regex("(?<=\"url\":\").*?(?=\")").Match(otv).Value;
                string filtersleft = new Regex("(?<=\"left\":).*?(?=,)").Match(otv).Value;
                string filterstop = new Regex("(?<=\"top\":).*?(?=,)").Match(otv).Value;
                string filtersright = new Regex("(?<=\"right\":).*?(?=,)").Match(otv).Value;
                string filtersbottom = new Regex("(?<=\"bottom\":).*?(?=})").Match(otv).Value;

                listTovar.Add(productId);       //0
                listTovar.Add(slug);            //1
                listTovar.Add(categoryId);      //2
                listTovar.Add(productGroup);    //3
                listTovar.Add(prodName);        //4
                listTovar.Add(serial);          //5
                listTovar.Add(article);         //6
                listTovar.Add(desc);            //7
                listTovar.Add(fulldesc);        //8
                listTovar.Add(price);           //9
                listTovar.Add(discountCoast);   //10
                listTovar.Add(seometa);         //11
                listTovar.Add(keywords);        //12
                listTovar.Add(title);           //13
                listTovar.Add(havenDetail);     //14
                listTovar.Add(canMakeOrder);    //15 купить с сайта в 1 клик
                                                //listTovar.Add(balance);
                listTovar.Add(showOnMain);      //16
                listTovar.Add(avatarId);        //17
                listTovar.Add(objektId);        //18
                listTovar.Add(timestamp);       //19
                listTovar.Add(type);            //20
                listTovar.Add(name);            //21
                listTovar.Add(descimg);         //22
                listTovar.Add(ext);             //23
                listTovar.Add(raw);             //24
                listTovar.Add(W215);            //25
                listTovar.Add(srimg);           //26
                listTovar.Add(minimg);          //27
                listTovar.Add(filesize);        //28
                listTovar.Add(alt);             //29
                listTovar.Add(isvisibleonmain); //30
                listTovar.Add(prioriti);        //31
                listTovar.Add(avatarurl);       //32
                listTovar.Add(filtersleft);     //33
                listTovar.Add(filterstop);      //34
                listTovar.Add(filtersright);    //35
                listTovar.Add(filtersbottom);   //36
                listTovar.Add(customDays);      //37
                listTovar.Add(isCustom);        //38
                listTovar.Add(reklama);         //39
                listTovar.Add(atribut);         //40
                listTovar.Add(productCastomGroup); //41
                listTovar.Add(alsoBuyStr);      //42
                listTovar.Add(balance);         //43
            }
            return listTovar;
        }

        internal void deleteProduct(CookieContainer cookie, List<string> getProduct)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/deleteproduct");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] ms = System.Text.Encoding.GetEncoding("utf-8").GetBytes("id=" + getProduct[0] + "&slug=" + getProduct[1] + "&categoryId=" + getProduct[2] + "&productGroup=" + getProduct[3] + "&name=" + getProduct[4] + "&serial=" + getProduct[5] + "&serialByUser=" + getProduct[6] + "&desc=" + getProduct[7] + "&descFull=" + getProduct[8] + "&cost=" + getProduct[9] + "&discountCost=" + getProduct[10] + "&seoMetaDesc=" + getProduct[11] + "&seoMetaKeywords=" + getProduct[12] + "&seoTitle=" + getProduct[13] + "&haveDetail=" + getProduct[14] + "&canMakeOrder=" + getProduct[15] + "&balance=100&showOnMain=" + getProduct[16] + "&isVisible=1&hasSale=0&avatar[id]=" + getProduct[17] + "&avatar[objectId]=" + getProduct[18] + "&avatar[timestamp]=" + getProduct[19] + "&avatar[type]=" + getProduct[20] + "&avatar[name]=" + getProduct[21] + "&avatar[desc]=" + getProduct[22] + "&avatar[ext]=" + getProduct[23] + "&avatar[formats][raw]=" + getProduct[24] + "&avatar[formats][W215]=" + getProduct[25] + "&avatar[formats][150x120]=" + getProduct[26] + "&avatar[formats][104x82]=" + getProduct[27] + "&avatar[formatParams][raw][fileSize]=" + getProduct[28] + "&avatar[alt]=" + getProduct[29] + "&avatar[isVisibleOnMain]=" + getProduct[30] + "&avatar[priority]=" + getProduct[31] + "&avatar[url]=" + getProduct[32] + "&avatar[filters][crop][left]=" + getProduct[33] + "&avatar[filters][crop][top]=" + getProduct[34] + "&avatar[filters][crop][right]=" + getProduct[35] + "&avatar[filters][crop][bottom]=" + getProduct[36] + "&customDays=" + getProduct[37] + "&isCustom=" + getProduct[38]);
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            HttpWebResponse res1 = (HttpWebResponse)req.GetResponse();
            StreamReader ressr1 = new StreamReader(res1.GetResponseStream());
        }

        internal void deleteProduct(CookieContainer cookie, string url)
        {
            List<string> getProduct = getProductList(cookie, url);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/deleteproduct");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] ms = System.Text.Encoding.GetEncoding("utf-8").GetBytes("id=" + getProduct[0] + "&slug=" + getProduct[1] + "&categoryId=" + getProduct[2] + "&productGroup=" + getProduct[3] + "&name=" + getProduct[4] + "&serial=" + getProduct[5] + "&serialByUser=" + getProduct[6] + "&desc=" + getProduct[7] + "&descFull=" + getProduct[8] + "&cost=" + getProduct[9] + "&discountCost=" + getProduct[10] + "&seoMetaDesc=" + getProduct[11] + "&seoMetaKeywords=" + getProduct[12] + "&seoTitle=" + getProduct[13] + "&haveDetail=" + getProduct[14] + "&canMakeOrder=" + getProduct[15] + "&balance=100&showOnMain=" + getProduct[16] + "&isVisible=1&hasSale=0&avatar[id]=" + getProduct[17] + "&avatar[objectId]=" + getProduct[18] + "&avatar[timestamp]=" + getProduct[19] + "&avatar[type]=" + getProduct[20] + "&avatar[name]=" + getProduct[21] + "&avatar[desc]=" + getProduct[22] + "&avatar[ext]=" + getProduct[23] + "&avatar[formats][raw]=" + getProduct[24] + "&avatar[formats][W215]=" + getProduct[25] + "&avatar[formats][150x120]=" + getProduct[26] + "&avatar[formats][104x82]=" + getProduct[27] + "&avatar[formatParams][raw][fileSize]=" + getProduct[28] + "&avatar[alt]=" + getProduct[29] + "&avatar[isVisibleOnMain]=" + getProduct[30] + "&avatar[priority]=" + getProduct[31] + "&avatar[url]=" + getProduct[32] + "&avatar[filters][crop][left]=" + getProduct[33] + "&avatar[filters][crop][top]=" + getProduct[34] + "&avatar[filters][crop][right]=" + getProduct[35] + "&avatar[filters][crop][bottom]=" + getProduct[36] + "&customDays=" + getProduct[37] + "&isCustom=" + getProduct[38]);
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            HttpWebResponse res1 = (HttpWebResponse)req.GetResponse();
            StreamReader ressr1 = new StreamReader(res1.GetResponseStream());
        }

        internal void TovarWriteInCSV(List<string> newProduct, string nameFile)
        {
            StreamWriter newProductcsv = new StreamWriter(nameFile + ".csv", true, Encoding.GetEncoding("windows-1251"));
            int count = newProduct.Count - 1;
            for (int i = 0; count > i; i++)
            {
                newProductcsv.Write(newProduct[i], Encoding.GetEncoding("windows-1251"));
                newProductcsv.Write(";");
            }
            newProductcsv.Write(newProduct[count], Encoding.GetEncoding("windows-1251"));
            newProductcsv.WriteLine();
            newProductcsv.Close();
        }

        internal string saveTovar(CookieContainer cookie, List<string> getProduct)
        {
            string otv = "";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/saveproduct");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            string descFull = getProduct[8].ToString();
            descFull = descFull.Replace("&laquo;", "«").Replace("&raquo;", "»").Replace("&ndash;", "-");
            getProduct[8] = descFull;
            string request = "id=" + getProduct[0] + "&slug=" + getProduct[1] + "&categoryId=" + getProduct[2] + "&productCustomGroup=" + getProduct[41] + "&productGroup=" + getProduct[3] + "&name=" + getProduct[4] + "&serial=" + getProduct[5] + "&serialByUser=" + getProduct[6] + "&desc=" + getProduct[7] + "&descFull=" + getProduct[8] + "&cost=" + getProduct[9] + "&discountCost=" + getProduct[10] + "&seoMetaDesc=" + getProduct[11] + "&seoMetaKeywords=" + getProduct[12] + "&seoTitle=" + getProduct[13] + "&haveDetail=" + getProduct[14] + "&canMakeOrder=" + getProduct[15] + "&balance=" + getProduct[43] + "&showOnMain=" + getProduct[16] + "&isVisible=1&hasSale=0" + "&customDays=" + getProduct[37] + "&isCustom=" + getProduct[38] + getProduct[39] + getProduct[40] + getProduct[42] + "&alsoBuyLabel=%D0%9F%D0%BE%D1%85%D0%BE%D0%B6%D0%B8%D0%B5%20%D1%82%D0%BE%D0%B2%D0%B0%D1%80%D1%8B%20%D0%B2%20%D0%BD%D0%B0%D1%88%D0%B5%D0%BC%20%D0%BC%D0%B0%D0%B3%D0%B0%D0%B7%D0%B8%D0%BD%D0%B5";
            request = request.Replace("false", "0").Replace("true", "1").Replace("&mdash;", "-").Replace("&laquo;", "\"").Replace("&raquo;", "\"").Replace("&mdash;", "-");

            request = request.Replace("false", "0").Replace("true", "1");
            byte[] ms = System.Text.Encoding.GetEncoding("utf-8").GetBytes(request);
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            try
            {
                HttpWebResponse res1 = (HttpWebResponse)req.GetResponse();
                StreamReader ressr1 = new StreamReader(res1.GetResponseStream());
                otv = ressr1.ReadToEnd();
            }
            catch
            {

            }
            return otv;
        }
    }
}
