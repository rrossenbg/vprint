/*****************************************************************************************
* Service Control Manager (v1.1)
* Written by:- Sumit Sengupta
* Date:- 16/08/2006
*****************************************************************************************/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Configuration;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

namespace FintraxServiceManager
{
	/// <summary>
	/// Service class which runs as a windwos service and triggers services specified in the xml config file. 
	/// The services are run based on the time specified in the config file.
	/// Multiple services can be run simultaneously by putting an entry in the Services.xml file.
	/// </summary>
	public class FintraxServiceManager : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		string serviceFile = "";
		XPathDocument docNav;
		XPathNavigator nav ;
		XPathNodeIterator nodeIterator1;
		XPathNodeIterator nodeIterator2;
		XPathNodeIterator nodeIterator3;
		XPathNodeIterator nodeIterator4;
		XPathNodeIterator nodeIterator5;
		String strExpression = "count(/Services/Service)";
		System.Timers.Timer timer = new System.Timers.Timer(30000);
		System.Diagnostics.EventLog eventLog = new EventLog();
		string logPath = Convert.ToString( ConfigurationSettings.AppSettings["LogPath"]);
		TypeParamCollection typeParamColl = new TypeParamCollection();

		public FintraxServiceManager()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

			this.timer.Elapsed +=new System.Timers.ElapsedEventHandler(timer_Elapsed);
			CreateEventSource();
			eventLog.Source = "FintraxServiceManager";
			serviceFile = Convert.ToString( ConfigurationSettings.AppSettings["ServiceFilePath"]);
			eventLog.WriteEntry("Service file path:" + serviceFile);
			docNav = new XPathDocument(serviceFile);
		}

		private void CreateEventSource()
		{
			// Create the source, if it does not already exist.
			if(!EventLog.SourceExists("FintraxServiceManager"))
			{
				// An event log source should not be created and immediately used.
				// There is a latency time to enable the source, it should be created
				// prior to executing the application that uses the source.
				// Execute this sample a second time to use the new source.
				EventLog.CreateEventSource("FintraxServiceManager", "FintraxServiceManagerLog");
				Console.WriteLine("CreatingEventSource");
				Console.WriteLine("Exiting, execute the application a second time to use the source.");
				// The source is created.  Exit the application to allow it to be registered.
				//return;
			}
                
			// Create an EventLog instance and assign its source.
			
			eventLog.Source = "FintraxServiceManager";
        
			// Write an informational entry to the event log.    
			eventLog.WriteEntry("Writing to event log.");

		}

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
	
			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new FintraxServiceManager() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// FintraxServiceManager
			// 
			this.ServiceName = "FintraxServiceManager";
			this.timer.Enabled = true;
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			eventLog.WriteEntry("Starting FintraxServiceManager v1.1");
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			eventLog.WriteEntry("Stopping FintraxServiceManager v1.1");
		}

		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine(e.SignalTime.ToString());
			nav = docNav.CreateNavigator();
			string count = nav.Evaluate(strExpression).ToString();
			string name = "";
			string time = "";
			string type = "";
			string method = "";
			string parameters = "";
			string str = "";

			CleanUpCollection();
			
			for(int i = 1; i <= int.Parse(count); i++)
			{
				name = "";
				time = "";
				type = "";
				method = "";
				parameters = "";
				str = "//Services//Service[position()=" + i + "]";

				//~1. Get the Name
				#region Get Name
				string nameExpr = str + "//name";
				nodeIterator1 = nav.Select(nameExpr);
				while(nodeIterator1.MoveNext())
				{
					name = nodeIterator1.Current.Value;
				}
				#endregion
				
				//2. Get the Time(s).
				#region Get Time
				string timeExpr = str + "//time";
				nodeIterator2 = nav.Select(timeExpr);
				while(nodeIterator2.MoveNext())
				{
					time = nodeIterator2.Current.Value;
				}
				string [] times = time.Split(',');//There can be more than one time specified.
					
				#endregion

				//3. Get the Type to run.
				#region Get Type to instantiate
				string typeExpr = str + "//type";
				nodeIterator3 = nav.Select(typeExpr);
				while(nodeIterator3.MoveNext())
				{
					type = nodeIterator3.Current.Value;
				}
				#endregion

				//4. Get the Method to run.
				#region Get the method to run
				string methodExpr = str + "//method";
				nodeIterator4 = nav.Select(methodExpr);
				while(nodeIterator4.MoveNext())
				{
					method = nodeIterator4.Current.Value;
				}
				#endregion

				//5. Get the parameters to run.

				#region Parameters to pass to the method
				string parameterExpr = str + "//parameters";
				nodeIterator5 = nav.Select(parameterExpr);
				while(nodeIterator5.MoveNext())
				{
					parameters = nodeIterator5.Current.Value;
				}
				#endregion

				foreach(string timeToRun in times)
				{
					System.Diagnostics.Trace.WriteLine("Time to run:" + timeToRun);

					if((DateTime.Parse(timeToRun).Hour == System.DateTime.Now.Hour) &&
						(DateTime.Parse(timeToRun).Minute == System.DateTime.Now.Minute))
					{
						System.Diagnostics.Trace.WriteLine("Storing Types to run in Collection");
						//Store the service to run , in a collection...
						TypeParam typeParam = new TypeParam(name, type, method, parameters);
						
						System.Diagnostics.Trace.WriteLine("Type: " + name);

						//If an object of this Type already exists in the collection, then do not insert 
						//a new one.
						typeParamColl.Insert(typeParam);
					}
				}
			}//~ end of for loop thru the XML

			if(typeParamColl.Count > 0)
			{
				//Invoke each of the service.
				System.Diagnostics.Trace.WriteLine("Calling Run()...");
				Run();
			}
		}

		/// <summary>
		/// Run each of the Service/Type in the Collection
		/// </summary>
		public void Run()
		{
			//Type is of the form assembly,classname.
			//Ex:- DiData.Ptf.Business,VoucherEntryAndModification.
			
			foreach(TypeParam t in typeParamColl)
			{
				//Run a service ONLY if the status is "New". If its marked for deletion OR "Running"
				//then no point in running it again...

				if(t.TypeState == State.New)
				{
					try
					{
						string [] typeName = t.Type.Split(',');
						string assembly = typeName[0];
						string typeToInstantiate = typeName[1];
				
						Assembly asm = Assembly.LoadFile(assembly);
						Type class1 = asm.GetType(typeToInstantiate);

						Object obj = Activator.CreateInstance(class1);
						//Object obj = System.Activator.CreateInstance(assembly,typeToInstantiate);

						MethodInfo mi = class1.GetMethod(t.Method);
						// Invoke method ('null' for no parameters).

						//Get the parameters
						string [] p = t.Parameters.Split(',');
						object [] paramArray = new object[p.Length];
					
						//Set the state of the object to "Running".
						t.TypeState = State.Running;

						if(t.Parameters != String.Empty)
						{
							for(int cnt = 0; cnt < paramArray.Length; cnt++)
							{
								string [] p2 = p[cnt].Split('-');
								switch(p2[1])
								{
									case "string":	paramArray[cnt] = p2[0];
										break;
									case "int": paramArray[cnt] = System.Convert.ToInt32(p2[0]);
										break;
									case "bool": paramArray[cnt] = System.Convert.ToBoolean(p2[0]);
										break;
									case "double": paramArray[cnt] = System.Convert.ToDouble(p2[0]);
										break;
									case "char": paramArray[cnt] = System.Convert.ToChar(p2[0]);
										break;
									case "decimal": paramArray[cnt] = System.Convert.ToDecimal(p2[0]);
										break;
								}
							}
							System.Diagnostics.Trace.WriteLine("Invoking method" + t.Method);
							mi.Invoke(obj, paramArray);
						}
						else
						{
							mi.Invoke(obj, null);
						}
						//Finished running.... Mark this type to be removed from the collection.
						t.TypeState = State.Delete;
					}
					catch (Exception ex)
					{
						//this try catch is a backup if the logging to event log fails(maybe due to log being full..
						try
						{
							eventLog.WriteEntry("Exception in service: " + t.ServiceName, EventLogEntryType.Error);
							eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
						}
						catch
						{ 
							// do nothing. Ignore
						}
						finally
						{
							System.IO.StreamWriter logFile = new System.IO.StreamWriter(logPath + String.Format("{0:dd_MM_yyyy}",System.DateTime.Now) + "_FintraxServiceManager.log",false);
							logFile.WriteLine(System.DateTime.Now + ": Service = " + t.ServiceName);
							logFile.WriteLine(System.DateTime.Now + ": " + ex.Message);
							logFile.WriteLine(System.DateTime.Now + ": " + ex.StackTrace);
							logFile.Close();
						}
					}
				}
				else
				{
					//Do not run the service.
				}
			}//~ end of FOREACH				
		} 

		private void CleanUpCollection()
		{
			if(typeParamColl.Count > 0)
			{
				foreach(TypeParam t in typeParamColl)
				{
					if(t.TypeState == State.Delete)
					{
						System.Diagnostics.Trace.WriteLine("Removing: " + t.ServiceName);
						typeParamColl.Remove(t);
					}
				}
			}
		}

		#region commented out - DELETE LATER!!!

		/*
		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine(e.SignalTime.ToString());
			nav = docNav.CreateNavigator();
			string count = nav.Evaluate(strExpression).ToString();

			for(int i = 1; i <= int.Parse(count); i++)
			{
				string name = "";
				string time = "";
				string type = "";
				string method = "";
				string parameters = "";
				string str = "//Services//Service[position()=" + i + "]";

				//~1. Get the Name
				#region Get Name
				string nameExpr = str + "//name";
				nodeIterator1 = nav.Select(nameExpr);
				while(nodeIterator1.MoveNext())
				{
					name = nodeIterator1.Current.Value;
				}
				#endregion
				
				//2. Get the Time.
				#region Get Time
				string timeExpr = str + "//time";
				nodeIterator2 = nav.Select(timeExpr);
				while(nodeIterator2.MoveNext())
				{
					time = nodeIterator2.Current.Value;
				}
				#endregion

				//3. Get the Type to run.
				#region Get Type to instantiate
				string typeExpr = str + "//type";
				nodeIterator3 = nav.Select(typeExpr);
				while(nodeIterator3.MoveNext())
				{
					type = nodeIterator3.Current.Value;
				}
				#endregion

				//4. Get the Method to run.
				#region Get the method to run
				string methodExpr = str + "//method";
				nodeIterator4 = nav.Select(methodExpr);
				while(nodeIterator4.MoveNext())
				{
					method = nodeIterator4.Current.Value;
				}
				#endregion

				//5. Get the parameters to run.

				#region Parameters to pass to the method
				string parameterExpr = str + "//parameters";
				nodeIterator5 = nav.Select(parameterExpr);
				while(nodeIterator5.MoveNext())
				{
					parameters = nodeIterator5.Current.Value;
				}
				#endregion

			
#if DEBUG
				Console.WriteLine("Name: {0} Time: {1} Type: {2} Method: {3}",name,time, type, method);
#endif

				//~Now run the method.
				Console.WriteLine(System.Convert.ToDateTime(time));
				Console.WriteLine(System.DateTime.Now);

				if(System.DateTime.Now == System.Convert.ToDateTime(time))
				{
					//Type is of the form assembly,classname.
					//Ex:- DiData.Ptf.Business,VoucherEntryAndModification.

					string [] typeName = type.Split(',');
					string assembly = typeName[0];
					string typeToInstantiate = typeName[1];
					try
					{
						Assembly asm = Assembly.Load(assembly);
						Type class1 = asm.GetType(typeToInstantiate);

						Object obj = Activator.CreateInstance(class1);
						//Object obj = System.Activator.CreateInstance(assembly,typeToInstantiate);

						MethodInfo mi = class1.GetMethod(method);
						// Invoke method ('null' for no parameters).

						//Get the parameters
						string [] p = parameters.Split(',');
						object [] paramArray = new object[p.Length];
						for(int cnt = 0; cnt < paramArray.Length; cnt++)
						{
							string [] p2 = p[cnt].Split('-');
							switch(p2[1])
							{
								case "string":	paramArray[cnt] = p2[0];
									break;
								case "int": paramArray[cnt] = System.Convert.ToInt32(p2[0]);
									break;
								case "bool": paramArray[cnt] = System.Convert.ToBoolean(p2[0]);
									break;
								case "double": paramArray[cnt] = System.Convert.ToDouble(p2[0]);
									break;
								case "char": paramArray[cnt] = System.Convert.ToChar(p2[0]);
									break;
								case "decimal": paramArray[cnt] = System.Convert.ToDecimal(p2[0]);
									break;
							}
						}
				
						mi.Invoke(obj, paramArray);
					}
					catch (Exception ex)
					{
						eventLog.WriteEntry(name + " exception: " + ex.Message);
					}
						
				}
				else
				{
					//Continue. Time to run this service doesn't match current time.
				}
			}
		}
		*/
		#endregion
	}
}
