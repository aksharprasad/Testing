using System;
using System.Text.RegularExpressions;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Features;

namespace ProseTutorial
{
    public class RankingScore : Feature<double>
    {
        public RankingScore(Grammar grammar) : base(grammar, "Score")
        {
        }

        [FeatureCalculator(nameof(Semantics.Add))]
        public static double Add(double v, double start, double end)
        {
            return start * end;
        }

        [FeatureCalculator(nameof(Semantics.Multiply))]
        public static double Multiply(double v, double start, double end)
        {
            return start * end;
        }

        [FeatureCalculator(nameof(Semantics.Element))]
        public static double Element(double v, double k)
        {
            return k;
        }

        [FeatureCalculator("k", Method = CalculationMethod.FromLiteral)]
        public static double K(int k)
        {
            return k;
        }

    }
}