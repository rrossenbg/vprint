using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPrinting.Razor.RazorTemplating;
using VPrinting.Razor.ModelsAndTemplates;

namespace VPrinting.Razor
{
    class SampleUsage
    {
        public void Test()
        {
            IRazorTemplateGenerator generator = new RazorTemplateGenerator();
            generator.RegisterTemplate<SampleModel>(SampleTemplateStrings.Sample1);
            generator.CompileTemplates();
            var output = generator.GenerateOutput(new SampleModel() { Prop1 = "p1", Prop2 = "p2", Prop3 = new List<string> { "pe1", "pe2", "pe3" } });
            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}
