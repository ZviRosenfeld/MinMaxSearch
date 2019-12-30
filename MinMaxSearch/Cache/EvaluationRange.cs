namespace MinMaxSearch.Cache
{
    public class EvaluationRange
    {
        public double MinEvaluation { get; set; }

        public double MaxEvaluation { get; set; }

        public EvaluationRange(double minEvaluation, double maxEvaluation)
        {
            MinEvaluation = minEvaluation;
            MaxEvaluation = maxEvaluation;
        }

        public EvaluationRange(double exactEvaluation)
        {
            MinEvaluation = exactEvaluation;
            MaxEvaluation = exactEvaluation;
        }

        public override string ToString() => "{" + MinEvaluation + ", " + MaxEvaluation + "}";

        public override bool Equals(object obj) => obj is EvaluationRange evaluationRange &&
                                                   MaxEvaluation == evaluationRange.MaxEvaluation &&
                                                   MinEvaluation == evaluationRange.MinEvaluation;

        public override int GetHashCode() => MinEvaluation.GetHashCode() + MaxEvaluation.GetHashCode();
    }
}
