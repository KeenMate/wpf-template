using System;

namespace WpfTemplate.Extensions
{
	public static class ObjectExtensions
	{
		public static bool IsNull(this object obj)
		{
			return obj == null;
		}

		public static bool IsNotNull(this object obj)
		{
			return !IsNull(obj);
		}

		public static T Or<T>(this T firstValue, T otherValue)
		{
			if (otherValue is string)
			{
				return string.IsNullOrEmpty(firstValue.ToString()) ? otherValue : firstValue;
			}

			return firstValue.IsNull() ? otherValue : firstValue;
		}

		public static TRet FirstNotNull<TObj, TRet>(this TObj obj, params Func<TObj, TRet>[] selectors) where TRet: class
		{
			foreach (var selector in selectors)
			{
				var value = selector(obj);
				if (typeof(TRet) == typeof(string))
				{
					var x = selector(obj).ToString();
					if (x.IsNotEmptyString())
						return value;
				}
				else if (!typeof(TRet).IsClass)
				{
					if (value != default(TRet))
						return value;
				}

				if (value.IsNotNull())
					return value;
			}

			return default(TRet);
		}

		public static bool ToBoolean(this int number)
		{
			return Convert.ToBoolean(number);
		}

		public static bool IsNullable<T>(this T obj)
		{
			if (obj == null) return true;
			Type type = typeof(T);
			if (!type.IsValueType) return true;
			if (Nullable.GetUnderlyingType(type) != null) return true;

			return false;
		}

		//public static bool CheckIfDisposed<T>(this T obj) where T : IDisposable
		//{
		//	try
		//	{
		//		var x = obj.ToString();
		//		return x.Length > 0;
		//	}
		//	catch (ObjectDisposedException ex)
		//	{
		//		return false;
		//	}
		//}
	}
}