using DbLib.DbEntities;
using DbLib.Defs;

namespace StowageSvr.Models
{
    public class Stowage
    {
        public long Id { get; set; }
        public string DelivDate { get; set; }
        public string ShukkaBatch { get; set; }
        public string ShukkaBatchName { get; set; }
        public string KyotenCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        public Status FgSStatus { get; set; }

        public BoxType StBoxType { get; set; }
        public int OrderBoxCount { get; set; }
        public int ResultBoxCount { get; set; }

        public string PersonName { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsChangeCount { get; set; }

        public TBSTOWAGEEntity Entity { get; set; }

        public Stowage(TBSTOWAGEEntity entity)
        {
            Entity = entity;
            Id = entity.IDSTOWAGE;
            DelivDate = entity.DTDELIVERY;
            ShukkaBatch = entity.CDSHUKKABATCH;
            ShukkaBatchName = entity.TBSTOWAGEMAPPING?.FirstOrDefault()?.NMSHUKKABATCH ?? string.Empty;
            KyotenCode = entity.CDKYOTEN;
            CustomerCode = entity.CDTOKUISAKI;
            CustomerName = entity.TBSTOWAGEMAPPING?.FirstOrDefault()?.NMTOKUISAKI ?? string.Empty;

            FgSStatus = (Status)entity.FGSSTATUS;

            StBoxType = (BoxType)entity.STBOXTYPE;
            OrderBoxCount = entity.NUOBOXCNT;
            ResultBoxCount = entity.NURBOXCNT;
            PersonName = entity.NMHENKOSHA;
            UpdatedAt = entity.UpdatedAt;
        }

        public void Update(int boxCount)
        {
            ResultBoxCount = boxCount;
            FgSStatus = Status.Completed;
            UpdatedAt = DateTime.Now;
            IsChangeCount = true;
        }

        // 数量変更無し、ステータスのみ更新
        public void UpdateStatus()
        {
            FgSStatus = Status.Completed;
            UpdatedAt = DateTime.Now;
            IsChangeCount = false;
        }
    }
}
