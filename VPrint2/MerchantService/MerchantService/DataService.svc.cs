/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using ReceivingServiceLib;
using VPrinting;

namespace MerchantService
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/hh556232(v=vs.110).aspx
    /// </summary>
    [ErrorHandlingBehavior(ExceptionToFaultConverter = typeof(MyServiceFaultProvider))]
    public class DataService : IDataService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        #region TRS

        public int TRSExecuteNonQuery(ArrayList sqlCommand)
        {
            try
            {
                DataAccess access = new DataAccess();
                var result = access.TRSExecuteNonQuery(sqlCommand);
                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public object TRSExecuteScalar(ArrayList sqlCommand)
        {
            try
            {
                DataAccess access = new DataAccess();
                var result = access.TRSExecuteScalar(sqlCommand);
                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public Datatable TRSExecuteReader(ArrayList sqlCommand)
        {
            try
            {
                DataAccess access = new DataAccess();
                var result = access.TRSExecuteReader(sqlCommand);
                return new Datatable() { Data = result.ToXml() };
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

        #region Images

        public int ImagesExecuteNonQuery(ArrayList sqlCommand)
        {
            try
            {
                DataAccess access = new DataAccess();
                var result = access.ImagesExecuteNonQuery(sqlCommand);
                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public object ImagesExecuteScalar(ArrayList sqlCommand)
        {
            try
            {
                DataAccess access = new DataAccess();
                var result = access.ImagesExecuteScalar(sqlCommand);
                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public Datatable ImagesExecuteReader(ArrayList sqlCommand)
        {
            try
            {
                DataAccess access = new DataAccess();
                var result = access.ImagesExecuteReader(sqlCommand);
                return new Datatable() { Data = result.ToXml() };
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public SelectVoucherInfo SelectVoucherInfo1(int Id)
        {
            try
            {
                DataAccess access = new DataAccess();
                var result = access.SelectVoucherInfo(Id);
                return new SelectVoucherInfo(result);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public SelectVoucherInfo SelectVoucherInfo2(int iso_id, int v_number)
        {
            try
            {
                DataAccess access = new DataAccess();
                var result = access.SelectVoucherInfo(iso_id, v_number);
                return new SelectVoucherInfo(result);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #region USER

        public List<UserInfo> SelectAllUsers()
        {
            try
            {
                UserDataAccess access = new UserDataAccess();
                var result = access.SelectAllUsers();
                return result.ConvertAll<UserInfo>(d => new UserInfo(d));
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<UserInfo> SelectAllUsersByCountry(int isoId)
        {
            try
            {
                UserDataAccess access = new UserDataAccess();
                var result = access.SelectAllUsersByCountry(isoId);
                return result.ConvertAll<UserInfo>(d => new UserInfo(d));
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<UserInfo> SelectAllUsersByBranches(int isoId, params int[] branchIds)
        {
            try
            {
                UserDataAccess access = new UserDataAccess();
                var result = access.SelectAllUsersByBranches(isoId, branchIds);
                return result.ConvertAll<UserInfo>(d => new UserInfo(d));
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public UserInfo SelectUserById(int userId)
        {
            try
            {
                UserDataAccess access = new UserDataAccess();
                var result = access.SelectUserById(userId);
                return new UserInfo(result);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void UpdateUser(UserInfo data)
        {
            try
            {
                UserDataAccess access = new UserDataAccess();
                access.UpdateUser((UserDataAccess.User_Data)data);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void RemoveUser(int us_id)
        {
            try
            {
                UserDataAccess access = new UserDataAccess();
                access.RemoveUser(us_id);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void SetUserActive(int us_id, bool active)
        {
            try
            {
                UserDataAccess access = new UserDataAccess();
                access.SetUserActive(us_id, active);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void UpdateUserRight(List<RightInfo> infolist)
        {
            try
            {
                UserDataAccess access = new UserDataAccess();
                access.UpdateUserRight(infolist.ConvertAll((i) => (UserDataAccess.Right_Data)i));
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

        #endregion
    }
}