using System;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

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
			string lvlstr = "ERROR";
			string appname = String.Format("{0}_APPENDER", cmdname).ToUpper();
			ConsoleAppender app=null;
			string lvlstr="DEBUG";
			string appname = String.Format("{0}_APPENDER", name).ToUpper();
			ConsoleAppender app=null;
			this._create_repository(name);
			if (LogManager.Exists(name,name) != null) {
				this.m_logger = LogManager.GetLogger(name,name);
				l = (Logger) this.m_logger.Logger;
				l.Level = l.Hierarchy.LevelMap[lvlstr];
			} else {
				PatternLayout patternLayout = new PatternLayout();
				Hierarchy hr;
				patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
				patternLayout.ActivateOptions();
				this.m_logger = LogManager.GetLogger(name,name);
				l = (Logger) this.m_logger.Logger;
				l.Level = l.Hierarchy.LevelMap[lvlstr];
				app = new ConsoleAppender();
				app.Name = appname;
				app.Layout = patternLayout;
				//app.Target = "Console.Error";
				l.AddAppender(app);
				hr = (Hierarchy)LogManager.GetRepository(name);
				hr.Configured = true;
			}
		}
		private interface _NC
		{
			int Count();
			void Add(object o);
			void Clear();
		}

		private class _NList<T> : _NC
		{
			private List<T> m_list = new List<T>();

			int _NC.Count()
			{
				return this.m_list.Count;
			}

			void _NC.Add(object o)
			{
				if (o is T) {
					this.m_list.Add((T)o);
				} else {
					throw new ParseException(String.Format("not type of  [{0}]", typeof(T).FullName));
				}
			}

			void _NC.Clear()
			{
				this.m_list.Clear();
			}

			T[] ToArray()
			{
				return this.m_list.ToArray();
			}
		}

		private void __add_object(_NC arrlist,params object[] oarr)
		{
			int i;
			if (oarr.Length > 0) {
				for (i=0;i < oarr.Length;i++) {
					arrlist.Add(oarr[i]);
				}
			}
			return;
		}

		private _NC __make_arrays(string typename)
		{
			Type t = Type.GetType(typename);
			Type[] typeargs = {t};
			Type tlist = typeof(_NList<>);
			Type me = tlist.MakeGenericType(typeargs);
			_NC arrlist = Activator.CreateInstance(me) as _NC;
			if (arrlist == null) {
				throw new ParseException(String.Format("can not create generic for [{0}]", typename));
			}
			return arrlist;
		}

		private object __get_arrays(_NC arrlist)
		{
			MethodInfo minfo;
			minfo = arrlist.GetType().GetMethod("ToArray", BindingFlags.NonPublic | BindingFlags.Instance);
			if (minfo == null) {
				throw new ParseException(String.Format("no ToArray"));
			}
			return (object) minfo.Invoke(arrlist,new object[0]);
		}


		private void __throw_exception(string s)
		{
			throw new ParseException(s);
		}

		private object[] _get_param_args(object[] args,ParameterInfo[] paraminfos)
		{
			object[] newargs;
			object[] lastargs;
			ParameterInfo lastparam;
			Type lasttype;
			int i,j;
			bool bsucc;
			_NC cc = null;
			if (args.Length > paraminfos.Length) {
				newargs = new object[paraminfos.Length];
				for (i=0;i < (paraminfos.Length - 1) ; i++) {
					if (!(args[i].GetType().IsSubclassOf(paraminfos[i].ParameterType) || args[i].GetType().Equals(paraminfos[i].ParameterType))) {
						this.__throw_exception(String.Format("[{0}] not subclass of [{1}] [{2}]", i, paraminfos[i].ParameterType.Name, args[i].GetType().Name));
					}
					newargs[i] = args[i];
				}

				lastparam = paraminfos[(paraminfos.Length - 1)];
				if (!lastparam.ParameterType.IsArray){
					this.__throw_exception(String.Format("last param not array"));
				}
				lasttype = lastparam.ParameterType.GetElementType();
				lastargs = new object[(args.Length - paraminfos.Length+1)];
				cc = this.__make_arrays(lastparam.ParameterType.GetElementType().FullName);
				for (i=(paraminfos.Length - 1),j=0;i<args.Length;i++,j ++) {
					if (!(args[i].GetType().IsSubclassOf(lasttype) || args[i].GetType().Equals(lasttype))) {
						this.__throw_exception(String.Format("[{0}] not subclass of [{1}] [{2}]", i, lastparam.ParameterType.Name,args[i].GetType().Name));
					}
					this.__add_object(cc,args[i]);
				}
				newargs[(paraminfos.Length - 1)] = this.__get_arrays(cc);
			} else if (args.Length < paraminfos.Length) {
				newargs = new object[paraminfos.Length];
				for (i=0; i < args.Length; i++) {
					if (!(args[i].GetType().IsSubclassOf(paraminfos[i].ParameterType) || args[i].GetType().Equals(paraminfos[i].ParameterType))) {
						this.__throw_exception(String.Format("[{0}] not subclass of [{1}]", i, paraminfos[i].ParameterType.Name));
					}
					newargs[i] = args[i];
				}
				for (i=args.Length;i < paraminfos.Length ;i ++) {
					bsucc = false;
					if (!paraminfos[i].HasDefaultValue){
						if (i==(paraminfos.Length - 1)) {
							if (paraminfos[i].ParameterType.IsArray) {
								bsucc = true;
								cc = this.__make_arrays(paraminfos[i].ParameterType.GetElementType().FullName);
								newargs[i] = this.__get_arrays(cc);
							}
						}
					} else {
						bsucc = true;
						newargs[i] = paraminfos[i].DefaultValue;
					}
					if (!bsucc) {
						this.__throw_exception(String.Format("[{0}] param not default", i));
					}
					
				}
			} else {
				/*that is equal*/
				newargs = args;
				for (i=0;i< args.Length ;i ++) {
					if (! (args[i].GetType().IsSubclassOf(paraminfos[i].ParameterType) || args[i].GetType().Equals(paraminfos[i].ParameterType) )) {
						bsucc = false;
						if (i == (args.Length - 1) && paraminfos[i].ParameterType.IsArray) {
							if (args[i].GetType().IsSubclassOf(paraminfos[i].ParameterType.GetElementType()) || 
								args[i].GetType().Equals(paraminfos[i].ParameterType.GetElementType())) {
								bsucc = true;
								cc = this.__make_arrays(paraminfos[i].ParameterType.GetElementType().FullName);
								this.__add_object(cc,args[i]);
								newargs[i] = this.__get_arrays(cc);
							}
						}
						if (! bsucc) {
							this.__throw_exception(String.Format("[{0}] not subclass of [{1}] [{2}]", i, paraminfos[i].ParameterType.Name, args[i].GetType().Name));	
						}
						
					}
				}
			}
			return newargs;
		}

		private MethodBase _check_funcname(Type tinfo,string funcname, params  object[] args)
		{
			MethodInfo[] infos;
			ParameterInfo[] paraminfos;
			List<MethodInfo> okmeths = new List<MethodInfo>();
			infos = tinfo.GetMethods();
			foreach (var curmeth in infos) {
				if (curmeth.Name == funcname) {
					paraminfos= curmeth.GetParameters();
					try {
						this._get_param_args(args,paraminfos);
						okmeths.Add(curmeth);
					}
					catch(ParseException e) {
						Console.Error.WriteLine("catch error [{0}]", e);
					}
				}
			}

			if (okmeths.Count == 0) {
				return null;
			}
			return okmeths[0];

		}

		private MethodBase _call_func_inner(string dllname, string nspc, string clsname,string fname, params object[] args)
		{
			int i,j;
			StackFrame frm;
			StackTrace stk;
			MethodBase curbase;
			MethodBase meth;
			Type curtype;
			string bindname;
			Assembly asbl;
			string dl;
			string[] sarr;
			if (dllname.Length <= 0 && 
				nspc.Length <= 0 && 
				clsname.Length <= 0) {
				/*this Method by call function*/
				stk = new StackTrace();
				for (i=0;i<stk.FrameCount;i++) {
					frm = stk.GetFrame(i);
					curbase = frm.GetMethod();
					curtype = curbase.DeclaringType;
					curtype = Type.GetType(curtype.FullName);
					meth = this._check_funcname(curtype,fname, args);
					if (meth != null) {
						return meth;
					}
				}
			} else if (dllname.Length <= 0 && 
				nspc.Length <= 0) {
				stk = new StackTrace();
				for (i=0;i < stk.FrameCount;i++) {
					frm = stk.GetFrame(i);
					curbase = frm.GetMethod();
					curtype = curbase.DeclaringType;
					sarr = curtype.FullName.Split('.');
					nspc = "";
					for (j=0;j < (sarr.Length - 1); j++) {
						if (nspc.Length > 0) {
							nspc += ".";
						}
						nspc += sarr[j];
					}
					curtype = Type.GetType(String.Format("{0}.{1}", nspc,clsname));
					if (curtype == null) {
						continue;
					}
					meth = this._check_funcname(curtype, fname, args);
					if (meth != null) {
						return meth;
					}
				}
			} else if (dllname.Length <= 0) {
				bindname = String.Format("{0}.{1}", nspc, clsname);
				curtype = Type.GetType(bindname);
				meth = this._check_funcname(curtype, fname, args);
				if (meth != null) {
					return meth;
				}
			} else {
				bindname = String.Format("{0}.{1}", nspc, clsname);
				dl = String.Format("{0}.dll",dllname);
				asbl = Assembly.LoadFrom(dl);
				if (asbl == null) {
					return null;
				}
				curtype = asbl.GetType(bindname,false,true);
				if (curtype == null) {
					return null;
				}
				meth = this._check_funcname(curtype,fname,args);
				if (meth != null){
					return meth;
				}
			}
			return null;
		}

		public object call_func(string funcname, params object[] args)
		{
			string[] sarr;
			string namespc = "";
			MethodBase meth = null;
			int i;
			object[] newargs;
			ParameterInfo[] paraminfos;
			if (funcname.Length == 0) {
				this.__throw_exception(String.Format("null funcname can not accept"));
			}

			sarr = funcname.Split('.');
			if (sarr.Length == 1) {
				meth = this._call_func_inner("","","",funcname,args);
			} else if (sarr.Length == 2) {
				/*this is the class name and function name*/
				meth = this._call_func_inner("","",sarr[0],sarr[1],args);
			} else if (sarr.Length == 3) {
				/*this is the namespace name and class name and function name*/
				meth = this._call_func_inner("",sarr[0],sarr[1],sarr[2],args);
			} else if (sarr.Length == 4) {
				meth = this._call_func_inner(sarr[0],sarr[1],sarr[2],sarr[3],args);
				if (meth == null) {
					namespc = String.Format("{0}.{1}", sarr[0],sarr[1]);
					meth = this._call_func_inner("",namespc, sarr[2],sarr[3],args);
				}
			} else {
				namespc =  "";
				for (i=1; i < (sarr.Length - 2) ;i ++) {
					if (namespc.Length > 0) {
						namespc += ".";
					}
					namespc += sarr[i];					
				}

				meth = this._call_func_inner(sarr[0], namespc, sarr[sarr.Length - 2], sarr[sarr.Length - 1]);
				if (meth == null) {
					i = 0;
					namespc = "";
					for (i = 0; i < (sarr.Length - 2) ;i ++) {
						if (namespc.Length > 0) {
							namespc += ".";
						}
						namespc += sarr[i];					
					}
					meth = this._call_func_inner("", namespc, sarr[sarr.Length - 2], sarr[sarr.Length - 1]);
				}
			}

			if (meth == null) {
				this.__throw_exception(String.Format("can not find [{0}] method", funcname));
			}
			paraminfos = meth.GetParameters();
			newargs = this._get_param_args(args,paraminfos);
			return meth.Invoke(null,newargs);
		}
	}

}