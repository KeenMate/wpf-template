namespace WpfTemplate.Extensions
{
	public static class IntExtensions
	{
		public static string ToOrdinalNumber(this int value)
		{
			if (value == 1)
				return $"{value}st";
			if (value == 2)
				return $"{value}nd";
			if (value == 3)
				return $"{value}rd";

			return $"{value}th";
		}
	}
}