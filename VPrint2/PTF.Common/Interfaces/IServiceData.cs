
using System.Collections;

namespace VPrinting.Interfaces
{
    public interface IServiceData
    {
        object RetrievePtfOfficeDetail(int iso, int hoid);
        object RetrieveRetailerDetail(int iso, int rid);
        object[] RetrieveTableData(string fieldsList, string tableName, string where);
    }

    public delegate ArrayList GetMetaDataDelegate(IServiceData servdata, int iso, int hoid, int reId);
}
