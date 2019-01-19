using System;
using System.Collections.Generic;

namespace extargsparse
{
	public class  ExtArgsParse  : _LogObject
	{
		private static Priority[]  m_defprior = {Priority.COMMAND_SET,Priority.SUB_COMMAND_JSON_SET,Priority.COMMAND_JSON_SET,Priority.ENVIRONMENT_SET,Priority.ENV_SUB_COMMAND_JSON_SET,Priority.ENV_COMMAND_JSON_SET,Priority.DEFAULT_SET};
		private Priority[] m_priority;
		private ExtArgsOptions  m_options;
		private _ParserCompact  m_maincmd;
		private string m_helphandler;
		private List<string> m_outputmode;
		private int m_ended;

		public ExtArgsParse(ExtArgsOptions options=null, object priority=null) : base()
		{
			string vtype;
			if (priority != null) {
				vtype = priority.GetType().FullName;
				if (vtype != "extargsparse.Priority") {
					this.throw_exception(String.Format("second parameter not Priority type"));
				}
				this.m_priority = (Priority[])priority;
			}  else {
				this.m_priority =  ExtArgsParse.m_defprior;
			}

			if (options == null)  {
				options = new ExtArgsOptions("{}");
			}
			this.m_options =  options;
			this.m_maincmd = new _ParseCompact(null,options);
			this.m_helphandler = this.m_options.get_string("helphandler");
			this.m_outputmode = new List<string>(); 
			this.m_ended = 0;
		}
	}

}