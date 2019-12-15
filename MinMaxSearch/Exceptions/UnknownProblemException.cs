using System.Text;

namespace MinMaxSearch.Exceptions
{
    public class UnknownProblemException : MinMaxSearchException
    {
        public UnknownProblemException(string message) : 
            base(CreateExceptionMessage(message))
        {
        }

        private static string CreateExceptionMessage(string message)
        {
            StringBuilder stringBuilder = new StringBuilder("Something seems to be wrong with the way you defined the seach domain.");
            stringBuilder.AppendLine("Specifically, " + message + ".");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Common reasons for this exception:");
            stringBuilder.AppendLine("  - Two state are considered equale even though one is Min's turn and the other is Max's turn.");
            stringBuilder.AppendLine("  - The evaluation of a state changed after it was already cahched.");

            return stringBuilder.ToString();
        }
    }
}
