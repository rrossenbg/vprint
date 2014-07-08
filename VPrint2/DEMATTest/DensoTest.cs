using System.Collections.Generic;
using System.Linq;
using DeNSo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace DEMATTest
{
    [TestClass]
    public class DensoTest
    {
        [TestMethod]
        public void Denso_test()
        {
            Configuration.EnableJournaling = true;
            Configuration.BasePath = @"c:\TEST";
            Session.Start();
            Session.DefaultDataBase = "test";

            using (var ss = Session.New)
            {
                var cc = new Class1()
                {
                    Prop1 = "1",
                    Prop2 = "2",
                    Prop3 = 3,
                    Prop4 = 7,
                    Child = new Class2() { Pppp1 = "kk", Pppp2 = "kkkkkk" }
                };
                ss.Set(cc);
            }

            using (var ss = Session.New)
            {
                var result = ss.Get<Class1>(c => c.Prop1 == "1" || (c.Child != null && c.Child.Pppp1 == "kk")).ToList();

                foreach (var v in result)
                {
                    Debug.WriteLine(v.Prop1);
                    Debug.WriteLine(v.Prop2);
                }
            }

            Session.ShutDown();
        }

        public class Class1
        {
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
            public int Prop3 { get; set; }
            public int Prop4 { get; set; }

            public List<string> Lista { get; set; }
            public Class2 Child { get; set; }
        }

        public class Class2
        {
            public string Pppp1 { get; set; }
            public string Pppp2 { get; set; }
        }
    }
}
