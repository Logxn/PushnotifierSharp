# Deprecated
This package will no longer be updated.<br>
Please visit the official package here.<br><br>

<a href="https://github.com/tomg404/pushnotifier-python">pushnotifier-python by tomg404</a>

# PushnotifierSharp
A small C# API to interact with pushnotifier.de 's service

# Usage
```csharp
using PushNotfierSharp;
class Program
{
  static void Main(string[] args)
  {
    PushNotifierSharp notifier = new PushNotifierSharp("USERNAME", "PASSWORD", "API-KEY", "PACKAGE-NAME");

    var loginResponse = notifier.Login();
    var devices = notifier.GetAllDevices();
    notifier.SendText("Text only - above URL only", devices.First().Id, false);
    notifier.SendURL("http://www.google.com", devices.First().Id, false);
    notifier.SendNotification("This is a text notification that redirects to a URL", "http://www.google.com", devices.First().Id, false);

    while (true) ;
   }
}
```
