﻿using System;
using System.Collections.Generic;

namespace Common;

public static class Utilities
{
	public static IEnumerable<T> Generate<T>(uint count, Func<T> function)
	{
		for (var i = 0; i < count; i++)
		{
			yield return function();
		}
	}

	public static T Identity<T>(T value) => value;
}
