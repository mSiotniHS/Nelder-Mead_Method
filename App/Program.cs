using System;
using Lib;
using Lib.Functions;

namespace App;

public static class Program
{
	public static void Main()
	{
		var rosenbrock = Rosenbrock.Classic();
		var classic = new Coefficients
		{
			Reflection = 1,
			Expansion = 2,
			Shrink = 0.5
		};

		var method = new NelderMeadMethod(classic, NoMoreThanNIterations);
		var min = method.FindMinimum(rosenbrock);
		Console.WriteLine(min);
	}

	private static bool NoMoreThanNIterations(Statistics<Simplex> statistics)
		=> statistics.IterationCount < 60;
}
