using LabelLib;
using LargeDistLabelLib;
using Microsoft.Extensions.Configuration;
using Prism.Events;
using Prism.Services.Dialogs;
using SatoPrintLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowLib.Utils;
using WindowLib.ViewModels;

namespace LargeDist.Models
{
    public class LargeDistLabelPrinter
    {
        private readonly IEventAggregator _eventAggregator;
        private IEnumerable<LargeDistLabel> _labels;

        public LargeDistLabelPrinter(IEnumerable<LargeDistLabel> labels, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _labels = labels;
        }

        public void Print(IDialogService dialogService)
        {
            WaitProgressDialog.ShowProgress(
                "印刷中",
                "印刷中です。しばらくお待ちください。",
                null,
                PrintMain,
                null,
                dialogService,
                _eventAggregator);
        }

        private string PrinterAddress => new ConfigurationBuilder()
            .AddJsonFile("common.json", true, true)
            .Build()
            .GetSection("pc")["iplabel"] ?? "";

        private void PrintMain(CancellationTokenSource source)
        {
            var client = new PrinterClient(PrinterAddress, source.Token);
            int count = 0;
            var labelBuilder = new LabelBuilder();
            var formatter = new SplFormatter();
            formatter.Start();
            formatter.Cut();
            formatter.End(0);

            var labels = _labels
                .SelectMany(x => labelBuilder.Build(x))
                .Append(formatter.GetString())
                .ToArray();

            foreach (var label in labels)
            {
                ++count;
                _eventAggregator.GetEvent<ProgressDialogEvent>()
                    .Publish(new ProgressMessage()
                    {
                        ProgressMax = _labels.Count(),
                        ProgressValue = count,
                        Message = "印刷中",
                    });


                while (true)
                {
                    try
                    {
                        client.Print(label);
                        break;
                    }
                    catch (OperationCanceledException) 
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _eventAggregator.GetEvent<ProgressDialogEvent>()
                            .Publish(new ProgressMessage()
                            {
                                ProgressMax = _labels.Count(),
                                ProgressValue = count,
                                Message = ex.Message,
                            });

                        Task.Delay(1000).Wait();
                    }
                }
            }
        }
    }
}
