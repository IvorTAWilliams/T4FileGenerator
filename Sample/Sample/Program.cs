// See https://aka.ms/new-console-template for more information


using System.Reflection;
using T4FileGenerator.Models;
using T4FileGenerator.Services;

var assembly = Assembly.GetExecutingAssembly();
var outputPath = "P:\\ClassLibraries\\T4FileGenerator\\Sample\\SampleOutput";
var config = new TemplateGeneratorConfiguration(assembly, outputPath);
var generator = new TemplateGenerator(config);
generator.Generate(new global::Sample.Models.Sample { Name = "Thomas" });
var something = generator.OutputFiles;
generator.WriteOutputFiles();