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
        public readonly ConcurrentSortedList<TimeSpan, TypeParam> ToDoList = new ConcurrentSortedList<TimeSpan, TypeParam>();
        public readonly ConcurrentSortedList<TimeSpan, TypeParam> DoneList = new ConcurrentSortedList<TimeSpan, TypeParam>();
        public event EventHandler<EntryEventArgs<string>> Info;
        public event ThreadExceptionEventHandler Error;
        public void Start()
        {
            this.m_Running = true;
            this.m_Worker = new Thread(new ThreadStart(this.Run));
            this.m_Worker.IsBackground = true;
            this.m_Worker.Start();
        }
        public void Stop()
        {
            try
            {
                this.m_Running = false;
                this.m_Worker.Abort();
            }
            catch (ThreadAbortException)
            {
            }
        }
        public void Run()
        {
            while (this.m_Running)
            {
                this.ToDoList.AddRange(this.DoneList);
                this.DoneList.Clear();
                foreach (KeyValuePair<TimeSpan, TypeParam> current in new ConcurrentSortedList<TimeSpan, TypeParam>(this.ToDoList))
                {
                    if (!string.IsNullOrWhiteSpace(current.Value.Type))
                    {
                        DateTime t = DateTime.Now.Date.Add(current.Key);
                        if (!(t < DateTime.Now))
                        {
                            Thread.Sleep(t.Subtract(DateTime.Now));
                            Task.Factory.StartNew(delegate(object o)
                            {
                                KeyValuePair<TimeSpan, TypeParam> keyValuePair = (KeyValuePair<TimeSpan, TypeParam>)o;
                                try
                                {
                                    string[] array = keyValuePair.Value.Type.Split(new char[]
									{
										','
									});
                                    string text = array[0];
                                    string text2 = array[1];
                                    if (!this.m_assemblies.ContainsKey(text))
                                    {
                                        this.FireInfo("Loading assembly " + text);
                                        Assembly value = Assembly.LoadFile(text);
                                        this.m_assemblies[text] = value;
                                    }
                                    this.FireInfo(string.Format("Calling {0}.{1} ", text2, keyValuePair.Value.Method));
                                    Type type = this.m_assemblies[text].GetType(text2);
                                    object obj = Activator.CreateInstance(type);
                                    MethodInfo method = type.GetMethod(keyValuePair.Value.Method);
                                    string[] array2 = keyValuePair.Value.Parameters.Split(new char[]
									{
										','
									});
                                    object[] array3 = new object[array2.Length];
                                    if (!string.IsNullOrWhiteSpace(keyValuePair.Value.Parameters))
                                    {
                                        this.FireInfo("Parameters " + keyValuePair.Value.Parameters);
                                        for (int i = 0; i < array3.Length; i++)
                                        {
                                            string[] array4 = array2[i].Split(new char[]
											{
												'-'
											});
                                            string a;
                                            if ((a = array4[1]) != null)
                                            {
                                                if (!(a == "string"))
                                                {
                                                    if (!(a == "int"))
                                                    {
                                                        if (!(a == "bool"))
                                                        {
                                                            if (!(a == "double"))
                                                            {
                                                                if (!(a == "char"))
                                                                {
                                                                    if (a == "decimal")
                                                                    {
                                                                        array3[i] = Convert.ToDecimal(array4[0]);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    array3[i] = Convert.ToChar(array4[0]);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                array3[i] = Convert.ToDouble(array4[0]);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            array3[i] = Convert.ToBoolean(array4[0]);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        array3[i] = Convert.ToInt32(array4[0]);
                                                    }
                                                }
                                                else
                                                {
                                                    array3[i] = array4[0];
                                                }
                                            }
                                        }
                                        method.Invoke(obj, array3);
                                    }
                                    else
                                    {
                                        method.Invoke(obj, null);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.FireError(ex);
                                }
                                finally
                                {
                                    this.ToDoList.Remove(keyValuePair.Key);
                                    this.DoneList.Add(keyValuePair.Key, keyValuePair.Value);
                                }
                            }, current);
                            Thread.Yield();
                        }
                    }
                }
                TimeSpan timeout = DateTime.Now.Date.AddDays(1.0) - DateTime.Now;
                Thread.Sleep(timeout);
            }
        }
        private void FireInfo(string message)
        {
            if (this.Info != null)
            {
                this.Info(this, new EntryEventArgs<string>(message));
            }
        }
        private void FireError(Exception ex)
        {
            if (this.Error != null)
            {
                this.Error(this, new ThreadExceptionEventArgs(ex));
            }
        }
    }
}
