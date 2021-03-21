using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace StreamWebServiceTest
{
  public class PriceStream
  {
    private HubConnection _connection;
    private ChannelReader<object> _reader;

    public PriceStream(string url, string uic, string assetType)
    {
      _connection  = new HubConnectionBuilder()
              .WithUrl(url)
              .Build();
      _connection.StartAsync().GetAwaiter().GetResult();
      _reader  = _connection.StreamAsChannelCoreAsync("SubscribeTyped", typeof(PriceUpdate), new[] {uic, assetType}).GetAwaiter().GetResult();
    }

    public async Task<string> GetNextMessage()
    {
      var value = await _reader.ReadAsync();
      var update = (PriceUpdate) value;
      return update.Message;
    }
    
    public class PriceUpdate
    {
      public string Message { get; init; }
      public int UpdateId { get; init; }
    }
  }
}