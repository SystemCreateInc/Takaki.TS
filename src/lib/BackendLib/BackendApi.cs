using System;
using System.Runtime.InteropServices;

namespace BackendLib
{
    public class BackendApi
    {
        [DllImport("backend64.dll", CharSet = CharSet.Ansi)]
        public static extern int BackendInit(ref IntPtr bes, int clientType, string name, string capability, string user, string passwd, string backendAddress);

        [DllImport("backend64.dll")]
        public static extern int BackendTerm(IntPtr bes);

        [DllImport("backend64.dll", CharSet = CharSet.Ansi)]
        public static extern int BackendCallByName(
                    IntPtr bes,
                    string capability,		    /*	送信先のキャパビリティー		*/
                    int message,				/*	アプリケーション定義メッセージ	*/
                    int sendsize,				/*	送信データサイズ				*/
                    byte[] sendbuf,	        	/*	送信データ						*/
                    out int recvsize,			/*	受信データサイズ				*/
                    [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] out byte[] recvbuf  		/*	受信データ						*/
            );

        [DllImport("backend64.dll")]
        public static extern int BackendGetMessage(IntPtr bes, out int from_clientid, out int message, out int size, out IntPtr data);

        [DllImport("backend64.dll")]
        public static extern int BackendCancel(IntPtr bes);

        [DllImport("backend64.dll")]
        public static extern int BackendPostData(
                    IntPtr bes,
                    int handle,		            /*	送信先のキャパビリティー		*/
                    short status,				/*	ステータス                       */
                    int message,				/*	アプリケーション定義メッセージ	*/
                    int sendsize,				/*	送信データサイズ				*/
                    byte[]? sendbuf	        	/*	送信データ						*/
            );

        [DllImport("backend64.dll")]
        public static extern int BackendPostDataByName(
                    IntPtr bes,
                    string capability,		    /*	送信先のキャパビリティー		*/
                    short status,				/*	ステータス                       */
                    int message,				/*	アプリケーション定義メッセージ	*/
                    int sendsize,				/*	送信データサイズ				*/
                    byte[] sendbuf	        	/*	送信データ						*/
            );
    }
}
