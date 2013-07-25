using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace HasK.Math
{
    /// <summary>
    /// Simple two-dimensional matrix with double items
    /// </summary>
    [Serializable]
    public class Matrix : ICloneable, IEquatable<Matrix>, ISerializable, IXmlSerializable
    {
        #region Private fields
        private double[,] data;
        [XmlAttribute]
        private int rows, cols;
        #endregion

        #region Public properties
        /// <summary>
        /// Rows count of matrix
        /// </summary>
        [XmlIgnoreAttribute]
        public int Rows { get { return rows; } private set { rows = value; } }
        /// <summary>
        /// Columns count of matrix
        /// </summary>
        [XmlIgnoreAttribute]
        public int Cols { get { return cols; } private set { cols = value; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Create empty matrix with zero size - for serialization
        /// </summary>
        public Matrix()
        {
            this.rows = 0;
            this.cols = 0;
            this.data = null;
        }

        protected Matrix(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info is null");
            rows = info.GetInt32("rows");
            cols = info.GetInt32("cols");
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Wrong cols or rows number");
            data = new double[rows, cols];
            int r, c;
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    data[r, c] = info.GetDouble(r + "_" + c);
        }

        /// <summary>
        /// Create new matrix with specified size
        /// </summary>
        /// <param name="rows">Rows of matrix</param>
        /// <param name="cols">Columns of matrix</param>
        public Matrix(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Wrong cols or rows number");
            this.rows = rows;
            this.cols = cols;
            data = new double[rows, cols];
        }
        #endregion

        #region String representation
        /// <summary>
        /// Returns data in matrix as string
        /// </summary>
        /// <param name="sep">Separator string between elements</param>
        public string ToStringData(string sep)
        {
            int end = cols - 1, r, c, len;
            int[] maxlen = new int[cols];
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                {
                    len = data[r, c].ToString().Length;
                    if (len > maxlen[c])
                        maxlen[c] = len;
                }
            string[] fmt = new string[cols];
            for (c = 0; c < cols; c++)
                fmt[c] = String.Format("{{0,-{0}}}", maxlen[c]);
            string[] lines = new string[rows];
            for (r = 0; r < rows; r++)
            {
                StringBuilder line = new StringBuilder();
                for (c = 0; c < cols; c++)
                {
                    line.Append(String.Format(fmt[c], data[r, c]));
                    if (c < end)
                        line.Append(sep);
                }
                lines[r] = line.ToString();
            }
            return String.Join("\n", lines);
        }

        /// <summary>
        /// Returns data in matrix as string
        /// </summary>
        public string ToStringData()
        {
            return ToStringData("  ");
        }

        /// <summary>
        /// Returns string representation of matrix
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("<Matrix {0}x{1}>", rows, cols);
        }
        #endregion

        #region Data checks
        /// <summary>
        /// Check if this matrix is equals other matrix dimension
        /// </summary>
        /// <param name="other">Other matrix to compare</param>
        public bool EqualsByDimension(Matrix other)
        {
            if (rows != other.rows || cols != other.cols)
                return false;
            return true;
        }

        /// <summary>
        /// Check if this matrix agreed with other matrix
        /// </summary>
        /// <param name="other">Other matrix</param>
        public bool AgreedWith(Matrix other)
        {
            return cols == other.Rows;
        }
        #endregion

        #region Data manipulations
        /// <summary>
        /// Returns array with data in matrix
        /// </summary>
        public double[,] GetData()
        {
            return data;
        }

        /// <summary>
        /// Returns value in specified cell of matrix
        /// </summary>
        /// <param name="row">Row of cell</param>
        /// <param name="col">Column of cell</param>
        public double Get(int row, int col)
        {
            return data[row, col];
        }

        /// <summary>
        /// Set value in specified matrix cell
        /// </summary>
        /// <param name="row">Row of cell</param>
        /// <param name="col">Column of cell</param>
        /// <param name="value">New value of cell</param>
        public void Set(int row, int col, double value)
        {
            data[row, col] = value;
        }

        /// <summary>
        /// Set all cells of matrix to specified value
        /// </summary>
        /// <param name="value">Value for each cell in matrix</param>
        public void SetAll(double value)
        {
            int r, c;
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    data[r, c] = value;
        }

        /// <summary>
        /// Returns cell value by specified row and column
        /// </summary>
        /// <param name="row">Row of cell</param>
        /// <param name="col">Column of cell</param>
        public double this[int row, int col]
        {
            get { return Get(row, col); }
            set { Set(row, col, value); }
        }

        /// <summary>
        /// Copy data from another matrix
        /// </summary>
        /// <param name="matrix">Other matrix - source of data</param>
        public void CopyDataFrom(Matrix other)
        {
            int r, c;
            int min_rows = System.Math.Min(rows, other.rows);
            int min_cols = System.Math.Min(cols, other.cols);
            double[,] odata = other.GetData();
            for (r = 0; r < min_rows; r++)
                for (c = 0; c < min_cols; c++)
                    data[r, c] = odata[r, c];
        }

        /// <summary>
        /// Exchange two rows of matrix
        /// </summary>
        /// <param name="row1">First row</param>
        /// <param name="row2">Second row</param>
        public void ExchangeRows(int row1, int row2)
        {
            double tmp;
            for (int c = 0; c < cols; c++)
            {
                tmp = data[row1, c];
                data[row1, c] = data[row2, c];
                data[row2, c] = tmp;
            }
        }

        /// <summary>
        /// Exchange two columns of matrix
        /// </summary>
        /// <param name="col1">First column</param>
        /// <param name="col2">Second column</param>
        public void ExchangeColumns(int col1, int col2)
        {
            double tmp;
            for (int r = 0; r < rows; r++)
            {
                tmp = data[r, col1];
                data[r, col1] = data[r, col2];
                data[r, col2] = tmp;
            }
        }

        /// <summary>
        /// Append other matrix to the right of the current
        /// </summary>
        /// <param name="other">Other matrix</param>
        /// <returns>Returns new matrix</returns>
        public Matrix AppendRight(Matrix other)
        {
            if (other.rows != rows)
                throw new ArgumentException("Rows number of other matrix not equal current rows number");
            int scols = cols + other.cols, r, c;
            Matrix res = new Matrix(rows, scols);
            double[,] rdata = res.GetData();
            double[,] odata = other.GetData();
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    rdata[r, c] = data[r, c];
            for (r = 0; r < rows; r++)
                for (c = cols; c < scols; c++)
                    rdata[r, c] = odata[r, c - cols];
            return res;
        }

        /// <summary>
        /// Append other matrix to the bottom of the current
        /// </summary>
        /// <param name="other">Other matrix</param>
        /// <returns>Returns new matrix</returns>
        public Matrix AppendDown(Matrix other)
        {
            if (other.cols != cols)
                throw new ArgumentException("Cols number of other matrix not equal current cols number");
            int srows = rows + other.rows, r, c;
            Matrix res = new Matrix(srows, cols);
            double[,] rdata = res.GetData();
            double[,] odata = other.GetData();
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    rdata[r, c] = data[r, c];
            for (r = rows; r < srows; r++)
                for (c = 0; c < cols; c++)
                    rdata[r, c] = odata[r - rows, c];
            return res;
        }

        /// <summary>
        /// Set given row in matrix data
        /// </summary>
        /// <param name="row">Row number</param>
        /// <param name="items">Row items</param>
        public void SetRow(int row, params double[] items)
        {
            int c;
            if (items.Length != cols)
                throw new ArgumentException("The number of input items differs from the cols number of matrix");
            for (c = 0; c < cols; c++)
                data[row, c] = items[c];
        }

        /// <summary>
        /// Set all items in matrix to specified values
        /// </summary>
        /// <param name="items">Items to put to matrix</param>
        public void SetData(params double[] items)
        {
            if (items.Length != rows*cols)
                throw new ArgumentException("The number of input items differs from total data items of matrix");
            int r, c, pos = 0;
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    data[r, c] = items[pos++];
        }
        #endregion

        #region Math operations
        /// <summary>
        /// Returns new identity matrix
        /// </summary>
        /// <param name="size">Dimension of matrix</param>
        /// <param name="value">Value on main diagonal of matrix</param>
        public static Matrix Identity(int size, double value)
        {
            Matrix m = new Matrix(size, size);
            double[,] data = m.GetData();
            for (int i = 0; i < size; i++)
                data[i, i] = value;
            return m;
        }

        /// <summary>
        /// Returns new identity matrix
        /// </summary>
        /// <param name="size">Dimension of matrix</param>
        public static Matrix Identity(int size)
        {
            return Identity(size, 1);
        }

        /// <summary>
        /// Transparent current matrix
        /// </summary>
        /// <returns>Returns new result matrix</returns>
        public Matrix Transparent()
        {
            Matrix res = new Matrix(cols, rows);
            double[,] newData = res.GetData();
            int r, c;
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    newData[c, r] = data[r, c];
            return res;
        }

        /// <summary>
        /// Add other matrix to current
        /// </summary>
        /// <param name="other">Other matrix</param>
        /// <returns>Returns new result matrix</returns>
        public Matrix Add(Matrix other)
        {
            if (!EqualsByDimension(other))
                throw new ArgumentException("Other matrix has different dimension");
            int c, r;
            Matrix res = Copy();
            double[,] rdata = res.GetData();
            double[,] odata = other.GetData();
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    rdata[r, c] += odata[r, c];
            return res;
        }

        /// <summary>
        /// Add second matrix to first
        /// </summary>
        /// <param name="first">First matrix</param>
        /// <param name="second">Second matrix</param>
        /// <returns>Returns new result matrix</returns>
        public static Matrix operator +(Matrix first, Matrix second)
        {
            return first.Add(second);
        }

        /// <summary>
        /// Multiplies matrix by scalar value
        /// </summary>
        /// <param name="value">Multiplier value</param>
        /// <returns>Returns new result matrix</returns>
        public Matrix Multiply(double value)
        {
            int c, r;
            Matrix res = Copy();
            double[,] rdata = res.GetData();
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    rdata[r, c] *= value;
            return res;
        }

        /// <summary>
        /// Multiplies matrix by scalar value
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <param name="value">Multiplier value</param>
        /// <returns>Returns new result matrix</returns>
        public static Matrix operator *(Matrix matrix, double value)
        {
            return matrix.Multiply(value);
        }

        /// <summary>
        /// Multiply matrix by other matrix
        /// </summary>
        /// <param name="other">Multiplier matrix</param>
        /// <returns>Returns new result matrix</returns>
        public Matrix Multiply(Matrix other)
        {
            double val;
            int ocols = other.cols, c, r, k;
            double[,] odata = other.GetData();
            Matrix res = new Matrix(rows, ocols);
            double[,] rdata = res.GetData();
            for (r = 0; r < rows; r++)
                for (c = 0; c < ocols; c++)
                {
                    val = 0;
                    for (k = 0; k < cols; k++)
                        val += data[r, k] * odata[k, c];
                    rdata[r, c] = val;
                }
            return res;
        }

        /// <summary>
        /// Multiply first matrix by second matrix
        /// </summary>
        /// <param name="first">First matrix</param>
        /// <param name="second">Second matrix</param>
        /// <returns>Returns new result matrix</returns>
        public static Matrix operator *(Matrix first, Matrix second)
        {
            return first.Multiply(second);
        }

        /// <summary>
        /// Check if first matrix equals second
        /// </summary>
        /// <param name="first">First matrix</param>
        /// <param name="second">Second matrix</param>
        public static bool operator ==(Matrix first, Matrix second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Check if first matrix NOT equals second
        /// </summary>
        /// <param name="first">First matrix</param>
        /// <param name="second">Second matrix</param>
        public static bool operator !=(Matrix first, Matrix second)
        {
            return !first.Equals(second);
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Create clone object of matrix
        /// </summary>
        /// <returns>Returns new matrix with exact copy of data as object</returns>
        public object Clone()
        {
            Matrix clone = new Matrix(cols, rows);
            clone.CopyDataFrom(this);
            return clone;
        }

        /// <summary>
        /// Create copy of matrix
        /// </summary>
        /// <returns>Returns new matrix with exact copy of data</returns>
        public Matrix Copy()
        {
            return Clone() as Matrix;
        }

        #endregion

        #region IEquatable<Matrix> Members
        /// <summary>
        /// Check if other matrix equals current
        /// </summary>
        /// <param name="other">Other matrix</param>
        public bool Equals(Matrix other)
        {
            if (!EqualsByDimension(other))
                return false;
            double[,] odata = other.GetData();
            int r, c;
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    if (data[r, c] != odata[r, c])
                        return false;
            return true;
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Get object data for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info is null");
            info.AddValue("rows", rows);
            info.AddValue("cols", cols);
            int r, c;
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    info.AddValue(r + "_" + c, data[r, c]);
        }
        #endregion

        #region IXmlSerializable Members
        /// <summary>
        /// Returns XML Schema - null for Matrix
        /// </summary>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Read matrix data from XML reader
        /// </summary>
        /// <param name="reader">XML reader with matrix</param>
        public void ReadXml(XmlReader reader)
        {
            rows = Int32.Parse(reader.GetAttribute("rows"));
            cols = Int32.Parse(reader.GetAttribute("cols"));
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Wrong cols or rows number");
            data = new double[rows, cols];
            int r, c;
            for (r = 0; r < rows; r++)
            {
                reader.ReadToFollowing("r" + r);
                for (c = 0; c < cols; c++)
                {
                    reader.MoveToAttribute("c" + c);
                    data[r, c] = reader.ReadContentAsDouble();
                }
            }
        }

        /// <summary>
        /// Write matrix to XML writer
        /// </summary>
        /// <param name="writer">XML writer</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute("rows");
            writer.WriteValue(rows);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("cols");
            writer.WriteValue(cols);
            writer.WriteEndAttribute();
            int r, c;
            for (r = 0; r < rows; r++)
            {
                writer.WriteStartElement("r" + r);
                for (c = 0; c < cols; c++)
                {
                    writer.WriteStartAttribute("c" + c);
                    writer.WriteValue(data[r, c]);
                    writer.WriteEndAttribute();
                }
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
