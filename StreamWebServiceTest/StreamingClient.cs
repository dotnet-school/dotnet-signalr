using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace StreamWebServiceTest
{
  public class StreamingClient<T>
  {
    private HubConnection _connection;
    private ChannelReader<object> _reader;

    public StreamingClient(string url)
    {
      _connection  = new HubConnectionBuilder()
              .WithUrl(url)
              .Build();
    }

    public async Task  StartAsync(string uic, string assetType)
    {
      await _connection.StartAsync();
      _reader = await _connection.StreamAsChannelCoreAsync("Subscribe", typeof(T), new[] {uic, assetType});
    }

    public async Task<T> GetNextMessage()
    {
      var value = await _reader.ReadAsync();
      return (T) value;
    }
  }
}