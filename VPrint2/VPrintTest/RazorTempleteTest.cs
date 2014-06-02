using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using test1 = VPrinting.Razor.Facts.RazorTemplateGeneratorFacts.TestModel1;
using test2 = VPrinting.Razor.Facts.RazorTemplateGeneratorFacts.TestModel2;
using VPrinting.Razor.RazorTemplating;

namespace VPrintTest
{
    [TestClass]
    public class RazorTempleteTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void Will_Compile_single_Template_String_registered()
        {
            var testable = new RazorTemplateGenerator();

            testable.RegisterTemplate<test1>("this is a test for @Model.Param1");
            testable.CompileTemplates();
            var result = testable.GenerateOutput(new test1() { Param1 = "p1" });

            Assert.AreEqual("this is a test for p1", result);
        }

        [TestMethod]
        public void Will_Compile_multiple_Template_String_registered()
        {
            var testable = new RazorTemplateGenerator();

            testable.RegisterTemplate<test1>("this is a test for @Model.Param1");
            testable.RegisterTemplate<test2>("this is another test for @Model.Param1");
            testable.CompileTemplates();
            var result1 = testable.GenerateOutput(new test1() { Param1 = "p1" });
            var result2 = testable.GenerateOutput(new test2() { Param1 = "p2" });

            Assert.AreEqual("this is a test for p1", result1);
            Assert.AreEqual("this is another test for p2", result2);
        }

        [TestMethod]
        public void Will_Compile_multiple_Template_String_with_same_model()
        {
            var testable = new RazorTemplateGenerator();

            testable.RegisterTemplate<test1>("this is a test for @Model.Param1");
            testable.RegisterTemplate<test1>("namedTemplate", "this is another test for @Model.Param1");
            testable.CompileTemplates();
            var result1 = testable.GenerateOutput(new test1() { Param1 = "p1" });
            var result2 = testable.GenerateOutput(new test1() { Param1 = "p2" }, "namedTemplate");

            Assert.AreEqual("this is a test for p1", result1);
            Assert.AreEqual("this is another test for p2", result2);
        }

        [TestMethod]
        public void Will_overwrite_existing_Template()
        {
            var testable = new RazorTemplateGenerator();

            testable.RegisterTemplate<test1>("this is a test for @Model.Param1");
            testable.RegisterTemplate<test1>("this is another test for @Model.Param1");
            testable.CompileTemplates();
            var result1 = testable.GenerateOutput(new test1() { Param1 = "p1" });

            Assert.AreEqual("this is another test for p1", result1);
        }

        [TestMethod]
        public void Will_throw_InvalidOperationException_if_attempt_to_generate_without_compiling()
        {
            var testable = new RazorTemplateGenerator();

            testable.RegisterTemplate<test1>("this is a test for @Model.Param1");
            var ex = Record.Exception(() => testable.GenerateOutput(new test1() { Param1 = "p1" }));

            Assert.IsTrue(ex is InvalidOperationException);
        }

        [TestMethod]
        public void Will_throw_InvalidOperationException_if_attempt_to_register_another_template_after_compiling()
        {
            var testable = new RazorTemplateGenerator();

            testable.RegisterTemplate<test1>("this is a test for @Model.Param1");
            testable.CompileTemplates();
            var ex = Record.Exception(() => testable.RegisterTemplate<test2>("this is another test for @Model.Param1"));

            Assert.IsTrue(ex is InvalidOperationException);
        }

        [TestMethod]
        public void Will_throw_NullArgumentException_if_attempt_to_register_template_with_null_name()
        {
            var testable = new RazorTemplateGenerator();

            var ex = Record.Exception(() => testable.RegisterTemplate<test1>(null, "this is a test for @Model.Param1"));

            Assert.IsTrue(ex is ArgumentNullException);
            Assert.AreEqual("templateName", (ex as ArgumentNullException).ParamName);
        }

        [TestMethod]
        public void Will_throw_NullArgumentException_if_attempt_to_register_template_with_null_templatestring()
        {
            var testable = new RazorTemplateGenerator();

            var ex = Record.Exception(() => testable.RegisterTemplate<test1>(null));

            Assert.IsTrue(ex is ArgumentNullException);
            Assert.AreEqual("templateString", (ex as ArgumentNullException).ParamName);
        }

        [TestMethod]
        public void Will_throw_NullArgumentException_if_attempt_to_compile_template_with_null_name()
        {
            var testable = new RazorTemplateGenerator();

            testable.RegisterTemplate<test1>("this is a test for @Model.Param1");
            testable.CompileTemplates();
            var ex = Record.Exception(() => testable.GenerateOutput(new test1(), null));

            Assert.IsTrue(ex is ArgumentNullException);
            Assert.AreEqual("templateName", (ex as ArgumentNullException).ParamName);
        }

        [TestMethod]
        public void Will_throw_argument_out_of_range_exception_if_trying_to_generate_unregistered_template()
        {
            var testable = new RazorTemplateGenerator();

            testable.RegisterTemplate<test1>("this is a test for @Model.Param1");
            testable.CompileTemplates();
            testable.GenerateOutput(new test1() { Param1 = "p1" });
            var ex = Record.Exception(() => testable.GenerateOutput(new test2() { Param1 = "p2" }));

            Assert.IsTrue(ex is ArgumentOutOfRangeException);
        }
    }

    public class Record
    {
        public static Exception Exception(Action act)
        {
            try
            {
                act();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
