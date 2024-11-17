![HassClient Logo](https://github.com/vicfergar/HassClient/raw/main/resources/Logo.png)

# HassClient
[![CI](https://github.com/vicfergar/HassClient/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/vicfergar/HassClient/actions/workflows/ci.yml)
[![CD](https://github.com/vicfergar/HassClient/actions/workflows/cd.yml/badge.svg?branch=main)](https://github.com/vicfergar/HassClient/actions/workflows/cd.yml)

[![HassClient.Core](https://img.shields.io/nuget/v/HassClient.Core?style=flat&label=HassClient.Core)](https://www.nuget.org/packages/HassClient.Core)
[![HassClient.WS](https://img.shields.io/nuget/v/HassClient.WS?style=flat&label=HassClient.WS)](https://www.nuget.org/packages/HassClient.WS)

A Home Assistant .NET client using [Web Socket API](https://developers.home-assistant.io/docs/api/websocket).

---

## Sample Usage
`HassWSApi` provides access to Web Socket communication and a multitude of common operations:

### Connection
To start using the Web Socket API client, it should be connected to a Home Assistant instance.

The method `ConnectAsync` with `ConnectionParameters` will be used:

- Using Instance base URL: Connects using the Home Assistant instance **base address** (e.g. "http://localhost:8123") and a **valid token**. You can obtain a token ("Long-Lived Access Token") by logging into the frontend using a web browser, and going [to your profile](https://www.home-assistant.io/docs/authentication/#your-account-profile) `http://IP_ADDRESS:8123/profile`.
```csharp
var hassWSApi = new HassWSApi();
var connectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl("http://localhost:8123", "HASS_TOKEN");
await hassWSApi.ConnectAsync(connectionParameters);
```

- Within an Add-on: Connects to a Home Assistant instance using the [add-ons internal proxy](https://developers.home-assistant.io/docs/add-ons/communication#home-assistant-core).
```csharp
var hassWSApi = new HassWSApi();
var connectionParameters = ConnectionParameters.CreateForAddonConnection();
await hassWSApi.ConnectAsync(connectionParameters);
```

### Services
The `KnownDomains` and `KnownServices` enums are available to reduce the use of strings.
```csharp
// Fetch all available services in the Home Assistant instance.
IEnumerable<ServiceDomain> serviceDomains = await hassWSApi.Services.ListAsync();

// This will call a service from the Home Assistant instance.
await hassWSApi.Services.CallAsync("homeassistant", "check_config");

// Optional data can be passed to the call operation.
await hassWSApi.Services.CallAsync(KnownDomains.Light, KnownServices.TurnOn, data: new { entity_id = "light.my_light", brightness_pct = 20});

// When only entity_id is needed for the invocation this overload can be used.
await hassWSApi.Services.CallForEntitiesAsync(KnownDomains.Light, KnownServices.Toggle, "light.my_light1", "light.my_light2");
```

 ### Fetching states
 ```csharp
# Gets a collection with the state of every registered entity in the Home Assistant instance.
IEnumerable<StateModel> states = await hassWSApi.ListStatesAsync();
```

An event is produced by the Home Assistant instance every time a `change_state` occurs. To simplify the subscription to these events the `StateChangedEventListener` can be used:

```csharp
public void Subscribe()
{
    // Subscribe to changes of a specific entity.
    hassWSApi.StateChangedEventListener.SubscribeEntityStatusChanged("light.my_light1", this.my_light1_StateChanged);
}

private void my_light1_StateChanged(object sender, StateChangedEvent stateChangedArgs)
{
    Debug.WriteLine($"my_light1_StateChanged {stateChangedArgs.EntityId} is now {stateChangedArgs.NewState.State}");
}
```

 ```csharp
public void Subscribe()
{   
    // Subscribe to changes by domain.
    hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged("switch", this.Switch_StateChanged);
}

private void Switch_StateChanged(object sender, StateChangedEvent stateChangedArgs)
{
    Debug.WriteLine($"Switch_StateChanged {stateChangedArgs.EntityId} is now {stateChangedArgs.NewState.State}");
}
```

### Fetching other data
```csharp
// Fetch current Home Assistant instance configuration.
ConfigurationModel config = await hassWSApi.GetConfigurationAsync();

// Fetch current registered panels in Home Assistant instance.
IEnumerable<PanelInfo> panels = await hassWSApi.ListPanelsAsync();
```

### Event subscription
The WS Client provides some methods to subscribe and unsubscribe to Home Assistant events.

Even any `eventType` can be specified as a string. The [KnownEventTypes](https://github.com/vicfergar/HassClient/blob/main/src/HassClient.Core/Models/Events/KnownEventTypes.cs) enum is available to reduce the use of strings.

 ```csharp
public async Task MyMethod()
{
    // Subscribe to every event type
    await hassWSApi.AddEventHandlerSubscriptionAsync(this.WS_OnEvent);

    // Subscribe to specific event type
    await hassWSApi.AddEventHandlerSubscriptionAsync(this.WS_OnEvent, KnownEventTypes.PanelsUpdated);

    // Unsubscribe using the same event type from subscription
    await hassWSApi.RemoveEventHandlerSubscriptionAsync(this.WS_OnEvent, KnownEventTypes.PanelsUpdated);
}

private void WS_OnEvent(object sender, HassEvent hassEvent)
{
    Debug.WriteLine($"WS_OnEvent {hassEvent.EventType} => {hassEvent.Data}");
}
```

### Render Template
Render a Home Assistant template. [See template docs for more information](https://www.home-assistant.io/topics/templating/).
```csharp
string result = await hassWSApi.RenderTemplateAsync("Paulus is at {{ states('sun.sun') }} {{ states('binary_sensor.is_rainy') }}!");
```

### RegistryEntry Collections
Home Assistant defines a `Registry Collection` as a registry of items identified by a unique id. Some common operations like `list`, `create`, `update` and `delete` are exposed through the Web Socket API and can be consumed using this client.

The following registry entries are supported:
- `Area`
- `Category`
- `Device`
- `EntityRegistryEntry`
- `Floor`
- `Label`
- `User`
- `Zone`

```csharp
// List
IEnumerable<Area> areas = await hassWSApi.Areas.ListAsync();

// Create
var hallArea = new Area("Hall");
bool createResult = await hassWSApi.Areas.CreateAsync(hallArea);

// Update
hallArea.Name = "Hall_1";
hallArea.Labels.Add("ground_floor");
bool updateResult = await hassWSApi.Areas.UpdateAsync(hallArea);

// Pending changes
bool hasChanges = hallArea.HasPendingChanges;
hallArea.DiscardPendingChanges();

// Delete
bool deleteResult = await hassWSApi.Areas.DeleteAsync(hallArea);
```

#### Storage Collections
A `Storage Collection` contains a special registry entry type to store entities. The Web Socket API exposes four methods to `list`, `create`, `update` and `delete` these types of entries.

At the moment only `InputBoolean`, `Person`, and `Zone` entity models are defined.

```csharp
// List
IEnumerable<InputBoolean> inputBooleans = await hassWSApi.StorageEntities.ListAsync<InputBoolean>();

// Create
var alarmInputBoolean = new InputBoolean("Alarm");
bool createResult = await hassWSApi.StorageEntities.CreateAsync(alarmInputBoolean);

// Update
alarmInputBoolean.Name = "Alarm_1";
alarmInputBoolean.Icon = "mdi:alarm";
bool updateResult = await hassWSApi.StorageEntities.UpdateAsync(alarmInputBoolean);

// Pending changes
bool hasChanges = alarmInputBoolean.HasPendingChanges;
alarmInputBoolean.DiscardPendingChanges();

// Delete
bool deleteResult = await hassWSApi.StorageEntities.DeleteAsync(alarmInputBoolean);
```

Key features of registry entries:

- **Pending Changes**: All registry entries track their modifications through the `HasPendingChanges` property
- **Force Update**: The update methods support a `forceUpdate` parameter to update all properties, not just the modified ones
- **Discard Changes**: You can revert pending changes using the `DiscardPendingChanges()` method
- **Property Tracking**: Properties like Name, Icon, Aliases, Labels, etc. are automatically tracked for changes


### Search related
All data stored in Home Assistant is interconnected, making it a graph. The client allows searching related items for a given `ItemTypes` and `itemId`.
```csharp
SearchRelatedResponse result = await hassWSApi.SearchRelatedAsync(ItemTypes.Entity, "weather.home");
```

### Raw Commands
Even though many commands are implemented natively by this API client, some may not. For this purpose, a raw command API is available to send custom commands and receive raw results.
```csharp
// Send raw command message with raw result
RawCommandResult rawResult = await hassWSApi.SendRawCommandWithResultAsync(new RawCommandMessage("get_config"));

// Send raw command message with success result
bool success = await hassWSApi.SendRawCommandWithSuccessAsync(new RawCommandMessage("get_config"));
```
