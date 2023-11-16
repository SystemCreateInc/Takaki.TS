using LogLib;
using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace WindowLib.Utils
{
    internal static class StyleLoader
    {
        internal static void Initialize()
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles("Styles", "*.xaml"))
                {
                    Syslog.Debug($"Loading style file {file}");
                    using (var fs = File.OpenRead(file))
                    {
                        var dictionary = XamlReader.Load(fs) as ResourceDictionary;
                        Application.Current.Resources.MergedDictionaries.Add(dictionary);
                    }
                }
            }
            catch (Exception e)
            {
                Syslog.Warn($"Cannot read resource file ({e.Message})");
            }
        }
    }
}
