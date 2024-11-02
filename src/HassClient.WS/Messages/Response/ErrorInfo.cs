using System.Collections.Generic;

namespace HassClient.WS.Messages
{
    /// <summary>
    /// Provides information about the error occurred.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public ErrorCodes Code { get; set; }

        /// <summary>
        /// A message provided by the server with detailed information about the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Translation key for the error message.
        /// </summary>
        public string TranslationKey { get; set; }

        /// <summary>
        /// Domain for translation.
        /// </summary>
        public string TranslationDomain { get; set; }

        /// <summary>
        /// Placeholders used in translation.
        /// </summary>
        public Dictionary<string, string> TranslationPlaceholders { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfo"/> class.
        /// </summary>
        public ErrorInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfo"/> class.
        /// </summary>
        internal ErrorInfo(ErrorCodes code)
        {
            this.Code = code;
            this.Message = code.ToString();
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.Code}: {this.Message}";
    }
}
