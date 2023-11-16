using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WindowLib.Views
{
    /// <summary>
    /// DataGrid 列設定ボタンコントロール
    /// </summary>
    public partial class GridColumnSetting : UserControl
    {
        #region Dependency Properties

        /// <summary>依存プロパティ定義 - 対象のDataGrid</summary>
        /// <summary></summary>
        public static readonly DependencyProperty TargetGridProperty =
            DependencyProperty.Register("TargetGrid",
                typeof(DataGrid),
                typeof(GridColumnSetting),
                new FrameworkPropertyMetadata(null));

        /// <summary>対象のDataGrid</summary>
        public DataGrid TargetGrid
        {
            get { return (DataGrid)GetValue(TargetGridProperty); }
            set { SetValue(TargetGridProperty, value); }
        }

        /// <summary>依存プロパティ定義 - 設定ファイルのキー</summary>
        public static readonly DependencyProperty SettingsKeyProperty =
            DependencyProperty.Register("SettingsKey",
                typeof(string),
                typeof(GridColumnSetting),
                new FrameworkPropertyMetadata(null));

        /// <summary>設定ファイルのキー</summary>
        public string SettingsKey
        {
            get { return (string)GetValue(SettingsKeyProperty); }
            set { SetValue(SettingsKeyProperty, value); }
        }
        #endregion

        private const string DEFAULT_REGKEY = @"Software\systemcreate.inc\";

        private bool _bInit = true;

        private Window? _window = null;

        private string _title = "";

        private string KeyName => $"{DEFAULT_REGKEY}{_title}\\{SettingsKey}";

        public GridColumnSetting()
        {
            InitializeComponent();
        }

        private void GridColumnSetting_Loaded(object sender, RoutedEventArgs e)
        {
            LoadColumnSettings();
        }

        private void GridColumnSetting_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveColumnSettings();
        }

        private void LoadColumnSettings()
        {
            if (TargetGrid is null)
            {
                return;
            }

            if (!_bInit)
            {
                return;
            }

            _bInit = false;

            try
            {
                _window = Window.GetWindow(this);
                _window.Closing += _window_Closing;
                _title = _window.Title.ToString();
            }
            catch (Exception)
            {
            }

            // デフォルトの並び順・幅を確保する
            using (var regkeyCreated = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(KeyName))
            {
                if (regkeyCreated == null)
                {
                    return;
                }

                var regkey = (Microsoft.Win32.RegistryKey)regkeyCreated;
                var settings = new List<DataGridColumnInfo>();
                int size = (int?)regkey.GetValue("size", 0) ?? 0;
                for (int i = 0; i < size; i++)
                {
                    DataGridColumnInfo p = new DataGridColumnInfo();
                    p.DisplayIndex = (int?)regkey.GetValue("DisplayIndex" + i.ToString()) ?? 0;
                    p.Width = double.Parse((string?)regkey.GetValue("Width" + i.ToString()) ?? "0");
                    settings.Add(p);
                }
                regkey.Close();
                setColumns(settings);
            }
        }

        private void _window_Closing(object? sender, CancelEventArgs e)
        {
            SaveColumnSettings();
        }

        private void SaveColumnSettings()
        {
            // 列情報コレクションに現在のDisplayIndexとWidthを確保
            var newSettings = TargetGrid.Columns
                .Select((x, index) => new DataGridColumnInfo()
                {
                    ColumnIndex = index,
                    DisplayIndex = x.DisplayIndex,
                    Width = x.ActualWidth,
                })
                .ToList();

            using (var regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(KeyName))
            {
                regkey.SetValue("size", newSettings.Count);
                foreach (var p in newSettings)
                {
                    regkey.SetValue("DisplayIndex" + p.ColumnIndex.ToString(), p.DisplayIndex);
                    regkey.SetValue("Width" + p.ColumnIndex.ToString(), p.Width);
                }
                regkey.Close();
            }

        }

        /// <summary>
        /// DataGrid列設定
        /// </summary>
        /// <param name="settings"></param>
        private void setColumns(List<DataGridColumnInfo> settings)
        {
            if (settings != null && settings.Count > 0)
            {
                for (var i = 0; i < this.TargetGrid.Columns.Count; i++)
                {
                    if (i < settings.Count)
                    {
                        var setting = settings[i];
                        TargetGrid.Columns[i].DisplayIndex = setting.DisplayIndex;
                        TargetGrid.Columns[i].Width = new DataGridLength(setting.Width);
                    }
                }
            }
        }

        /// <summary>
        /// 列情報クラス
        /// </summary>
        public class DataGridColumnInfo
        {
            /// <summary>列index</summary>
            public int ColumnIndex { get; set; }
            /// <summary>表示index</summary>
            public int DisplayIndex { get; set; }
            /// <summary>横幅</summary>
            public double Width { get; set; }
        }
    }
}
