using DbLib;
using LogLib;
using System;
using System.Windows;

namespace LightTest.Models
{
    internal class LightDefaultManager
    {
        // デフォルト点灯ボタン読み込み
        public static bool[] LoadDefaultButtons()
        {
            bool[] buttons = new bool[] { true, false, false, false, false };

            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    var settings = new Settings(tr);
                    for (int i = 0; i < 5; i++)
                    {
                        buttons[i] = settings.Get("LightTestButton" + i.ToString()) == "True" ? true : false;
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"LightDefaultManager::LoadDefaultButtons:Err:{e.Message}");
                    MessageBox.Show(e.Message);
                }
            }

            // 必ず一か所は点灯
            bool bCheck = false;
            foreach (var p in buttons)
            {
                if (p == true)
                {
                    bCheck = true;
                    break;
                }
            }

            if (bCheck == false)
            {
                buttons[0] = true;
            }

            return buttons;
        }

        // デフォルト点灯タイプ読み込み
        public static bool[] LoadDefaultBlinkTypes()
        {
            bool[] blinks = new bool[] { true, false };

            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    var settings = new Settings(tr);
                    for (int i = 0; i < 2; i++)
                    {
                        blinks[i] = settings.Get("LightTestBlinkType" + i.ToString()) == "True" ? true : false;
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"LightDefaultManager::LoadDefaultBlinkType:Err:{e.Message}");
                    MessageBox.Show(e.Message);
                }
            }

            // 必ず一か所は点灯
            bool bCheck = false;
            foreach (var p in blinks)
            {
                if (p == true)
                {
                    bCheck = true;
                    break;
                }
            }

            if (bCheck == false)
            {
                blinks[0] = true;
            }

            return blinks;
        }

        // デフォルト表示タイプ読み込み
        public static bool[] LoadDefaultDispTypes()
        {
            bool[] disptypes = new bool[] { true, false, false };

            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    var settings = new Settings(tr);
                    for (int i = 0; i < 3; i++)
                    {
                        disptypes[i] = settings.Get("LightTestDispType" + i.ToString()) == "True" ? true : false;
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"LightDefaultManager::LoadDefaultDispType:Err:{e.Message}");
                    MessageBox.Show(e.Message);
                }
            }

            // 必ず一か所は点灯
            bool bCheck = false;
            foreach (var p in disptypes)
            {
                if (p == true)
                {
                    bCheck = true;
                    break;
                }
            }

            if (bCheck == false)
            {
                disptypes[0] = true;
            }

            return disptypes;
        }

        // デフォルト点灯ボタン書き込み
        public static bool SaveDefaultButtons(bool[] buttons)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    var settings = new Settings(tr);
                    for (int i = 0; i < 5; i++)
                    {
                        settings.Set("LightTestButton" + i.ToString(), buttons[i]);
                    }
                    tr.Commit();
                }
                catch (Exception e)
                {
                    Syslog.Error($"LightDefaultManager::SaveDefaultButtons:Err:{e.Message}");
                    MessageBox.Show(e.Message);
                    return false;
                }
            }

            return true;
        }

        // デフォルト点灯タイプ書き込み
        public static bool SaveDefaultBlinkTypes(bool[] blinks)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    var settings = new Settings(tr);
                    for (int i = 0; i < 2; i++)
                    {
                        settings.Set("LightTestBlinkType" + i.ToString(), blinks[i]);
                    }
                    tr.Commit();
                }
                catch (Exception e)
                {
                    Syslog.Error($"LightDefaultManager::SaveDefaultBlinkTypes:Err:{e.Message}");
                    MessageBox.Show(e.Message);
                    return false;
                }
            }

            return true;
        }

        // デフォルト表示タイプ書き込み
        public static bool SaveDefaultDispTypes(bool[] disptypes)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    var settings = new Settings(tr);
                    for (int i = 0; i < 3; i++)
                    {
                        settings.Set("LightTestDispType" + i.ToString(), disptypes[i]);
                    }
                    tr.Commit();
                }
                catch (Exception e)
                {
                    Syslog.Error($"LightDefaultManager::SaveDefaultDispTypes:Err:{e.Message}");
                    MessageBox.Show(e.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
