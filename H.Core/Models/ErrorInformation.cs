using System.Windows;
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
            base.Message = msg;
        }

        #endregion

        #region Properties
        #endregion 
    }
}