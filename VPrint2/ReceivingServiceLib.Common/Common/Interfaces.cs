using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ReceivingServiceLib
{
    public interface IReadable
    {
        void Load(IDataReader reader);
    }

    public interface IDbReadable
    {
        IDbReadable Load(SqlDataReader reader);
    }

    public interface IBinaryReadable
    {
        void Read(BinaryReader stream);
    }   
}
