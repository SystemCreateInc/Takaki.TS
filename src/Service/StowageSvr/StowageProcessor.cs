using ProcessorLib;
using ProcessorLib.Models;
using StowageSvr.HostRequest;
using StowageSvr.Models;
using StowageSvr.Reporitories;

namespace StowageSvr
{
    public class StowageProcessor : PrintableCommandProcessor
    {
        private readonly IRepositoryFactory<IStowageRepository> _repositoryFactory;

        public StowageProcessor(IRepositoryFactory<IStowageRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public GetBlockResponse GetBlock(GetBlockRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {   
                return new GetBlockResponse
                {  
                    Block = request.Code,
                };
            }
        }

        public GetDistGroupResponse GetDistGroup(GetDistGroupRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                return new GetDistGroupResponse
                {
                    DistGroup = request.Code,
                    DistGroupName = "仕分名" + request.Code,
                };
            }
        }

        public GetDistGroupListResponse GetDistGroupList(GetDistGroupListRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                return new GetDistGroupListResponse
                {
                    DistGroups = new List<ListRow> 
                    { 
                        new ListRow
                        {
                            Code = "001",
                            Name = "仕分001"
                        },
                        new ListRow
                        {
                            Code = "002",
                            Name = "仕分002"
                        },
                        new ListRow
                        {
                            Code = "003",
                            Name = "仕分003"
                        },
                    }
                };
            }
        }

        public GetTdInfoResponse GetTdInfo(GetTdInfoRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                return new GetTdInfoResponse
                {
                    TdCode = request.TdCode,
                    ThickBoxPs = 1,
                    WeakBoxPs = 2,
                    OtherPs = 3,
                    BlueBoxPs = 4,

                    Batchs = new List<ListRow>
                    {
                        new ListRow
                        {
                            Code = "00001",
                            Name = "バッチ00001",
                        },

                        new ListRow
                        {
                            Code = "00002",
                            Name = "バッチ00002",
                        },
                    },


                    Customers = new List<ListRow> 
                    { 
                        new ListRow
                        {
                            Code = "001",
                            Name = "得意先名001"
                        },
                        new ListRow
                        {
                            Code = "002",
                            Name = "得意先名002"
                        },
                        new ListRow
                        {
                            Code = "003",
                            Name = "得意先名003"
                        },
                    }
                };
            }
        }

        public ResistStowageResponse ResistStowage(ResistStowageRequest request)
        {
            using (var repo = _repositoryFactory.Create())
            {
                return new ResistStowageResponse
                {
                    Success = true,
                };
            }
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
    }
}
