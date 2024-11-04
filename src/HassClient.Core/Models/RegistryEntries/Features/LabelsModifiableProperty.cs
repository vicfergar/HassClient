namespace HassClient.Models
{
    internal class LabelsModifiableProperty : ModifiablePropertyCollection<string>
    {
        internal LabelsModifiableProperty()
            : base(nameof(ILabelable.Labels))
        {
        }
    }
}
