using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Cs
{
	public static class Program
	{
		private static int Main(string[] args)
		{
			return Task.Run(async () => await Run(args[0])).GetAwaiter().GetResult();
		}

		private static async Task<int> Run(string path)
		{
			var code = await LoadCodeAndSanitize(path);

			try
			{
				await CSharpScript.RunAsync(code);

				return 0;
			}
			catch (CompilationErrorException e)
			{
				Console.WriteLine(e.Message);

				return 1;
			}
		}

		private static async Task<string> LoadCodeAndSanitize(string path)
		{
			var lines = await File.ReadAllLinesAsync(path);
			var sanitizedLines = SanitizeLines(lines);
			var code = string.Join(Environment.NewLine, sanitizedLines);

			return code;
		}

		// Sanitize lines beginning #! by commenting them out.
		// Commenting out ensures source line numbers match stack traces.
		private static IEnumerable<string> SanitizeLines(IEnumerable<string> lines)
		{
			foreach (var line in lines)
			{
				var sanitizedLine = line;

				if (line.StartsWith("#!"))
				{
					sanitizedLine = $"//{line}";
				}

				yield return sanitizedLine;
			}
		}
	}
}
