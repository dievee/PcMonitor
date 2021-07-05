using Microsoft.AspNetCore.Http;
using PCMonitor.Shared;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrackerService.Services
{
    public class WebSocketService : IWebSocketService
    {
        private readonly IDeviceTrackerService _deviceTrackerService;
        public WebSocketService(IDeviceTrackerService deviceTrackerService)
        {
            this._deviceTrackerService = deviceTrackerService;
        }

        /// <summary>
        /// Receives information from client and sends it to processing
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public async Task Send(HttpContext context, WebSocket webSocket)
        {
            try
            {
                var buffer = new byte[1024 * 4];

                var result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    string msg = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));

                    var pcInfo = Deserializer.DeserializeJson<PcInfo>(msg);

                    await _deviceTrackerService.Start(pcInfo);

                    Console.WriteLine($"client says: {msg}");

                    result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None);
                }

                await webSocket.CloseAsync(
                    result.CloseStatus.Value,
                    result.CloseStatusDescription,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
