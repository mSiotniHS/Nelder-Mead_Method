using Lib.Math;

namespace Lib.Tests;

public static class PointTests
{
	public sealed class AddOperator
	{
		[Theory]
		[MemberData(nameof(CalculatesExpectedValueData))]
		public void CalculatesExpectedValue(Point first, Point second, Point expected)
		{
			var actual = first + second;
			Assert.Equal(expected, actual);
		}

		public static IEnumerable<object[]> CalculatesExpectedValueData()
		{
			yield return new object[] {new Point(1, 2), new Point(3, 4), new Point(4, 6)};
			yield return new object[] {new Point(0, 2, 7), new Point(2.3, 0, -3.2), new Point(2.3, 2, 3.8)};
			yield return new object[] {new Point(-3, -1, 4, 5), new Point(0, -1, 4, -4), new Point(-3, -2, 8, 1)};
		}
	}
}
