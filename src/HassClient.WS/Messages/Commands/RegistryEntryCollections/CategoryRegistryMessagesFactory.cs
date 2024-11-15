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

        public BaseOutgoingMessage BuildListMessage(string scope)
        {
            return this.BuildListMessage(mergedObject: new { scope });
        }

        public new BaseOutgoingMessage BuildCreateMessage(Category category)
        {
            return base.BuildCreateMessage(category);
        }

        public new BaseOutgoingMessage BuildUpdateMessage(Category category, bool forceUpdate)
        {
            return base.BuildUpdateMessage(category, forceUpdate);
        }

        public BaseOutgoingMessage BuildDeleteMessage(Category category)
        {
            return this.BuildDeleteMessage(category, mergedObject: new { category.Scope });
        }
    }
}
