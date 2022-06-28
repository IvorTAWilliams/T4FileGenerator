namespace T4FileGenerator.Helpers
{
	public static class StringHelpers
	{
		public static string NormalizePath(string path)
		{
			return path
				.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
				.Replace("\\", "/")
				.ToUpperInvariant();
		}
		public static string RemoveCarriageReturns(this string sample)
		{
			return sample.Replace("\r\n", "\n");
		}
	}
}