/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FintraxServiceManager.Common;

namespace FintraxServiceManager
{
    public class CircularWorker
    {
        private readonly ConcurrentDictionary<string, Assembly> m_assemblies = new ConcurrentDictionary<string, Assembly>();
        private volatile bool m_Running;
        private Thread m_Worker;

        public event EventHandler<EntryEventArgs<string>> Info;
        public event ThreadExceptionEventHandler Error;
        public readonly ConcurrentSortedList<TimeSpan, TypeParam> ToDoList = new ConcurrentSortedList<TimeSpan, TypeParam>();
        public readonly ConcurrentSortedList<TimeSpan, TypeParam> DoneList = new ConcurrentSortedList<TimeSpan, TypeParam>();
        
        public void Start()
        {
            m_Running = true;
            m_Worker = new Thread(Run);
            m_Worker.IsBackground = true;
            m_Worker.Start();
        }

        public void Stop()
        {
            try
            {
                m_Running = false;
                m_Worker.Abort();
            }
            catch (ThreadAbortException)
            {
                //Don't log abortion
            }
        }

        /// <summary>
        /// Run each of the Service/Type in the Collection
        /// </summary>
        public void Run()
        {
            while (m_Running)
            {
                ToDoList.AddRange(DoneList);
                DoneList.Clear();

                foreach (var item in new ConcurrentSortedList<TimeSpan, TypeParam>(ToDoList))
                {
                    if (string.IsNullOrWhiteSpace(item.Value.Type))
                        continue;

                    var toRun = DateTime.Now.Date.Add(item.Key);
                    if (toRun < DateTime.Now)
                        continue;
                    else
                        Thread.Sleep(toRun.Subtract(DateTime.Now));

                    Task.Factory.StartNew((o) =>
                    {

                        #region SUB THREAD FUNCTION

                        KeyValuePair<TimeSpan, TypeParam> job = (KeyValuePair<TimeSpan, TypeParam>)o;
                        try
                        {
                            string[] typeName = job.Value.Type.Split(',');
                            string assemblyName = typeName[0];
                            string typeToInstantiate = typeName[1];

                            if (!m_assemblies.ContainsKey(assemblyName))
                            {
                                FireInfo(string.Concat("Loading assembly ", assemblyName));

                                var asm = Assembly.LoadFile(assemblyName);
                                m_assemblies[assemblyName] = asm;
                            }

                            FireInfo(string.Format("Calling {0}.{1} ", typeToInstantiate, job.Value.Method));

                            Type class1 = m_assemblies[assemblyName].GetType(typeToInstantiate);

                            Object obj = Activator.CreateInstance(class1);

                            MethodInfo mi = class1.GetMethod(job.Value.Method);
                            // Invoke method ('null' for no parameters).

                            //Get the parameters
                            string[] p = job.Value.Parameters.Split(',');
                            object[] paramArray = new object[p.Length];

                            if (!string.IsNullOrWhiteSpace(job.Value.Parameters))
                            {
                                FireInfo(string.Concat("Parameters ", job.Value.Parameters));

                                for (int cnt = 0; cnt < paramArray.Length; cnt++)
                                {
                                    string[] p2 = p[cnt].Split('-');
                                    switch (p2[1])
                                    {
                                        case "string":
                                            paramArray[cnt] = p2[0];
                                            break;
                                        case "int":
                                            paramArray[cnt] = Convert.ToInt32(p2[0]);
                                            break;
                                        case "bool":
                                            paramArray[cnt] = Convert.ToBoolean(p2[0]);
                                            break;
                                        case "double":
                                            paramArray[cnt] = Convert.ToDouble(p2[0]);
                                            break;
                                        case "char":
                                            paramArray[cnt] = Convert.ToChar(p2[0]);
                                            break;
                                        case "decimal":
                                            paramArray[cnt] = Convert.ToDecimal(p2[0]);
                                            break;
                                    }
                                }

                                mi.Invoke(obj, paramArray);
                            }
                            else
                            {
                                mi.Invoke(obj, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            FireError(ex);
                        }
                        finally
                        {
                            ToDoList.Remove(job.Key);
                            DoneList.Add(job.Key, job.Value);
                        }

                        #endregion

                    }, item);

                    Thread.Yield();
                }

                var tillTomorrow = DateTime.Now.Date.AddDays(1) - DateTime.Now;
                Thread.Sleep(tillTomorrow);
            }
        }

        private void FireInfo(string message)
        {
            if (Info != null)
                Info(this, new EntryEventArgs<string>(message));
        }

        private void FireError(Exception ex)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex));
        }
    }
}