using System.Linq;

namespace Lib.Math;

public class RealCoordinateSpace : ISet<Point>
{
	public virtual bool Has(Point point) => point.All(double.IsRealNumber);
}
