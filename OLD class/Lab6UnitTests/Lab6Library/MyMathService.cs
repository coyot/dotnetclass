using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6Library
{
    public class MyMathService: IMathService
    {
        public double Power(double x, double y)
        {
            return Math.Pow(x, y);
        }
    }
}
