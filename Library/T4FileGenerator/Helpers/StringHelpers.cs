namespace T4FileGenerator.Helpers
{
	public static class StringHelpers
	{
		public static string NormalizePath(string path)
		{
			return Path.GetFullPath(new Uri(path).LocalPath)
				.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
				.ToUpperInvariant();
		}
		public static string RemoveCarriageReturns(this string sample)
		{
			return sample.Replace("\r\n", "\n");
		}
	}
}