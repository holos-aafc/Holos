namespace H.Infrastructure
{
    public abstract class MessageBase : IMessage
    {
        /// <summary>
        /// The string that should be displayed to the user.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A scope to categorize messages that belong together (but don't necessarily have to be shown at the same time).
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Required) A string that will be used to search for a message. This could be different than the message text since the message text could change since the time the notification was created but the
        /// key should always remain constant so lookups will succeed.
        /// </summary>
        public string Key { get; set; }
    }

    public interface IMessage
    {
        string Message { get; set; }
        string Scope { get; set; }
        string Key { get; set; }
    }
}