/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Serialization;
using System.Diagnostics;

namespace PremierTaxFree.PTFLib.Net
{
    public static class DBConfigValue
    {
        /// <summary>
        /// Safely reads value from config datatable and deserializes it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static T ReadSf<T>(string key, T @default)
        {
            try
            {
                ObjectSerializer serializer = new ObjectSerializer(true);
                var buffer = ClientDataAccess.SelectConfigValue(key);
                return serializer.Deserialize<T>(buffer);
            }
            catch
            {
                return @default;
            }
        }

        /// <summary>
        /// Serializes value and saves it into config table
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void Save(string key, object data)
        {
            ObjectSerializer serializer = new ObjectSerializer(true);
            var buffer = serializer.Serialize(data);
            ClientDataAccess.InsertConfigValue(key, buffer);
        }
    }
}
