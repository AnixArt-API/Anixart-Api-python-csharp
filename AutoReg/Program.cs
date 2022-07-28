using AE.Net.Mail;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AutoReg
{
    class Program
    {
        private static string path = Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                try
                {
                    tokens = File.ReadAllLines("tokens.txt").ToList();
                }
                catch
                {

                }

                Console.WriteLine("Начинаем?!");

                Console.ReadLine();
                while (true)
                {
                    RegistrAccount(RandomString(random.Next(10, 18)), GetEmail());
                    ///Thread.Sleep(365900/3);
                    Thread.Sleep(3500);
                }
            }
            else
            {

            }
        }
        static int indexProxy = 0;
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray()).ToLower();
        }
        static string url = "https://api.anixart.tv/";
        static RestClient client = new RestClient(url);
  
        static void RegistrAccount(string name, string email)
        {
            try
            {
                Console.WriteLine(name + " " + email);


                string currentNick = name;
                string currentEmail = email;
                string pass = RandomString(random.Next(5, 10));
 
                string str = client.Post(new RestRequest("auth/signUp", Method.Post).AddParameter("login",currentNick).AddParameter("email",currentEmail).AddParameter("password",pass)).Content;
                var json = JObject.Parse(str);
                string hash = json["hash"].ToString();
                Console.WriteLine(json.ToString());
                if(json["hash"].ToString().Trim() == "") { Console.WriteLine("Bad");  return; }
                Thread.Sleep(4000);
                string code = GetCode().Trim();
  
                Console.WriteLine(code);


                RestRequest request = new RestRequest($"auth/verify", Method.Post);
                request.AddParameter("login", currentNick);
                request.AddParameter("email", currentEmail);
                request.AddParameter("hash", hash);
                request.AddParameter("password", pass);
                request.AddParameter("code", code);

                //Заходим на профиль (Чтобы не банило аккаунт)
                var res = client.Post(request);
                Console.WriteLine(res.Content);
                json = JObject.Parse(res.Content);
                tokens.Add(json["profileToken"]["token"].ToString()+$":{currentNick}:{currentEmail}:{pass}");
                File.WriteAllLines("tokens.txt", tokens);
                Thread.Sleep(500);
                request = new RestRequest($"/profile/{json["profile"]["id"].ToString()}?token={json["profileToken"]["token"].ToString()}", Method.Get);
                Console.WriteLine(client.Get(request).Content);

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            indexProxy++;

        }
        static List<string> tokens = new List<string>();
        static Random random = new Random();
        static string GetCode()
        {

            string[] emails = File.ReadAllLines("emails.txt");
            string[] currentEmail = emails[indexEmail].Split(':');
            Console.WriteLine(currentEmail[0]);
            Console.WriteLine(currentEmail[2]);
            ImapClient client = new ImapClient("imap.mail.ru", currentEmail[0], currentEmail[2], AuthMethods.Login, 993, true);
            string text = client.GetMessages(0, client.GetMessageCount(), false).Last().Body;

            text = StripHTML(text).Trim();
            string res = "";
            for (int i = 0; i < 4; i++)
            {
                res += text[i].ToString();
            }
     

            return res.Trim();

        }
        static string GetCode(string login, string pass)
        {


            ImapClient client = new ImapClient("imap.mail.ru", login, pass, AuthMethods.Login, 993, true);
            string text = client.GetMessages(0, client.GetMessageCount(), false).Last().Body;

            text = StripHTML(text).Trim();
            string res = "";
            for (int i = 0; i < 4; i++)
            {
                res += text[i].ToString();
            }
            /*
            var json = JObject.Parse(GET("https://10minutemail.net/address.api.php?sessionid=kcdfkf3pt9qho48tfpu281n7kb&_=1658920835268"));
            string mailId = json["mail_list"][0]["mail_id"].ToString();
            Thread.Sleep(3200);
            json = JObject.Parse(GET($"https://10minutemail.net/mail.api.php?mailid={mailId}&sessionid=kcdfkf3pt9qho48tfpu281n7kb"));

            string html = json["html"][0].ToString();
            string text = StripHTML(html).Trim();
            string res = "";
            for(int i = 0; i <4; i++)
            {
                res += text[i].ToString();
            }
           */

            return res.Trim();

        }

        static string UnicodeToUTF8(string from)
        {
            var bytes = Encoding.UTF8.GetBytes(from);
            return new string(bytes.Select(b => (char)b).ToArray());
        }
        public static string StripHTML(string input)
        {
            string g = Regex.Replace(input, "<.*?>", String.Empty);
            g = Regex.Replace(g, "{.*?}", String.Empty);
            g = g.Substring(g.IndexOf(".es-button  a[x-apple-data-detectors]  .es-desk-hidden") + ".es-button  a[x-apple-data-detectors]  .es-desk-hidden".Length);
            return g;
        }
        static int indexEmail = 0;
        static string GetEmail()
        {
            string[] emails = File.ReadAllLines("emails.txt");
           
            //string json = GET("https://10minutemail.net/address.api.php?new=1&sessionid=kcdfkf3pt9qho48tfpu281n7kb&_=1658920835268");
            //var j = JObject.Parse(json);
            //email = j["mail_get_mail"].ToString();
            if (indexEmail < emails.Length - 1)
            {
                indexEmail++;
            }
            else
            {
                indexEmail = 0;
            }
            return emails[indexEmail].Split(':')[0];
        }

        static string GET(string url)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(url);
        }

    }
}
