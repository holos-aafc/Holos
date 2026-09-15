using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.TemporaryComponentStorage
{
    public class InputHelper
    {
        /// <summary>
        /// Console.ReadLine returns null once there is no input left to read - a redirected or closed input stream,
        /// which is the normal state when the CLI runs from a script, a scheduled job or a build pipeline. These
        /// answer false for that rather than throwing, so a prompt nobody can answer is simply not a yes and not a no.
        /// Callers must still decide what to do when neither is true; see the prompt sites, which take a default
        /// instead of asking again.
        /// </summary>
        public bool IsNotApplicableInput(string input)
        {
            return string.IsNullOrWhiteSpace(input) == false &&
                   input.Equals("N/A", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsYesResponse(string input)
        {
            return string.IsNullOrWhiteSpace(input) == false &&
                   (input.Equals("y", StringComparison.InvariantCultureIgnoreCase) ||
                    input.Equals("o", StringComparison.InvariantCultureIgnoreCase) || // French "Oui"
                    input.Equals(Properties.Resources.LabelYes, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsNoResponse(string input)
        {
            return string.IsNullOrWhiteSpace(input) == false &&
                   (input.Equals("n", StringComparison.InvariantCultureIgnoreCase) ||
                    input.Equals(Properties.Resources.LabelNo, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
