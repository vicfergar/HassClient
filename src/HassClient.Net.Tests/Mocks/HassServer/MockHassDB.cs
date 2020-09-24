using HassClient.Net.Helpers;
using HassClient.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class MockHassDB
    {
        private Dictionary<Type, HashSet<object>> collectionsByType = new Dictionary<Type, HashSet<object>>();

        private bool CreateObject(Type key, object value)
        {
            if (!this.collectionsByType.TryGetValue(key, out var collection))
            {
                collection = new HashSet<object>();
                this.collectionsByType.Add(key, collection);
            }

            return collection.Add(value);
        }

        private bool UpdateObject(Type key, object value)
        {
            return this.collectionsByType.TryGetValue(key, out var collection) &&
                                        collection.Remove(value) &&
                                        collection.Add(value);
        }

        private bool DeleteObject(Type key, object value)
        {
            if (this.collectionsByType.TryGetValue(key, out var collection))
            {
                return collection.Remove(value);
            }

            return false;
        }

        public bool CreateObject<T>(T value)
        {
            var key = typeof(T);
            return CreateObject(key, value);
        }
        public bool CreateObject(RegistryEntryBase value)
        {
            var key = value.GetType();
            return CreateObject(key, value);
        }

        public bool UpdateObject<T>(T value)
        {
            var key = typeof(T);
            return UpdateObject(key, value);
        }

        public bool UpdateObject(RegistryEntryBase value)
        {
            var key = value.GetType();
            return UpdateObject(key, value);
        }

        public IEnumerable<T> GetObjects<T>()
        {
            var key = typeof(T);
            if (this.collectionsByType.TryGetValue(key, out var collection))
            {
                return collection.Cast<T>();
            }

            return Enumerable.Empty<T>();
        }

        public IEnumerable<object> GetObjects(Type type)
        {
            if (this.collectionsByType.TryGetValue(type, out var collection))
            {
                return collection;
            }

            return Enumerable.Empty<object>();
        }

        public bool DeleteObject<T>(T value)
        {
            var key = typeof(T);
            return this.DeleteObject(key, value);
        }

        public bool DeleteObject(RegistryEntryBase value)
        {
            var key = value.GetType();
            return this.DeleteObject(key, value);
        }

        public IEnumerable<RegistryEntryBase> GetAllEntityEntries()
        {
            return this.collectionsByType.Values.Where(x => x.FirstOrDefault() is RegistryEntryBase)
                                                .SelectMany(x => x.Cast<RegistryEntryBase>());
        }

        public IEnumerable<RegistryEntryBase> GetAllEntityEntries(string domain)
        {
            var domainCollection = this.collectionsByType.Values.FirstOrDefault(x => (x.FirstOrDefault() is RegistryEntryBase entry) &&
                                                                                     entry.EntityId.GetDomain() == domain)?
                                         .Cast<RegistryEntryBase>();
            return domainCollection ?? Enumerable.Empty<RegistryEntryBase>();
        }

        public RegistryEntryBase FindEntityEntry(string entityId)
        {
            var domainCollection = this.GetAllEntityEntries(entityId.GetDomain());
            return domainCollection?.FirstOrDefault(x => x.EntityId == entityId);
        }
    }
}
