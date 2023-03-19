namespace Lib.Tests;

public static class SimplexTests
{
	public sealed class ReplaceMethod
	{
		[Fact]
		public void ReplaceShouldGenerateCorrectSimplex()
		{
			var added = new Point(0, 0);

			var simplex = new Simplex(new List<Point> {new(1, 2), new(3, 4)});
			var modifiedSimplex = simplex.Replace(new(1, 2), added);

			Assert.Contains(added, modifiedSimplex);
			Assert.DoesNotContain(new Point(1, 2), modifiedSimplex);
		}

		[Fact]
		public void ReplaceShouldNotModifyInitialSimplex()
		{
			var added = new Point(0, 0);

			var simplex = new Simplex(new List<Point> {new(1, 2), new(3, 4)});
			var _ = simplex.Replace(new(1, 2), added);

			Assert.Contains(new(1, 2), simplex);
			Assert.DoesNotContain(new Point(0, 0), simplex);
		}
	}
}
