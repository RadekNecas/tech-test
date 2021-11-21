using System.Collections.Generic;
using System.Linq;

namespace Order.Service.Exceptions
{
    public class BadRequestErrors
    {
        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

        public void AddError(string parameterName, string message)
        {
            if(Errors.TryGetValue(parameterName, out var errorMessages))
            {
                errorMessages.Add(message);
            }
            else
            {
                Errors.Add(parameterName, new List<string>(new[] { message }));
            }
        }

        public void AddErrors(string parameterName, IEnumerable<string> messages)
        {
            if (Errors.TryGetValue(parameterName, out var errorMessages))
            {
                errorMessages.AddRange(messages);
            }
            else
            {
                Errors.Add(parameterName, messages.ToList());
            }
        }
    }
}
