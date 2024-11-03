using HassClient.Models;
using HassClient.WS.Messages.Commands;

namespace HassClient.WS.Messages
{
    internal class CategoryRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<Category>
    {
        public static CategoryRegistryMessagesFactory Instance = new CategoryRegistryMessagesFactory();

        public CategoryRegistryMessagesFactory()
            : base("config/category_registry", "category")
        {
        }

        public BaseOutgoingMessage CreateListMessage(string scope)
        {
            return this.CreateListMessage(mergedObject: new { scope });
        }

        public new BaseOutgoingMessage CreateCreateMessage(Category category)
        {
            return base.CreateCreateMessage(category);
        }

        public new BaseOutgoingMessage CreateUpdateMessage(Category category, bool forceUpdate)
        {
            return base.CreateUpdateMessage(category, forceUpdate);
        }

        public BaseOutgoingMessage CreateDeleteMessage(Category category)
        {
            return this.CreateDeleteMessage(category, mergedObject: new { category.Scope });
        }
    }
}
