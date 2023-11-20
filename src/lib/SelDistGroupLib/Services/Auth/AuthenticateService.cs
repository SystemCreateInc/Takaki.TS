using Prism.Services.Dialogs;
using SelDistGroupLib.Models;
using SelDistGroupLib.Views;
using System;

namespace SelDistGroupLib.Services.Auth
{
    public static class AuthenticateService
    {
        public static DistGroup? AuthDistGroupDialog(IDialogService dialogService)
        {
            DistGroup? result = null;

            dialogService.ShowDialog(
                nameof(SelDistGroupDlg),
                new DialogParameters
                {
                },
                (rc) =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        return;
                    }

                    result = rc.Parameters.GetValue<Models.DistGroup>("DistGroup");
                    result.DT_DELIVERY = rc.Parameters.GetValue<DateTime>("DT_DELIVERY");
                });

            return result;
        }
    }
}
