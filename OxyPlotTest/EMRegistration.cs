using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CoherentPointDrift
{
    public abstract class EMRegistration
    {
        protected Matrix<double> TargetPoints;
        protected Matrix<double> SourcePoints;
        protected Matrix<double> TransformSourcePoints;
        protected double Sigma2;
        protected int NumTargetPoints;
        protected int NumSourcePoints;
        protected int DimOfTargetPoints;
        protected int CurrentIteration;
        protected int MaxIteration;
        protected double Tolerance;
        protected double Weight;
        protected double MissAlignment;
        /// <summary>
        /// diff: double (positive)
        ///The absolute difference between the current and previous objective function values.
        /// </summary>
        protected double Diff;
        protected Matrix<double> ProbabilitiesMatrix;
        protected Matrix<double> Pt1;
        protected Matrix<double> P1;
        protected Matrix<double> PX;
        protected double SumOfProbabilities;
        public EMRegistration(Matrix<double> target, Matrix<double> source, double sigma2 = 0, int max_iterations = 0  ,double tolerance = 0, double weight = 0   )
        {
            this.TargetPoints = target.Clone();
            this.SourcePoints = source.Clone();
            this.TransformSourcePoints = source.Clone();
            this.Sigma2 = this.InitializeSigma2(this.TargetPoints, this.SourcePoints);
            this.NumSourcePoints = this.SourcePoints.RowCount;
            this.NumTargetPoints = this.TargetPoints.RowCount;
            this.DimOfTargetPoints = this.TargetPoints.ColumnCount;
            this.Tolerance = (tolerance != 0 ? tolerance : 0.001);
            this.Weight = ( weight < 1 && weight > 0 ? weight : 0.5);
            this.MaxIteration = (max_iterations > 0 ? max_iterations : 300);
            this.CurrentIteration = 0;
            this.Diff = double.MaxValue;
            this.MissAlignment = double.MaxValue;
            this.ProbabilitiesMatrix = DenseMatrix.OfArray(new double[this.NumSourcePoints, this.NumTargetPoints] );
            this.Pt1 = DenseMatrix.OfArray(new double[this.NumTargetPoints, 1]);
            this.P1 = DenseMatrix.OfArray(new double[this.NumSourcePoints, 1]);
            this.PX = DenseMatrix.OfArray(new double[this.NumSourcePoints, this.DimOfTargetPoints]);
            this.SumOfProbabilities = 0;

        }
        /// <summary>
        /// Initialize the variance (sigma2)
        /// </summary>
        /// <param name="X">X : NxD array of points for target</param>
        /// <param name="Y">Y:  MxD array of points for source.</param>
        /// <returns>sigma2  Initial variance.</returns>
        public double InitializeSigma2(Matrix<double> X , Matrix<double> Y)
        {
            double sigma = 0;
            
            for( int i = 0; i < X.RowCount; ++i)
            {
                for( int j = 0; j < Y.RowCount; ++j)
                {
                    sigma += Math.Pow((X.Row(i) - Y.Row(j)).Norm(2), 2);
                }
            }
            return sigma / (X.RowCount * Y.RowCount * X.ColumnCount); 
        }
        public void Expectation()
        {
            double sum;
            double constant = Math.Pow(2 * Math.PI * this.Sigma2, this.DimOfTargetPoints / 2) * (this.Weight / (1 - this.Weight)) * (this.NumSourcePoints / this.NumTargetPoints);
            for( int i = 0;i < this.ProbabilitiesMatrix.ColumnCount; ++i)
            {
                sum = 0;
                for( int j = 0; j < this.ProbabilitiesMatrix.RowCount; ++j)
                {
                    this.ProbabilitiesMatrix[j, i] = Math.Exp(-Math.Pow((TargetPoints.Row(i) - TransformSourcePoints.Row(j)).Norm(2), 2)/(2*this.Sigma2));
                    sum += ProbabilitiesMatrix[j, i];
                }
             
                this.ProbabilitiesMatrix.SetColumn(i, this.ProbabilitiesMatrix.Column(i) / (sum+constant));
            }
            //this.Pt1 = ProbabilitiesMatrix.ColumnSums().ToColumnMatrix();
            this.Pt1 = ProbabilitiesMatrix.Transpose() * DenseMatrix.Create(this.ProbabilitiesMatrix.Transpose().ColumnCount, 1, 1);
            //this.P1 = ProbabilitiesMatrix.RowSums().ToColumnMatrix();
            this.P1 = this.ProbabilitiesMatrix * DenseMatrix.Create(this.ProbabilitiesMatrix.ColumnCount, 1, 1);
            this.SumOfProbabilities = P1.ColumnSums().Sum();
            this.PX = this.ProbabilitiesMatrix * this.TargetPoints;
        }
        public void Maximization()
        {
            this.UpdateTransform();
            this.TransformPointCloud();
            this.UpdateVariance();
        }
        public void Iterate()
        {
            this.Expectation();
            this.Maximization();
            this.CurrentIteration += 1;
        }
        public List<object> Register(Func<Dictionary<string , object> , bool > callback  = null)
        {
            this.TransformPointCloud();
            while( this.CurrentIteration < this.MaxIteration && this.Diff > this.Tolerance*0.01)
            {
                this.Iterate();
                if (callback != null)
                {
                    Dictionary<string, object> kwargs = new Dictionary<string, object>
                    {
                        {"iteration" , this.CurrentIteration },
                        {"error" , this.MissAlignment },
                        {"X" , this.TargetPoints },
                        {"Y" , this.TransformSourcePoints }
                    };
                    callback(kwargs );
                }
            }
            Console.WriteLine("Targetpoints :");
            Console.WriteLine(this.TargetPoints);
            Console.WriteLine("TransformSourcePoints :");
            Console.WriteLine(this.TransformSourcePoints);
            Console.WriteLine("iteration : {0}", this.CurrentIteration);
            Console.WriteLine("error : {0}", this.MissAlignment);
            return this.GetRegistrationParameters();
        }
        public abstract void TransformPointCloud(Matrix<double> Y = null);
        public abstract List<object> GetRegistrationParameters();
        public abstract void UpdateTransform();
        public abstract void UpdateVariance();
        

    }
}
