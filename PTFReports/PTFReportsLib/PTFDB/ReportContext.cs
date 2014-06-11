/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using PTF.Reports.Common;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports.PTFDB
{
    partial class PTFContext
    {
        public static PTFContext Current
        {
            get
            {
                lock (typeof(PTFContext))
                {
                    var ocKey = string.Concat("ctx1_", HttpContext.Current.GetHashCode().ToString("x"));
                    if (!HttpContext.Current.Items.Contains(ocKey))
                        HttpContext.Current.Items.Add(ocKey, new PTFContext());
                    return HttpContext.Current.Items[ocKey] as PTFContext;
                }
            }
        }

        partial void OnContextCreated()
        {
            //Current = this;
        }

        protected override void Dispose(bool disposing)
        {
            var ocKey = string.Concat("ctx1_", HttpContext.Current.GetHashCode().ToString("x"));
            HttpContext.Current.Items.Remove(ocKey);
            base.Dispose(disposing);
        }
    }

    partial class ISO_ptf : INamedObject
    {
        public static string GetNameByID(int id)
        {
            var ctx = PTFContext.Current;
            Debug.Assert(ctx != null, "No default context");

            return (id == 0) ? "" : ctx.ISO_ptf.FirstOrDefault(iso => iso.iso_number == id).SafeGetValue("iso_country", "");
        }

        public static ISO_ptf GetUserDefault(UserDetail user)
        {
            var ctx = PTFContext.Current;
            Debug.Assert(ctx != null, "No default context");
            return ctx.ISO_ptf.FirstOrDefault(i => i.iso_number == user.Ud_iso_id);
        }

        public string GetName()
        {
            return this.iso_country;
        }

        public Guid GetID()
        {
            return CommonTools.ToGuid(this.iso_number);
        }
    }

    partial class HeadOffice : INamedObject
    {
        public IQueryable<Branch> Branches
        {
            get
            {
                return CommonTools.Cached<IQueryable<Branch>>(string.Concat(this.ho_iso_id, " ", this.ho_id, "HeadOffice.Branches"),
                    () =>
                    {
                        var ctx1 = PTFContext.Current;
                        Debug.Assert(ctx1 != null, "No default context");
                        return ctx1.Branches.Where(br => br.br_iso_id == this.ho_iso_id && br.br_ho_id == this.ho_id);
                    });
            }
        }

        public static HeadOffice GetUserDefault(UserDetail user)
        {
            var ctx1 = PTFContext.Current;
            Debug.Assert(ctx1 != null, "No default context");
            return ctx1.HeadOffices.FirstOrDefault(ho => ho.ho_iso_id == user.Ud_iso_id && ho.ho_id == user.Ud_ho_id);
        }

        public static string GetNameByID(int iso_id, int ho_id)
        {
            var ctx1 = PTFContext.Current;
            Debug.Assert(ctx1 != null, "No default context");
            return ctx1.HeadOffices.FirstOrDefault(ho => ho.ho_iso_id == iso_id && ho.ho_id == ho_id).SafeGetValue("ho_name", "");
        }

        public string GetName()
        {
            return this.ho_name;
        }

        public Guid GetID()
        {
            return CommonTools.ToGuid(this.ho_iso_id, this.ho_id, 0, 0);
        }
    }

    partial class Branch : INamedObject
    {
        public string FullName
        {
            get
            {
                return string.Format("{0} - {1} - {2} - {3}", this.br_name, this.br_add_1, this.br_add_2, this.br_add_city);
            }
        }

        public string FullName2
        {
            get
            {
                return string.Format("{0} - {1} - {2}", this.br_add_1, this.br_add_2, this.br_add_city);
            }
        }

        public HeadOffice HeadOffice
        {
            get
            {
                var ctx = PTFContext.Current;
                return ctx.HeadOffices.FirstOrDefault(ho => ho.ho_iso_id == this.br_iso_id && ho.ho_id == this.br_ho_id);
            }
        }

        public static Branch GetUserDefault(UserDetail user)
        {
            var ctx = PTFContext.Current;
            Debug.Assert(ctx != null, "No default context");
            return ctx.Branches.FirstOrDefault(br => br.br_iso_id == user.Ud_iso_id && br.br_ho_id == user.Ud_ho_id && br.br_id == user.Ud_br_id);
        }

        public static string GetNameByID(int iso_id, int ho_id, int br_id)
        {
            var ctx = PTFContext.Current;
            Debug.Assert(ctx != null, "No default context");
            return ctx.Branches.FirstOrDefault(br => br.br_iso_id == iso_id && br.br_ho_id == ho_id && br.br_id == br_id)
                .SafeGetValue("br_name", "");
        }

        public static string GetFullNameByID(int iso_id, int ho_id, params int[] br_id)
        {
            var ctx = PTFContext.Current;
            Debug.Assert(ctx != null, "No default context");

            int count = br_id.Count();
            Debug.Assert(count > 0, "No retailers");

            if (count == 1)
            {
                int id = br_id.First();
                return ctx.Branches.FirstOrDefault(br => br.br_iso_id == iso_id && br.br_ho_id == ho_id && br.br_id == id).SafeGetValue("FullName", "");
            }
            else if (count == ctx.Branches.Count())
            {
                return "ALL";
            }
            else
            {
                NumberToEnglish en = new NumberToEnglish();
                return en.ChangeNumericToWords(count, "Selected ");
            }
        }

        public static List<Branch> FindForPermissions(UserDetail user)
        {
            var ctx1 = PTFContext.Current;
            var ctx2 = PTFReportsContext.Current;
            var branches = new List<Branch>();

            var userBracnhes = ctx1.Branches.Where(br => br.br_iso_id == user.Ud_iso_id).ToList();

            foreach (var pr in ctx2.Permissions.Where(p => p.UserID == user.Ud_id))
                branches.AddRange(userBracnhes.Where(br => br.br_iso_id == pr.IsoID && br.br_ho_id == pr.HoID && br.br_id == pr.RetailerID));

            userBracnhes.Clear();

            return branches.OrderBy(b=>b.br_name).ToList();
        }

        public string GetName()
        {
            return this.FullName2;
        }

        public Guid GetID()
        {
            return CommonTools.ToGuid(this.br_iso_id, this.br_ho_id, this.br_id, 0);
        }
    }
}
