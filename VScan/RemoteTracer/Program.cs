/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteTracerLib;

namespace RemoteTracer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tracer started.");
            RemoteTracerClient client = new RemoteTracerClient(Console.Out);
            ConsoleKeyInfo info;
            while (true)
            {
                info = Console.ReadKey();
                if (info.Modifiers == ConsoleModifiers.Control && (info.Key == ConsoleKey.N || info.Key == ConsoleKey.E))
                    Console.Clear();
                else if (info.Key == ConsoleKey.Escape)
                    break;
            }
        }
    }
}
