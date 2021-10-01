using System;

namespace WpfTemplate.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="maxLength">Maximum string length to still be considered an empty string</param>
		/// <returns></returns>
		public static bool IsEmptyString(this string text, int maxLength = -1)
		{
			return maxLength > -1 ? string.IsNullOrEmpty(text) || text.Length <= maxLength : string.IsNullOrEmpty(text);
		}

		public static bool IsNotEmptyString(this string text, int minLength = 0)
		{
			return !IsEmptyString(text) && text.Length >= minLength;
		}

		public static string Fill(this string text, params object[] data)
		{
			return string.Format(text, data);
		}
		
		public static string NormalizeFilename(this string text)
		{
			return text.Replace("\\", "-")
				.Replace("/", "-")
				.Replace(":", "-")
				.Replace("*", "-")
				.Replace("?", "-")
				.Replace("\"", "-")
				.Replace("<", "-")
				.Replace(">", "-")
				.Replace("|", "-");
		}

		public static bool ToBoolean(this string text, bool defaultValue = false)
		{
			if (text.IsEmptyString()) return defaultValue;
			return Boolean.Parse(text);
		}
		public static byte ToByte(this string text, byte defaultValue = default(byte))
		{
			if (text.IsEmptyString()) return defaultValue;
			return Byte.Parse(text);
		}

		public static short ToShort(this string text, short defaultValue = default(short))
		{
			if (text.IsEmptyString()) return defaultValue;
			return Int16.Parse(text);
		}

		public static int ToInt(this string text, int defaultValue = default(int))
		{
			if (text.IsEmptyString()) return defaultValue;
			return Int32.Parse(text);
		}

		public static long ToLong(this string text, long defaultValue = default(long))
		{
			if (text.IsEmptyString()) return defaultValue;
			return Int64.Parse(text);
		}

		public static Uri ToUri(this string text, UriKind kind = UriKind.Absolute, Uri defaultValue = null)
		{
			if (text.IsEmptyString()) return defaultValue;
			return new Uri(text, kind);
		}
	}

}