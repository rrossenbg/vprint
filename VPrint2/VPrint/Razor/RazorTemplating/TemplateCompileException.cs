using System;
using System.CodeDom.Compiler;
using System.Text;

namespace VPrinting.Razor.RazorTemplating
{
    public class TemplateCompileException : Exception
    {
        public TemplateCompileException(CompilerErrorCollection errors, string sourceCode)
        {
            Errors = errors;
            SourceCode = sourceCode;
        }
        public CompilerErrorCollection Errors { get; private set; }
        public string SourceCode { get; private set; }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            foreach (CompilerError e in Errors)
                b.AppendLine(e.ToString());

            return b.ToString();
        }
    }
}
