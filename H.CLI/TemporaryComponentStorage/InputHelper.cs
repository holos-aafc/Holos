using System;

namespace H.CLI.TemporaryComponentStorage
{
    public class InputHelper
    {
        public bool IsNotApplicableInput(string input)
        {
            return input.Equals("N/A", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsYesResponse(string input)
        {
            return input.Equals("y", StringComparison.InvariantCultureIgnoreCase) ||
                   input.Equals("o", StringComparison.InvariantCultureIgnoreCase) || // French "Oui"
                   input.Equals(Properties.Resources.LabelYes, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsNoResponse(string input)
        {
            return input.Equals("n", StringComparison.InvariantCultureIgnoreCase) ||                   
                   input.Equals(Properties.Resources.LabelNo, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
