using System;
using System.Linq;
using System.Net;
using System.Text;
using Autodesk.Revit.UI;

namespace RevitRemoteControl
{
    public class RevitHttpListener
    {
        private readonly HttpListener _listener;
        private readonly RequestHandler _requestHandler;

        public RevitHttpListener(
            RequestHandler requestHandler)
        {
            if (!HttpListener.IsSupported)
            {
                throw new InvalidOperationException(
                    "Unsupported operating system!");
            }

            var url = "http://127.0.0.1";
            string port = "8080";
            string prefix = $"{url}:{port}/";
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _requestHandler = requestHandler;
        }

        public async void Start()
        {
            if (_listener.IsListening)
            {
                return;
            }

            try
            {
                _listener.Start();
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.Message);
                return;
            }

            while (true)
            {
                try
                {
                    var context = await _listener
                        .GetContextAsync();

                    var request = context
                        .Request
                        .RawUrl
                        .Replace("/", string.Empty)
                        .ToUpper();

                    var response = context.Response;

                    if (request == "STOP")
                    {
                        Stop();
                        break;
                    }

                    if (request.Contains("FAVICON.ICO"))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;

                        response.OutputStream.Close();

                        continue;
                    }

                    var result = await _requestHandler
                        .HandleAsync(request);

                    var list = string.Join("", result
                        .Select((x, i) => "<p>" + (i + 1) + ". " + x + "</p>"));

                    var responseString =
                        $"<HTML><BODY>" +
                        list +
                        $"</BODY></HTML>";

                    var buffer = Encoding.UTF8
                        .GetBytes(responseString);

                    response.ContentLength64 = buffer.Length;

                    response.OutputStream.Write(buffer, 0, buffer.Length);

                    response.OutputStream.Close();
                }
                catch (HttpListenerException)
                { }
            }
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
