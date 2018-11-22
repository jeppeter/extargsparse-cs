using System;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System.Diagnostics;

namespace extargsparse
{
	public class _LogObject
	{
		private ILog m_logger;

		private string get_namespace(int stknum)
		{
			StackTrace stk = new StackTrace();
			MethodBase info = stk.GetFrame(stknum).GetMethod();
			return info.ReflectedType.Namespace;
		}

		private bool _create_repository(string name)
		{
			var repos = LogManager.GetAllRepositories();
			foreach (var c in repos) {
				if (c.Name == name) {
					return false;
				}
			}
			LogManager.CreateRepository(name);
			return true;
		}

		public _LogObject(string cmdname="extargsparse")
		{
			string curnamespace,callernamespace;
			Logger l;
			string lvlstr = "ERROR";
			string appname = String.Format("{0}_APPENDER", cmdname).ToUpper();
			ConsoleAppender app=null;
			/*now first to get the class*/
			curnamespace = get_namespace(1);
			callernamespace = get_namespace(2);
			if (curnamespace != callernamespace) {
				throw new ParseException(String.Format("can not be call directly by outer namespace [{0}]", curnamespace));
			}
			this._create_repository(cmdname);
			if (LogManager.Exists(cmdname,cmdname) == null) {

			} else {

			}


		}
	}

}