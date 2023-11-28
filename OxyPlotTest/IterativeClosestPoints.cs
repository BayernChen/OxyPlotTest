using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxyPlotTest
{
    public class IterativeClosestPoints
    {
        private double Error = 100;   //當前匹配誤差
        private double Threshold = 0.001; //當小於這個數值就停止迭代  
        private KDtree Tree;
        private List<double[]> Source;   //點雲來源
        private List<double[]> Target;   //目標點雲
        private Matrix<double> SourceMatrix;
        private Matrix<double> TargetMatrix;
        private Matrix<double> FinalTargetMatrix; //找到最小loss的Target
        private Matrix<double> Rotation;  //旋轉矩陣
        private Vector<double> Translate; //平移向量

        private Vector<double> TargetCenter = DenseVector.OfArray(new double[2] { 0, 0 }); //Target中心座標
        private Vector<double> TargetLeft = DenseVector.OfArray(new double[2] { 0, 0 });   //Target左邊邊緣座標
        private Vector<double> TargetRight = DenseVector.OfArray(new double[2] { 0, 0 });   //Target右邊邊緣座標
        private Vector<double> TargetUp = DenseVector.OfArray(new double[2] { 0, 0 });     //Target上面邊緣座標
        private Vector<double> TargetDown = DenseVector.OfArray(new double[2] { 0, 0 });   //Target下面邊緣座標

        private int Iterator = 0;   //當前迭代次數
        private int MaxIterator = 30; // 最大迭代次數
        private double Angle; // 旋轉幅度

        /// <summary>
        /// 能夠計算出兩個點雲之間的平移向量以及旋轉角度，Source點雲先通過旋轉再進行平移使得Source點雲與Target點雲能夠重合，點雲的資料可以是沒有順序(除了點雲數量只有兩筆的情況)，可以少對多(但點雲的數量差異不能太多)
        /// </summary>
        /// <param name="source">點雲來源</param>
        /// <param name="target">目標點雲</param>
        public IterativeClosestPoints(List<double[]> source, List<double[]> target)
        {

            List<double[]> tmp = new List<double[]>();
            double[] p;
            this.Target = target;
            for (int i = 0; i < target.Count; ++i)
            {
                p = new double[2];
                target[i].CopyTo(p, 0);
                tmp.Add(p);
            }

            this.Source = source;
            this.Tree = new KDtree(tmp);
            this.SourceMatrix = ListToMatrix(source);
            this.FinalTargetMatrix = DenseMatrix.OfArray(new double[this.SourceMatrix.RowCount, this.SourceMatrix.ColumnCount]);
            this.TargetCenter = (ListToMatrix(target).Transpose()).RowSums().Divide(target.Count);
            target.Sort((x, y) => Compare(x, y, 1));
            this.TargetUp[0] = this.TargetCenter[0];
            this.TargetUp[1] = target[target.Count - 1][1];
            this.TargetDown[0] = this.TargetCenter[0];
            this.TargetDown[1] = target[0][1];
            target.Sort((x, y) => Compare(x, y, 0));
            this.TargetLeft[1] = this.TargetCenter[1];
            this.TargetLeft[0] = target[0][0];
            this.TargetRight[1] = this.TargetCenter[1];
            this.TargetRight[0] = target[target.Count - 1][0];

        }
        public IterativeClosestPoints(Matrix<double> t , Matrix<double> s)
        {
            List<double[]> target = this.MatrixToList(t);
            List<double[]> source = this.MatrixToList(s);
            List<double[]> tmp = new List<double[]>();
            double[] p;
            this.Target = target;
            for (int i = 0; i < target.Count; ++i)
            {
                p = new double[2];
                target[i].CopyTo(p, 0);
                tmp.Add(p);
            }

            this.Source = source;
            this.Tree = new KDtree(tmp);
            this.SourceMatrix = ListToMatrix(source);
            this.FinalTargetMatrix = DenseMatrix.OfArray(new double[this.SourceMatrix.RowCount, this.SourceMatrix.ColumnCount]);
            this.TargetCenter = (ListToMatrix(target).Transpose()).RowSums().Divide(target.Count);
            target.Sort((x, y) => Compare(x, y, 1));
            this.TargetUp[0] = this.TargetCenter[0];
            this.TargetUp[1] = target[target.Count - 1][1];
            this.TargetDown[0] = this.TargetCenter[0];
            this.TargetDown[1] = target[0][1];
            target.Sort((x, y) => Compare(x, y, 0));
            this.TargetLeft[1] = this.TargetCenter[1];
            this.TargetLeft[0] = target[0][0];
            this.TargetRight[1] = this.TargetCenter[1];
            this.TargetRight[0] = target[target.Count - 1][0];
        }
        public IterativeClosestPoints() { }

        /// <summary>
        /// 將List轉成Matrix
        /// </summary>
        /// <param name="s"></param>
        /// <returns> n by 2 matrix( deep copy )</returns>
        private Matrix<double> ListToMatrix(List<double[]> s)
        {
            Matrix<double> a = DenseMatrix.OfArray(new double[s.Count, 2]);
            for (int i = 0; i < s.Count; ++i)
            {
                for (int j = 0; j < 2; ++j) a[i, j] = s[i][j];
            }
            return a;
        }
        private List<double[]> MatrixToList(Matrix<double> s)
        {
            List<double[]> SL = new List<double[]>();
            for ( int i = 0;i < s.RowCount; ++i)
            {
                SL.Add(s.Row(i).ToArray());
            }
            return SL;
        }
        /// <summary>
        /// 輸入Source Matrix，找出Source點雲中每一個點在Target點雲最靠近的對應點
        /// </summary>
        /// <param name="s">where S is 2 by n matrix </param>
        /// <returns>回傳List</returns>
        private List<double[]> FindClosestPoints(Matrix<double> s)
        {
            List<double[]> closepoints = new List<double[]>();
            for (int i = 0; i < s.ColumnCount; ++i)
            {
                closepoints.Add(Tree.NearestNode(s.Column(i).ToArray()).Data);
            }
            return closepoints;
        }
        /// <summary>
        /// 計算Loss，計算的值越小代表匹配效果越好
        /// </summary>
        /// <param name="a">Source </param>
        /// <param name="b">Target </param>
        /// <returns>Loss value </returns>
        private double Loss(Matrix<double> a, Matrix<double> b)
        {
            double loss = 0;
            Matrix<double> tmp = a - b;    /// where A , B are 2 by n matrix 
            for (int i = 0; i < tmp.ColumnCount; ++i) loss += Math.Sqrt(tmp[0, i] * tmp[0, i] + tmp[1, i] * tmp[1, i]);
            return loss / a.ColumnCount;
        }
        /// <summary>
        /// 將Source點雲移動到指定位置
        /// </summary>
        /// <param name="centerT">為移動到的位置</param>
        private void CenterTranslateTo(Vector<double> centerT)
        {

            Matrix<double> s = ListToMatrix(Source).Transpose();
            Vector<double> centerS = s.RowSums().Divide(s.ColumnCount);
            Matrix<double> x = DenseMatrix.OfArray(new double[2, 0]);
            Vector<double> trans = centerT - centerS;
            for (int i = 0; i < s.ColumnCount; ++i)
            {
                x = x.Append(DenseMatrix.OfColumnVectors(s.Column(i) + trans));
            }
            this.SourceMatrix = x.Transpose();
        }
        /// <summary>
        /// 計算旋轉與平移量
        /// </summary>
        /// <param name="s">為Source 2Xn Matrix </param>
        /// <param name="t">為Target 2Xn Matrix</param>
        public void CaculateRotationTranslate(Matrix<double> s, Matrix<double> t) //where S is source 2 by n matrix and T is target 2 by n matrix 
        {

            Vector<double> avergS = s.RowSums().Divide(s.ColumnCount);
            Vector<double> avergT = t.RowSums().Divide(t.ColumnCount);

            Matrix<double> x = DenseMatrix.OfArray(new double[2, 0]);
            Matrix<double> y = DenseMatrix.OfArray(new double[2, 0]);
            for (int i = 0; i < s.ColumnCount; ++i)
            {
                x = x.Append(DenseMatrix.OfColumnVectors(s.Column(i) - avergS));
                y = y.Append(DenseMatrix.OfColumnVectors(t.Column(i) - avergT));
            }
            y = y.Transpose();
            Svd<double> singular = (x * y).Svd();
            Matrix<double> identity = DenseMatrix.OfArray(new double[,] { { 1, 0 }, { 0, (singular.VT.Transpose() * singular.U.Transpose()).Determinant() } });
            this.Rotation = singular.VT.Transpose() * identity * singular.U.Transpose();
            this.Translate = avergT - (this.Rotation * avergS);
            this.CalAngleByAtan(this.Rotation[0, 0], this.Rotation[1, 0], ref this.Angle);
        }
        public Matrix<double> GetRotation
        {
            get { return this.Rotation; }
        }
        public Matrix<double> GetTranslate
        {
            get { return this.Translate.ToRowMatrix(); }
        }
        /// <summary>
        /// 輸入旋轉矩陣與平移向量，對Source進行對位 
        /// </summary>
        /// <param name="rota">旋轉矩陣</param>
        /// <param name="trans">平移向量</param>
        /// <returns>對位之後的Source n by 2 </returns>
        private Matrix<double> Alignment(Matrix<double> rota, Vector<double> trans)
        {
            this.SourceMatrix = rota * ((this.SourceMatrix).Transpose());
            Matrix<double> x = DenseMatrix.OfArray(new double[2, 0]);
            for (int i = 0; i < this.SourceMatrix.ColumnCount; ++i) x = x.Append(DenseMatrix.OfColumnVectors(this.SourceMatrix.Column(i) + this.Translate));
            return x.Transpose();

        }
        /// <summary>
        /// 根據sp的數字來對Array a 、Array b 進行比較，sp = i代表根據array[i]來比較大小
        /// </summary>
        /// <param name="a">為一維陣列</param>
        /// <param name="b">為一維陣列</param>
        /// <param name="sp">指array的index</param>
        /// <returns>1: a > b  0:  a = b  -1: a < b  </returns>
        private int Compare(double[] a, double[] b, int sp)
        {
            if (a[sp] > b[sp])
            {
                return 1;
            }
            else if (a[sp] == b[sp])
            {
                return 0;
            }
            else
            {
                return -1;
            }

        }


        /// <summary>
        /// 開始進行迭代
        /// </summary>
        /// <returns>true : 迭代成功 false : 迭代失敗</returns>
        public bool Iteration(Func<Dictionary<string, object>, bool> callback = null)
        {
            if (this.Source.Count == 1) //當點雲中的點集數量只有一個時
            {
                this.Translate = DenseVector.OfArray(new double[2] { this.Target[0][0] - this.Source[0][0], this.Target[0][1] - this.Source[0][1] });
                this.Angle = 0;
                this.Error = 0;
                return true;
            }
            else if (this.Source.Count == 2)//當點雲中的點集數量只有兩個時
            {
                Vector<double> s = DenseVector.OfArray(new double[2] { this.Source[1][0] - this.Source[0][0], this.Source[1][1] - this.Source[0][1] });
                Vector<double> t = DenseVector.OfArray(new double[2] { this.Target[1][0] - this.Target[0][0], this.Target[1][1] - this.Target[0][1] });
                double crossproduct = DenseMatrix.OfRowVectors(s, t).Determinant();
                this.Angle = Math.Atan(crossproduct / (s * t)) * 180 / Math.PI;
                double radius = Math.Atan(crossproduct / (s * t));
                this.Rotation = DenseMatrix.OfArray(new double[2, 2] { { Math.Cos(radius), -Math.Sin(radius) }, { Math.Sin(radius), Math.Cos(radius) } });
                Vector<double> center_s = DenseVector.OfArray(new double[2] { (this.Source[1][0] + this.Source[0][0]) * 0.5, (this.Source[1][1] + this.Source[0][1]) * 0.5 });
                Vector<double> center_t = DenseVector.OfArray(new double[2] { (this.Target[1][0] + this.Target[0][0]) * 0.5, (this.Target[1][1] + this.Target[0][1]) * 0.5 });
                this.Translate = center_t - this.Rotation * center_s;
                this.Error = Math.Abs(s.Norm(2) - t.Norm(2)) * 0.5;
                return (this.Error < this.Threshold);
            }
            double E = 100;
            while (this.Error > this.Threshold && this.Iterator < this.MaxIterator *6)
            {
                /*

                if (this.Iterator / this.MaxIterator == 1 && this.Iterator % this.MaxIterator == 0)
                {
                    this.CenterTranslateTo(this.TargetUp);
                }
                else if (this.Iterator / this.MaxIterator == 2 && this.Iterator % this.MaxIterator == 0)
                {
                    this.CenterTranslateTo(this.TargetLeft);
                }
                else if (this.Iterator / this.MaxIterator == 3 && this.Iterator % this.MaxIterator == 0)
                {
                    this.CenterTranslateTo(this.TargetCenter);
                }
                else if (this.Iterator / this.MaxIterator == 4 && this.Iterator % this.MaxIterator == 0)
                {
                    this.CenterTranslateTo(this.TargetDown);
                }
                else if (this.Iterator / this.MaxIterator == 5 && this.Iterator % this.MaxIterator == 0)
                {
                    this.CenterTranslateTo(this.TargetRight);
                }
                */
                //else
                //{
                    this.TargetMatrix = this.ListToMatrix(this.FindClosestPoints(this.SourceMatrix.Transpose()));  //Calculate correspondence
                    this.CaculateRotationTranslate(this.SourceMatrix.Transpose(), this.TargetMatrix.Transpose()); //Calculate alignment
                    this.SourceMatrix = this.Alignment(this.Rotation, this.Translate);                            //Apply alignment
                    this.Error = this.Loss(this.SourceMatrix.Transpose(), this.TargetMatrix.Transpose());         //Update Error
                    if (callback != null)
                    {
                        Dictionary<string, object> kwargs = new Dictionary<string, object>
                    {
                        {"iteration" , this.Iterator },
                        {"error" , this.Error },
                        {"X" , this.ListToMatrix(this.Target) },
                        {"Y" , this.SourceMatrix }
                    };
                        callback(kwargs);
                    }
                //}
                if (E > this.Error)  //把匹配效果最好的點雲暫時存在FinalTargetMatrix變數中
                {
                    E = this.Error;
                    this.TargetMatrix.CopyTo(this.FinalTargetMatrix);
                }
                this.Iterator++;
            }
            this.SourceMatrix = this.ListToMatrix(this.Source);
            this.CaculateRotationTranslate(this.SourceMatrix.Transpose(), this.FinalTargetMatrix.Transpose());
            this.SourceMatrix = this.Alignment(this.Rotation, this.Translate);
            this.Error = this.Loss(this.SourceMatrix.Transpose(), this.FinalTargetMatrix.Transpose());
            if (this.CalAngleByAtan(this.Rotation[0, 0], this.Rotation[1, 0], ref this.Angle) && this.Error < this.Threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CalAngleByAtan(double cos, double sin, ref double angle, double maxAngle = 100)
        {
            if (cos < -1 || cos > 1)
                return false;
            angle = Math.Round((Math.Atan(sin / cos) / Math.PI) * 180, 5);
            return (Math.Abs(angle) < maxAngle);
        }

    }
}
