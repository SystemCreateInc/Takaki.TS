using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WindowLib.Utils
{
    public class WaitCursor : IDisposable
    {
        public WaitCursor()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public WaitCursor(Cursor cursor)
        {
            Mouse.OverrideCursor = cursor;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = null;
        }
    }
}
