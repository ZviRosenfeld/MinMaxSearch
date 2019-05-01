using System;
using System.Text;

namespace MinMaxSearch.Banckmarking
{
    public class BenckmarkResult
    {
        public BenckmarkResult(TimeSpan time, int leaves, int internalNodes)
        {
            Time = time;
            Leaves = leaves;
            InternalNodes = internalNodes;
        }

        public TimeSpan Time { get; }

        public int Leaves { get; }

        public int InternalNodes { get; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Time: " + Time);
            stringBuilder.AppendLine("Leaves: " + Leaves);
            stringBuilder.AppendLine("InternalNodes: " + InternalNodes);
            return stringBuilder.ToString();
        }
    }
}
