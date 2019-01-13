using System;


namespace extargsparse
{
	public class ExtArgsOptions : _LogObject
	{
		private static Dictionary<string,object> m_defdict = new Dictionary<string,object>{
			{"prog", Environment.GetCommandLineArgs().Length > 0 ? Environment.GetCommandLineArgs()[0] : ""},
			{"usage", ""},
			{"description", ""},
			{"epilog", ""},
			{"version","0.0.1"},
			{"errorhandler" , "exit"},
			{"helphandler", null},
			{"longprefix", "--"},
			{"shortprefix", "-"},
			{"nohelpoption", false},
			{"nojsonoption", false},
			{"helplong", "help"},
			{"helpshort", "h"},
			{"jsonlong", "json"},
			{"cmdprefixadded", true},
			{"parseall", true},
			{"screenwidth", 80},
			{"flagnochange", false}
		};

		private Dictionary<string,object> m_dict;

		private void __set_options(Dictionary<string,object> setting)
		{
			foreach (KeyValuePair<string,object> kv in setting) {
				this.m_dict[kv.Key] = kv.Value;
			}
			return;
		}

		public ExtArgsOptions(Dictionary<string,object> setting) : _LogObject()
		{
			this.__set_options(ExtArgsOptions.m_defdict);
			this.__set_options(setting);
		}

		public ExtArgsOptions(string setting) : _LogObject()
		{
			
		}
	}
}