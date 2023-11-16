using Dapper;
using DbLib;
using DryIoc;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TdDpsLib.Defs;

namespace TdDpsLib.Models
{
    // 表示器アドレス設定情報読み込みクラス
    public class TdLoader
    {
        // TKCOMファイルより情報取得
        static public List<TdPortData> ?LoadTdComs()
        {
            using (var con = DbFactory.CreateConnection())
            {
                try
                {
                    string sql = @"select distinct tdunitmst.tdunitportcode,tdunitportcom,tdunitport.tdunitporttype,t1.typename tdunitporttypename from tdunitaddr"
                                + " inner join tdunitmst on tdunitmst.tdunitcode=tdunitaddr.tdunitcode"
                                + " inner join tdunitport on tdunitport.tdunitportcode=tdunitmst.tdunitportcode"
                                + " left join typelists t1 on t1.typefield = 'tdunitporttype' and t1.typecode = tdunitport.tdunitporttype"
                                + " order by tdunitportcode,tdunitportcom";

                    return con.Query(sql, null)
                         .Select(q => new TdPortData
                         {
                             TdUnitPortCode = q.tdunitportcode,
                             TdPortCom = q.tdunitportcom,
                             Baudrate = 0,
                             TdUnitPortType = (TdControllerType)q.tdunitporttype,
                             TdUnitPortTypeName = q.tdunitporttypename,

                             Stno = q.tdunitportcode,
                         }).ToList();
                }
                catch (Exception e)
                {
                    Syslog.Error($"TdSetting::LoadTdComs:Err:" + e.Message);
                    MessageBox.Show(e.Message);
                    return null;
                }
            }
        }

        // 表示器情報取得
        static public List<TdAddrData> ?LoadTdAddrs()
        {
            using (var con = DbFactory.CreateConnection())
            {
                try
                {
                    string sql = @"select tdunitaddrcode,tdunitaddr.tdunitcode,tdunitareacode,tdunitmst.tdunitportcode,tdunitgroup,tdunitaddr,tdunitbutton,tdunitportcom,tdunitporttype,usageid,tdunitseq from tdunitaddr"
                                + " inner join tdunitmst on tdunitmst.tdunitcode=tdunitaddr.tdunitcode"
                                + " inner join tdunitport on tdunitport.tdunitportcode=tdunitmst.tdunitportcode"
                                + " order by tdunitaddrcode,tdunitaddr.tdunitcode";

                    return con.Query(sql, null)
                         .Select(q => new TdAddrData
                         {
                             TdUnitCode = q.tdunitcode,
                             TdUnitAddrCode = q.tdunitaddrcode,
                             TddGroup = q.tdunitgroup,
                             TddAddr = q.tdunitaddr,
                             TddButton = q.tdunitbutton,
                             TdUnitAreaCode = q.tdunitareacode,
                             TdUnitPortCode = q.tdunitportcode,
                             TdPortCom = q.tdunitportcom,
                             TdUsageid = q.usageid,
                             TdUnitSeq = q.tdunitseq,

                             Stno = q.tdunitportcode,
                             TdUnitPortType = (TdControllerType)q.tdunitporttype,
                         }).ToList();
                }
                catch (Exception e)
                {
                    Syslog.Error($"TdSetting::LoadTdAddrs:Err:" + e.Message);
                    MessageBox.Show(e.Message);
                    return null;
                }
            }
        }
        // エリア表示灯表示器情報取得
        static public List<TdAreaData> ?LoadTdAreas(List<TdAddrData>? tdunitaddrs)
        {
            using (var con = DbFactory.CreateConnection())
            {
                try
                {
                    string sql = @"select tdunitarea.tdunitcode,tdunitarea.tdunitareacode,tdunitmst.tdunitportcode,tdunitgroup,tdunitaddr,tdunitbutton,tdunitareaname,tdunitportcom,tdunitporttype from tdunitarea"
                                + " inner join tdunitmst on tdunitmst.tdunitcode=tdunitarea.tdunitcode"
                                + " inner join tdunitareamst on tdunitareamst.tdunitareacode=tdunitarea.tdunitareacode"
                                + " inner join tdunitport on tdunitport.tdunitportcode=tdunitmst.tdunitportcode"
                                + " order by tdunitarea.tdunitareacode,tdunitarea.tdunitcode";

                    List<TdAreaData> tdunitareas = con.Query(sql, null)
                         .Select(q => new TdAreaData
                         {
                             TdUnitCode = q.tdunitcode,
                             TddGroup = q.tdunitgroup,
                             TddAddr = q.tdunitaddr,
                             TddButton = q.tdunitbutton,
                             TdUnitAreaCode = q.tdunitareacode,
                             TdUnitAreaName = q.tdunitareaname,
                             TdUnitPortCode = q.tdunitportcode,
                             TdPortCom = q.tdunitportcom,

                             Stno = q.tdunitportcode,
                             TdUnitPortType = (TdControllerType)q.tdunitporttype,
                         }).ToList();

                    if (tdunitaddrs != null)
                    {
                        foreach (var x in tdunitaddrs)
                        {
                            if (x.TdUnitAreaCode != null)
                            {
                                foreach (var ar in tdunitareas)
                                {
                                    if (ar.TdUnitAreaCode == x.TdUnitAreaCode)
                                    {
                                        ar.RangeTdUnitCode.Add(x.TdUnitCode);
                                    }
                                }
                            }
                        }
                    }
                    return tdunitareas;

                }
                catch (Exception e)
                {
                    Syslog.Error($"TdSetting::LoadTdAreas:Err:" + e.Message);
                    MessageBox.Show(e.Message);
                    return null;
                }
            }
        }
        static public bool IsLightArea(TdAddrData tdunitaddr, List<TdAreaData> areas)
        {
            return true;
        }
    }
}
