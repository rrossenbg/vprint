/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

namespace PremierTaxFree.PTFLib.Data
{
    public enum eMessageTypes
    {
        //The user was informed
        Info = 0,
        //The user was warned
        Warning = 1,
        //User operation was completed 
        //despite of incompleted data
        Exclamation = 2,
        //User operation was stopped 
        //because of invalid data
        Stop = 3,
        //Error occurred into the system
        //No restart is neeeded
        Error = 4,
        //Error occurred into the system
        //Immediate restart is required
        Critical = 5,
    }

    public enum eSources
    {
        VScan = 0,
        TransferringService = 1,
        ReceivingService = 2,
        ClientSQL = 3,
        ServerSQL = 4,
    }
}
