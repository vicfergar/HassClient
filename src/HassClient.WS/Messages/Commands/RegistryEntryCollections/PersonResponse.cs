using HassClient.Models;

namespace HassClient.WS.Messages
{
    internal class PersonResponse
    {
        public Person[] Storage { get; set; }

        public Person[] Config { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(this.Storage)}: {this.Storage?.Length ?? 0}\t{nameof(this.Config)}: {this.Config?.Length ?? 0}";
    }
}
