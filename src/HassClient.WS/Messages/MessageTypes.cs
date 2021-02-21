namespace HassClient.WS.Messages
{
    // Extracted from: https://github.com/home-assistant/core/search?q=async_register_command
    internal enum MessageTypes
    {
        Unknown,

        // Commands
        SubscribeTrigger,
        TestCondition,
    }
}
