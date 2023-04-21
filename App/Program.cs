using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lib;
using Lib.Common;
using Lib.Helpers;
using Lib.Math;

namespace App;

public static class Program
{
	public static void Main()
	{
		var rosenbrock = Rosenbrock.Classic();
		var himmelblau = new Himmelblau();

		Test(rosenbrock);
		// Test(himmelblau);
	}

	private static void Test(RealMultivariableFunction function)
	{
		var classic = new Coefficients
		{
			Reflection = 1,
			Expansion = 2,
			Shrink = 0.5
		};

		var initialSimplex = new Simplex(GenerateRandomPoints(3, 2));

		var iterationCount = EvaluationStrategyCollection.NoMoreThanNIterations(60);
		var lastVariance = EvaluationStrategyCollection.LastVarianceIsLessThan(0.0001);
		var lastVariances = EvaluationStrategyCollection.LastNVariancesAreLessThan(5, 0.0001);

		var consoleLogger = new ConsoleLogger();
		var fileLogger = new FileLogger(new StreamWriter(@"C:\Users\MrAto\Desktop\log.txt", false, Encoding.Unicode));
		var emptyLogger = new EmptyLogger();

		var space = new ConstrainedRealCoordinateSpace(2, new ConstrainedRealCoordinateSpace.Constraint[]
		{
			point => point[0] + point[1] > 2
		});

		var method = new NelderMeadMethod(classic, lastVariance);
		var min = method.FindMinimum(function, space, initialSimplex, emptyLogger, out var statistics);

		Console.WriteLine(min);
		Console.WriteLine(statistics.IterationCount);
	}

	private static IEnumerable<Point> GenerateRandomPoints(uint count, uint dimension) =>
		Utilities.Generate(count, () => Utilities.RandomPoint(dimension));
}
