using Lib.Math;
using static System.Math;

namespace App;

public sealed class Himmelblau : RealMultivariableFunction
{
	public override uint Dimension => 2;

	protected override double BaseCalculate(Point point)
	{
		var x = point[0];
		var y = point[1];

		return Pow(Pow(x, 2) + y - 11, 2) + Pow(x + Pow(y, 2) - 7, 2);
	}
}
