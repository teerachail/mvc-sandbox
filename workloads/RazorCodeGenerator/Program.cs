using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.AspNet.Mvc.Razor.Directives;
using Microsoft.AspNet.Razor;

namespace RazorCodeGenerator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                Console.WriteLine("Press the ANY key to start.");
                Console.ReadLine();

                Console.WriteLine($"Starting Code Generation: {args[0]}");
                var timer = Stopwatch.StartNew();
                GenerateCodeFile(Path.GetFullPath(args[0]), "Test");
                Console.WriteLine($"Completed after {timer.Elapsed}");
                
                Console.WriteLine("Press the ANY key to exit.");
                Console.ReadLine();
                return 0;
            }
            else
            {
                Console.WriteLine("usage: dnx run <file.cshtml>");
                return -1;
            }
        }

        private static void GenerateCodeFile(string file, string @namespace)
        {
            var basePath = Path.GetDirectoryName(file);
            var fileName = Path.GetFileName(file);

            var fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            var codeLang = new CSharpRazorCodeLanguage();

            var host = new MvcRazorHost(new DefaultChunkTreeCache(new PhysicalFileProvider(basePath)));
            var engine = new RazorTemplateEngine(host);

            using (var fileStream = File.OpenText(file))
            {
                var code = engine.GenerateCode(
                    input: fileStream,
                    className: fileNameNoExtension,
                    rootNamespace: Path.GetFileName(@namespace),
                    sourceFileName: fileName);

                File.WriteAllText(Path.Combine(basePath, string.Format("{0}.cs", fileNameNoExtension)), code.GeneratedCode);
            }
        }
    }
}