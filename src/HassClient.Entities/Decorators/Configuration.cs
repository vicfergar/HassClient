using HassClient.Models;
using HassClient.WS;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents the Home Assistant instance configuration.
    /// </summary>
    public class Configuration : ConfigurationModel
    {
        /// <summary>
        /// Gets a value indicating whether the configuration is marked as dirty and is pending to be updated.
        /// </summary>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Occurs when the configuration is updated.
        /// </summary>
        public event EventHandler Updated;

        internal void MarkAsDirty()
        {
            this.IsDirty = true;
        }

        internal async Task<bool> UpdateAsync(HassWSApi hassWSApi, CancellationToken cancellationToken)
        {
            if (await hassWSApi.RefreshConfigurationAsync(this, cancellationToken))
            {
                this.IsDirty = false;
                this.Updated?.Invoke(this, EventArgs.Empty);
            }

            return !this.IsDirty;
        }
    }
}
