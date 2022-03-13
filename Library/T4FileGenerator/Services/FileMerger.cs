using DiffMatchPatch;
using T4FileGenerator.Helpers;
using T4FileGenerator.Models;

namespace T4FileGenerator.Services
{
	public class LineInformation
	{
		public int LineNumber { get; set; }
		public string Contents { get; set; }
	}
	public static class FileMerger
	{
		public static async Task<OutputFile> Merge(OutputFile original, OutputFile human, OutputFile generator)
		{
			var dmp = new diff_match_patch();
			return await Task.Run(() =>
			{
				var generatorPatches = dmp.patch_make(original.Contents.RemoveCarriageReturns(), generator.Contents.RemoveCarriageReturns());
				var output = dmp.patch_apply(generatorPatches, human.Contents.RemoveCarriageReturns())[0].ToString();
				return new OutputFile {FileName = generator.FileName, Contents = output, RelativeFileName = generator.RelativeFileName};
			});
		}
	}
}