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
		private string m_longprefix;
		private string m_shortprefix;
		private bool m_nohelpoption;
		private bool m_nojsonoption;
		private string m_helplong;
		private string m_helpshort;
		private string m_jsonlong;
		private bool m_cmdprefixadded;
		private string m_errorhandler;

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
			this.m_maincmd = new _ParserCompact(null,options);
			this.m_helphandler = this.m_options.get_string("helphandler");
			this.m_outputmode = new List<string>(); 
			this.m_ended = 0;
			this.m_longprefix = options.get_string("longprefix");
			this.m_shortprefix = options.get_string("shortprefix");
			this.m_nohelpoption = options.get_bool("nohelpoption");
			this.m_nojsonoption = options.get_bool("nojsonoption");
			this.m_helplong = options.get_string("helplong");
			this.m_helpshort = options.get_string("helpshort");
			this.m_jsonlong = options.get_string("jsonlong");
			this.m_cmdprefixadded = options.get_bool("cmdprefixadded");
			this.m_errorhandler = options.get_string("errorhandler");
		}
	}

}