using System.Net;
using System.Text.Json;

namespace Speed.Results;

public class Results 
{
  public static IResult Ok<T>(T data)
  {
    return new TheResult<T>(data);
  }

  public static IResult NotFound<T>(T data)
  {
    return new TheResult<T>(data, 404);
  }
}

internal class TheResult<T> : IResult {
  private T data;
  private int status = 200;
  private string contentType = "text/html";

  public TheResult(T data, int status = 200, string contentType = "text/html") {
    this.data = data;
    this.status = status;
    this.contentType = contentType;
  }

  public void Serve(HttpListenerRequest request, HttpListenerResponse response)
  {
    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(convert(data));
    response.ContentLength64 = buffer.Length;
    response.StatusCode = status;
    response.ContentType = this.contentType;

    System.IO.Stream output = response.OutputStream;
    output.Write(buffer, 0, buffer.Length);
    output.Close();
  }
  

  private string convert(T data)
  {
    if(data != null && data.GetType().Name == "String")
    {
      return data.ToString() ?? "";
    }

    this.contentType = "application/json";
    return JsonSerializer.Serialize(data);

  }
}