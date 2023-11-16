using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorLib.Service
{
    public class ServiceManager : IDisposable
    {
        private IntPtr _scHandle;
        private bool _disposed = false;

        public ServiceManager() 
        {
            _scHandle = WinSvcApi.OpenSCManager(null, null, (uint)SCM_ACCESS.SC_MANAGER_ALL_ACCESS);
            if (_scHandle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                WinSvcApi.CloseServiceHandle(_scHandle);
                _disposed = true;
            }
        }

        public bool CreateService(string serviceName, string displayName, string path)
        {
            var hService = WinSvcApi.CreateService(
                _scHandle, 
                serviceName, 
                displayName,
                (uint)SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                (uint)SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS,
                (uint)SERVICE_START.SERVICE_AUTO_START,
                (uint)SERVICE_ERROR.SERVICE_ERROR_NORMAL,
                path,
                null,
                null,
                null,
                null,
                null);

            WinSvcApi.CloseServiceHandle(hService);

            return hService != IntPtr.Zero;
        }

        public bool UnregisterService(string serviceName) 
        { 
            var hService = WinSvcApi.OpenService(_scHandle, serviceName, (uint)SERVICE_ACCESS.SERVICE_ALL_ACCESS);
            if (hService == IntPtr.Zero)
            {
                return false;
            }

            WinSvcApi.DeleteService(hService);
            WinSvcApi.CloseServiceHandle(hService);

            return true;

        }
    }
}
