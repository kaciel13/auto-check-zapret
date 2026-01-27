using System;
using System.Collections.Generic;
using System.Text;

namespace auto_check_zapret
{
    partial class VersionChecker
    {

        private List<string> oldVersions = new List<string>
        {
            "1.6.0",
            "1.6.1",
            "1.6.4",
            "1.6.5",
            "1.6.6",
            "1.7.1",
        };

        private List<string> newVersions = new List<string>
        {
            "1.7.2",
            "1.7.2b",
            "1.8.0",
            "1.8.1",
            "1.8.2",
            "1.8.3",
            "1.8.4",
            "1.8.5",
            "1.9.0",
            "1.9.0b",
            "1.9.1",
            "1.9.2",
            "1.9.3",
        };

        public string Check(string version)
        {
            if (newVersions.Contains(version)) 
                return "new";


            if (oldVersions.Contains(version))
                return "old";

            return "notsupported";

        }
        
    }
}
