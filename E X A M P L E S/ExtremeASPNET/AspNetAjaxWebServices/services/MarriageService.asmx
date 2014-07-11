<%@ WebService Language="C#" Class="MsdnMagazine.MarriageService" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Microsoft.Web.Script.Services;
using MsdnMagazine.SampleTypes;

namespace MsdnMagazine
{
    [WebService(Namespace = "http://msdnmagazine.com/ws")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    [GenerateScriptType(typeof(Person))]
    public class MarriageService : WebService
    {
        // Marry method takes two Person objects modifies their
        // marital status and last names to reflect their married status
        //
        [WebMethod]
        public Person[] Marry(Person[] couple)
        {
            if (couple.Length != 2)
                throw new ArgumentException("Array of Person[] must contain exactly 2 Person references");
            if (couple[0].Married || couple[1].Married)
                throw new ApplicationException("Persons entering marriage cannot already be married!");
            
            couple[0].LastName += "-" + couple[1].LastName;
            couple[1].LastName = couple[0].LastName;
            couple[0].Married = couple[1].Married = true;
            return couple;
        }
    }
}
