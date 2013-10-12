using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using HasK.Math.Matrix;
using HasK.Math.Graph;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            // TestMatrix();
            TestGraph();

            Console.ReadLine();
        }

        private static void TestGraph()
        {
            var G = new Graph("G");
            for (int i = 1; i <= 10; i++)
                G.AddVertex(i.ToString());

            G.AddLink("1", "2"); G.AddLink("1", "3"); G.AddLink("1", "4");
            G.AddLink("2", "5"); G.AddLink("3", "6"); G.AddLink("3", "7"); G.AddLink("4", "8");
            G.AddLink("5", "9"); G.AddLink("6", "10");

            int max = 0, depth = 0;
            Console.WriteLine("\nIn Depth:");
            G.DepthFirstSearch(delegate(Vertex v, Link l, int level) {
                if (l != null)
                    Console.WriteLine("go through vertex {0} from {1}, level {2}", v, l.From, level);
                else
                    Console.WriteLine("go through vertex {0} from NULL", v);
            });


            Console.WriteLine("\nIn Breadth:");
            G.BreadthFirstSearch(delegate(Vertex v) { Console.WriteLine("go through vertex {0}", v); });

            Console.WriteLine(G);
        }

        static void TestMatrix()
        {
            // Test serialization
            Console.WriteLine("Test serialization");
            Matrix G = new Matrix(3, 5);
            G.SetData(
                1, 0, 1, 1, 1,
                0, 1, 0, 1, 0,
                0, 0, 1, 0, 1);

            Console.WriteLine("Source matrix G:");
            Console.WriteLine(G.ToStringData());
            Console.WriteLine();

            // Binary serialization
            var fname = "Matrix.bin";
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Stream stream = new FileStream(fname, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, G);
            stream.Close();
            // Binary deserialization
            stream = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.None);
            Matrix G2 = formatter.Deserialize(stream) as Matrix;
            stream.Close();
            // Binary serialization check
            Console.WriteLine("Binary serialization result (file {0}):", fname);
            Console.WriteLine(G2.ToStringData());
            Console.WriteLine("Equals to source: {0}", G.Equals(G2));
            Console.WriteLine();

            // XML serialization
            fname = "Matrix.xml";
            XmlSerializer xml_serializer = new XmlSerializer(typeof(Matrix));
            stream = new FileStream(fname, FileMode.Create, FileAccess.Write, FileShare.None);
            xml_serializer.Serialize(stream, G);
            stream.Close();
            // XML deserialization
            stream = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.None);
            G2 = xml_serializer.Deserialize(stream) as Matrix;
            stream.Close();
            // XML serialization check
            Console.WriteLine("XML serialization result (file {0}):", fname);
            Console.WriteLine(G2.ToStringData());
            Console.WriteLine("Equals to source: {0}", G.Equals(G2));
            Console.WriteLine();

            // Test math
            Console.WriteLine("Test math");
            Matrix H = new Matrix(2, 5);
            H.SetData(
                1, 1, 0, 1, 0,
                1, 0, 1, 0, 1);
            Console.WriteLine("Matrix H:");
            Console.WriteLine(H.ToStringData());

            Console.WriteLine("R = multiply G to transpose H:");
            Matrix R = G * H.Transpose();
            Console.WriteLine(R.ToStringData());
        }
    }
}
