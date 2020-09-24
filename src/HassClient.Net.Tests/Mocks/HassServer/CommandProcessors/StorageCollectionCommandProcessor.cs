using Bogus;
using HassClient.Net.ClientWebSocket.Messages.Commands;
using HassClient.Net.Serialization;
using HassClient.Net.WSMessages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HassClient.Net.Tests.Mocks.HassServer
{
    public class StorageCollectionCommandProcessor<TFactory, TModel> : BaseCommandProcessor
        where TFactory : StorageCollectionMessagesFactory
    {
        protected TFactory modelFactory;

        protected PropertyInfo idPropertyInfo;

        protected Faker faker;

        protected string modelIdPropertyName;

        protected string apiPrefix;

        protected string modelName;

        private bool isContextReady;

        public StorageCollectionCommandProcessor()
        {
            this.modelFactory = Activator.CreateInstance<TFactory>();
            this.faker = new Faker();

            this.modelIdPropertyName = $"{modelFactory.ModelName}_id";
            this.apiPrefix = this.modelFactory.ApiPrefix;
            this.modelName = this.modelFactory.ModelName;
            this.idPropertyInfo = this.GetModelIdPropertyInfo();
        }

        public override bool CanProcess(BaseIdentifiableMessage receivedCommand) =>
            receivedCommand is RawCommandMessage &&
            receivedCommand.Type.StartsWith(this.apiPrefix) &&
            this.IsValidCommandType(receivedCommand.Type);

        public override BaseIdentifiableMessage ProccessCommand(MockHassServerRequestContext context, BaseIdentifiableMessage receivedCommand)
        {
            try
            {
                if(!this.isContextReady)
                {
                    this.isContextReady = true;
                    this.PrepareHassContext(context);
                }

                var merged = (receivedCommand as RawCommandMessage).MergedObject as JToken;
                var commandType = receivedCommand.Type;
                object result = null;

                if (commandType.EndsWith("list"))
                {
                    result = this.ProccessListCommand(context, merged);
                }
                else if (commandType.EndsWith("create"))
                {
                    result = this.ProccessCreateCommand(context, merged);
                }
                else if (commandType.EndsWith("delete"))
                {
                    result = this.ProccessDeleteCommand(context, merged);
                }
                else if (commandType.EndsWith("update"))
                {
                    result = (object)this.ProccessUpdateCommand(context, merged) ?? ErrorCodes.NotFound;
                }
                else
                {
                    result = this.ProccessUnknownCommand(commandType, context, merged);
                }

                if (result is ErrorCodes errorCode)
                {
                    return this.CreateResultMessageWithError(new ErrorInfo(errorCode));
                }
                else
                {
                    var resultObject = new JRaw(HassSerializer.SerializeObject(result));
                    return this.CreateResultMessageWithResult(resultObject);
                }
            }
            catch (Exception ex)
            {
                return this.CreateResultMessageWithError(new ErrorInfo(ErrorCodes.UnknownError) { Message = ex.Message });
            }
        }

        protected virtual PropertyInfo GetModelIdPropertyInfo()
        {
            var modelType = typeof(TModel);
            var properties = modelType.GetProperties();
            var modelIdProperty = properties.FirstOrDefault(x => HassSerializer.GetSerializedPropertyName(x) == this.modelIdPropertyName);
            return modelIdProperty ?? properties.Where(x => x.Name.EndsWith("Id")).FirstOrDefault();
        }

        protected virtual bool IsValidCommandType(string commandType)
        {
            return commandType.EndsWith("create") ||
                   commandType.EndsWith("list") ||
                   commandType.EndsWith("update") ||
                   commandType.EndsWith("delete");
        }

        protected virtual TModel DeserializeModel(JToken merged)
        {
            var modelSerialized = HassSerializer.SerializeObject(merged);

            var idPropertyName = HassSerializer.GetSerializedPropertyName(idPropertyInfo);
            if (this.modelIdPropertyName != idPropertyName)
            {
                modelSerialized = modelSerialized.Replace(this.modelIdPropertyName, idPropertyName);
            }

            return HassSerializer.DeserializeObject<TModel>(modelSerialized);
        }

        protected virtual IEnumerable<TModel> ProccessListCommand(MockHassServerRequestContext context, JToken merged)
        {
            return context.HassDB.GetObjects<TModel>();
        }

        protected virtual object ProccessCreateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var model = this.DeserializeModel(merged);
            this.idPropertyInfo.SetValue(model, this.faker.RandomUUID());
            context.HassDB.CreateObject(model);
            return model;
        }

        protected virtual TModel ProccessUpdateCommand(MockHassServerRequestContext context, JToken merged)
        {
            var model = this.DeserializeModel(merged);
            context.HassDB.UpdateObject(model);
            return model;
        }

        protected virtual object ProccessDeleteCommand(MockHassServerRequestContext context, JToken merged)
        {
            var model = this.DeserializeModel(merged);
            context.HassDB.DeleteObject(model);
            return null;
        }

        protected virtual object ProccessUnknownCommand(string commandType, MockHassServerRequestContext context, JToken merged)
        {
            return ErrorCodes.NotSupported;
        }

        protected virtual void PrepareHassContext(MockHassServerRequestContext context)
        {
        }
    }
}
