/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MerchantService
{
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        string GetData(int value);

        #region TRS

        [OperationContract]
        int TRSExecuteNonQuery(ArrayList sqlCommand);

        [OperationContract]
        object TRSExecuteScalar(ArrayList sqlCommand);

        [OperationContract]
        Datatable TRSExecuteReader(ArrayList sqlCommand);

        #endregion

        #region Images

        #region COMMON

        [OperationContract]
        int ImagesExecuteNonQuery(ArrayList sqlCommand);

        [OperationContract]
        object ImagesExecuteScalar(ArrayList sqlCommand);

        [OperationContract]
        Datatable ImagesExecuteReader(ArrayList sqlCommand);

        #endregion

        #region VOUCHER

        [OperationContract]
        SelectVoucherInfo SelectVoucherInfo1(int Id);

        [OperationContract]
        SelectVoucherInfo SelectVoucherInfo2(int iso_id, int v_number);

        #endregion

        #region USER

        [OperationContract]
        List<UserInfo> SelectAllUsers();

        [OperationContract]
        List<UserInfo> SelectAllUsersByCountry(int isoId);

        [OperationContract]
        List<UserInfo> SelectAllUsersByBranches(int isoId, params int[] branchIds);

        [OperationContract]
        UserInfo SelectUserById(int userId);

        [OperationContract]
        void UpdateUser(UserInfo data);

        [OperationContract]
        void RemoveUser(int us_id);

        [OperationContract]
        void SetUserActive(int us_id, bool active);

        [OperationContract]
        void UpdateUserRight(List<RightInfo> infolist);

        #endregion

        #endregion
    }

    [DataContract]
    public class Datatable
    {
        [DataMember]
        public string Data { get; set; }
    }

    [DataContract]
    public class SelectVoucherInfo
    {
        [DataMember]
        public int iso_id { get; set; }
        [DataMember]
        public int branch_id { get; set; }
        [DataMember]
        public int v_number { get; set; }
        [DataMember]
        public string sitecode { get; set; }
        [DataMember]
        public int location { get; set; }
        [DataMember]
        public string session_Id { get; set; }

        public SelectVoucherInfo()
        {
        }

        public SelectVoucherInfo(DataAccess.SelectVoucherInfo_Data data)
        {
            if (data != null)
            {
                iso_id = data.iso_id;
                branch_id = data.branch_id;
                v_number = data.v_number;
                sitecode = data.sitecode;
                location = data.location;
                session_Id = data.session_Id;
            }
        }
    }

    [DataContract]
    public class RightInfo
    {
        [DataMember]
        public int r_id { get; set; }
        [DataMember]
        public int r_us_id { get; set; }
        [DataMember]
        public int r_ho_iso_id { get; set; }
        [DataMember]
        public int r_ho_id { get; set; }
        [DataMember]
        public int r_br_id { get; set; }
        [DataMember]
        public bool r_active { get; set; }
        [DataMember]
        public int r_granted_by { get; set; }
        [DataMember]
        public DateTime r_granthed_at { get; set; }

        public RightInfo()
        {
        }

        public RightInfo(UserDataAccess.Right_Data data)
        {
            r_id = data.r_id;
            r_us_id = data.r_us_id;
            r_ho_iso_id = data.r_ho_iso_id;
            r_ho_id = data.r_ho_id;
            r_br_id = data.r_br_id;
            r_active = data.r_active;
            r_granted_by = data.r_granted_by;
            r_granthed_at = data.r_granthed_at;
        }

        public static explicit operator UserDataAccess.Right_Data(RightInfo info)
        {
            UserDataAccess.Right_Data data = new UserDataAccess.Right_Data();
            data.r_id = info.r_id;
            data.r_us_id = info.r_us_id;
            data.r_ho_iso_id = info.r_ho_iso_id;
            data.r_ho_id = info.r_ho_id;
            data.r_br_id = info.r_br_id;
            data.r_active = info.r_active;
            data.r_granted_by = info.r_granted_by;
            data.r_granthed_at = info.r_granthed_at;
            return data;
        }
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public int us_id { get; set; }
        [DataMember]
        public int us_iso_id { get; set; }
        [DataMember]
        public string us_first_name { get; set; }
        [DataMember]
        public string us_last_name { get; set; }
        [DataMember]
        public string us_login { get; set; }
        [DataMember]
        public string us_password { get; set; }
        [DataMember]
        public string us_email { get; set; }
        [DataMember]
        public int us_gp_id { get; set; }
        [DataMember]
        public bool us_active { get; set; }
        [DataMember]
        public string us_salt { get; set; }
        [DataMember]
        public int us_br_id { get; set; }

        public UserInfo()
        {
        }

        public UserInfo(UserDataAccess.User_Data data)
        {
            us_id = data.us_id;
            us_iso_id = data.us_iso_id;
            us_first_name = data.us_first_name;
            us_last_name = data.us_last_name;
            us_login = data.us_login;
            us_password = data.us_password;
            us_email = data.us_email;
            us_gp_id = data.us_gp_id;
            us_active = data.us_active;
            us_salt = data.us_salt;
            us_br_id = data.us_br_id;
        }

        public static explicit operator UserDataAccess.User_Data(UserInfo info)
        {
            UserDataAccess.User_Data data = new UserDataAccess.User_Data();
            data.us_id = info.us_id;
            data.us_iso_id = info.us_iso_id;
            data.us_first_name = info.us_first_name;
            data.us_last_name = info.us_last_name;
            data.us_login = info.us_login;
            data.us_password = info.us_password;
            data.us_email = info.us_email;
            data.us_gp_id = info.us_gp_id;
            data.us_active = info.us_active;
            data.us_salt = info.us_salt;
            data.us_br_id = info.us_br_id;
            return data;
        }
    }
}
