using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace StreamWebServiceTest
{
    public class UnitTest1
    {

      [Fact]
      public async Task TestClient()
      {
        var url = "http://localhost:5000/subscribe/infoprice";

        var client = new PriceStream(url, "211", "Stock");
        var client2 = new PriceStream(url, "33", "FxSpot");
        
        Assert.Equal("0 : 211-Stock" ,await client.GetNextMessage());
        Assert.Equal("0 : 33-FxSpot" ,await client2.GetNextMessage());
        
        Assert.Equal("1 : 211-Stock", await client.GetNextMessage());
        Assert.Equal("1 : 33-FxSpot" ,await client2.GetNextMessage());
        
        Assert.Equal("2 : 211-Stock", await client.GetNextMessage());
        Assert.Equal("2 : 33-FxSpot" ,await client2.GetNextMessage());
      }

      [Fact]
        public async Task ShouldConnectToAnEndpoint()
        {
          var url = "http://localhost:5000/subscribe/infoprice";
          HubConnection connection  = new HubConnectionBuilder()
                  .WithUrl(url)
                  .Build();

          connection.Closed += async (error) =>
          {
            await Task.Delay(new Random().Next(0,5) * 1000);
            await connection.StartAsync();
          };
          
          await connection.StartAsync();

          connection.On<string, string>("ReceiveMessage", (user, message) =>
          {
            Console.WriteLine($"{user}: {message}");
          });

          var uic = "21";
          var assetType = "Stox";
          string returnType = "";
          ChannelReader<object> reader = await connection.StreamAsChannelCoreAsync("Subscribe", typeof(string), new []{uic, assetType});
          var returned = await reader.ReadAsync();
          var returned2 = await reader.ReadAsync();
          Console.Write(returned);
          Console.Write(returned2);
        }
    }
}
