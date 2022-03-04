using H.Core.Tools;

namespace H.Core.Converters
{
	public abstract class ConverterBase
	{
        protected ConverterBase()
		{
			HTraceListener.AddTraceListener();
		}

		protected string GetLettersAsLowerCase(string input)
		{
			var cleanedInput = "";
			foreach (var a in input)
			{
				if (char.IsLetter(a))
				{
					cleanedInput += a;
				}
			}

			return cleanedInput.ToLowerInvariant()
							   .Trim();
		}

        protected string GetLowerCase(string input)
        {
            return input.ToLowerInvariant();
        }
	}
}