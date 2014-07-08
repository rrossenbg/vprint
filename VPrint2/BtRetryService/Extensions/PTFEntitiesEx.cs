/***************************************************
//  Copyright (c) Premium Tax Free 2012
***************************************************/

namespace BtRetryService
{
    public partial class PTFEntities
    {
    }

    public partial class EmailLog
    {
    }

    public partial class Logging
    {
    }

    public partial class EmailList
    {
        public bool IsValid()
        {
            return this.el_iso_id != 0 &&
                !this.el_list.IsNullOrEmpty() &&
                !this.el_subject.IsNullOrEmpty() &&
                !this.el_filter.IsNullOrEmpty() &&
                !this.el_body_template.IsNullOrEmpty();
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", this.el_id);
        }
    }
}
