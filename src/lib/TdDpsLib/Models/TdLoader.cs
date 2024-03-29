﻿using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DryIoc;
using LogLib;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
        }

        // 表示器情報取得
        static public List<TdAddrData>? LoadTdAddrs(int Tdunittype)
        {
            int tdtype1 = Tdunittype, tdtype2 = 0;
            if (Tdunittype == (int)TdUnitType.TdRack)
            {
                tdtype2 = (int)TdUnitType.TdRackBox;
            } else
            {
                tdtype2 = (int)TdUnitType.TdCeilingBox;
            }

            using (var con = DbFactory.CreateConnection())
            {
                string sql = @"select tdunitaddrcode,tdunitaddr.tdunitcode,tdunitareacode,tdunitmst.tdunitportcode,tdunitgroup,tdunitaddr,tdunitbutton,tdunitportcom,tdunitporttype,usageid,tdunitseq,tdunitzonecode from tdunitaddr"
                            + " inner join tdunitmst on tdunitmst.tdunitcode=tdunitaddr.tdunitcode"
                            + " inner join tdunitport on tdunitport.tdunitportcode=tdunitmst.tdunitportcode"
                            + " where usageid in (@tdtype1,@tdtype2)"
                            + " order by tdunitzonecode,CONVERT(int,tdunitseq),tdunitaddrcode,tdunitaddr.tdunitcode";

                var r =  con.Query(sql, new { @tdtype1 = tdtype1, @tdtype2 = tdtype2 })
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
                         TdUnitZoneCode = q.tdunitzonecode,
                         TdUnitFront = q.tdunitseq,

                         Stno = q.tdunitportcode,
                         TdUnitPortType = (TdControllerType)q.tdunitporttype,
                     }).ToList();

                int maxseq = r.Max(x => x.TdUnitSeq) + 1;

                // ＳＥＱ降順設定
                foreach (var row in r)
                {
                    if (row.TdUsageid == Tdunittype)
                    {
                        row.TdUnitSeqReverse = maxseq - row.TdUnitSeq;

                        Syslog.Info($"LoadTdAddrs:tdunitaddr={row.TdUnitAddrCode} addr={row.TddGroup}-{row.TddAddr} zone={row.TdUnitZoneCode} seq={row.TdUnitSeq} seqreverse={row.TdUnitSeqReverse} Front={row.TdUnitFront}");
                    }
                }

                return r;
            }
        }
        // エリア表示灯表示器情報取得
        static public List<TdAreaData> ?LoadTdAreas(List<TdAddrData>? tdunitaddrs)
        {
            using (var con = DbFactory.CreateConnection())
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
        }
        static public bool IsLightArea(TdAddrData tdunitaddr, List<TdAreaData> areas)
        {
            return true;
        }
        // TKCOMファイルより情報取得
        static public int LoadTdUnitType()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            var idpc = int.Parse(config.GetSection("pc")?["idpc"] ?? "1");


            using (var con = DbFactory.CreateConnection())
            {
                string sql = @"select ST_TDUNIT_TYPE from TB_BLOCK"
                            + " where TB_BLOCK.CD_BLOCK = @idpc;";

                var r = con.Query(sql, new { idpc = idpc })
                    .Select(q => new TBBLOCKEntity
                    {
                        STTDUNITTYPE = q.ST_TDUNIT_TYPE,
                    }).FirstOrDefault();

                return r == null ? (int)TdUnitType.TdRack : r.STTDUNITTYPE;
            }
        }
    }
}
