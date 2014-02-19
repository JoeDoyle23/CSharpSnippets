using System.Web;

namespace JoeDoyle.CSharpSnippets.SessionVariables
{
    [CoverageExclude]
    public class TypedSession<T> where T : TypedSession<T>, new()
    {
        private static HttpSessionStateBase _session;

        public static HttpSessionStateBase Session
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return new HttpSessionStateWrapper(HttpContext.Current.Session);
                }

                return _session;
            }
            set
            {
                _session = value;
            }
        }

        private static string Key
        {
            get { return typeof(TypedSession<T>).FullName; }
        }

        private static T Value
        {
            get { return (T)Session[Key]; }
            set { Session[Key] = value; }
        }

        public static T Current
        {
            get
            {
                var instance = Value;
                if (instance == null)
                    lock (typeof(T)) // not ideal to lock on a type -- but it'll work
                    {
                        // standard lock double-check
                        instance = Value;
                        if (instance == null)
                            Value = instance = new T();
                    }
                return instance;
            }
            set { Value = value; }
        }

        public static bool IsAvailable
        {
            get { return HttpContext.Current != null && HttpContext.Current.Session != null; }
        }
    }
}