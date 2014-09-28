/***************************************************
//  Copyright (c) Premium Tax Free 2014
***************************************************/

using System;
using System.Collections;
using System.Collections.Specialized;

namespace FintraxServiceManager
{
	public enum State
	{
		New,
		Delete,
		Running
	}
	/// <summary>
	/// Class to represent a service specified in the Services.xml file. This class is used by the ServiceManager.
	/// </summary>
	public class TypeParam
	{
		//Service name
		private string serviceName = "";
		//Type to instantiate
		private string typeToRun = "";
		//Method to invoke from the type specified above
		private string methodToRun = "";
		//Parameters to be passed to the invoked method
		private string parameters = "";
		//State of the type object. Could be New, Delete or Running
		private State state = State.New;

		public string ServiceName
		{
			get{return serviceName;}
		}

		public string Type
		{
			get{ return typeToRun;}
		}
		
		public string Method
		{
			get{ return methodToRun;}
		}
		

		public string Parameters
		{
			get{ return parameters;}
		}

		public State TypeState
		{
			get{return state;}
			set{ state = value;}
		}
		
		public TypeParam()
		{
        
        }
			
		public TypeParam(string serviceNameIn, string typeIn, string methodIn, string parametersIn)
		{
			this.serviceName = serviceNameIn;
			this.typeToRun = typeIn;
			this.methodToRun = methodIn;
			this.parameters = parametersIn;
			this.TypeState = State.New;
		}

		/// <summary>
		/// Overrirde the Object.Equals() method. Implements logic to do a value-comparison of two TypeParam objects 
		/// </summary>
		/// <param name="obj">The TypeParam object to compared to</param>
		/// <returns>Bool parameter. true if equal else false</returns>
		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;

			TypeParam t = obj as TypeParam;
			if(t == null)
				return false;

			//Two TypeParam objects match only if the type to instantiate and the methods to invoke are exactly same
			if (string.Equals(typeToRun, t.Type) && string.Equals(methodToRun, t.Method))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Override the Object.GetHashCode() method. This needs to be done since the Equals object is being overriden.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			string uniqueString = ToString();
			return uniqueString.GetHashCode();
		}
	}

	/// <summary>
	/// Collection which holds TypeParam objects
	/// </summary>
	public class TypeParamCollection : CollectionBase
	{
		protected override void OnInsert(int index, object value)
		{
			try
			{
				TypeParam typeParam = (TypeParam)value;
			}
			catch (FormatException ex)
			{
				throw new ArgumentException(" Argument not of type TypeParam", "value", ex);
			}
		}

		/// <summary>
		/// Insert an instance of a TypeParam object into the collection
		/// </summary>
		/// <param name="t">The instance of TypeParam to be inserted</param>
		public void Insert(TypeParam t)
		{
			foreach(TypeParam obj in this.List)
			{
				if((obj.Equals(t)) && (obj.TypeState == State.Running))
				{
					//No need to re-enter this object into the collection as there is a service alreay running
					return;
				}
			}
			this.List.Add(t);
		}

		/// <summary>
		/// Remove an instance of a TypeParam object from the collection
		/// </summary>
		/// <param name="t"></param>
		public void Remove(TypeParam t)
		{
			this.List.Remove(t);
		}

		/// <summary>
		/// Remove all the objects from the collection.
		/// </summary>
		public void RemoveAll()
		{
			this.List.Clear();
		}
	}
}
