namespace HassClient.Models
{
    internal class AliasesModifiableProperty : ModifiablePropertyCollection<string>
    {
        internal AliasesModifiableProperty()
            : base(nameof(IAliasable.Aliases))
        {
        }
    }
}
