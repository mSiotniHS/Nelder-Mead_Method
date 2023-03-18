using System;
using Lib;

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

		var method = new NelderMeadMethod(classic, NoMoreThanNIterations);
		var min = method.FindMinimum(function);
		Console.WriteLine(min);
	}

	private static bool NoMoreThanNIterations(Statistics<Simplex> statistics)
		=> statistics.IterationCount < 60;
}
