using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    public class ADBException
    {

    }
    public class PermissionDeniedException : ApplicationException
    {
        public PermissionDeniedException()
            : base("Permission to access the specified resourcee was denied")
        { }
    }
    public class DoneWithReadException : ApplicationException
    {
        public DoneWithReadException()
            : base("Done with read")
        {

        }
    }
    public class ADBConnectionException : ApplicationException
    {
        public ADBConnectionException()
            : base("Adb Connection Error.")
        {

        }
    }
}
