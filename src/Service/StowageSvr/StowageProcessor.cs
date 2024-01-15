using DbLib.DbEntities;
using ProcessorLib;
using ProcessorLib.Models;
using StowageSvr.HostRequest;
using StowageSvr.Models;
using StowageSvr.Reporitories;
using Microsoft.IdentityModel.Tokens;
using DbLib.Defs;

namespace StowageSvr
{
    public class StowageProcessor : PrintableCommandProcessor
    {
        private readonly IRepositoryFactory<IStowageRepository> _repositoryFactory;

        public StowageProcessor(IRepositoryFactory<IStowageRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        // 仕分グループ選択
        public GetDistGroupResponse GetDistGroup(GetDistGroupRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                var stowageEntitys = repo.GetStowageEntitys(request.Block, request.DeliveryDate);

                if (!stowageEntitys.Any())
                {
                    throw new Exception("指定ブロック、納品日の仕分グループがありません");
                }

                var stowageDistInfo = stowageEntitys.SelectMany(x => x.TBSTOWAGEMAPPING!).FirstOrDefault(x => x.CDDISTGROUP == request.Code);

                if (stowageDistInfo is null)
                {
                    throw new Exception("該当する仕分グループがありません");
                }
                repo.Commit();

                return new GetDistGroupResponse
                {
                    DistGroup = stowageDistInfo.CDDISTGROUP!,
                    DistGroupName = stowageDistInfo.NMDISTGROUP ?? string.Empty,
                };
            }
        }

        // 仕分グループ一覧
        public GetDistGroupListResponse GetDistGroupList(GetDistGroupListRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                var stowageEntitys = repo.GetStowageEntitys(request.Block, request.DeliveryDate);

                if (!stowageEntitys.Any())
                {
                    throw new Exception("指定ブロック、納品日の仕分グループがありません");
                }

                repo.Commit();
                return new GetDistGroupListResponse
                {
                    DistGroups = stowageEntitys.SelectMany(x => x.TBSTOWAGEMAPPING!).GroupBy(x => x.CDDISTGROUP)
                    .Where(x => !x.Key.IsNullOrEmpty()).OrderBy(x => x.Key).Select(x => new ListRow
                    {
                        Code = x.Key ?? string.Empty,
                        Name = x.Max(x => x.NMDISTGROUP) ?? string.Empty,
                    }),
                };
            }
        }

        // 表示器ﾊﾞｰｺｰﾄﾞｽｷｬﾝ、出荷バッチ選択、得意先選択
        public GetStowageResponse GetStowage(GetStowageRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                var stowages = repo.GetStowageEntitys(request.Block, request.DeliveryDate, request.DistGroup, request.TdCode,
                        request.Batch, request.Customer)
                    .Select(x => new Stowage(x));

                if (!stowages.Any())
                {
                    throw new Exception("該当する積付情報がありません");
                }

                // 出荷バッチ選択
                var stowageBatchs = stowages.GroupBy(x => x.ShukkaBatch).OrderBy(x => x.Key).Select(x => new ListRow
                {
                    Code = x.Key,
                    Name = x.First().ShukkaBatchName ?? string.Empty,
                });

                // 得意先選択
                var stowageCustomers = stowages.GroupBy(x => x.CustomerCode).OrderBy(x => x.Key).Select(x => new ListRow
                {
                    Code = x.Key,
                    Name = x.First().CustomerName ?? string.Empty,
                });

                var targetStowages = Enumerable.Empty<Stowage>();

                // 得意先抽出完了
                if (stowageCustomers.Count() == 1)
                {
                    // 箱毎のレコード
                    targetStowages = stowages.Where(x => x.CustomerCode == stowageCustomers.First().Code);
                }

                repo.Commit();

                return new GetStowageResponse
                {
                    TdCode = request.TdCode,
                    Batchs = stowageBatchs,
                    Customers = stowageCustomers,

                    IsLastCustomer = targetStowages.Any(),
                    // 空リスト時にパースエラーとなる為、ダミーId追加
                    StowageIds = targetStowages.Any() ? targetStowages.Select(x => x.Id) : new List<long> { 9999 },
                    LargeBoxPs = targetStowages.FirstOrDefault(x => x.StBoxType == DbLib.Defs.BoxType.LargeBox)?.GetBoxCount() ?? 0,
                    SmallBoxPs = targetStowages.FirstOrDefault(x => x.StBoxType == DbLib.Defs.BoxType.SmallBox)?.GetBoxCount() ?? 0,
                    OtherPs = targetStowages.FirstOrDefault(x => x.StBoxType == DbLib.Defs.BoxType.EtcBox)?.GetBoxCount() ?? 0,
                    BlueBoxPs = targetStowages.FirstOrDefault(x => x.StBoxType == DbLib.Defs.BoxType.BlueBox)?.GetBoxCount() ?? 0,
                };
            }
        }

        // 最小表示器コード取得(F4：スキャン無)
        public GetTdCodResponse GetTdCode(GetTdCodeRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                var stowages = repo.GetStowageEntitys(request.Block, request.DeliveryDate, request.DistGroup)
                    .Select(x => new Stowage(x));

                if (!stowages.Any())
                {
                    throw new Exception("該当する積付情報がありません");
                }

                var unfinishedTdCodes = stowages.Where(x => x.FgSStatus != Status.Completed && x.TdCodes.Any())
                    .SelectMany(x => x.TdCodes).OrderBy(x => x);

                if (!unfinishedTdCodes.Any())
                {
                    // 自動取得時は空欄返し、F4押下時はエラー表示
                    if (request.IsAuto)
                    {
                        return new GetTdCodResponse { TdCode = string.Empty };
                    }

                    throw new Exception("全件積付済みです");
                }

                return new GetTdCodResponse { TdCode = unfinishedTdCodes.First() };
            }
        }

        // 積付確定（得意先情報確認）
        public ResistStowageResponse ResistStowage(ResistStowageRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                var targetStowages = repo.GetStowageEntitys(request.StowageIds).Select(x => new Stowage(x));

                if (!targetStowages.Any())
                {
                    throw new Exception("更新対象の積付情報が見つかりません");
                }

                // 各箱毎に更新
                foreach (var stowage in targetStowages)
                {
                    stowage.Update(GetBoxCount(stowage.StBoxType, request));
                    repo.UpdateStowageEntity(stowage);
                }
                repo.Commit();
            }

            return new ResistStowageResponse { Success = true };
        }

        public override IEnumerable<string> GetLabelData(PrintLabelRequest request)
        {
            //ILabelBuilder builder;
            //var prm = new LabelBuilderParam(request.Code);

            //switch ((LabelType)request.LabelType)
            //{
            //    case LabelType.InventoryItemLabel:
            //        builder = new LabelBuilder(new InventoryLabelRepository());
            //        break;

            //    default:
            //        throw new Exception("不明なラベルタイプです");
            //}

            //return builder.Build(prm);
            return Enumerable.Empty<string>();
        }

        public override PrinterType GetPrinterType(PrintLabelRequest request)
        {
            return PrinterType.Sato;
        }

        private int GetBoxCount(BoxType stBoxType, ResistStowageRequest request)
        {
            switch (stBoxType)
            {
                case BoxType.LargeBox:
                    return request.LargeBoxPs;

                case BoxType.SmallBox:
                    return request.SmallBoxPs;

                case BoxType.EtcBox:
                    return request.OtherPs;

                case BoxType.BlueBox:
                    return request.BlueBoxPs;

                default:
                    return 0;
            }
        }
    }
}
