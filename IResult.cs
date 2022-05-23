using System.Net;

namespace Speed;

public interface IResult
{
  public void Serve(HttpListenerRequest request, HttpListenerResponse response);
}