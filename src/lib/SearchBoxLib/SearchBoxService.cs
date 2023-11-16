using DbLib;
using Prism.Services.Dialogs;
using SearchBoxLib.Models;
using SearchBoxLib.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SearchBoxLib
{
    public class SearchBoxService
    {
        private SearchQueryParam _searchQueryParam = new SearchQueryParam();

        public void Clear()
        {
            _searchQueryParam = new SearchQueryParam();
        }

        public SearchQueryParam GetQuery()
        {
            return _searchQueryParam;
        }

        public bool ShowSearchBox(IDialogService dialogService, IEnumerable<Content> contents)
        {
            SearchBoxResult? result = null;

            dialogService.ShowDialog(
                nameof(SearchBoxDlg),
                new DialogParameters
                {
                    { "Contents", contents },
                },
                (rc) =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        return;
                    }

                    result = new SearchBoxResult
                    (
                        rc.Parameters.GetValue<IEnumerable<string>>("SearchFields"),
                        rc.Parameters.GetValue<string>("SearchText")
                    );
                });


            if(result is null || string.IsNullOrEmpty(result.Text.Trim()))
            {
                return false;
            }

            _searchQueryParam.WhereSql += GetSearchSql(result!);

            return true;
        }

        // データグリッドから検索対象列取得 *Bindingメンバ名とテーブルのカラム名が同一必須
        public IEnumerable<Content> GetContentsForDataGrid(DataGrid dataGrid, string tableName,  List<string> disableMembers)
        {
            return dataGrid.Columns.Where(x => !disableMembers.Contains(x.SortMemberPath)).Select(x => new Content
            {
                ContentName = x.Header?.ToString() ?? string.Empty,
                TableName = $"{tableName}.{x.SortMemberPath}",
            }).ToList();
        }

        // 検索SQL作成
        private string GetSearchSql(SearchBoxResult result)
        {
            return QueryHelper.SearchFields(result.Fields, 
                result.Text, 
                _searchQueryParam.prms,
                !string.IsNullOrEmpty(_searchQueryParam.WhereSql),
                _searchQueryParam.NextParameterIndex);
        }
    }
}
