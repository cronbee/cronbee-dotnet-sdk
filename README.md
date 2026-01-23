# Cronbee-SDK

Cronbee SDK is a lightweight C# client designed to interact with the Cronbee monitoring API.
It allows you to initialize a monitor session, send pings, trigger events, and properly close sessions, with strong testability and clear error handling.


## ✨ Features

🚀 Simple static SDK API (Init, Ping, Event, End)
🔁 Multi-session usage

## 📦 Installation
Execute the following command:  
`dotnet add package Cronbee-SDK`

## 🚀 Basic Usage
### Initialize the SDK
```csharp
CronbeeSDK.Init("My-Monitor-ID");
```
This **MUST** be called before doing any event. An exception of type `CronbeeException` is thrown if SDK is not initialized.  

### Ping monitor
```csharp
CronbeeSDK.Ping();
```

### Trigger an event
```csharp
CronbeeSDK.Event("My-Custom-Event");
```

### End session
```csharp
CronbeeSDK.End();
```


## 🔁 Multi-Session usage
Cronbee-SDK support multi-session usage  
Create sessions by calling
```csharp
CronbeeSession firstSession = CornbeeSDK.CreateCronbeeSession("My-Monitor-ID");
CronbeeSession SecondSession = CornbeeSDK.CreateCronbeeSession("My-Second-Monitor-ID");
```

Any of the static calls can be made from a `CronbeeSession` instance
```csharp
firstSession.Ping();
...
secondSession.Event("My-Event");
```

All the session need to be close individualy:
```csharp
firstSession.Ping();
secondSession.Event("My-Event");
...
firstSession.End();
...
secondSession.End();
```