using H.Infrastructure;

namespace H.Core.Models
{
    public class ErrorInformation : MessageBase
    {
        #region Constructors

        public ErrorInformation()
        {
        }

        public ErrorInformation(string msg) : this()
        {
            Message = msg;
        }

        #endregion
    }
}