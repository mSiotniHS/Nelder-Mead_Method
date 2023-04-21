using System;
using Lib.Helpers;
using Lib.Math;

namespace Lib;

public sealed class NelderMeadMethod
{
	public delegate bool EvaluationStrategy(Statistics<Simplex> statistics);

	private readonly Coefficients _coefficients;
	private readonly EvaluationStrategy _shouldWork;

	public NelderMeadMethod(Coefficients coefficients, EvaluationStrategy shouldWork)
	{
		_coefficients = coefficients;
		_shouldWork = shouldWork;
	}

	public Point FindMinimum(
		RealMultivariableFunction function,
		RealCoordinateSpace space,
		Simplex initialSimplex,
		ILogger logger,
		out Statistics<Simplex> statistics
	)
	{
		var dimension = function.Dimension;
		var simplex = initialSimplex;
		statistics = new Statistics<Simplex>();

		if (simplex.Size != dimension + 1)
		{
			throw new ArgumentException($"Initial simplex is of wrong size: got {simplex.Size}, require {dimension}",
				nameof(initialSimplex));
		}

		statistics.Save(simplex);

		while (_shouldWork(statistics))
		{
			logger.Log("\n\nCurrent simplex");
			for (var i = 0; i < simplex.Size; i++)
			{
				logger.Log($"{i + 1}) {simplex[i]} ({function.Calculate(simplex[i])})");
			}

			logger.Log("");

			simplex = PerformIteration(simplex, function, space, logger);
			statistics.Save(simplex);
		}

		return FindMin(simplex, function).Item1;
	}

	private Simplex PerformIteration(
		Simplex simplex,
		RealMultivariableFunction function,
		RealCoordinateSpace space,
		ILogger logger)
	{
		logger.Log("/ Начинаем итерацию");

		var keyPoints = FindKeyPoints(simplex, function);

		var bestPoint = keyPoints.BestPoint;
		var bestValue = keyPoints.BestValue;

		var secondWorstPoint = keyPoints.SecondWorstPoint;
		var secondWorstValue = keyPoints.SecondWorstValue;

		var worstPoint = keyPoints.WorstPoint;
		var worstValue = keyPoints.WorstValue;

		logger.Log("| Ключевые точки:");
		logger.Log($"| *) Лучшая: {bestPoint} [{bestValue}]");
		logger.Log($"| *) Second worst: {secondWorstPoint} [{secondWorstValue}]");
		logger.Log($"| *) Худшая: {worstPoint} [{worstValue}]");

		var centroid = simplex.Centroid(except: worstPoint);
		logger.Log($"| Центр масс всех, кроме худшей: {centroid}");

		var reflection = Reflect(space, worstPoint, centroid);
		var reflectionValue = function.Calculate(reflection);
		logger.Log($"| Reflection: {reflection} [{reflectionValue}]");

		if (reflectionValue < bestValue)
		{
			logger.Log("| reflectionValue лучше bestValue!");

			var expansion = Expand(space, reflection, centroid);
			var expansionValue = function.Calculate(expansion);
			logger.Log($"\\ Expansion: {expansion} [{expansionValue}]");

			return simplex.Replace(
				worstPoint,
				expansionValue < reflectionValue ? expansion : reflection
			);
		}

		logger.Log("| reflectionValue хуже bestValue");

		if (reflectionValue < secondWorstValue) // between!
		{
			logger.Log("\\ reflectionValue лучше secondWorstValue!");
			return simplex.Replace(worstPoint, reflection);
		}

		logger.Log("| reflectionValue хуже secondWorstValue");

		var (betterPoint, betterValue) = reflectionValue < worstValue
			? (reflection, reflectionValue)
			: (worstPoint, worstValue);
		logger.Log($"| Лучшая между reflection и worst: {betterPoint} [{betterValue}]");

		var shrunk = Shrink(space, betterPoint, centroid);
		var shrunkValue = function.Calculate(shrunk);
		logger.Log($"| Shrunk: {shrunk} [{shrunkValue}]");

		if (shrunkValue < betterValue)
		{
			logger.Log("\\ shrunkValue лучше betterValue!");
			return simplex.Replace(worstPoint, shrunk);
		}

		logger.Log("\\ Никакая из вычисленных не лучше --- global shrink");
		return simplex.Map(point => Shrink(space, point, bestPoint));
	}

	private static Point MapPoint(RealCoordinateSpace space, Point mapped, Point basis, double coef)
	{
		var added = coef * (basis - mapped);
		var map = basis + added;

		while (!space.Has(map))
		{
			added *= 0.98;
			map = basis + added;
		}

		return map;
	}

	private Point Reflect(RealCoordinateSpace space, Point reflected, Point basis) =>
		MapPoint(space, reflected, basis, _coefficients.Reflection);

	private Point Expand(RealCoordinateSpace space, Point expanded, Point basis) =>
		MapPoint(space, expanded, basis, -_coefficients.Expansion);

	private Point Shrink(RealCoordinateSpace space, Point shrunk, Point basis) =>
		MapPoint(space, shrunk, basis, -_coefficients.Shrink);

	private delegate bool SavingCondition<in T>(T newValue, T currentValue);

	private static (Point, double) FindPoint(
		Simplex simplex,
		RealMultivariableFunction function,
		SavingCondition<double> shouldSaveNewValue,
		Point initialValue
	)
	{
		var currentPoint = initialValue;
		var currentValue = function.Calculate(currentPoint);

		for (var i = 0; i < simplex.Size; i++)
		{
			var point = simplex[i];
			var value = function.Calculate(point);

			if (shouldSaveNewValue(value, currentValue))
			{
				currentPoint = point;
				currentValue = value;
			}
		}

		return (currentPoint, currentValue);
	}

	private static (Point, double) FindPoint(
		Simplex simplex,
		RealMultivariableFunction function,
		SavingCondition<double> shouldSaveNewValue
	)
		=> FindPoint(simplex, function, shouldSaveNewValue, simplex[0]);

	private static (Point, double) FindMin(Simplex simplex, RealMultivariableFunction function)
		=> FindPoint(simplex, function, (newValue, currentValue) => newValue < currentValue);

	private static (Point, double) FindMax(Simplex simplex, RealMultivariableFunction function)
		=> FindPoint(simplex, function, (newValue, currentValue) => newValue > currentValue);

	// best, secondWorst, worst
	private static KeyPoints FindKeyPoints(Simplex simplex, RealMultivariableFunction function)
	{
		var (bestPoint, bestValue) = FindMin(simplex, function);
		var (worstPoint, worstValue) = FindMax(simplex, function);

		var (secondWorstPoint, secondWorstValue) = FindPoint(
			simplex,
			function,
			(newValue, currentValue) => currentValue < newValue && newValue < worstValue,
			bestPoint
		);

		return new KeyPoints
		{
			BestPoint = bestPoint,
			BestValue = bestValue,
			SecondWorstPoint = secondWorstPoint,
			SecondWorstValue = secondWorstValue,
			WorstPoint = worstPoint,
			WorstValue = worstValue
		};
	}

	private readonly struct KeyPoints
	{
		public Point BestPoint { get; init; }
		public double BestValue { get; init; }

		public Point SecondWorstPoint { get; init; }
		public double SecondWorstValue { get; init; }

		public Point WorstPoint { get; init; }
		public double WorstValue { get; init; }
	}
}
