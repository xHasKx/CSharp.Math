using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HasK.Math;

namespace TestMatrixLib
{
    class Program
    {
        static void Main(string[] args)
        {
            Matrix G = new Matrix(3, 5);
            G.SetData(
                1, 0, 0, 1, 1,
                0, 1, 0, 1, 0,
                0, 0, 1, 0, 1);

            Matrix H = new Matrix(2, 5);
            H.SetData(
                1, 1, 0, 1, 0,
                1, 0, 1, 0, 1);

            Matrix rr = G * H.Transparent();

            Console.WriteLine(rr);
            Console.WriteLine(rr.ToStringData());

            Console.ReadLine();
            Matrix v = new Matrix(1, 4);
            v.SetRow(0,    1, 0, 1, 0);

            /*
            Matrix g = new Matrix(4, 7);
            g.SetRow(0,    1, 0, 0, 0, 1, 1, 0);
            g.SetRow(1,    0, 1, 0, 0, 0, 1, 1);
            g.SetRow(2,    0, 0, 1, 0, 1, 1, 1);
            g.SetRow(3,    0, 0, 0, 1, 1, 0, 1);
            */
            Matrix g0 = new Matrix(4, 3);
            g0.SetData(
                1, 1, 0,
                0, 1, 1,
                1, 1, 1,
                1, 0, 1);
            Console.WriteLine("g0:\n" + g0.ToStringData());
            Matrix g = g0.AppendRight(Matrix.Identity(4));


            Matrix g1 = Matrix.Identity(3).AppendDown(g0);
            Console.WriteLine("g1:\n" + g1.ToStringData());

            Console.WriteLine("v:\n" + v.ToStringData());
            Console.WriteLine("g:\n" + g.ToStringData());

            Matrix r = v * g;
            Console.WriteLine("r:\n" + r.ToStringData());

            r[0, 0] = 0; r[0, 1] = 0;
            Matrix r2 = r * g1;
            Console.WriteLine("r2:\n" + r2.ToStringData());

            /*
            var m = new Matrix(3, 4);
            m.SetRow(0, 1, 1, 1);
            m.SetRow(1, 0, 1, 1);
            m.SetRow(2, 1, 0, 1);
            m.SetRow(3, 1, 1, 0);
            Console.WriteLine(m.ToStringData());
            m = m.Transparent();
            Console.WriteLine(m.ToStringData());
            */
            Console.ReadLine();
        }
    }
}
