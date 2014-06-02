/***************************************************
//  Copyright (c) Premium Tax Free 2012-2013
/***************************************************/

using System;
using System.Threading;
using System.Windows.Forms;

namespace VPrinting.Forms
{
    /// <summary>
    ///  Generate Sitecode
    /// using (var mngr = new AsyncFormManager("Please enter file number"))
    /// {
    ///     mngr.RunWait();
    ///     if (string.IsNullOrWhiteSpace(mngr.Result))
    ///         throw new ApplicationException("Cancelled by user");
    ///     var fileNumber = mngr.Result;
    ///     using (var soupClient = new VoucherEntryAndModificationSoapClient())
    ///        item.SiteCode = soupClient.GenerateSiteCode(countryId, "");
    /// }
    /// </summary>
    public class AsyncFormManager<T> : IDisposable where T : Form, new() 
    {
        private readonly string m_Caption;

        public readonly ManualResetEventSlim Done = new ManualResetEventSlim(false);

        public AsyncFormManager(string caption)
        {
            m_Caption = caption;
        }

        public object Result { get; set; }

        public void RunWait()
        {
            new Thread((o) =>
            {
                var mngr = (AsyncFormManager<T>)o;
                T form = new T();
                form.Text = m_Caption;
                ((IAsyncFormManagerTarget<T>)form).Target = mngr;
                Application.Run(form);
                mngr.Done.Set();
            })
            {
                IsBackground = true
            }.Start(this);
            this.Done.Wait();
        }

        public void Dispose()
        {
            using (Done) ;
        }
    }

    public interface IAsyncFormManagerTarget<T> where T : Form, new() 
    {
        AsyncFormManager<T> Target { get; set; }
    }
}
