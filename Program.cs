using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Reflection;

namespace HttpListenerExample
{
    class HttpServer
    {
        private static string url = "http://*:80/";
        private static string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static HttpListener listener;
        private static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                try
                {
                    if (req.HttpMethod == "GET")
                    {
                        string path = ("html/" + req.Url.AbsolutePath[1..]).Replace('/', Path.DirectorySeparatorChar);
                        if (File.Exists(path))
                        {
                            byte[] buffer = File.ReadAllBytes(path);
                            resp.ContentLength64 = buffer.Length;
                            Stream output = resp.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Close();
                        }
                        else if (File.Exists(path + Path.DirectorySeparatorChar + "index.html"))
                        {
                            byte[] buffer = File.ReadAllBytes(path + Path.DirectorySeparatorChar + "index.html");
                            resp.ContentLength64 = buffer.Length;
                            Stream output = resp.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Close();
                        }
                        else if (File.Exists("404.html"))
                        {
                            byte[] buffer = File.ReadAllBytes("404.html");
                            resp.ContentLength64 = buffer.Length;
                            Stream output = resp.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Close();
                        }
                        else
                        {
                            byte[] buffer = File.ReadAllBytes("Page not found");
                            resp.ContentLength64 = buffer.Length;
                            Stream output = resp.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Close();
                        }
                    }
                }
                catch
                {
                    byte[] buffer = Encoding.UTF8.GetBytes("ERROR");
                    resp.ContentLength64 = buffer.Length;
                    Stream output = resp.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }
        public static void Main(string[] args)
        {
            Directory.CreateDirectory("html");
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Listening for connections on {0}", url);
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();
            listener.Close();
        }
    }
}