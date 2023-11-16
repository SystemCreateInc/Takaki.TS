using LogLib;
using SyslogCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib
{
    public class ScopeLogger
    {
        private Stack<string> _scopes = new Stack<string>();
        private string _scopeText = "";

        public ScopeLogger(string scope)
        {
            PushScope(scope);
        }

        public IDisposable BeginScope(string scope)
        {
            PushScope(scope);
            return new ScopeHolder(this);
        }

        public void Debug(string text) => ScopeLog(Severity.Debug, text);

        public void Info(string text) => ScopeLog(Severity.Info, text);

        public void Notice(string text) => ScopeLog(Severity.Notice, text);

        public void Warn(string text) => ScopeLog(Severity.Warn, text);

        public void Error(string text) => ScopeLog(Severity.Err, text);

        public void Crit(string text) => ScopeLog(Severity.Crit, text);

        public void Alert(string text) => ScopeLog(Severity.Alert, text);

        public void Emerg(string text) => ScopeLog(Severity.Emerg, text);

        private void ScopeLog(Severity severity, string text) => Syslog.SLPrintf(severity, $"{_scopeText}: {text}");

        private class ScopeHolder : IDisposable
        {
            private ScopeLogger _logger;

            public ScopeHolder(ScopeLogger logger)
            {
                _logger = logger;
            }

            public void Dispose()
            {
                _logger.PopScope();
            }
        }

        private void PushScope(string scope)
        {
            _scopes.Push(scope);
            CreateScopeText();
        }

        private void PopScope()
        {
            _scopes.Pop();
            CreateScopeText();
        }

        private void CreateScopeText()
        {
            _scopeText = string.Join(":", _scopes.Reverse());
        }
    }

    public class ScopeLogger<T> : ScopeLogger
    {
        public ScopeLogger() : base(typeof(T).Name)
        {
        }
    }

}
