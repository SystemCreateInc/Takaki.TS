using DbLib;
using LargeDist.Infranstructures;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.Models
{
    public class LargeDistProcessingUnit
    {
        public IEnumerable<DistItem> Items { get; private set; }

        public string? CdBlock { get; }
        public string CdJuchuBin { get; }
        public string? CdDistGroup { get; }
        public string? NmDistGroup { get; }
        public string? CdShukkaBatch { get; }
        public string? NmShukkaBatch { get; }
        public string? CdHimban { get; }
        public string? CdGtin13 { get; }
        public string? NmHinSeishikimei { get; }
        public int NuBoxUnit { get; set; }

        public bool IsCompleted => Items.All(x => x.IsCompleted);

        public LargeDistProcessingUnit(IEnumerable<DistItem> list)
        {
            Items = list;
            var first = list.First();
            CdBlock = first.CdBlock;
            CdJuchuBin = first.CdJuchuBin;
            CdDistGroup = first.CdDistGroup;
            NmDistGroup = first.NmDistGroup;
            CdShukkaBatch = first.CdShukkaBatch;
            NmShukkaBatch += first.NmShukkaBatch;
            CdHimban = first.CdHimban;
            CdGtin13 = first.CdGtin13;
            NmHinSeishikimei = first.NmHinSeishikimei;
            NuBoxUnit = first.NuBoxUnit;
        }

        public void SaveItems(Person person)
        {
            var logger = new ScopeLogger(nameof(LargeDistProcessingUnit));

            using (logger.BeginScope("SaveCurrentItem"))
            {
                logger.Debug("begin");

                using (var repo = new LargeDistRepository())
                {
                    foreach (var item in Items)
                    {
                        logger.Debug($"save {item.Id} {item.CdHimban} {item.NmHinSeishikimei} {item.ResultPiece} {item.InputPiece}");

                        if (!item.IsStopped)
                        {
                            repo.Save(item, person);
                            item.IsCompleted = true;
                        }
                        else
                        {
                            logger.Debug($"Stopped");
                        }
                    }

                    repo.Commit();

                }

                logger.Debug("end");
            }

            UpdateItemQuantity();
        }

        private void UpdateItemQuantity()
        {
            foreach (var item in Items)
            {
                if (!item.IsStopped)
                {
                    item.RefrectInputPiece();
                }
            }
        }
    }
}