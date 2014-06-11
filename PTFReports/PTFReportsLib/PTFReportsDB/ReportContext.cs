/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

//#define NOPASS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using PTF.Reports.PTFDB;

namespace PTF.Reports.PTFReportsDB
{
    partial class PTFReportsContext
    {
        public static Hashtable BlockedIPsTable = Hashtable.Synchronized(new Hashtable(StringComparer.CurrentCultureIgnoreCase));

        public static PTFReportsContext Current
        {
            get
            {
                Debug.Assert(HttpContext.Current != null, "Context is null");

                lock (typeof(PTFReportsContext))
                {
                    var ocKey = string.Concat("ctx2_", HttpContext.Current.GetHashCode().ToString("x"));
                    if (!HttpContext.Current.Items.Contains(ocKey))
                        HttpContext.Current.Items.Add(ocKey, new PTFReportsContext());
                    return HttpContext.Current.Items[ocKey] as PTFReportsContext;
                }
            }
        }

        partial void OnContextCreated()
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            var ocKey = string.Concat("ctx2_", HttpContext.Current.GetHashCode().ToString("x"));
            HttpContext.Current.Items.Remove(ocKey);
            base.Dispose(disposing);
        }

        public bool ValidateUser(string pass, UserDetail user)
        {
            if (user == null)
                return false;
#if NOPASS
            return true;
#endif
            if (pass.IsNullEmptyOrWhite())
                return false;
            pass = pass.Encript();
            return (user.Ud_password == pass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login">Non-case sensitive username</param>
        /// <param name="pass">Case sensitive password</param>
        /// <param name="user">Found UserObject</param>
        /// <returns></returns>
        public bool ValidateUser(string login, string pass, out UserDetail user)
        {
            user = null;
            if (login.IsNullEmptyOrWhite())
                return false;
            var lname = login.ToLowerInvariant();
            user = this.UserDetails.FirstOrDefault(u => u.Ud_loginName.ToLowerInvariant() == login);
            if (user == null)
                return false;
#if NOPASS
            return true;
#endif
            if (pass.IsNullEmptyOrWhite())
                return false;
            pass = pass.Encript();
            return (user.Ud_password == pass);
        }

        public bool IsUserBlocked(string login, out UserDetail user)
        {
            user = this.UserDetails.FirstOrDefault(u => u.Ud_loginName == login);
            if (user == null)
                return false;

            return (user.BlockedAt.HasValue);
        }

        public bool SetPasswordForgotten(string userName, string email)
        {
            var user = this.UserDetails.FirstOrDefault(ud => ud.Ud_loginName == userName && ud.Ud_email == email);
            if (user != null)
            {
                user.Ud_forgottenPassword = true;
                this.SaveChanges();
                return true;
            }
            return false;

        }

        public UserDetail FindUser(string login, string pass)
        {
            pass = pass.Encript();
#if NOPASS
            UserDetail user = this.UserDetails.FirstOrDefault(u => u.Ud_loginName == login);
#else
            UserDetail user = this.UserDetails.FirstOrDefault(u => u.Ud_loginName == login && u.Ud_password == pass);
#endif
            return user;
        }

        public UserDetail FindUserByEmail(string login, string email)
        {
            UserDetail user = this.UserDetails.FirstOrDefault(u => u.Ud_loginName == login && u.Ud_email == email);
            return user;
        }

        public void SaveError(Exception ex)
        {
            try
            {
                var lastWeek = DateTime.Now.AddDays(-7);
                this.Errors.RemoveAllWhere(e => e.Date < lastWeek);
                var msg = ex.Message.SubString(255);
                var typ = ex.GetType().Name.SubString(255);
                var err = ex.ToXml();
                this.AddToErrors(new Error()
                {
                    Message = msg,
                    Type = typ,
                    Error1 = err,
                    Date = DateTime.Now
                });
                this.SaveChanges();
            }
            catch (Exception ex2)
            {
                //No farther error processing
                Trace.WriteLine(ex);
                Trace.WriteLine(ex2);
            }
        }
    }

    partial class UserDetail
    {
        public UserDetail Origin { get; private set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.Ud_firstName, this.Ud_lastName);
            }
        }

        public bool IsAdmin
        {
            get
            {
                return this.Ud_userType == 1;
            }
        }

        public bool IsLoggedAs
        {
            get
            {
                return Origin != null;
            }
        }

        private HeadOffice m_HeadOffice;
        public HeadOffice HeadOffice
        {
            get
            {
                if (m_HeadOffice == null)
                    m_HeadOffice = GetCompany();
                return m_HeadOffice;
            }
        }

        private Branch m_Branch;
        public Branch Branch
        {
            get
            {
                if (m_Branch == null)
                    m_Branch = GetBranch();
                return m_Branch;
            }
        }

        public void Add()
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            ctx2.AddToUserDetails(this);
            ctx2.SaveChanges();
        }

        public void Update()
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            this.EntityKey = new EntityKey("Users", "UserID", this.Ud_id);
            ctx2.Attach(this);
            ctx2.ObjectStateManager.ChangeObjectState(this, EntityState.Modified);
        }

        public void LogAs(UserDetail origin)
        {
            Origin = origin;
        }

        public static string GetFullNameById(int userdId)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            if (ctx2 == null)
                return "na";
            return (string)ctx2.UserDetails.First(u => u.Ud_id == userdId).SafeGetValue("FullName", "na");
        }

        public static UserDetail GetByID(int userId)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            var user = ctx2.UserDetails.FirstOrDefault(p => p.Ud_id == userId);
            return user;
        }

        public HeadOffice GetCompany()
        {
            var ctx1 = PTFContext.Current;
            var company = ctx1.HeadOffices.FirstOrDefault(ho => ho.ho_iso_id == this.Ud_iso_id &&
                ho.ho_id == this.Ud_ho_id);
            return company;
        }

        public Branch GetBranch()
        {
            var ctx1 = PTFContext.Current;
            var branch = ctx1.Branches.FirstOrDefault(br => br.br_iso_id == this.Ud_iso_id &&
                br.br_ho_id == this.Ud_ho_id && br.br_id == this._Ud_br_id);
            return branch;
        }
    }

    partial class Folder
    {
        public static IEnumerable<Folder> GetByParentId(Guid? item)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            foreach (var folder in ctx2.Folders.Where(f =>
                (!f.ParentID.HasValue && !item.HasValue) || (f.ParentID == item)))
                yield return folder;
        }

        public static IEnumerable<Folder> GetByParentId(Guid? item, UserDetail user)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            foreach (var folder in user.Folders.Where(f => (!f.ParentID.HasValue && !item.HasValue) || (f.ParentID == item)))
                yield return folder;
        }

        public bool HasChildren()
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            bool result = ctx2.Folders.Any(f => f.ParentID == FolderID) || ctx2.Reports.Any(r => r.FolderID == FolderID);
            return result;
        }

        public bool HasChildren(UserDetail user)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            bool result = user.Folders.Any(f => f.ParentID == FolderID) || ctx2.Reports.Any(r => r.FolderID == FolderID);
            return result;
        }

        public static string GetNameById(Guid? folderId)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            if (ctx2 == null)
                return "na";
            return (string)ctx2.Folders.First(u => u.FolderID == folderId).SafeGetValue("Name", "na");
        }

        public string FullPath
        {
            get
            {
                StringBuilder b = new StringBuilder();

                Folder folder = this;

                while (folder != null)
                {
                    b.Insert(0, string.Format("{0}/", folder.Name));
                    folder = folder.Folder1;
                }

                return b.ToString();
            }
        }
    }

    partial class Report
    {
        public static IEnumerable<Report> GetByParentId(Guid? item)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            foreach (var report in ctx2.Reports.Where(r => r.FolderID == item))
                yield return report;
        }

        public static string GetNameById(Guid? reportId)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            if (ctx2 == null)
                return "na";
            return (string)ctx2.Reports.First(u => u.ReportID == reportId).SafeGetValue("Name", "na");
        }
    }

    partial class Parameter
    {
        public static List<Parameter> Default = new List<Parameter>();

        public bool CanMapTo(ParamMapping mapping)
        {
            return this.ParamType == (int)mapping;
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Name) && !string.IsNullOrWhiteSpace(this.Text);
            }
        }
    }

    partial class Permission
    {
        public static void AddSafe(int userId, int isoId, int compId, int? branchId)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");

            if (ctx2.Permissions.FirstOrDefault(
                    p => p.UserID == userId &&
                    p.IsoID == isoId &&
                    p.HoID == compId &&
                    p.RetailerID == branchId) == null)
            {
                ctx2.AddToPermissions(new Permission()
                {
                    UserID = userId,
                    IsoID = isoId,
                    HoID = compId,
                    RetailerID = branchId,
                });
            }
        }
    }

    partial class Session
    {
        public static Session GetByID(string sessionId)
        {
            var ctx2 = PTFReportsContext.Current;
            Debug.Assert(ctx2 != null, "Context problem");
            return ctx2.Sessions.FirstOrDefault(s => s.BrowserSessionID == sessionId);
        }
    }
}