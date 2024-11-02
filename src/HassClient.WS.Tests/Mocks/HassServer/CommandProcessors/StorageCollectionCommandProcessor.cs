using HassClient.Models;
using HassClient.WS.Messages;
using HassClient.WS.Messages.Commands;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public class StorageCollectionCommandProcessor<TModel> :
        RegistryEntryCollectionCommandProcessor<RegistryEntryCollectionMessagesFactory<TModel>, TModel>
        where TModel : StorageEntityRegistryEntryBase
    {
        public StorageCollectionCommandProcessor()
            : base(StorageCollectionMessagesFactory<TModel>.Create())
        {
        }

        protected override object ProcessListCommand(MockHassServerRequestContext context, JToken merged)
        {
            var result = base.ProcessListCommand(context, merged);
            if (typeof(TModel) == typeof(Person))
            {
                var persons = (IEnumerable<Person>)result;
                return new PersonResponse()
                {
                    Storage = persons.Where(p => p.IsStorageEntry).ToArray(),
                    Config = persons.Where(p => !p.IsStorageEntry).ToArray(),
                };
            }

            return result;
        }
    }
}
