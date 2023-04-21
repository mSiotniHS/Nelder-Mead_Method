using System.Collections.Generic;
using System.Linq;

namespace Lib.Math;

public class ConstrainedRealCoordinateSpace : RealCoordinateSpace
{
	/// <summary>
	/// Возвращает <c>true</c>, если точка удовлетворяет ограничению; иначе <c>false</c>
	/// </summary>
	public delegate bool Constraint(Point point);

	private readonly Constraint[] _constraints;

	public ConstrainedRealCoordinateSpace(uint dimension, IEnumerable<Constraint> constraints) : base(dimension)
	{
		_constraints = constraints.ToArray();
	}

	public override bool Has(Point point)
	{
		return
			base.Has(point)
			&& _constraints.All(constraint => constraint(point));
	}
}
