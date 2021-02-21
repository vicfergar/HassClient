![HassClient Logo](https://github.com/vicfergar/HassClient/raw/main/resources/Logo.png)

# HassClient
[![HassClient.Core](https://img.shields.io/nuget/v/HassClient.Core?style=flat&label=HassClient.Core)](https://www.nuget.org/packages/HassClient.Core)
[![HassClient.WS](https://img.shields.io/nuget/v/HassClient.WS?style=flat&label=HassClient.WS)](https://www.nuget.org/packages/HassClient.WS)

A Home Assistant client using [Web Sockect API](https://developers.home-assistant.io/docs/api/websocket).

---

## Sample Usage
`HassWSApi` provides access to Web Socket communication and a multitude of common operations:

### Connection
To start we should connect with the Home Assistant instance:

- As client: Connects using the Home Assistant instance **base address** (e.g. "http://localhost:8123") and a **valid token**. You can obtain a token ("Long-Lived Access Token") by logging into the frontend using a web browser, and going [to your profile](https://www.home-assistant.io/docs/authentication/#your-account-profile) `http://IP_ADDRESS:8123/profile`.
```csharp
var hassWSApi = new HassWSApi();
await hassWSApi.ConnectAsClientAsync("http://localhost:8123", HASS_TOKEN);
```
- From Addon: Connects to a Home Assistant instance using the [add-ons internal proxy] https://developers.home-assistant.io/docs/add-ons/communication#home-assistant-core.
```csharp
var hassWSApi = new HassWSApi();
await hassWSApi.ConnectFromAddonAsync();
```

### Services
```csharp
// Fetch all available services in the Home Assistant instance.
IEnumerable<ServiceDomain> serviceDomains = await hassWSApi.GetServicesAsync("homeassistant", "check_config");

// This will call a service from the Home Assistant instance.
await hassWSApi.CallServiceAsync("homeassistant", "check_config");

// Optional data can be passed to the call operation.
await hassWSApi.CallServiceAsync("light", "turn_on", data: new { entity_id = "light.my_light", brightness_pct = 20});

// When only entity_id is needed for the invocation this overload can be used.
await hassWSApi.CallServiceForEntitiesAsync("light", "turn_on", "light.my_light1", "light.my_light2");
```

 ### Fetching states
 ```csharp
# Gets a collection with the state of every registered entity in the Home Assistant instance.
IEnumerable<StateModel> states = await hassWSApi.GetStatesAsync();
```

An event is produced by the Home Assistant instance every time a `change_state` occurred. To simplify the subscription to these events the `StateChagedEventListener` can be used:

```csharp
// Subscribe to changes of specific entity.
hassWSApi.StateChagedEventListener.SubscribeEntityStatusChanged("light.my_light1", this.my_light1_StateChanged);

[...]

private static void my_light1_StateChanged(object sender, StateChangedEvent stateChangedArgs)
{
    Debug.WriteLine($"my_light1_StateChanged {stateChangedArgs.EntityId} is now {stateChangedArgs.NewState.State}");
}
```

 ```csharp
// Subscribe to changes by domain.
hassWSApi.StateChagedEventListener.SubscribeDomainStatusChanged("switch", this.Switch_StateChanged);

[...]

private static void Switch_StateChanged(object sender, StateChangedEvent stateChangedArgs)
{
    Debug.WriteLine($"Switch_StateChanged {stateChangedArgs.EntityId} is now {stateChangedArgs.NewState.State}");
}
```

### Fetching other data
```csharp
// Fetch current Home Assistant instance configuration.
Configuration config = await hassWSApi.GetConfigurationAsync();

// Fetch current registered panels in Home Assistant instance.
IEnumerable<PanelInfo> panels = await hassWSApi.GetPanelsAsync();
```

### Event subscription
The WS Client provides some methods to subscribe and unsubscribe to Home Assistant events.

Even any `eventType` can be specified as a string. The [KnownEventTypes](https://github.com/vicfergar/HassClient/blob/main/src/HassClient.Core/Models/Events/KnownEventTypes.cs) enum is available to reduce the use of strings.

 ```csharp
// Subscribe to every event type
await hassWSApi.AddEventHandlerSubscriptionAsync(this.WS_OnEvent);

// Subscribe to specific event type
await hassWSApi.AddEventHandlerSubscriptionAsync(this.WS_OnEvent, KnownEventTypes.PanelsUpdated);

// Unsubscribe using the same event type from subscription
await hassWSApi.RemoveEventHandlerSubscriptionAsync(this.WS_OnEvent, KnownEventTypes.PanelsUpdated);

[...]

private static void WS_OnEvent(object sender, EventResultInfo eventResultInfo)
{
    Debug.WriteLine($"WS_OnEvent {eventResultInfo} => {eventResultInfo.Data}");
}
```

### Render Template
Render a Home Assistant template. [See template docs for more information](https://www.home-assistant.io/topics/templating/).
```csharp
string result = await hassWSApi.RenderTemplateAsync("Paulus is at {{ states('sun.sun') }} {{ states('binary_sensor.is_rainy') }}!");
```

### Storage Collections
Home Assistant defines a `Storage Collections` as a registry of items identified by a unique id. Some common operations like `list`, `create`, `update` and `delete` are exposed through the Web Socket API and can be consumed using this client.

```csharp
// List
IEnumerable<Area> areas = await hassWSApi.GetAreasAsync();

// Create
Area hallArea = await this.hassWSApi.CreateAreaAsync($"Hall");

// Update
hallArea.Name = "Hall1";
bool updateResult = await hassWSApi.UpdateArea(hallArea);

// Delete
bool deleteResult = await hassWSApi.DeleteAreaAsync(hallArea);
```

### Search related
All data stored in Home Assistant is interconnected, making it a graph. The client allows to search related items for a given `ItemTypes` and `itemId`.
```csharp
SearchRelatedResponse result = await hassWSApi.SearchRelated(ItemTypes.Entity, "weather.home");
```

### Raw Commands
Even many commands are implemented natively by this API client, some may not. For this purpose, a raw command API is available to send custom commands and receive raw results.
```csharp
// Send raw command message with raw result
RawCommandResult rawResult = await hassWSApi.SendRawCommandWithResultAsync(new RawCommandMessage("get_config"));

// Send raw command message with success result
bool success = await hassWSApi.SendRawCommandWithSuccessAsync(new RawCommandMessage("get_config"));
```
