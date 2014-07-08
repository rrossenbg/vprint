using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BtRetryService
{
    public partial class TransferDbEntities
    {
    }

    public partial class VoucherTransfer
    {
        public DateTime GetDate()
        {
            return vt_last_modification_date;
        }

        public override int GetHashCode()
        {
            return _vt_id.GetHashCode();
        }
    }

    public class DatetimeComparer : IEqualityComparer<DateTime>
    {
        public enum ComparerType
        {
            Date = 1
        }

        private ComparerType m_Type;

        public DatetimeComparer(ComparerType type = ComparerType.Date)
        {
            m_Type = type;
        }

        public bool Equals(DateTime x, DateTime y)
        {
            Debug.Assert(m_Type == ComparerType.Date, "No other comparers available");
            return x.Date.Equals(y.Date);
        }

        public int GetHashCode(DateTime date)
        {
            return date.GetHashCode();
        }
    }

    public class VoucherTransferComparer : IEqualityComparer<VoucherTransfer> 
    {
        public enum ComparerType
        {
            Id = 0,
            Date = 1,
        }

        private ComparerType m_Type;

        public VoucherTransferComparer(ComparerType type = ComparerType.Date)
        {
            m_Type = type;
        }

        public bool Equals(VoucherTransfer x, VoucherTransfer y)
        {
            switch (m_Type)
            {
                case ComparerType.Date:
                    return x.vt_last_modification_date.Date.Equals(y.vt_last_modification_date.Date);
                default:
                case ComparerType.Id:
                    return x.vt_id.Equals(y.vt_id);
            }
        }

        public int GetHashCode(VoucherTransfer date)
        {
            return date.GetHashCode();
        }
    }
}
