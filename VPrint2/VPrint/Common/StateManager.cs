/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using VPrinting.Extentions;
using VPrinting.Forms;
using VPrinting.ScanServiceRef;
using VPrinting.Data;

namespace VPrinting.Common
{
    public class StateManager 
    {
        public enum eState
        {
            NA = 0,
            //Scanned not sent
            OK = 1,
            Sent = 2,
            Err = 3,
            //Voucher in database
            VOUCHER = 5,
            //Coversheet in database
            COVER = 6
        }

        /// <summary>
        /// 
        /// </summary>
        public enum eMode
        {
            NA = 0,
            Barcode = 1,
            TransferFile = 2,
            Sitecode = 3,
            TransferFileAndBarcode = 4,
        }

        public class Item : IEquatable<Item>
        {
            public event EventHandler Updated;

            #region SERVER

            public int Id { get; set; }
            public int JobID { get; set; }
            public int CountryID { get; set; }
            public string Name { get; set; }

            #endregion

            #region CLIENT

            public eState State { get; set; }
            public Guid SessionID { get; protected set; }
            public List<FileInfo> FileInfoList { get; set; }
            public Image Thumbnail { get; set; }
            public bool? IsSignatureValid { get; set; }
            public byte[] Signature { get; set; }
            
            public string Message { get; set; }
            public string FullFileName { get; set; }

            public bool Selected { get; set; }

            private bool m_Forsed = false;
            public bool Forsed
            {
                get
                {
                    return m_Forsed;
                }
                set
                {
                    m_Forsed = value;
                    m_Ignore = false;
                    FireUpdated();
                }
            }

            private bool m_Ignore = false;
            public bool Ignored
            {
                get
                {
                    return m_Ignore;
                }
                set
                {
                    m_Ignore = value;
                    m_Forsed = false;
                    FireUpdated();
                }
            }

            #endregion

            public Item()
            {
                SessionID = Guid.NewGuid();
                FileInfoList = new List<FileInfo>();
            }

            public Item(int id, int jobId, int countryId, eState state, Guid sessionID, string name)
            {
                Id = id;
                JobID = jobId;
                State = state;
                CountryID = countryId;
                SessionID = sessionID;
                FileInfoList = new List<FileInfo>();
                Name = name;
            }

            public virtual void Zero()
            {
                JobID = 0;
                CountryID = 0;

                State = eState.NA;

                //Don't delete SessionID
                //SessionID = Guid.Empty;
                FileInfoList.Clear();
                Thumbnail = null;
                Selected = false;
                Message = null;
            }

            public void FireUpdated()
            {
                if (Updated != null)
                    Updated(this, EventArgs.Empty);
            }

            public bool Equals(Item other)
            {
                Debug.Assert(other != null);
                return (SessionID == other.SessionID);
            }
        }

        public class VoucherItem : Item, IEquatable<VoucherItem>
        {
            #region SERVER PROPS   
         
            private int m_RetailerID, m_VoucherID;

            public int RetailerID
            {
                get
                {
                    return m_RetailerID;
                }
                set
                {
                    m_RetailerID = value;
                    RetailerIDCD = value.CheckDigit();
                }
            }

            public int VoucherID
            {
                get
                {
                    return m_VoucherID;
                }
                set
                {
                    m_VoucherID = value;
                    VoucherIDCD = value.CheckDigit();
                }
            }

            private string m_Barcode;

            public string Barcode
            {
                get
                {
                    return m_Barcode;
                }
                set
                {
                    m_Barcode = value;
                    HasBarcode = !value.IsNullOrEmpty();
                }
            }
            public string SiteCode { get; set; }

            #endregion

            #region CLIENT PROPS

            /// <summary>
            /// HasBarcode
            /// </summary>
            public bool HasBarcode { get; private set; }

            /// <summary>
            /// RetailerID == 0 and VoucherID == 0 and FileInfoList.Count == 0
            /// </summary>
            public bool IsEmpty
            {
                get
                {
                    return RetailerID == 0 && VoucherID == 0 && FileInfoList.Count == 0;
                }
            }

            /// <summary>
            /// RetailerID != 0 and VoucherID != 0
            /// </summary>
            public bool IsSetup
            {
                get
                {
                    return RetailerID != 0 && VoucherID != 0;
                }
            }

            public int RetailerIDCD { get; private set; }
            public int VoucherIDCD { get; private set; }

            public object Tag { get; set; }

            #endregion

            public VoucherItem()
            {                
                JobID = 1;                
            }

            public VoucherItem(int id, int countryId, int retailerId, int voucherId, eState state, Guid session, string siteCode, string name)
            {
                Id = id;
                JobID = 1;
                CountryID = countryId;
                RetailerID = retailerId;
                VoucherID = voucherId;
                State = state;
                SessionID = session;
                SiteCode = siteCode;
                Name = name;
                //TODO:
                Barcode = CommonTools.ToBarcode(countryId, 20, RetailerID, VoucherID);
                FileInfoList = new List<FileInfo>();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            /// <example>JobID,RetailerID,SiteCode,VoucherID,State,FileName</example>
            public static VoucherItem Parse(string text)
            {
                var strs = text.SplitSafe(",");

                var item = new VoucherItem();
                item.JobID = int.Parse(strs[0]);
                item.RetailerID = int.Parse(strs[1]);
                item.SiteCode = strs[2];
                item.VoucherID = int.Parse(strs[3]);
                item.State = (eState)int.Parse(strs.Get<string>(4, "0"));
                item.Barcode = string.Format("{0}{1}{2}{3}", "000", "00", item.RetailerID, item.VoucherID);

                foreach (var fname in strs.Get<string>(5, "").Split(';'))
                {
                    if (!fname.IsNullOrEmpty())
                    {
                        var f = new FileInfo(fname);
                        if (f.Exists())
                            item.FileInfoList.Add(f);
                    }
                }

                return item;
            }

            public static bool TryParse(string text, ref VoucherItem item)
            {
                try
                {
                    item = Parse(text);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public void CopyFromWithBarcode(VoucherItem itemHasBarcode)
            {
                Debug.Assert(itemHasBarcode != null);
                Debug.Assert(itemHasBarcode.HasBarcode);

                JobID = itemHasBarcode.JobID;
                CountryID = itemHasBarcode.CountryID;
                RetailerID = itemHasBarcode.RetailerID;
                VoucherID = itemHasBarcode.VoucherID;
                Barcode = itemHasBarcode.Barcode;

                //Don't copy site code
                SiteCode = itemHasBarcode.SiteCode ?? SiteCode;
                State = itemHasBarcode.State;
                HasBarcode = itemHasBarcode.HasBarcode;
                //Don't delete SessionID
                //SessionID = item.SessionID;

                FileInfoList.Clear();
                FileInfoList.AddRange(itemHasBarcode.FileInfoList);

                Thumbnail = itemHasBarcode.Thumbnail;
                Selected = itemHasBarcode.Selected;
                Message = itemHasBarcode.Message;
            }

            public void CopyFromNoBarcode(VoucherItem itemNoBarcode)
            {
                Debug.Assert(itemNoBarcode != null);
                //Debug.Assert(!itemNoBarcode.HasBarcode);
                //Debug.Assert(HasBarcode);

                //JobID = itemNoBarcode.JobID;
                //CountryID = item.CountryID;
                //RetailerID = item.RetailerID;
                //VoucherID = item.VoucherID;
                //Barcode = item.Barcode;

                //Don't copy site code
                //SiteCode = item.SiteCode;
                State = itemNoBarcode.State;
                HasBarcode = true;
                //Don't delete SessionID
                //SessionID = item.SessionID;

                FileInfoList.Clear();
                FileInfoList.AddRange(itemNoBarcode.FileInfoList);

                Thumbnail = itemNoBarcode.Thumbnail;
                Selected = itemNoBarcode.Selected;
                Message = itemNoBarcode.Message;
            }

            public override void Zero()
            {
                RetailerID = 0;
                VoucherID = 0;
                Barcode = null;
                SiteCode = null;
                base.Zero();
            }

            /// <summary>
            /// When scanning
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(VoucherItem other)
            {
                Debug.Assert(other != null);

#if DEBUGGER

                Trace.WriteLine(string.Concat("\r\n   ", CountryID, " <> ", RetailerID, " <> ", VoucherID,
                                              "\r\n   ", other.CountryID, " <> ", other.RetailerID, " <> ", other.VoucherID), Strings.VRPINT);
#endif

                return RetailerID.CompareSmart(RetailerIDCD, other.RetailerID, other.RetailerIDCD) &&
                       VoucherID.CompareSmart(VoucherIDCD, other.VoucherID, other.VoucherIDCD); //(CountryID == other.CountryID) 
            }

            /// <summary>
            /// When loading
            /// </summary>
            /// <param name="countryId"></param>
            /// <param name="retailerId"></param>
            /// <param name="voucherId"></param>
            /// <returns></returns>
            public bool Equals(int countryId, int retailerId, int voucherId)
            {
#if DEBUGGER
                Trace.WriteLine(string.Concat("\r\n   ", CountryID, " <> ", RetailerID, " <> ", VoucherID,
                                              "\r\n   ", countryId, " <> ", retailerId, " <> ", voucherId), Strings.VRPINT);
#endif

                return (CountryID == countryId) &&
                        RetailerID.CompareSmart(RetailerIDCD, retailerId, 0) &&
                        VoucherID.CompareSmart(VoucherIDCD, voucherId, 0);
            }
            
            public override string ToString()
            {
                var a = new StringBuilder();
                foreach (var f in this.FileInfoList)
                    if (f.Exists())
                        a.AppendFormat("{0};", f);

                var b = new StringBuilder();
                b.AppendFormat("{0},{1},{2},{3},{4},{5}", JobID, RetailerID, SiteCode, VoucherID, (int)State, a.ToString());
                return b.ToString();
            }
        }

        public static StateManager Default { get; set; }

        public StateManager()
        {
            Default = this;
        }

        private Item m_CurrentItem = null;
        private Item m_PrevItem = null;

        private readonly SynchronizedCollection<Item> m_ItemCollection = new SynchronizedCollection<Item>();

        public bool InDocumentMode
        {
            get
            {
                return Mode == eMode.TransferFile;
            }
        }

        #region EVENTS

        public event EventHandler<ItemEventArgs> NewItemAdded;
        public event EventHandler<ItemEventArgs> ItemRemoved;
        public event EventHandler<ItemEventArgs> NextItemExpected;
        public event EventHandler<CurrentItemEventArgs> CurrentItemSelected;
        public event EventHandler<CurrentItemEventArgs> CurrentItemCompleted;
        public event EventHandler<CurrentItemEventArgs> LastItemProcessing;
        public event EventHandler ItemsCleared;

        #endregion

        public volatile bool VoucherMustExist;

        public eMode Mode { get; set; }

        public bool Loaded
        {
            get
            {
                return m_ItemCollection.Count != 0;
            }
        }

        public bool HasUnProcItems
        {
            get
            {
                lock (m_ItemCollection.SyncRoot)
                    return m_ItemCollection.FirstOrDefault((i) => i.State == eState.NA && !i.Ignored) != null;
            }
        }

        public bool CanCreate
        {
            get
            {
                return m_CurrentItem == null || !((VoucherItem)m_CurrentItem).IsEmpty;
            }
        }

        public void Load(string filePath)
        {
            if (filePath.IsNullOrEmpty() || !File.Exists(filePath))
                return;

            try
            {
                var lines = File.ReadAllLines(filePath);

                lock (m_ItemCollection.SyncRoot)
                {
                    foreach (var line in lines)
                    {
                        var str = line.TrimSafe();
                        if (!str.IsNullOrEmpty())
                        {
                            VoucherItem item = null;

                            if (VoucherItem.TryParse(str, ref item))
                            {
                                m_ItemCollection.Add(item);
                                FireNewItemAdded(item);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {                
                Clear();
                throw new ApplicationException("Wrong file format.", ex);
            }
            finally
            {
            }
        }

        public void Load(IList<JobItem> list)
        {
            try
            {
                lock (m_ItemCollection.SyncRoot)
                    foreach (var i in list)
                        AddVoucherItem(0, i.CountryId, i.RetailerId, i.VoucherId,
                            eState.NA, Guid.NewGuid(), i.Sitecode, "");
            }
            catch (Exception ex)
            {
                Clear();
                throw new ApplicationException("Wrong file format.", ex);
            }
        }

        [Obsolete]
        public void Save(string filePath)
        {
            if (filePath.IsNullOrEmpty() || !Directory.Exists(Path.GetDirectoryName(filePath)))
                return;

            if (m_ItemCollection.Count == 0)
                return;

            lock (m_ItemCollection.SyncRoot)
            {
                using (var file = File.Open(filePath, FileMode.Create))
                using (var writer = new StreamWriter(file))
                    foreach (var item in m_ItemCollection)
                        writer.WriteLine(item.ToString());
            }
        }

        public void AddItem(int id, int countryId, eState state, Guid session, string name)
        {
            lock (m_ItemCollection.SyncRoot)
            {
                Item item = new Item(id, 0, countryId, state, session, name);
                m_ItemCollection.Add(item);
                FireNewItemAdded(item);
            }
        }

        public void AddVoucherItem(int id, int countryId, int retailerId, int voucherId, eState state, Guid session, string siteCode, string name)
        {
            lock (m_ItemCollection.SyncRoot)
            {
                if (m_ItemCollection.FirstOrDefault((ii) => ii is VoucherItem && ((VoucherItem)ii).Equals(countryId, retailerId, voucherId)) == null)
                {
                    VoucherItem item = new VoucherItem(id, countryId, retailerId, voucherId, state, session, siteCode, name);
                    m_ItemCollection.Add(item);
                    FireNewItemAdded(item);
                }
            }
        }

        public void ShowNextItemExpected()
        {
            var nextItem = m_ItemCollection.FindFirstOrDefault((ii) => ii.State == eState.NA && !ii.Ignored);
            FireNextItemExpected(nextItem);
        }

        public void Remove(Item item)
        {
            if (!m_ItemCollection.Remove(item))
                throw new Exception(item.ToString());

            FireItemRemoved(item);
        }

        public void Clear()
        {
            lock (m_ItemCollection.SyncRoot)
            {
                m_CurrentItem = m_PrevItem = null;

                foreach (var item in new SynchronizedCollection<Item>(m_ItemCollection.SyncRoot, m_ItemCollection))
                    foreach (var file in item.FileInfoList)
                        file.DeleteSafe();

                m_ItemCollection.Clear();

                FireItemsCleared();
            }
        }

        public void ForceAll()
        {
            lock (m_ItemCollection.SyncRoot)
            {
                foreach (var i in m_ItemCollection)
                {
                    var ii = i as VoucherItem;
                    if (ii != null && ii.State == eState.NA)
                        ii.Forsed = !ii.Forsed;
                }
            }
        }

        #region STATE-MANAGER ITEM

        /// <summary>
        /// True - Voucher, False - CoverSheet
        /// </summary>
        /// <param name="isVoucher"></param>
        /// <returns></returns>
        public Item ProcessItem_Begin(bool isVoucher)
        {
            return isVoucher ? new VoucherItem() : new Item();
        }

        public void ProcessItem_End(Item item)
        {
            if (item == null)
                return;

            lock (this)
            {
                try
                {
                    var vitem = item as VoucherItem;
                    if (vitem == null)
                    {
                        m_ItemCollection.Add(item);
                        m_CurrentItem = item;
                        FireNewItemAdded(item);
                    }
                    else
                    {
                        #region Voucher

                        switch (Mode)
                        {
                            /// No document. No order.
                            /// Everything is going in. 
                            /// Must be a barcode.
                            case eMode.Barcode:
                                {
                                    #region

                                    if (!vitem.IsSetup)
                                    {
                                        using (var mngr = new AsyncFormManager<RetailerForm>("Enter voucher details"))
                                        {
                                            mngr.Result = vitem;
                                            mngr.RunWait();
                                            if (!vitem.IsSetup)
                                                throw new ApplicationException("Cannot find barcode.");
                                        }
                                    }

                                    m_CurrentItem = new VoucherItem();
                                    m_CurrentItem.JobID = 1;

                                    if (VoucherMustExist)
                                    {
                                        var da = ServiceDataAccess.Instance;
                                        vitem.SiteCode = da.FindVoucher(vitem.CountryID, vitem.VoucherID, vitem.VoucherIDCD);

                                        if (string.IsNullOrWhiteSpace(vitem.SiteCode))
                                        {
                                            throw new FileInfoApplicationException("Cannot find sitecode.")
                                            {
                                                Info = vitem.FileInfoList.FirstOrDefault(fi => !fi.Name.Contains("barcode"))
                                            };
                                        }
                                    }
                                    m_ItemCollection.Add(m_CurrentItem);
                                    FireNewItemAdded(m_CurrentItem);

                                    ((VoucherItem)m_CurrentItem).CopyFromWithBarcode(vitem);

                                    break;
                                    #endregion
                                }

                            /// There is document.
                            /// They scan. Multi scan TIFF document.
                            /// If there is a barcode the barcode has been validated
                            /// Otherwise it has been inserted by using expected barcode details.
                            case eMode.TransferFile:
                                {
                                    #region

                                    if (vitem.IsSetup)
                                    {
                                        var itm = m_ItemCollection.FindFirstOrDefault((ii) => (ii.State == eState.NA && ii.Forsed) || ((ii.State == eState.NA) && ((VoucherItem)ii).Equals(vitem)));
                                        if (itm == null)
                                        {
                                            throw new FileInfoApplicationException(
                                                string.Concat("Cannot match voucher.\r\nCountry: ", vitem.CountryID, "\r\nRetailer: ",
                                                vitem.RetailerID, "\r\nVoucher: ", vitem.VoucherID, "\r\nState: NA"))
                                                {
                                                    Info = vitem.FileInfoList.FirstOrDefault(fi => !fi.Name.Contains("barcode"))
                                                };
                                        }

                                        m_CurrentItem = itm;
                                        ((VoucherItem)m_CurrentItem).CopyFromWithBarcode(vitem);
                                    }
                                    else
                                    {
                                        var itm = m_ItemCollection.FindFirstOrDefault((ii) => (ii.State == eState.NA && !ii.Ignored));
                                        if (itm == null)
                                            throw new ApplicationException("No more vouchers are expected.");
                                        m_CurrentItem = itm;
                                        ((VoucherItem)m_CurrentItem).CopyFromNoBarcode(vitem);
                                    }
                                    break;
                                    #endregion
                                }

                            /// No order.
                            /// Barcode is mandatory
                            /// Must be sitecode
                            /// System calls TRS to resolve barcode
                            case eMode.Sitecode:
                                {
                                    #region
                                    m_CurrentItem = new VoucherItem();
                                    m_CurrentItem.JobID = 1;
                                    m_ItemCollection.Add(m_CurrentItem);
                                    FireNewItemAdded(m_CurrentItem);
                                    ((VoucherItem)m_CurrentItem).CopyFromNoBarcode(vitem);
                                    break;
                                    #endregion
                                }

                            default:
                                throw new NotImplementedException();
                        }

                        #endregion
                    }
                }
                finally
                {
                    if (m_CurrentItem != null)
                    {
                        m_CurrentItem.State = eState.OK;

                        FireCurrentItemCompleted(m_CurrentItem);

                        m_CurrentItem.FireUpdated();

                        var nextItem = m_ItemCollection.FindFirstOrDefault((ii) => ii.State == eState.NA && !ii.Ignored);

                        FireNextItemExpected(nextItem);
                    }
                }
            }
        }

        public void CreateNewItem_AnyNoDoc()
        {
            FireCurrentItemCompleted(m_CurrentItem);

            m_CurrentItem = new VoucherItem();
            m_CurrentItem.JobID = 1;
            m_ItemCollection.Add(m_CurrentItem);
            FireNewItemAdded(m_CurrentItem);
        }

        public void AddNewItem(Item item)
        {
            m_ItemCollection.Add(item);
            m_CurrentItem = item;
            FireNewItemAdded(item);
        }

        public void CompleteItem(Item item)
        {
            item.State = eState.OK;

            FireCurrentItemCompleted(item);

            item.FireUpdated();
        }

        public VoucherItem AddTransferFileItem(VoucherItem vitem)
        {
            if (vitem.IsSetup)
            {
                var itm = m_ItemCollection.FindFirstOrDefault((ii) => (ii.State == eState.NA && ii.Forsed) || ((ii.State == eState.NA) && ((VoucherItem)ii).Equals(vitem)));
                if (itm == null)
                {
                    throw new FileInfoApplicationException(
                        string.Concat("Cannot match voucher.\r\nCountry: ", vitem.CountryID, "\r\nRetailer: ",
                        vitem.RetailerID, "\r\nVoucher: ", vitem.VoucherID, "\r\nState: NA"))
                    {
                        Info = vitem.FileInfoList.FirstOrDefault(fi => !fi.Name.Contains("barcode"))
                    };
                }

                m_CurrentItem = itm;
                ((VoucherItem)m_CurrentItem).CopyFromWithBarcode(vitem);
            }
            else
            {
                var itm = m_ItemCollection.FindFirstOrDefault((ii) => (ii.State == eState.NA && !ii.Ignored));
                if (itm == null)
                    throw new ApplicationException("No more vouchers are expected.");
                m_CurrentItem = itm;
                ((VoucherItem)m_CurrentItem).CopyFromNoBarcode(vitem);
            }
            return ((VoucherItem)m_CurrentItem);
        }

        #endregion

        #region PRIVATE METHODS

        private void SetResetSelected()
        {
            if (m_PrevItem != null)
            {
                m_PrevItem.Selected = false;
                m_PrevItem.FireUpdated();
            }

            if (m_CurrentItem != null)
            {
                m_CurrentItem.Selected = true;
                m_CurrentItem.FireUpdated();
            }
        }

        private void FireCurrentItemCompleted(Item item)
        {
            if (item != null && CurrentItemCompleted != null)
                CurrentItemCompleted(this, new CurrentItemEventArgs() { CurrentItem = item, PrevItem = m_PrevItem });
        }

        private void FireItemsCleared()
        {
            if (ItemsCleared != null)
                ItemsCleared(this, EventArgs.Empty);
        }

        private void FireNewItemAdded(Item item)
        {
            if (NewItemAdded != null)
                NewItemAdded(this, new ItemEventArgs() { Item = item });
        }

        private void FireItemRemoved(Item item)
        {
            if (ItemRemoved != null)
                ItemRemoved(this, new ItemEventArgs() { Item = item });
        }

        private void FireCurrentItemSelected(Item currentItem, Item prevItem)
        {
            if (m_CurrentItem != null && CurrentItemSelected != null)
                CurrentItemSelected(this, new CurrentItemEventArgs() { CurrentItem = currentItem, PrevItem = prevItem });
        }

        private void FireLastItemProcessing()
        {
            if(LastItemProcessing != null)
                LastItemProcessing(this, new CurrentItemEventArgs() { CurrentItem = m_CurrentItem, PrevItem = m_PrevItem });
        }

        private void FireNextItemExpected(Item nextItem)
        {
            if (NextItemExpected != null && nextItem != null)
                NextItemExpected(this, new ItemEventArgs() { Item = nextItem });
        }

        #endregion
    }

    public class JobItem
    {
        public int JobId { get; set; }
        public int CountryId { get; set; }
        public int RetailerId { get; set; }
        public int VoucherId { get; set; }
        public string Sitecode { get; set; }

        public JobItem(TransferFileInfo info, int countryId)
        {
            int id = 0;
            int.TryParse(info.InvNo, out id);
            JobId = id;
            CountryId = countryId;
            RetailerId = info.BranchId;
            VoucherId = info.VoucherNumber;
            Sitecode = info.SiteLocationNo;
        }
    }

    public class ItemEventArgs : EventArgs
    {
        public StateManager.Item Item { get; set; }
    }

    public class CurrentItemEventArgs : EventArgs
    {
        public StateManager.Item PrevItem { get; set; }
        public StateManager.Item CurrentItem { get; set; }
    }

    public class FileInfoApplicationException : ApplicationException
    {
        public FileInfo Info { get; set; }

        public FileInfoApplicationException(string message)
            : base(message)
        {
        }
    }

}
