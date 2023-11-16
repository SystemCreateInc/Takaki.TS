﻿using LogLib;
using ProcessorLib;
using System;
using System.Configuration;

namespace ColumnVisibilityLib.Services
{
    public class ConfigManager
    {
        public static string? GetConfig(string configKey)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                return config.AppSettings.Settings[configKey]?.Value;
            }
            catch (Exception ex)
            {
                Syslog.Debug($"ConfigManager:GetConfig Fail msg={ex.Message}");
                return null;
            }
        }
        public static void SetConfig(string targetStr, string configKey)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings[configKey].Value = targetStr;
                config.AppSettings.CurrentConfiguration.Save();
            }
            catch (Exception ex)
            {
                Syslog.Debug($"ConfigManager:SetConfig Fail msg={ex.Message}");
            }
        }

        public static void SetConfigJson(object targetData, string configKey)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings[configKey].Value = Json.Serialize(targetData);
                config.AppSettings.CurrentConfiguration.Save();
            }
            catch (Exception ex)
            {
                Syslog.Debug($"ConfigManager:SetConfig Fail msg={ex.Message}");
            }
        }
    }
}
