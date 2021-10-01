using System;

namespace WpfTemplate.Extensions
{
	public static class LongExtensions
	{
		public static float ToMegabytes(this long size)
		{
			return size / 1024f / 1024f;
		}

		public static float ToKilobytes(this long size)
		{
			return size / 1024f;
		}

		public static bool ToBoolean(this long value)
		{
			return Convert.ToBoolean(value);
		}

		public static string ToOrdinalNumber(this long value)
		{
			if (value == 1)
				return $"{value}st";
			if (value == 2)
				return $"{value}nd";
			if (value == 3)
				return $"{value}rd";

			return $"{value}th";
		}



		public static DateTime ToDateTime(this long value)
		{
			return DateTimeOffset.FromUnixTimeMilliseconds(value).DateTime;
		}
	}
}