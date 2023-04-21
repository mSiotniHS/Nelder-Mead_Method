using Lib.Common;
using Lib.Helpers;
using Lib.Math;

namespace Lib.Tests;

public static class NelderMeadMethodTests
{
	public sealed class FindMinimum
	{
		private sealed class Rosenbrock : RealMultivariableFunction
		{
			private const double A = 1;
			private const double B = 100;

			public override uint Dimension => 2;

			protected override double BaseCalculate(Point point)
			{
				var x = point[0];
				var y = point[1];

				return System.Math.Pow(A - x, 2) + B * System.Math.Pow(y - System.Math.Pow(x, 2), 2);
			}
		}

		[Fact]
		public void FindsCloseEnoughSolutionForRosenbrock()
		{
			var method = new NelderMeadMethod(
				new Coefficients
				{
					Reflection = 1,
					Expansion = 2,
					Shrink = 0.5
				},
				EvaluationStrategyCollection.LastVarianceIsLessThan(0.0001)
			);

			var actual = method.FindMinimum(
				new Rosenbrock(),
				new RealCoordinateSpace(2),
				new Simplex(new Point[] {new(-1, 2), new(3, -4), new(2, 1)}),
				new EmptyLogger(),
				out _
			);
			var expected = new Point(1, 1);

			var difference = expected - actual;
			foreach (var d in difference)
			{
				Assert.Equal(0, d, 0.01);
			}
		}
	}
}
