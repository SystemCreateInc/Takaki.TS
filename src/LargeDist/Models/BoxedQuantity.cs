using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Models
{
    public class BoxedQuantity
    {
        public int BoxUnit { get; }

        public int Total { get; }

        public int Box => BoxUnit == 0 ? 0 : Total / BoxUnit;
        public int Piece => BoxUnit == 0 ? Total : Total % BoxUnit;

        public BoxedQuantity(int boxUnit, int total)
        {
            BoxUnit = boxUnit;
            Total = total;
        }

        public BoxedQuantity(int boxUnit, int box, int piece)
        {
            BoxUnit = boxUnit;
            Total = boxUnit == 0 ? box + piece : box * boxUnit + piece;
        }

        public BoxedQuantity() { }
    }
}
