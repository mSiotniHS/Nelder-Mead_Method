namespace Lib;

public abstract class RealMultivariableFunction
{
	public abstract uint Dimension { get; }

	protected abstract double BaseCalculate(Point point);

	public double Calculate(Point point)
	{
		point.DimensionAssert(Dimension);
		return BaseCalculate(point);
	}
}
