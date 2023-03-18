using Common;

namespace Lib;

public static class PointUtilities
{
	public static Point Random(uint dimension) =>
		new(
			Utilities.Generate(
				dimension,
				() => System.Random.Shared.NextDouble() * 100
			)
		);
}
