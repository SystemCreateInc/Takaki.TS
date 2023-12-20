using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace PrintPreviewLib
{
    /// <summary>
    /// PrintPreview.xaml の相互作用ロジック
    /// </summary>
    public partial class PrintPreview : MetroWindow
    {
        public PrintPreview()
        {
            InitializeComponent();

            Viewer.PropertyChanged += Viewer_PropertyChanged;
        }

        // 印刷ボタン押下後に閉じる
        private void Viewer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 最大ページ数
            MaxPage.Content = "/" + Viewer.PageCount;
            // 指定ページ数の最大文字数＝最大ページ数の文字数
            CurPage.MaxLength = Viewer.PageCount.ToString().Length;

            // 画面の検索削除
            if (Viewer.Template.FindName("PART_FindToolBarHost", Viewer) as ContentControl is ContentControl cc)
            {
                cc.Visibility = Visibility.Collapsed;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Viewer.NextPage();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Viewer.PreviousPage();
        }

        private void First_Click(object sender, RoutedEventArgs e)
        {
            Viewer.FirstPage();
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            Viewer.LastPage();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Viewer_PageViewsChanged(object sender, System.EventArgs e)
        {
            int pageNum = 1;

            // 表示中のページ情報取得
            if (Viewer.PageViews.Count > 0)
            {
                // 1番目のページ情報からページ数取得
                var pageInfo = Viewer.PageViews[0];
                pageNum = pageInfo.PageNumber + 1;
            }

            // 現在のページ数更新
            CurPage.Text = pageNum.ToString();
        }

        /// <summary>
        /// ページ数指定(TextBox上Enter押下)
        /// </summary>
        private void CurPage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (int.TryParse(CurPage.Text, out int pageNo))
                {
                    // 最大ページ数より大きい場合は最大ページ数にする
                    // 1以下なら1にする
                    if (pageNo > Viewer.PageCount)
                    {
                        pageNo = Viewer.PageCount;
                    }
                    else if (pageNo < 1)
                    {
                        pageNo = 1;
                    }
                }
                else
                {
                    // 表示中のページ情報取得
                    if (Viewer.PageViews.Count > 0)
                    {
                        // 1番目のページ情報からページ数取得
                        var pageInfo = Viewer.PageViews[0];
                        pageNo = pageInfo.PageNumber + 1;
                    }
                    else
                    {
                        pageNo = 1;
                    }
                }

                CurPage.Text = pageNo.ToString();
                Viewer.GoToPage(pageNo);
            }
        }
    }
}
