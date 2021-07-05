using Quartz;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DataCollector.Jobs
{
    public class SendPcInfoJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (ClientWebSocket client = new ClientWebSocket())
            {
                try
                {
                    Uri serviceUri = new Uri($"ws://{Program.ServiceDomain}/send");
                    await client.ConnectAsync(serviceUri, CancellationToken.None);
                    var json = JsonSerializer.Serialize(Program.GetPcInfo());
                    if (!string.IsNullOrEmpty(json))
                    {
                        ArraySegment<byte> byteToSend = new ArraySegment<byte>(
                            Encoding.UTF8.GetBytes(json));
                        await client.SendAsync(
                            byteToSend, 
                            WebSocketMessageType.Text, 
                            true, 
                            CancellationToken.None);
                    }

                    await client.CloseOutputAsync(
                        WebSocketCloseStatus.NormalClosure, 
                        string.Empty, 
                        CancellationToken.None);
                }
                catch (Exception e)
                {
                    await client.CloseOutputAsync(
                        WebSocketCloseStatus.InternalServerError,
                        string.Empty,
                        CancellationToken.None);
                }
            }
        }
    }
}
