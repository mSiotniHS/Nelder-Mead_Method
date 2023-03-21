namespace Lib.Tests;

public static class SimplexTests
{
	public sealed class ReplaceMethod
	{
		[Fact]
		public void ReplaceShouldGenerateCorrectSimplex()
		{
			var simplex = new Simplex(new List<Point> {new(1, 2), new(3, 4)});
			var added = new Point(0, 0);

			var modifiedSimplex = simplex.Replace(new(1, 2), added);

			Assert.Contains(added, modifiedSimplex);
			Assert.DoesNotContain(new Point(1, 2), modifiedSimplex);
		}

		[Fact]
		public void ReplaceShouldNotModifyInitialSimplex()
		{
			var simplex = new Simplex(new List<Point> {new(1, 2), new(3, 4)});
			var added = new Point(0, 0);

			var _ = simplex.Replace(new(1, 2), added);

			Assert.Contains(new(1, 2), simplex);
			Assert.DoesNotContain(new Point(0, 0), simplex);
		}

		[Fact]
		public void ReplacingNonExistentPointInSimplexShouldThrow()
		{
			var simplex = new Simplex(new List<Point> {new(1, 2), new(3, 4)});
			var added = new Point(0, 0);
			var nonExistent = new Point(10, 10);

			Assert.Throws<ArgumentException>(() => simplex.Replace(nonExistent, added));
		}
	}

	public sealed class CentroidMethod
	{
		[Theory]
		[MemberData(nameof(CentroidShouldBeCalculatedCorrectlyData))]
		public void CentroidShouldBeCalculatedCorrectly(Simplex simplex, Point? ignore, Point expected)
		{
			var actual = simplex.Centroid(ignore);
			Assert.Equal(expected, actual);
		}

		public static IEnumerable<object?[]> CentroidShouldBeCalculatedCorrectlyData()
		{
			yield return new object?[]
			{
				new Simplex(new List<Point> {new(0, 9), new(9, 0), new(0, 0)}),
				null,
				new Point(3, 3)
			};

			yield return new object?[]
			{
				new Simplex(new List<Point>
				{
					new(1, 2, 3), new(3, 4, 1), new(4, 5, 6), new(2, 3, 2)
				}),
				new Point(4, 5, 6),
				new Point(2, 3, 2)
			};
		}
	}
}
