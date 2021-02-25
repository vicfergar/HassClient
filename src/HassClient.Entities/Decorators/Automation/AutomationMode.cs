namespace HassClient.Entities.Decorators
{
    /// <summary>
    /// Represents the automation’s mode configuration option.
    /// <para>
    /// More information at <see href="https://www.home-assistant.io/docs/automation/modes"/>.
    /// </para>
    /// </summary>
    public enum AutomationMode
    {
        /// <summary>
        /// (Default) Do not start a new run. Issue a warning.
        /// </summary>
        Single,

        /// <summary>
        /// Start a new run after first stopping previous run.
        /// </summary>
        Restart,

        /// <summary>
        /// Start a new run after all previous runs complete.
        /// Runs are guaranteed to execute in the order they were queued.
        /// </summary>
        Queued,

        /// <summary>
        /// Start a new, independent run in parallel with previous runs.
        /// </summary>
        Parallel,
    }
}
