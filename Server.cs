using System.Net;
using System.Collections.Generic;

namespace Speed;

public class Server
{
  private HttpListener listener;
  private Dictionary<string, Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, IResult>>> routes;

  const string GET = "GET";
  const string POST= "POST";
  const string PUT = "PUT";
  const string PATCH= "PATCH";
  const string DELETE = "DELETE";
  public Server()
  {
    listener = new HttpListener();
    routes = new();
    routes.Add(GET, new());
    routes.Add(POST, new());
    routes.Add(PUT, new());
    routes.Add(PATCH, new());
    routes.Add(DELETE, new());
  }

  public void Get(string path, Func<HttpListenerRequest, HttpListenerResponse, IResult> handler)
  {
    routes[GET].Add(path, handler);
  }

  public void Post(string path, Func<HttpListenerRequest, HttpListenerResponse, IResult> handler)
  {
    routes[POST].Add(path, handler);
  }

  public void Put(string path, Func<HttpListenerRequest, HttpListenerResponse, IResult> handler)
  {
    routes[PUT].Add(path, handler);
  }

  public void Patch(string path, Func<HttpListenerRequest, HttpListenerResponse, IResult> handler)
  {
    routes[PATCH].Add(path, handler);
  }

  public void Delete(string path, Func<HttpListenerRequest, HttpListenerResponse, IResult> handler)
  {
    routes[DELETE].Add(path, handler);
  }

  public void Start()
  {
    listener.Prefixes.Add("http://localhost:8080/");

    while (true)
    {
      listener.Start();

      IAsyncResult result = listener.BeginGetContext(new AsyncCallback((IAsyncResult result) =>
       {
         HttpListenerContext context = listener.EndGetContext(result);
         HttpListenerRequest request = context.Request;
         HttpListenerResponse response = context.Response;


         Console.WriteLine(request.HttpMethod);

         string route = request.RawUrl ?? "/";
         Func<HttpListenerRequest, HttpListenerResponse, IResult>? handler;

         if(routes[request.HttpMethod].TryGetValue(route, out handler)) {
           handler(request, response).Serve(request, response);
         } else {
           Console.WriteLine("Handler not found");
           // 404 time
           string responseString = $"Page not found: {route}";
           byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
          // Get a response stream and write the response to it.
           response.ContentLength64 = buffer.Length;
           response.StatusCode = 404;
            System.IO.Stream output = response.OutputStream;
           output.Write(buffer, 0, buffer.Length);
           output.Close();
         }

       }), listener);

      result.AsyncWaitHandle.WaitOne();

    }

  }

}
