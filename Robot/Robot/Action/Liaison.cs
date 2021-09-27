using System;
using System.Collections.Generic;
using System.Text;

namespace Robot.Action
{
    public class Liaison
    {
        public PointPosition[] Point;
        public string IDFrom;
        public string IDTo;
        public Dictionary<string, object> Variable;

        public Liaison(PointPosition[] Point, string IDFrom, string IDTo, Dictionary<string, object> Variable)
        {
            this.Point = Point;
            this.IDFrom = IDFrom;
            this.IDTo = IDTo;
            this.Variable = Variable;
        }

        public class PointPosition
        {
            public int X;
            public int Y;

            public PointPosition(int x, int y){
                X = x;
                Y = y;
            }
        }    
    }
}
 