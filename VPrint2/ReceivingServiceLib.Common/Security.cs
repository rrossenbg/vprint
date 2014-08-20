/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/
//  Believe me this code may not have been here

using System;
using System.Reflection;
using ReceivingServiceLib.Data;

namespace ReceivingServiceLib
{
    //[Obfuscation(StripAfterObfuscation = true)]
    //class Security
    //{
    //    [Obfuscation]
    //    private static DateTime ms_d = new DateTime(2015, 1, 1);

    //    [Obfuscation]
    //    public static void Check(bool smart = false)
    //    {
    //        if (smart)
    //        {
    //            var now = (DateTime)BaseDataAccess.SelectT(true.Random());
    //            if (now > ms_d)
    //                throw new ProjectTimedOutException();
    //        }
    //        else
    //        {
    //            if (DateTime.Now > ms_d)
    //                throw new ProjectTimedOutException();
    //        }
    //    }
    //}

    public class ProjectTimedOutException : Exception
    {
        public ProjectTimedOutException()
            : base("Project is cancelled. I'm sorry about inconvenience it might cause.")
        {
        }
    }
}
