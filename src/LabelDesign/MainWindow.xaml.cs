using LabelLib;
using LargeDistLabelLib;
using LogLib;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace LabelDesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Syslog.Init();
            address.Text = "192.168.10.124";
        }

        private void Print(IEnumerable<string> datas, string address)
        {
            try
            {
                using (var sock = new TcpClient())
                {
                    sock.Connect(address, 1024);
                    using (var stream = sock.GetStream())
                    {
                        foreach (var data in datas)
                        {
                            Syslog.Debug($"print: {data}");

                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            var bytes = Encoding.GetEncoding("SJIS").GetBytes(data);
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public class Param : ILabelBuilderParam
        {
            public string Code { get; set; } = "";
            public bool IsVertical { get; set; } = false;
        }

        private void LargeDist_Click(object sender, RoutedEventArgs e)
        {
            var prm = new LargeDistLabel
            {
                DtDelivery = "2018/10/15",
                CdJuchuBin = "001",
                CdBlock = "6",
                CdDistGroup = "02001",
                NmDistGroup = "広島1便(通常)",
                CdShukkaBatch = "02001",
                NmShukkaBatch = "広島常温1便",
                CdHimban = "000022499",
                CdJan = "4904730002302",
                NmHinSeishikimei = "ミルクフランス",
                NuBoxUnit = 12,
                BoxPs = 1,
                BaraPs = 2,
                TotalPs = 14,
            };
            var data = new LargeDistLabelLib.LabelBuilder().Build(prm);
            Print(data, address.Text);
        }
    }
}