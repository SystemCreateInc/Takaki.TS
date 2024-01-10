using Picking.Defs;
using Prism.Mvvm;
using SelDistGroupLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImTools.ImMap;

namespace Picking.Models
{
    public class DistBase : BindableBase
    {
        public const int ITEMMAX = 9;
        public const int COLORMAX = 5;


        public long IdDist { get; set; } = 0;

        private int? _inseq = 0;
        public int? InSeq
        {
            get => _inseq;
            set => SetProperty(ref _inseq, value);
        }

        private string _dt_delivery = "";
        public string DtDelivery
        {
            get => _dt_delivery;
            set => SetProperty(ref _dt_delivery, value);
        }

        private string _cd_dist_group = "";
        public string CdDistGroup
        {
            get => _cd_dist_group;
            set => SetProperty(ref _cd_dist_group, value);
        }

        private string _nm_dist_group = "";
        public string NmDistGroup
        {
            get => _nm_dist_group;
            set => SetProperty(ref _nm_dist_group, value);
        }

        private string _cd_shukka_batch = "";
        public string CdShukkaBatch
        {
            get => _cd_shukka_batch;
            set => SetProperty(ref _cd_shukka_batch, value);
        }

        private string _cd_juchu_bin = "";
        public string CdJuchuBin
        {
            get => _cd_juchu_bin;
            set => SetProperty(ref _cd_juchu_bin, value);
        }

        private string _tokuisaki = "";
        public string CdTokuisaki
        {
            get => _tokuisaki;
            set => SetProperty(ref _tokuisaki, value);
        }

        private string _nm_tokuisaki = "";
        public string NmTokuisaki
        {
            get => _nm_tokuisaki;
            set => SetProperty(ref _nm_tokuisaki, value);
        }

        private string _cd_course = "";
        public string CdCourse
        {
            get => _cd_course;
            set => SetProperty(ref _cd_course, value);
        }

        private string _cd_route = "";
        public string CdRoute
        {
            get => _cd_route;
            set => SetProperty(ref _cd_route, value);
        }

        private string _cd_himban = "";
        public string CdHimban
        {
            get => _cd_himban;
            set => SetProperty(ref _cd_himban, value);
        }

        private string _nm_hin_seishikimei = "";
        public string NmHinSeishikimei
        {
            get => _nm_hin_seishikimei;
            set => SetProperty(ref _nm_hin_seishikimei, value);
        }

        private string cd_gtin13 = "";
        public string CdGtin13
        {
            get => cd_gtin13;
            set => SetProperty(ref cd_gtin13, value);
        }

        private string cd_gtin14 = "";
        public string CdGtin14
        {
            get => cd_gtin14;
            set => SetProperty(ref cd_gtin14, value);
        }

        private int _csunit = 0;
        public int Csunit
        {
            get => _csunit;
            set
            {
                SetProperty(ref _csunit, value);
                Display_Csunit = QtyFieldSpace(value);
            }
        }
        public int GetCsunit
        {
            get => Csunit == 0 || Csunit == 1 ? 99999 : Csunit;
        }

        private string _display_csunit = "";
        public string Display_Csunit
        {
            get => _display_csunit;
            set => SetProperty(ref _display_csunit, value);
        }

        private int _ops = 0;
        public int Ops
        {
            get => _ops;
            set
            {
                SetProperty(ref _ops, value);

                Display_Ops = QtyFieldSpace(value);
                Display_Ops_Cs = QtyFieldSpace(value / GetCsunit);
                Display_Ops_Ps = QtyFieldSpace(value % GetCsunit);
            }
        }

        private string _display_ops = "";
        public string Display_Ops
        {
            get => _display_ops;
            set => SetProperty(ref _display_ops, value);
        }

        private int _dops = 0;
        public int Dops
        {
            get => _dops;
            set
            {
                SetProperty(ref _dops, value);

                Display_Dops = QtyFieldSpace(value);
                Display_Dops_Cs = QtyFieldSpace(value / GetCsunit);
                Display_Dops_Ps = QtyFieldSpace(value % GetCsunit);
            }
        }

        private string _display_dops = "";
        public string Display_Dops
        {
            get => _display_dops;
            set => SetProperty(ref _display_dops, value);
        }

        private int _drps = 0;
        public int Drps
        {
            get => _drps;
            set
            {
                SetProperty(ref _drps, value);

                Display_Drps = QtyFieldSpace(value);
                Display_Drps_Cs = QtyFieldSpace(value / GetCsunit);
                Display_Drps_Ps = QtyFieldSpace(value % GetCsunit);
            }
        }

        private string _display_drps = "";
        public string Display_Drps
        {
            get => _display_drps;
            set => SetProperty(ref _display_drps, value);
        }

        private int _ddps = 0;
        public int Ddps
        {
            get => _ddps;
            set
            {
                SetProperty(ref _ddps, value);

                Display_Ddps = QtyFieldSpace(value);
                Display_Ddps_Cs = QtyFieldSpace(value / GetCsunit);
                Display_Ddps_Ps = QtyFieldSpace(value % GetCsunit);
            }
        }

        private string _display_ddps = "";
        public string Display_Ddps
        {
            get => _display_ddps;
            set => SetProperty(ref _display_ddps, value);
        }

        private string _cd_shain = "";
        public string CdShain
        {
            get => _cd_shain;
            set => SetProperty(ref _cd_shain, value);
        }

        private string _nm_shain = "";
        public string NmShain
        {
            get => _nm_shain;
            set => SetProperty(ref _nm_shain, value);
        }

        private int _nu_boxunit = 0;
        public int NuBoxUnit
        {
            get => _nu_boxunit;
            set => SetProperty(ref _nu_boxunit, value);
        }
        private int _st_boxtype = 0;
        public int StBoxType
        {
            get => _st_boxtype;
            set => SetProperty(ref _st_boxtype, value);
        }

        private string _tdunitaddrcode = "";
        public string Tdunitaddrcode
        {
            get => _tdunitaddrcode;
            set => SetProperty(ref _tdunitaddrcode, value);
        }

        private int _tdunitzonecode = 0;
        public int Tdunitzonecode
        {
            get => _tdunitzonecode;
            set => SetProperty(ref _tdunitzonecode, value);
        }

        private DateTime _dworkdt;
        public DateTime Dworkdt
        {
            get => _dworkdt;
            set => SetProperty(ref _dworkdt, value);
        }

        // 色
        private int _distcolor_code = 0;
        public int DistColor_code
        {
            get => _distcolor_code;
            set => SetProperty(ref _distcolor_code, value);
        }

        private string _distcolor_name = "";
        public string DistColor_name
        {
            get => _distcolor_name;
            set => SetProperty(ref _distcolor_name, value);
        }

        private string _distcolor_func_name = "";
        public string DistColor_Func_name
        {
            get => _distcolor_func_name;
            set => SetProperty(ref _distcolor_func_name, value);
        }

        private int _dstatus;
        public int DStatus
        {
            get => _dstatus;
            set
            {
                SetProperty(ref _dstatus, value);

                switch (value)
                {
                    case 0:
                        DStatus_name = "未処理";
                        break;
                    case 1:
                        DStatus_name = "欠品";
                        break;
                    case 2:
                        DStatus_name = "完了";
                        break;
                }
            }
        }

        // 配分タイプ
        private int _disttype;
        public int DistType
        {
            get => _disttype;
            set
            {
                SetProperty(ref _disttype, value);

                switch (_disttype)
                {
                    case (int)DistTypeStatus.Ready:
                        DistType_name = "";
                        break;
                    case (int)DistTypeStatus.Inprog:
                        DistType_name = "欠品";
                        break;
                    case (int)DistTypeStatus.Completed:
                        DistType_name = "完了";
                        break;
                    case (int)DistTypeStatus.DistWait:
                        DistType_name = "準備中";
                        break;
                    case (int)DistTypeStatus.DistWorking:
                        DistType_name = "仕分中";
                        break;
                    case (int)DistTypeStatus.CheckWorking:
                        DistType_name = "検品中";
                        break;
                }
            }
        }
        private string _dstatus_name = "";
        public string DStatus_name
        {
            get => _dstatus_name;
            set => SetProperty(ref _dstatus_name, value);
        }

        private string _disttype_name = "";
        public string DistType_name
        {
            get => _disttype_name;
            set => SetProperty(ref _disttype_name, value);
        }

        private int _lstatus = 0;
        public int LStatus
        {
            get => _lstatus;
            set => SetProperty(ref _lstatus, value);
        }

        // 総店舗数
        private int _order_shop_cnt;
        public int Order_shop_cnt
        {
            get => _order_shop_cnt;
            set
            {
                SetProperty(ref _order_shop_cnt, value);

                DisplayOrderShop_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayordershop_cnt = "";
        public string DisplayOrderShop_cnt
        {
            get => _displayordershop_cnt;
            set => SetProperty(ref _displayordershop_cnt, value);
        }

        // 仕分店舗数
        private int _plan_shop_cnt;
        public int Plan_shop_cnt
        {
            get => _plan_shop_cnt;
            set
            {
                SetProperty(ref _plan_shop_cnt, value);

                DisplayPlanShop_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayplanshop_cnt = "";
        public string DisplayPlanShop_cnt
        {
            get => _displayplanshop_cnt;
            set => SetProperty(ref _displayplanshop_cnt, value);
        }

        // 済み店舗数
        private int _result_shop_cnt;
        public int Result_shop_cnt
        {
            get => _result_shop_cnt;
            set
            {
                SetProperty(ref _result_shop_cnt, value);

                DisplayResultShop_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayresultshop_cnt = "";
        public string DisplayResultShop_cnt
        {
            get => _displayresultshop_cnt;
            set => SetProperty(ref _displayresultshop_cnt, value);
        }

        // 残店舗数
        private int _remain_shop_cnt;
        public int Remain_shop_cnt
        {
            get => _remain_shop_cnt;
            set
            {
                SetProperty(ref _remain_shop_cnt, value);

                DisplayRemainShop_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayremainshop_cnt = "";
        public string DisplayRemainShop_cnt
        {
            get => _displayremainshop_cnt;
            set => SetProperty(ref _displayremainshop_cnt, value);
        }

        // 左通路店舗数
        private int _right_shop_cnt;
        public int Right_shop_cnt
        {
            get => _right_shop_cnt;
            set
            {
                SetProperty(ref _right_shop_cnt, value);

                DisplayRightShop_cnt = QtyFieldSpace(value);
            }
        }

        // 右通路店舗数
        private string _displayrightshop_cnt = "";
        public string DisplayRightShop_cnt
        {
            get => _displayrightshop_cnt;
            set => SetProperty(ref _displayrightshop_cnt, value);
        }

        private int _left_shop_cnt;
        public int Left_shop_cnt
        {
            get => _left_shop_cnt;
            set
            {
                SetProperty(ref _left_shop_cnt, value);

                DisplayLeftShop_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayleftshop_cnt = "";
        public string DisplayLeftShop_cnt
        {
            get => _displayleftshop_cnt;
            set => SetProperty(ref _displayleftshop_cnt, value);
        }

        // 左通路店舗数
        private int _right_ps_cnt;
        public int Right_ps_cnt
        {
            get => _right_ps_cnt;
            set
            {
                SetProperty(ref _right_ps_cnt, value);

                DisplayRightPs_cnt = QtyFieldSpace(value);
            }
        }

        // 右通路店舗数
        private string _displayrightps_cnt = "";
        public string DisplayRightPs_cnt
        {
            get => _displayrightps_cnt;
            set => SetProperty(ref _displayrightps_cnt, value);
        }

        private int _left_ps_cnt;
        public int Left_ps_cnt
        {
            get => _left_ps_cnt;
            set
            {
                SetProperty(ref _left_ps_cnt, value);

                DisplayLeftPs_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayleftps_cnt = "";
        public string DisplayLeftPs_cnt
        {
            get => _displayleftps_cnt;
            set => SetProperty(ref _displayleftps_cnt, value);
        }


        // 総商品数
        private int _order_item_cnt;
        public int Order_item_cnt
        {
            get => _order_item_cnt;
            set
            {
                SetProperty(ref _order_item_cnt, value);

                DisplayOrderItem_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayorderitem_cnt = "";
        public string DisplayOrderItem_cnt
        {
            get => _displayorderitem_cnt;
            set => SetProperty(ref _displayorderitem_cnt, value);
        }

        // 仕分商品数
        private int _plan_item_cnt;
        public int Plan_item_cnt
        {
            get => _plan_item_cnt;
            set
            {
                SetProperty(ref _plan_item_cnt, value);

                DisplayPlanItem_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayplanitem_cnt = "";
        public string DisplayPlanItem_cnt
        {
            get => _displayplanitem_cnt;
            set => SetProperty(ref _displayplanitem_cnt, value);
        }

        // 済み商品数
        private int _result_item_cnt;
        public int Result_item_cnt
        {
            get => _result_item_cnt;
            set
            {
                SetProperty(ref _result_item_cnt, value);

                DisplayResultItem_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayresultitem_cnt = "";
        public string DisplayResultItem_cnt
        {
            get => _displayresultitem_cnt;
            set => SetProperty(ref _displayresultitem_cnt, value);
        }

        // 残商品数
        private int _remain_item_cnt;
        public int Remain_item_cnt
        {
            get => _remain_item_cnt;
            set
            {
                SetProperty(ref _remain_item_cnt, value);

                DisplayRemainItem_cnt = QtyFieldSpace(value);
            }
        }

        private string _displayremainitem_cnt = "";
        public string DisplayRemainItem_cnt
        {
            get => _displayremainitem_cnt;
            set => SetProperty(ref _displayremainitem_cnt, value);
        }

        private bool _isModified = false;
        public bool IsModified
        {
            get => _isModified;
            set => SetProperty(ref _isModified, value);
        }

        // 数量変更で使用
        private int _old_plan_ps;
        public int Old_Plan_ps
        {
            get => _old_plan_ps;
            set => SetProperty(ref _old_plan_ps, value);
        }

        public DateTime? TdUnitPushTm { get; set; } = null;

        private string _display_ops_cs = "";
        public string Display_Ops_Cs
        {
            get => _display_ops_cs;
            set => SetProperty(ref _display_ops_cs, value);
        }

        private string _display_ops_ps = "";
        public string Display_Ops_Ps
        {
            get => _display_ops_ps;
            set => SetProperty(ref _display_ops_ps, value);
        }

        private string _display_dops_cs = "";
        public string Display_Dops_Cs
        {
            get => _display_dops_cs;
            set => SetProperty(ref _display_dops_cs, value);
        }

        private string _display_dops_ps = "";
        public string Display_Dops_Ps
        {
            get => _display_dops_ps;
            set => SetProperty(ref _display_dops_ps, value);
        }

        private string _display_drps_cs = "";
        public string Display_Drps_Cs
        {
            get => _display_drps_cs;
            set => SetProperty(ref _display_drps_cs, value);
        }
        private string _display_drps_ps = "";
        public string Display_Drps_Ps
        {
            get => _display_drps_ps;
            set => SetProperty(ref _display_drps_ps, value);
        }

        private string _display_ddps_cs = "";
        public string Display_Ddps_Cs
        {
            get => _display_ddps_cs;
            set => SetProperty(ref _display_ddps_cs, value);
        }
        private string _display_ddps_ps = "";
        public string Display_Ddps_Ps
        {
            get => _display_ddps_ps;
            set => SetProperty(ref _display_ddps_ps, value);
        }
        public string Display_Ops_CsPs_Text
        {
            get
            {
                return string.Format("{0}箱 {1}個 ({2:###0})"
                    , Display_Ops_Cs
                    , Display_Ops_Ps
                    , Ops
                   );
            }
        }
        public string Display_DOps_CsPs_Text
        {
            get
            {
                return string.Format("{0}箱 {1}個 ({2:###0})"
                    , Display_Dops_Cs
                    , Display_Dops_Ps
                    , Dops
                   );
            }
        }

        public string Display_DRps_CsPs_Text
        {
            get
            {
                return string.Format("{0}箱 {1}個 ({2:###0})"
                    , Display_Drps_Cs
                    , Display_Drps_Ps
                    , Drps
                   );
            }
        }
        public virtual string QtyFieldSpace(int ps)
        {
            if (CdHimban == "")
                return string.Empty;
            else
                return string.Format("{0, 4}", ps);
        }

        public void Clear()
        {
            CdDistGroup = "";
            NmDistGroup = "";
            CdHimban = "";
            NmHinSeishikimei = "";
            NuBoxUnit = 0;
            CdGtin13 = "";
            CdGtin14 = "";
            CdShukkaBatch = "";
            CdJuchuBin = "";
            CdTokuisaki = "";
            NmTokuisaki = "";
            CdCourse = "";
            CdRoute = "";
            Csunit = 0;
            Ops = 0;
            Dops = 0;
            Drps = 0;
            Ddps = 0;
            Order_shop_cnt = 0;
            Result_shop_cnt = 0;
            Remain_shop_cnt = 0;
            Order_item_cnt = 0;
            Result_item_cnt = 0;
            Remain_item_cnt = 0;
            Left_shop_cnt = 0;
            Right_shop_cnt = 0;
            Left_ps_cnt = 0;
            Right_ps_cnt = 0;
            _isModified = false;
            TdUnitPushTm = null;
        }
        public string GetUniqueItemKey
        {
            get => CdShukkaBatch + CdJuchuBin + CdHimban;
        }
    }

    public class DistGroupEx : DistGroup
    {
        private int _tdunittype = 0;
        public int Tdunittype
        {
            get => _tdunittype;
            set => SetProperty(ref _tdunittype, value);
        }


        public override string ToString()
        {
            return CdDistGroup + " " + NmDistGroup;
        }

    }
}
