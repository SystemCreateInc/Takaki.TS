using DbLib;
using LargeDist.Infranstructures;
using System.Collections.Generic;
using System.Linq;

namespace LargeDist.Models
{
    public class BlockLargeDistItem
    {
        private BlockLargeDistKey _key;
        public IEnumerable<DistItem> Items { get; private set; }

        public string? CdBlock => _key.CdBlock;
        public string CdJuchuBin => _key.CdJuchuBin;
        public string? CdDistGroup => _key.CdDistGroup;
        public string? NmDistGroup => Items.First().NmDistGroup;
        public string? CdShukkaBatch => _key.CdShukkaBatch;
        public string? NmShukkaBatch => Items.First().NmShukkaBatch;

        public int OrderPiece => Items.Sum(x => x.OrderPiece);
        public int ResultPiece => Items.Sum(x => x.ResultPiece);
        public int RemainPiece => Items.Sum(x => x.RemainPiece);
        public int InputPiece => Items.Sum(x => x.InputPiece);

        public bool IsCompleted => Items.All(x => x.IsCompleted);

        public BlockLargeDistItem(BlockLargeDistKey key, IEnumerable<DistItem> list)
        {
            _key = key;
            Items = list;
        }

        public void SaveItems(Person person)
        {
            var logger = new ScopeLogger(nameof(BlockLargeDistItem));

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