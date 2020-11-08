Refer : https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-3.1&tabs=visual-studio-code



### Create a webapp 

```
# Creat a webapp for streaming to client
dotnet new webapp -o StreamWebService

cd StreamWebService

# Create dockerignore for the project
dotnet new dockerignore
```



open http://localhost:5000/

![image-20201108122924736](docs/images/image-20201108122924736.png)

```bash
git add --all
git commit -m "Create web app"
```



### Add SignalR to web app

**Add support for SignalR in server app**

In `Startup.cs`, initialize SignalR on server side by adding this to `ConfigureServices`

```diff
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddRazorPages();
+		services.AddSignalR();
	}

```



**Create SignalR Hub**

Create a file `PricingHub.cs`: 

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StreamWebService
{
  public class PricingHub : Hub
  {
    public async Task SendMessage(string user, string message)
    {
      await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
  }
}
```

Add the PricingHub to endoints in `Startup.cs`

```diff
  app.UseEndpoints(endpoints =>
  {
  	endpoints.MapRazorPages();
+  	endpoints.MapHub<PricingHub>("/subcribe/priceinfo");
  });
```





**Add SignalR client side library with libman**

```bash
# Install libman
dotnet tool install -g Microsoft.Web.LibraryManager.Cli

# Add SignalR client using libman
libman install @microsoft/signalr@latest -p unpkg -d wwwroot/js/signalr --files dist/browser/signalr.js --files dist/browser/signalr.min.js
# wwwroot/js/signalr/dist/browser/signalr.js written to disk
# wwwroot/js/signalr/dist/browser/signalr.min.js written to disk
# Installed library "@microsoft/signalr@latest" to "wwwroot/js/signalr"
```

Check files created by libman : 

```bash
git status
#	new file:   libman.json
#	new file:   wwwroot/js/signalr/dist/browser/signalr.js
#	new file:   wwwroot/js/signalr/dist/browser/signalr.min.js
```



### Create html view for client

Change `Pages\Index.cshtml` to : 

```xml
@page
<div class="container">
    <div class="row">&nbsp;</div>
    <div class="row">
        <div class="col-2">User</div>
        <div class="col-4"><input type="text" id="userInput" /></div>
    </div>
    <div class="row">
        <div class="col-2">Message</div>
        <div class="col-4"><input type="text" id="messageInput" /></div>
    </div>
    <div class="row">&nbsp;</div>
    <div class="row">
        <div class="col-6">
            <input type="button" id="sendButton" value="Send Message" />
        </div>
    </div>
</div>
<div class="row">
    <div class="col-12">
        <hr />
    </div>
</div>
<div class="row">
    <div class="col-6">
        <ul id="messagesList"></ul>
    </div>
</div>
<script src="~/js/signalr/dist/browser/signalr.js"></script>
<script src="~/js/pricing.js"></script>
```



### Add javascript for client side 

Creat file `wwwroot/js/pricing.js`

```javascript
// wwwroot/js/pricing.js
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/subcribe/priceinfo").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
```

Update `csproj` : 

```xml
  <ItemGroup>
    <Content Update="wwwroot\js\pricing.js">
      <ExcludeFromSingleFile>true</ExcludeFromSingleFile>
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
  </ItemGroup>
```

