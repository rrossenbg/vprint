/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Data;
using System.Data.SqlClient;

namespace CardCodeCover
{
    public interface ILoadable
    {
        ILoadable Load(SqlDataReader reader);
    }
}
