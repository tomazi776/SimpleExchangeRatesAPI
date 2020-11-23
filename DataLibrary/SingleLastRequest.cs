using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class SingleLastRequest
    {
        public string Endpoint { get; set; }
        //public HashSet<string> LookupKeys { get; set; } = new HashSet<string>();
        public List<string> LookupKeys { get; set; } = new List<string>();
        public HashSet<string> Holidays { get; set; } = new HashSet<string>();

        public List<string> RequestParameters { get; set; }
        public TimeSpan LastRequestAbsoluteExpireTime { get; set; }


        private static readonly object padlock = new object();
        public static SingleLastRequest instance = null;
        public static SingleLastRequest Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance ?? (instance = new SingleLastRequest());
                }
            }
        }
    }
}
