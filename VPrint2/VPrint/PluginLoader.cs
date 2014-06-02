/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VPrint;

namespace VPrinting
{
    public class PluginLoader
    {
        public static event ThreadExceptionEventHandler Error;

        public void Start()
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            string[] addons = Directory.GetFiles(path, "*_addon.dll");

            var interf = typeof(IRunnable);

            foreach (string file in addons)
            {
                var asm = Assembly.LoadFile(file);
                var types = asm.GetTypes().Where(t => interf.IsAssignableFrom(t));

                foreach (var type in types)
                {
                    Task.Factory.StartNew((o) =>
                    {
                        IRunnable i = (IRunnable)System.Activator.CreateInstance((Type)o);
                        try
                        {
                            i.Run();
                        }
                        catch (Exception ex)
                        {
                            if (Error != null)
                                Error(i, new ThreadExceptionEventArgs(ex));
                        }
                    }, type, TaskCreationOptions.LongRunning);
                }
            }
        }
    }
}
