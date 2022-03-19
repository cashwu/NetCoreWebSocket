using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace testWebSocket.Controllers;

public class WebSocketController : Controller
{
    private readonly ILogger<WebSocketContext> _logger;

    public WebSocketController(ILogger<WebSocketContext> logger)
    {
        _logger = logger;
    }

    [HttpGet("/test-ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task Echo(WebSocket webSocket)
    {
        _logger.LogInformation($"connect - state : {webSocket.State}");

        if (webSocket.State == WebSocketState.Open)
        {
            var buff = Encoding.UTF8.GetBytes($"{DateTime.Now:MM-dd HH:mm:ss}\t Hello World !!");
            var data = new ArraySegment<byte>(buff, 0, buff.Length);

            await webSocket.SendAsync(data,
                                      WebSocketMessageType.Text,
                                      true,
                                      CancellationToken.None);
        }

        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            var cmd = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

            if (!string.IsNullOrEmpty(cmd))
            {
                _logger.LogInformation($"received command  - {cmd}");

                //if (cmd.StartsWith("/USER "))
                {
                    //userName = cmd.Substring(6);
                    // Broadcast($"{userName}:\t{cmd}");
                    var buff = Encoding.UTF8.GetBytes($"{DateTime.Now:MM-dd HH:mm:ss}\t{cmd}");
                    var data = new ArraySegment<byte>(buff, 0, buff.Length);

                    // Parallel.ForEach(WebSockets.Values, async (webSocket) =>
                    // {
                    //     if (webSocket.State == WebSocketState.Open)
                    //         await webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                    // }); 
                    if (webSocket.State == WebSocketState.Open)
                    {
                        await webSocket.SendAsync(data,
                                                  WebSocketMessageType.Text,
                                                  true,
                                                  CancellationToken.None);
                    }
                }
            }

            // await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveResult.Count),
            //                           receiveResult.MessageType,
            //                           receiveResult.EndOfMessage,
            //                           CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        _logger.LogInformation($"close - {receiveResult.CloseStatus.Value} , {receiveResult.CloseStatusDescription}");

        await webSocket.CloseAsync(receiveResult.CloseStatus.Value,
                                   receiveResult.CloseStatusDescription,
                                   CancellationToken.None);
    }
}