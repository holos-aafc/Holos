using System;

namespace H.CLI.TemporaryComponentStorage
{
    /// <summary>
    /// A custom exception to notify the developer that there is a mistake when creating the TemporaryInput class because a property is not in the correct format
    /// or mispelled. (A property should be in the format: Example Format and be spelled according to the corresponding key in the list of keys. For example, in the ShelterBeltKeys,
    /// we have a key called "Average Circumference(cm)", the following proeprty in the ShelterBeltTemporaryInput class should be called "AverageCircumference"
    /// </summary>
    public class InvalidPropertyNameException : Exception
    {
        #region Fields
        private string message = String.Empty;
        #endregion


        #region Constructors
        public InvalidPropertyNameException() { }
        #endregion

        #region Public Methods
        public InvalidPropertyNameException(string messageDetails)
        {
            message = messageDetails;
        }

        public override string Message
        {
            get
            {
                return message;
            }
        } 
        #endregion

    }
}
