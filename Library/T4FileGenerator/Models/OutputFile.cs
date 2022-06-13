namespace T4FileGenerator.Models;

public class OutputFile
{
	public string Contents { get; set; }
	public string RelativeFileName { get; set; }
	public string FileName { get; set; }
	public bool? DoGenerate { get; set; }
}