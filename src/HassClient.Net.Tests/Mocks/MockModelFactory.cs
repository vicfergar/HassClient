using Bogus;
using HassClient.Net.Helpers;
using HassClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Net.Tests.Mocks
{
    public static class MockHassModelFactory
    {
        public static readonly Faker<PanelInfo> PanelInfoFaker =
            new Faker<PanelInfo>()
            .RuleFor(x => x.ComponentName, f => f.Commerce.Product())
            .RuleFor(x => x.Icon, f => f.RandomIcon())
            .RuleFor(x => x.RequireAdmin, f => f.Random.Bool())
            .RuleFor(x => x.Title, (f, x) => x.ComponentName)
            .RuleFor(x => x.UrlPath, (f, x) => x.ComponentName);

        public static readonly Faker<EntitySource> EntitySourceFaker =
            new Faker<EntitySource>()
            .RuleFor(x => x.ConfigEntry, f => f.RandomUUID())
            .RuleFor(x => x.EntityId, f => f.RandomEntityId())
            .RuleFor(x => x.Domain, (f, x) => x.EntityId.GetDomain())
            .RuleFor(x => x.Source, "config_entry");

        public static readonly Faker<UnitSystemModel> UnitSystemFaker =
            new Faker<UnitSystemModel>()
            .RuleFor(x => x.Length, f => f.PickRandom("km", "mi"))
            .RuleFor(x => x.Mass, f => f.PickRandom("g", "lb"))
            .RuleFor(x => x.Pressure, f => f.PickRandom("Pa", "psi"))
            .RuleFor(x => x.Temperature, f => f.PickRandom("°C", "°F"))
            .RuleFor(x => x.Volume, f => f.PickRandom("L", "gal"));

        public static readonly Faker<Configuration> ConfigurationFaker =
            new Faker<Configuration>()
            .RuleFor(x => x.AllowedExternalDirs, f => f.Make(3, () => f.System.DirectoryPath()))
            .RuleFor(x => x.AllowedExternalUrls, f => f.Make(3, () => f.Internet.Url()))
            .RuleFor(x => x.Components, f => f.Make(10, () => f.RandomDomain()).Distinct().ToList())
            .RuleFor(x => x.ConfigDirectory, "/config")
            .RuleFor(x => x.ConfigSource, "storage")
            .RuleFor(x => x.Elevation, f => f.Random.Int(min: 0))
            .RuleFor(x => x.ExternalUrl, f => f.Internet.Url())
            .RuleFor(x => x.InternalUrl, f => f.Internet.Url())
            .RuleFor(x => x.Latitude, f => (float)f.Address.Latitude())
            .RuleFor(x => x.Longitude, f => (float)f.Address.Longitude())
            .RuleFor(x => x.LocationName, f => "Fake Home")
            .RuleFor(x => x.SafeMode, f => f.Random.Bool())
            .RuleFor(x => x.State, "RUNNING")
            .RuleFor(x => x.TimeZone, f => f.Date.TimeZoneString())
            .RuleFor(x => x.UnitSystem, UnitSystemFaker.Generate())
            .RuleFor(x => x.Version, Version.Parse("0.115.3"));

        public static readonly Faker<Context> ContextFaker =
            new Faker<Context>()
            .RuleFor(x => x.Id, f => f.RandomUUID())
            .RuleFor(x => x.ParentId, f => f.RandomUUID())
            .RuleFor(x => x.UserId, f => f.RandomUUID());

        public static readonly Faker<StateModel> StateModelFaker =
            new Faker<StateModel>()
            .RuleForType(typeof(DateTimeOffset), f => DateTimeOffset.Now)
            .RuleFor(x => x.EntityId, f => f.RandomEntityId())
            .RuleFor(x => x.Context, f => ContextFaker.Generate())
            .RuleFor(x => x.State, f => f.RandomEntityState())
            .RuleFor(x => x.Attributes, (f, x) => new Dictionary<string, object>() { { "friendly_name", x.EntityId.SplitEntityId()[1] } });

        public static readonly Faker<StateChangedEvent> StateChangedEventFaker =
            new Faker<StateChangedEvent>()
            .RuleFor(x => x.EntityId, f => f.RandomEntityId())
            .RuleFor(x => x.OldState, (f, x) => StateModelFaker.GenerateWith(x.EntityId, null))
            .RuleFor(x => x.NewState, (f, x) => StateModelFaker.GenerateWith(x.EntityId, x.OldState.Context));

        public static IEnumerable<EntitySource> GenerateWithEntityIds(this Faker<EntitySource> faker, params string[] entityIds)
        {
            foreach (var item in entityIds)
            {
                yield return faker.RuleFor(x => x.EntityId, f => item)
                                  .Generate();
            }
        }

        public static StateModel GenerateWith(this Faker<StateModel> faker, string entityId, Context context) =>
            faker.RuleFor(x => x.EntityId, f => entityId ?? f.RandomEntityId())
                 .RuleFor(x => x.Context, f => context ?? ContextFaker.Generate())
                 .Generate();

        public static StateChangedEvent GenerateWithEntityId(this Faker<StateChangedEvent> faker, string entityId) =>
            faker.RuleFor(x => x.EntityId, f => entityId ?? f.RandomEntityId())
                 .Generate();

        public static string RandomUUID(this Faker faker) => faker.Random.AlphaNumeric(32);

        public static string RandomDomain(this Faker faker) => faker.PickRandom(
            "alarm_control_panel",
            "alert",
            "alexa",
            "automation",
            "binary_sensor",
            "camera",
            "climate",
            "device_tracker",
            "fan",
            "group",
            "image_processing",
            "input_boolean",
            "input_select",
            "light",
            "media_player",
            "person",
            "remote",
            "script",
            "sensor",
            "sun",
            "switch",
            "timer");

        public static string RandomEntityId(this Faker faker) => $"{faker.RandomDomain()}.{faker.Commerce.Product()}";

        public static string RandomEntityState(this Faker faker) => faker.PickRandom(
            "on",
            "off",
            "unavailable",
            "idle",
            "open",
            "closed",
            "home",
            "unknown");

        public static string RandomIcon(this Faker faker) => faker.PickRandom(
            "mdi: lightbulb",
            "mdi: lightbulb-outline",
            "mdi: light-switch",
            "mdi: ceiling-light",
            "mdi: floor-lamp",
            "mdi: flash",
            "mdi: fan",
            "mdi: radiator",
            "mdi: cast",
            "mdi: account");

        public static IDictionary<TKey, TValue> ToDistinctDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        {
            return source.GroupBy(x => keySelector(x))
                         .Select(x => x.First())
                         .ToDictionary(x => keySelector(x));
        }

        public static IEnumerable<T> Generate<T>(this Faker faker, int count, Func<Faker, T> generator)
        {
            for (int i = 0; i < count; i++)
            {
                yield return generator(faker);
            }
        }
    }
}
