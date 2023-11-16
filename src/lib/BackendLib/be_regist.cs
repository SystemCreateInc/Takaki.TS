using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct be_regist
    {
        public short clienttype;                /*	登録クライアントタイプ	*/
        public int uid;                         /*	uid						*/
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
        public string name;                     /*	名称					*/
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string capability;               /*	能力					*/
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string user;                     /*	ユーザー				*/
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string passwd;                   /*	パスワード				*/
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string address;				    /*	アドレス				*/
    }
}
