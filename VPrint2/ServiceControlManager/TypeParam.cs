/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/
using System;

namespace FintraxServiceManager
{
	public class TypeParam
	{
        public string ServiceName { get; private set; }
        public string Type { get; private set; }
        public string Method { get; set; }
        public string Parameters { get; set; }
        public TimeSpan Time { get; private set; }
		
		public TypeParam()
		{
        }

        public TypeParam(string serviceNameIn, string typeIn, string methodIn, string parametersIn, TimeSpan time)
        {
            this.ServiceName = serviceNameIn;
            this.Type = typeIn;
            this.Method = methodIn;
            this.Parameters = parametersIn;
            this.Time = time;
        }
	}
}
