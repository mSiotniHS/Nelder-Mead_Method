using Lib.Math;
using static System.Math;

namespace App;

public sealed class Rosenbrock : RealMultivariableFunction
{
	private readonly double _a;
	private readonly double _b;

	public Rosenbrock(double a, double b)
	{
		_a = a;
		_b = b;
	}

	public static Rosenbrock Classic() => new(1, 100);

	public override uint Dimension => 2;

	protected override double BaseCalculate(Point point)
	{
		var x = point[0];
		var y = point[1];

		return Pow(_a - x, 2) + _b * Pow(y - Pow(x, 2), 2);
	}
}
