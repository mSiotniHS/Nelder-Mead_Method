using System.Linq;

namespace Lib.Math;

public class RealCoordinateSpace : ISet<Point>
{
	public uint Dimension { get; init; }

	public RealCoordinateSpace(uint dimension)
	{
		Dimension = dimension;
	}

	public virtual bool Has(Point point) => point.Dimension == Dimension && point.All(double.IsRealNumber);
}
