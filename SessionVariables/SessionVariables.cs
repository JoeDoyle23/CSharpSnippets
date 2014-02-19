using System;
using System.Collections.Generic;

namespace JoeDoyle.CSharpSnippets.SessionVariables
{
    public class SessionVariables : TypedSession<SessionVariables>
    {
		//Session Variables go here
        public string UserName {get; set;}
    }
}