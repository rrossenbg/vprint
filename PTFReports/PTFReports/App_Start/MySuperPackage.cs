using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(PTF.Reports.App_Start.MySuperPackage), "PreStart")]

namespace PTF.Reports.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}