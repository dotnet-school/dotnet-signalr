using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StreamWebService
{
  public class PricingHub : Hub
  {
    
    public async IAsyncEnumerable<string> Subscribe(
            string uic,
            string assetType,
            [EnumeratorCancellation]
            CancellationToken cancellationToken)
    {
      Console.WriteLine($"Starting to stream for {uic}-{assetType}");

      for (var i = 0; i < 10; i++)
      {
        // Check the cancellation token regularly so that the server will stop
        // producing items if the client disconnects.
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine("Sending message");

        var message = $"{i} : {uic}-{assetType}";
        yield return message;
        Console.WriteLine($"Send : {message}");

        // Use the cancellationToken in other APIs that accept cancellation
        // tokens so the cancellation can flow down to them.
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
      }
    }
  }
}