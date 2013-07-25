using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HasK.Math;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TestMatrixLib
{
    class Program
    {
        static void Main(string[] args)
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
            Console.WriteLine("Equals to source: {0}", G == G2);
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
            Console.WriteLine("Equals to source: {0}", G == G2);
            Console.WriteLine();

            Console.WriteLine("Test math");
            Matrix H = new Matrix(2, 5);
            H.SetData(
                1, 1, 0, 1, 0,
                1, 0, 1, 0, 1);
            Console.WriteLine("Matrix H:");
            Console.WriteLine(H.ToStringData());

            Console.WriteLine("R = multiply G to transparent H:");
            Matrix R = G * H.Transparent();
            Console.WriteLine(R.ToStringData());

            Console.ReadLine();
        }
    }
}
