using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace StreamWebServiceTest
{
  public class PriceStream<T>
  {
    private HubConnection _connection;
    private ChannelReader<object> _reader;

    public PriceStream(string url, string uic, string assetType)
    {
      _connection  = new HubConnectionBuilder()
              .WithUrl(url)
              .Build();
      _connection.StartAsync().GetAwaiter().GetResult();
      _reader  = _connection.StreamAsChannelCoreAsync("Subscribe", typeof(T), new[] {uic, assetType}).GetAwaiter().GetResult();
    }

    public async Task<T> GetNextMessage()
    {
      var value = await _reader.ReadAsync();
      return (T) value;
    }
  }
}