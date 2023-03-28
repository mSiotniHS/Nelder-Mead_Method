using System;
using System.Collections.Generic;
using Lib;
using Lib.Helpers;

namespace App;

public static class Program
{
	public static void Main()
	{
		var rosenbrock = Rosenbrock.Classic();
		var himmelblau = new Himmelblau();

		Test(rosenbrock);
		Test(himmelblau);
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

		var method = new NelderMeadMethod(classic, NoMoreThanNIterations);
		var min = method.FindMinimum(function, initialSimplex, new ConsoleLogger(), out _);
		Console.WriteLine(min);
	}

	private static bool NoMoreThanNIterations(Statistics<Simplex> statistics)
		=> statistics.IterationCount < 60;

	private static IEnumerable<Point> GenerateRandomPoints(uint count, uint dimension) =>
		Utilities.Generate(count, () => Utilities.RandomPoint(dimension));
}
