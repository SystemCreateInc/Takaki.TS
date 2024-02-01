using Prism.Services.Dialogs;
using SelDistGroupLib.Models;
using SelDistGroupLib.Views;
using System;

namespace SelDistGroupLib.Services.Auth
{
    public static class AuthenticateService
    {
        public static DistGroup? AuthDistGroupDialog(IDialogService dialogService, bool bAll=true)
        {
            DistGroup? result = null;

            dialogService.ShowDialog(
                nameof(SelDistGroupDlg),
                new DialogParameters
                {
                    { "bAll", bAll },
                },
                (rc) =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        return;
                    }

                    result = rc.Parameters.GetValue<Models.DistGroup>("DistGroup");
                    result.DtDelivery = rc.Parameters.GetValue<DateTime>("DtDelivery");
                });

            return result;
        }
    }
}
