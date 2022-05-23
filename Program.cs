using Speed;
using Speed.Results;

Server server = new();
server.Get("/home", (request, response) =>
{
  return Results.Ok(new {
    name = "Unshift bit",
    time = "late"
  });
});

server.Get("/about", (request, response) => {
  return Results.Ok($"Hello from /about handler");
});

server.Post("/users", (request, response) => {
  return Results.Ok(new { success = true });
});


server.Start();