using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorLib.Service
{
    /// <summary>
    /// Access to the service. Before granting the requested access, the
    /// system checks the access token of the calling process.
    /// </summary>
    [Flags]
    internal enum SERVICE_ACCESS : uint
    {
        /// <summary>
        /// Required to call the QueryServiceConfig and
        /// QueryServiceConfig2 functions to query the service configuration.
        /// </summary>
        SERVICE_QUERY_CONFIG = 0x00001,

        /// <summary>
        /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function
        /// to change the service configuration. Because this grants the caller
        /// the right to change the executable file that the system runs,
        /// it should be granted only to administrators.
        /// </summary>
        SERVICE_CHANGE_CONFIG = 0x00002,

        /// <summary>
        /// Required to call the QueryServiceStatusEx function to ask the service
        /// control manager about the status of the service.
        /// </summary>
        SERVICE_QUERY_STATUS = 0x00004,

        /// <summary>
        /// Required to call the EnumDependentServices function to enumerate all
        /// the services dependent on the service.
        /// </summary>
        SERVICE_ENUMERATE_DEPENDENTS = 0x00008,

        /// <summary>
        /// Required to call the StartService function to start the service.
        /// </summary>
        SERVICE_START = 0x00010,

        /// <summary>
        ///     Required to call the ControlService function to stop the service.
        /// </summary>
        SERVICE_STOP = 0x00020,

        /// <summary>
        /// Required to call the ControlService function to pause or continue
        /// the service.
        /// </summary>
        SERVICE_PAUSE_CONTINUE = 0x00040,

        /// <summary>
        /// Required to call the EnumDependentServices function to enumerate all
        /// the services dependent on the service.
        /// </summary>
        SERVICE_INTERROGATE = 0x00080,

        /// <summary>
        /// Required to call the ControlService function to specify a user-defined
        /// control code.
        /// </summary>
        SERVICE_USER_DEFINED_CONTROL = 0x00100,

        /// <summary>
        /// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.
        /// </summary>
        SERVICE_ALL_ACCESS = (ACCESS_MASK.STANDARD_RIGHTS_REQUIRED |
            SERVICE_QUERY_CONFIG |
            SERVICE_CHANGE_CONFIG |
            SERVICE_QUERY_STATUS |
            SERVICE_ENUMERATE_DEPENDENTS |
            SERVICE_START |
            SERVICE_STOP |
            SERVICE_PAUSE_CONTINUE |
            SERVICE_INTERROGATE |
            SERVICE_USER_DEFINED_CONTROL),

        GENERIC_READ = ACCESS_MASK.STANDARD_RIGHTS_READ |
            SERVICE_QUERY_CONFIG |
            SERVICE_QUERY_STATUS |
            SERVICE_INTERROGATE |
            SERVICE_ENUMERATE_DEPENDENTS,

        GENERIC_WRITE = ACCESS_MASK.STANDARD_RIGHTS_WRITE |
            SERVICE_CHANGE_CONFIG,

        GENERIC_EXECUTE = ACCESS_MASK.STANDARD_RIGHTS_EXECUTE |
            SERVICE_START |
            SERVICE_STOP |
            SERVICE_PAUSE_CONTINUE |
            SERVICE_USER_DEFINED_CONTROL,

        /// <summary>
        /// Required to call the QueryServiceObjectSecurity or
        /// SetServiceObjectSecurity function to access the SACL. The proper
        /// way to obtain this access is to enable the SE_SECURITY_NAME
        /// privilege in the caller's current access token, open the handle
        /// for ACCESS_SYSTEM_SECURITY access, and then disable the privilege.
        /// </summary>
        ACCESS_SYSTEM_SECURITY = ACCESS_MASK.ACCESS_SYSTEM_SECURITY,

        /// <summary>
        /// Required to call the DeleteService function to delete the service.
        /// </summary>
        DELETE = ACCESS_MASK.DELETE,

        /// <summary>
        /// Required to call the QueryServiceObjectSecurity function to query
        /// the security descriptor of the service object.
        /// </summary>
        READ_CONTROL = ACCESS_MASK.READ_CONTROL,

        /// <summary>
        /// Required to call the SetServiceObjectSecurity function to modify
        /// the Dacl member of the service object's security descriptor.
        /// </summary>
        WRITE_DAC = ACCESS_MASK.WRITE_DAC,

        /// <summary>
        /// Required to call the SetServiceObjectSecurity function to modify
        /// the Owner and Group members of the service object's security
        /// descriptor.
        /// </summary>
        WRITE_OWNER = ACCESS_MASK.WRITE_OWNER,
    }

    /// <summary>
    /// Service types.
    /// </summary>
    [Flags]
    public enum SERVICE_TYPE : uint
    {
        /// <summary>
        /// Driver service.
        /// </summary>
        SERVICE_KERNEL_DRIVER = 0x00000001,

        /// <summary>
        /// File system driver service.
        /// </summary>
        SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,

        /// <summary>
        /// Service that runs in its own process.
        /// </summary>
        SERVICE_WIN32_OWN_PROCESS = 0x00000010,

        /// <summary>
        /// Service that shares a process with one or more other services.
        /// </summary>
        SERVICE_WIN32_SHARE_PROCESS = 0x00000020,

        /// <summary>
        /// The service can interact with the desktop.
        /// </summary>
        SERVICE_INTERACTIVE_PROCESS = 0x00000100,
    }

    /// <summary>
    /// Service start options
    /// </summary>
    public enum SERVICE_START : uint
    {
        /// <summary>
        /// A device driver started by the system loader. This value is valid
        /// only for driver services.
        /// </summary>
        SERVICE_BOOT_START = 0x00000000,

        /// <summary>
        /// A device driver started by the IoInitSystem function. This value
        /// is valid only for driver services.
        /// </summary>
        SERVICE_SYSTEM_START = 0x00000001,

        /// <summary>
        /// A service started automatically by the service control manager
        /// during system startup. For more information, see Automatically
        /// Starting Services.
        /// </summary>        
        SERVICE_AUTO_START = 0x00000002,

        /// <summary>
        /// A service started by the service control manager when a process
        /// calls the StartService function. For more information, see
        /// Starting Services on Demand.
        /// </summary>
        SERVICE_DEMAND_START = 0x00000003,

        /// <summary>
        /// A service that cannot be started. Attempts to start the service
        /// result in the error code ERROR_SERVICE_DISABLED.
        /// </summary>
        SERVICE_DISABLED = 0x00000004,
    }
}
