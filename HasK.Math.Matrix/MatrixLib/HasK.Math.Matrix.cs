using System;
using System.Collections.Generic;
using System.Text;

namespace HasK.Math
{
    public class Matrix: ICloneable, IEquatable<Matrix>
    {
        private double[,] data;
        private int rows, cols;

        public int Rows { get { return rows; } private set { rows = value; } }
        public int Cols { get { return cols; } private set { cols = value; } }

        public Matrix(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Wrong cols or rows number");
            this.rows = rows;
            this.cols = cols;
            data = new double[rows, cols];
        }

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

        public double[,] GetData()
        {
            return data;
        }

        public double Get(int row, int col)
        {
            return data[row, col];
        }

        public void Set(int row, int col, double value)
        {
            data[row, col] = value;
        }

        public double this[int row, int col]
        {
            get { return Get(row, col); }
            set { Set(row, col, value); }
        }

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
            return ToStringData(", ");
        }

        public override string ToString()
        {
            return String.Format("<Matrix {0}x{1}>", rows, cols);
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
        /// Transparent current matrix
        /// </summary>
        /// <returns>Returns new result matrix</returns>
        public Matrix Transparent()
        {
            Matrix res = new Matrix(cols, rows);
            double[,] newData = res.GetData();
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    newData[c, r] = data[r, c];
            return res;
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

        #region ICloneable Members

        public object Clone()
        {
            Matrix clone = new Matrix(cols, rows);
            clone.CopyDataFrom(this);
            return clone;
        }

        public Matrix Copy()
        {
            return Clone() as Matrix;
        }

        #endregion

        #region IEquatable<Matrix> Members

        public bool Equals(Matrix other)
        {
            if (!EqualsByDimension(other))
                return false;
            double[,] odata = other.GetData();
            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                    if (data[r, c] != odata[r, c])
                        return false;
            return true;
        }

        #endregion

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

        public void SetData(params double[] items)
        {
            if (items.Length != rows*cols)
                throw new ArgumentException("The number of input items differs from total data items of matrix");
            int r, c, pos = 0;
            for (r = 0; r < rows; r++)
                for (c = 0; c < cols; c++)
                    data[r, c] = items[pos++];
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

        public static Matrix operator +(Matrix first, Matrix second)
        {
            return first.Add(second);
        }

        /// <summary>
        /// Multiply matrix by scalar value
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

        public static Matrix operator *(Matrix matrix, double value)
        {
            return matrix.Multiply(value);
        }

        /// <summary>
        /// Check if this matrix agreed with other matrix
        /// </summary>
        /// <param name="other">Other matrix</param>
        public bool AgreedWith(Matrix other)
        {
            return cols == other.Rows;
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

        public static Matrix operator *(Matrix first, Matrix second)
        {
            return first.Multiply(second);
        }
    }
}
