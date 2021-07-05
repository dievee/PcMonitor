using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace TrackerService.Services
{
    public interface IWebSocketService
    {
        Task Send(HttpContext context, WebSocket webSocket);
    }
}
