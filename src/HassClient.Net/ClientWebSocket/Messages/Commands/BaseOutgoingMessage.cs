namespace HassClient.Net.WSMessages
{
    /// <summary>
    /// Represents an identifiable outgoing message (any but authentication messages).
    /// </summary>
    public abstract class BaseOutgoingMessage : BaseIdentifiableMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseOutgoingMessage"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc/></param>
        public BaseOutgoingMessage(string type)
            : base(type)
        {
        }
    }
}
