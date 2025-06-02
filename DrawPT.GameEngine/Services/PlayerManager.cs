using System.Collections.Concurrent;
using System.Threading.Channels;
using DrawPT.GameEngine.Interfaces;
using DrawPT.GameEngine.Models;

namespace DrawPT.GameEngine.Services;

public class PlayerManager : IPlayerManager
{
    private readonly Channel<int> _playerSlots;

    public PlayerManager()
    {
        _playerSlots = Channel.CreateBounded<int>(8);

        // Initialize player slots
        for (int i = 0; i < 8; i++)
            _playerSlots.Writer.TryWrite(0);
    }

    public async Task<bool> AddPlayerAsync(string connectionId, Player player)
    {
        return false;
    }

    public async Task RemovePlayerAsync(string connectionId)
    {
    }

    public Task<bool> IsRoomFullAsync()
    {
        return Task.FromResult(_playerSlots.Reader.TryRead(out _));
    }

    public async Task BroadcastPlayerUpdateAsync(Player player, PlayerUpdateType updateType)
    {
        //TODO:  implement
    }

    public Task<IEnumerable<PlayerState>> GetPlayersAsync()
    {
        var test = new List<PlayerState>();
        return Task.FromResult<IEnumerable<PlayerState>>(test);
    }
}