namespace Lib.Math;

public interface ISet<in T>
{
	public bool Has(T item);
}
