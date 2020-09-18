using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using http_server_demo.responses;
using Microsoft.VisualBasic.CompilerServices;

namespace http_server_demo
{
    class Program
    {
        const string addLine = "\r\n";
        static async Task Main(string[] args)
        {

            //await ReadData();

            //codding HTTP Server

            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 12345);

            tcpListener.Start();

            var resp = new ReturnResponse();

            StringBuilder chat = new StringBuilder();


            //deamon, service

            while (true)
            {
                var client = tcpListener.AcceptTcpClient();

                using var stream = client.GetStream();

                byte[] buffer = new byte[10000000];

                var lenght = stream.Read(buffer, 0, buffer.Length);

                string requestString = Encoding.UTF8.GetString(buffer, 0, lenght);

                //word processing for "tweeter" kind of screen presentation
                if (requestString.Contains("username"))
                {
                    //find index of username
                    var index = requestString.IndexOf("username");
                    //find index of password - start and end
                    var startPasswordIndex = requestString.IndexOf("password=");
                    var endPasswordIndex = requestString.IndexOf("&text%2F");
                    //setting text that needed to be replaced
                    var textToReplace = requestString.Substring(index);
                    //setting password
                    var password = requestString.Substring(startPasswordIndex, endPasswordIndex - startPasswordIndex);
                    //setting string to be replaced. This is needed to hide password from correspondece
                    var stringToReplace = $"&{password}&text%2F=";

                    StringBuilder builder = new StringBuilder(textToReplace);
                    builder.Replace("username=", "User: ");//replace username with exchanged visualisation as User
                    builder.Replace(stringToReplace, " Message: "); //replace password, just to hide it, and exchange visualisation as Message
                    builder.Replace("+", " "); //replaces "+" with space

                    chat.AppendLine($"<p>{builder.ToString().TrimEnd()}</p>");
                }

                //different "pages" handling
                //set regex patern
                string pattern = @"\/[a-z]+";

                var match = Regex.Match(requestString, pattern).ToString();

                //set pages handler as variable
                string value = match.Length > 0 ? match.Substring(match.Length - (match.Length - 1)) : match;

                //set empty response variable
                string response = string.Empty;

                //processing pages
                if (value == "main") response = ReturnString(resp.Main());
                else if (value == "response") response = ReturnString(resp.Response());
                else if (value == "html") response = ReturnString(resp.Html());
                else response = ReturnString(resp.Html());

                byte[] responseBytes = Encoding.UTF8.GetBytes(response + " " + chat);

                stream.Write(responseBytes);

                Console.WriteLine(new string('*', 30));
            }
        }

        //reusable string returning server response
        public static string ReturnString(string variable)
        {
            return "HTTP/1.1 200 OK" + addLine +
                    "Server: MitkoServer 2020" + addLine +
                    "Content-Type: text/html; charset =utf-8" + addLine +
                    "Content-Lenght: " + variable.Length + addLine +
                    addLine +
                    variable + addLine;
        }

        public static async Task ReadData()
        {
            string url = "https://softuni.bg/trainings/3164/csharp-web-basics-september-2020#lesson-18198";

            HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync(url);

            Console.WriteLine(response.StatusCode);
            Console.WriteLine(string.Join(Environment.NewLine, response.Headers.Select(h => h.Key + " : " + h.Value.First())));

            //var html = await httpClient.GetStringAsync(url);

            //Console.WriteLine(html);
        }
    }
}
