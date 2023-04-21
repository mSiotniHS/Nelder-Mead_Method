using System.Linq;
using Lib.Math;

namespace Lib.Common;

public static class EvaluationStrategyCollection
{
	public static NelderMeadMethod.EvaluationStrategy NoMoreThanNIterations(uint iterationCount)
		=> statistics => statistics.IterationCount < iterationCount;

	public static NelderMeadMethod.EvaluationStrategy LastVarianceIsLessThan(double epsilon)
		=> LastNVariancesAreLessThan(1, epsilon);

	public static NelderMeadMethod.EvaluationStrategy LastNVariancesAreLessThan(int count, double epsilon)
		=> statistics =>
		{
			if (statistics.IterationCount == 0) return true;

			var lastVariancesSum = statistics.Trace
				.Skip(System.Math.Max(0, statistics.Trace.Count - count))
				.Select(Variance)
				.Sum();

			return lastVariancesSum >= epsilon;
		};

	private static double NormSquared(Point point) =>
		point.Sum(coordinate => coordinate * coordinate);

	private static double Variance(Simplex simplex)
	{
		var centroid = simplex.Centroid();

		return simplex
			.Select(point => NormSquared(point - centroid))
			.Sum() / simplex.Size;
	}
}
