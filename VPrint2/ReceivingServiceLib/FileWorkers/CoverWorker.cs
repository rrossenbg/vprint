/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReceivingServiceLib.Data;
using ReceivingServiceLib.Drawing;
using VPrinting;
using VPrinting.Colections;

namespace ReceivingServiceLib.FileWorkers
{
    public class CoverWorker : WorkerBase
    {
        protected static CoverWorker ms_instance;

        public static CoverWorker Default
        {
            get
            {
                if (ms_instance == null)
                    ms_instance = new CoverWorker();
                return ms_instance;
            }
        }

        public void Test()
        {
            WorkerThreadFunction();
        }

        private const int MAX_BUFF_SIZE_50MB = 50 * 1024 * 1024; //33262605;

        protected override void WorkerThreadFunction()
        {
            List<Task> tasks = new List<Task>();

            while (true)
            {
                try
                {
                    var coverFolder = new DirectoryInfo(Global.Strings.COVERWORKFOLDER);
                    coverFolder.CreateIfNotExist();
                    coverFolder.ClearSafe();

                    var isos = CoverDataAccess.Default.GetIsos();

                    foreach (var iso in isos)
                    {
                        var templates = CoverDataAccess.Default.SelectAllTemplatesInfo(iso);
                        if (templates.Count == 0)
                            continue;

                        foreach (var tmp in templates)
                            CoverDataAccess.Default.SelectTemplate(tmp);

                        var priorityTemplateQueue = new PriorityQueue<CoverDataAccess.SelectTemplates_Data>(templates);

                        var vouchers = CoverDataAccess.Default.SelectNextNotCoveredVouchers(0, iso);

                        do
                        {
                            int lastId = 0;

                            foreach (var v in vouchers)
                            {
                                Trace.WriteLine("============================", Strings.COVER);

                                lastId = v.Id;

                                Trace.WriteLine(lastId.ToString(), Strings.COVER);

                                #region

                                var task = Task.Factory.StartNew((o) =>
                                {
                                    FileInfo binFile = null;
                                    FileInfo zipFile = null;
                                    DirectoryInfo sessionFolder = null;

                                    try
                                    {
                                        var voucher = (CoverDataAccess.SelectNextNotCoveredVouchers_Data)o;

                                        using (var buffer = new MemoryBuffer(MAX_BUFF_SIZE_50MB))
                                        {
                                            new CoverDataAccess().SelectVoucher(voucher.Id, buffer.Buffer, voucher.Size);

                                            binFile = coverFolder.CombineFileName(string.Concat(voucher.SessionId, ".bin"));
                                            zipFile = coverFolder.CombineFileName(string.Concat(voucher.SessionId, ".zip"));

                                            if (voucher.IsProtected)
                                            {
                                                Trace.WriteLine("Protected", Strings.COVER);

                                                binFile.WriteAllBytes(buffer.Buffer, voucher.Size);
                                                binFile.DecriptFile(zipFile);
                                            }
                                            else
                                            {
                                                Trace.WriteLine("Not protected", Strings.COVER);   
                                            
                                                zipFile.WriteAllBytes(buffer.Buffer, voucher.Size);
                                            }
                                        }

                                        sessionFolder = coverFolder.Combine(voucher.SessionId);
                                        sessionFolder.EnsureDirectory();

                                        zipFileAccess.Instance.RestoreZip(zipFile.FullName, sessionFolder.FullName);

                                        var files = sessionFolder.GetFiles("*.jpg");
                                        var vouhcerImage = files.Max((f1, f2) => f1.Length > f2.Length);

                                        if (vouhcerImage == null)
                                        {
                                            Trace.WriteLine("Not voucher found", Strings.COVER);
                                            return;
                                        }

                                        using (var buffer = new MemoryBuffer(MAX_BUFF_SIZE_50MB))
                                        {
                                            vouhcerImage.ReadAllBytes(buffer.Buffer, vouhcerImage.Length.ConvertTo<long, int>());

                                            foreach (var tmp in priorityTemplateQueue)
                                            {
                                                string cover = null;

                                                if (new ImageToolsCV().MatchTemplateSafe(buffer.Buffer, tmp.TemplateImage, tmp.HiddenAreas, ref cover))
                                                {
                                                    Trace.WriteLine("Match found", Strings.COVER);
                                                    Trace.WriteLine(cover, Strings.COVER);

                                                    new CoverDataAccess().UpdateVoucher(voucher.Id, cover, tmp.Id);

                                                    priorityTemplateQueue.Set(tmp);
                                                    break;
                                                }
                                                else
                                                {
                                                    Trace.WriteLine("No match found", Strings.COVER);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.WriteLine(ex.toString(), Strings.COVER);
                                        FireError(ex);
                                    }
                                    finally
                                    {
                                        binFile.DeleteSafe();
                                        zipFile.DeleteSafe();
                                        sessionFolder.DeleteSafe(true);

                                        Thread.Yield();
                                    }
                                }, v);

                                #endregion

                                tasks.Add(task);
                            }

                            Task.WaitAll(tasks.ToArray());

                            tasks.Clear();

                            vouchers = CoverDataAccess.Default.SelectNextNotCoveredVouchers(lastId, iso);
                        }
                        while (vouchers.Count != 0);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.toString(), Strings.COVER);

                    FireError(ex);
                }
                finally
                {
                    Thread.Sleep(TimeSpan.FromHours(1));
                }
            }
        }
    }
}