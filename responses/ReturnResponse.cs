using System;
namespace http_server_demo.responses
{
    public class ReturnResponse
    {

        public string Response()
        {
            return $"<h1>RESPONSE {DateTime.Now}</h1>";
        }

        public string Main()
        {
            return $"<h1>MAIN PAGE</h1>";
        }

        public string Html()
        {
            return $"<h1>Hello from MitkoServer {DateTime.Now}</h1>" +
                $"<form method=post><input name=username placeholder = \"username\" /><input name=password placeholder = \"password\"/>" +
                $"<input name=text placeholder = \"place some text here\"/>" +
                    $"<input type=submit /></form>";
        }

    }
}
