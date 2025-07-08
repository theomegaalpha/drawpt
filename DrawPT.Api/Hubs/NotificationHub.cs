using System.Text.Json;
using Azure.Messaging.ServiceBus;
using DrawPT.Common.Interfaces;
using DrawPT.Common.Models.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using DrawPT.Api.Services;

namespace DrawPT.Api.Hubs
{
    [Authorize]
    public partial class NotificationHub : Hub<INotificationClient>
    {
        protected readonly ILogger<NotificationHub> _logger;
        protected readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        protected readonly ICacheService _cache;
        protected readonly ServiceBusClient _serviceBusClient;
        protected readonly TtsService _ttsService;

        public NotificationHub(
            ILogger<NotificationHub> logger,
            ICacheService cacheService,
            ServiceBusClient serviceBusClient,
            TtsService ttsService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
            _cache = cacheService;
            _serviceBusClient = serviceBusClient;
            _ttsService = ttsService;
        }
    }
}
