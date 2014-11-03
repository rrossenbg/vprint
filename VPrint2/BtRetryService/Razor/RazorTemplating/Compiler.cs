using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using Microsoft.CSharp;

namespace BtRetryService.Razor.RazorTemplating
{
    public class Compiler
    {
        private static GeneratorResults GenerateCode(RazorTemplateEntry entry)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = string.Format("BtRetryService.Razor.RazorTemplating.RazorTemplateBase<{0}>", entry.ModelType.FullName);
            host.DefaultNamespace = "BtRetryService";
            host.DefaultClassName = entry.TemplateName + "Template";
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.Data");
            host.NamespaceImports.Add("System.Collections");
            host.NamespaceImports.Add("System.Collections.Generic");
            host.NamespaceImports.Add("System.Linq");
            host.NamespaceImports.Add("System.Text");
            host.NamespaceImports.Add("System.Text.RegularExpressions");
            host.NamespaceImports.Add("System.Xml.Serialization");
            host.NamespaceImports.Add("System.Web");
            host.NamespaceImports.Add("System.Data.Entity");

            using (TextReader reader = new StringReader(entry.TemplateString))
                return new RazorTemplateEngine(host).GenerateCode(reader);
        }

        private static CompilerParameters BuildCompilerParameters()
        {
            var @params = new CompilerParameters();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic)// assembly.ManifestModule.Name != "<In Memory Module>")
                    @params.ReferencedAssemblies.Add(assembly.Location);
            }
 
            @params.GenerateInMemory = true;
            @params.IncludeDebugInformation = false;
            @params.GenerateExecutable = false;
            @params.CompilerOptions = "/target:library /optimize";
            return @params;
        }

        public static Assembly Compile(IEnumerable<RazorTemplateEntry> entries)
        {
            var builder = new StringBuilder();
            var codeProvider = new CSharpCodeProvider();
            using (var writer = new StringWriter(builder))
            {
                foreach (var razorTemplateEntry in entries)
                {
                    var generatorResults = GenerateCode(razorTemplateEntry);
                    codeProvider.GenerateCodeFromCompileUnit(generatorResults.GeneratedCode, writer, new CodeGeneratorOptions());
                }
            }

            string str = builder.ToString();

            var result = codeProvider.CompileAssemblyFromSource(BuildCompilerParameters(), new[] { str });
            if (result.Errors != null && result.Errors.Count > 0)
            {
                Trace.WriteLine(result.Errors.toString(), "EML");
                throw new TemplateCompileException(result.Errors, str);
            }

            return result.CompiledAssembly;
        }
    }
}