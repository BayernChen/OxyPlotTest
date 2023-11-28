using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics;
using System.Numerics;

namespace CoherentPointDrift
{
    public class DeformableRegistration:EMRegistration
    {
        private Matrix<double> W;
        private Matrix<double> GaussianKernel;
        private Matrix<double> Inv_S;
        private Matrix<double> S;
        private Matrix<double> Q;
        private double Alpha;
        private double Beta;
        private double E;
        private int NumEig;
        private bool LowRank;

        public DeformableRegistration( Matrix<double> Target , Matrix<double> Source , double alpha = 2, double beta = 2, int numEig=100, bool lowRank=false):base(Target , Source)
        {
            this.NumEig = numEig;
            this.LowRank = lowRank;
            this.Alpha = (alpha <= 0 ? 2 : alpha) ;
            this.Beta = (beta <= 0 ? 2 : beta) ;
            this.W = DenseMatrix.Create(base.SourcePoints.RowCount, base.DimOfTargetPoints, 0);
            this.GaussianKernel = CreateGaussianKernel(base.SourcePoints, this.Beta);
            if( lowRank)
            {
                this.LowRankEigen(this.GaussianKernel, this.NumEig , out this.Q , out this.S );
                this.Inv_S = S.Inverse();
                this.E = 0;
            }
            
        }
        public override void UpdateTransform()
        {
            if (this.LowRank)
            {
                Matrix<double> dP = DenseMatrix.OfDiagonalVector(base.P1.Column(0));
                Matrix<double> dPQ = dP * this.Q;
                Matrix<double> f = base.PX - dP * base.SourcePoints;

                this.W = (1 / (this.Alpha * base.Sigma2)) * (f - dPQ * (this.Alpha * base.Sigma2 * this.Inv_S + this.Q.Transpose() * dPQ).Solve(this.Q.Transpose() * f));
                Matrix<double> QtW = this.Q.Transpose() * this.W;
                this.E = this.E + (this.Alpha / 2) * (QtW.Transpose() * this.S * QtW).Trace();
            }
            else
            {
                Matrix<double> A = DenseMatrix.OfDiagonalVector(base.P1.Column(0)) * this.GaussianKernel + this.Alpha * this.Sigma2 * DenseMatrix.CreateIdentity(base.NumSourcePoints);
                Matrix<double> B = this.PX - DenseMatrix.OfDiagonalVector(base.P1.Column(0)) * base.SourcePoints;
                this.W = A.Solve(B);
            }
        }
        public override void TransformPointCloud(Matrix<double> Y = null)
        {
            if (Y != null)
            {
                this.GaussianKernel = this.CreateGaussianKernel(Y, this.Beta, base.SourcePoints);
                base.TransformSourcePoints = this.GaussianKernel * this.W;
            }else if (this.LowRank)
            {
                base.TransformSourcePoints = base.SourcePoints + this.Q * this.S * this.Q.Transpose() * this.W;
            }
            else
            {
                base.TransformSourcePoints = base.SourcePoints + this.GaussianKernel * this.W;
            }
        }
        public override void UpdateVariance()
        {
            double qprev = base.Sigma2;
            base.MissAlignment = double.MaxValue;
            double xPx = (base.TargetPoints.Transpose() * DenseMatrix.OfDiagonalVector(base.Pt1.Column(0)) * base.TargetPoints).Trace();
            double yPy = (base.TransformSourcePoints.Transpose() * DenseMatrix.OfDiagonalVector(base.P1.Column(0)) * base.TransformSourcePoints).Trace();
            double trPXY = ((base.ProbabilitiesMatrix * base.TargetPoints).Transpose() * base.TransformSourcePoints).Trace();
            base.Sigma2 = (xPx - 2 * trPXY + yPy) / (base.DimOfTargetPoints * base.SumOfProbabilities);
            if (base.Sigma2 <= 0) base.Sigma2 = base.Tolerance / 10;
            base.Diff = Math.Abs(base.Sigma2 - qprev);
        }
        public override List<object> GetRegistrationParameters()
        {
            List<object> parameters = new List<object>();
            Console.WriteLine("Gaussia kernel matrix :");
            Console.WriteLine(this.GaussianKernel);
            Console.WriteLine("Deformable transformation matrix : ");
            Console.WriteLine(this.W);
            parameters.Add(this.GaussianKernel);
            parameters.Add(this.W);
            return parameters;
        }
        public void LowRankEigen(Matrix<double> G , int numEig , out Matrix<double> eigVectors , out Matrix<double> eigValues)
        {
            Evd<double> evd = G.Evd();
            int[] indices = Enumerable.Range(0, evd.EigenValues.Count).ToArray();
            double[] eigenValues = evd.EigenValues.Real().PointwiseAbs().ToArray();
            Array.Sort(eigenValues, indices);
            Array.Reverse(indices);
            double[] eigvalues = new double[(numEig > indices.Length ? indices.Length : numEig)];
            eigVectors = DenseMatrix.Create(G.RowCount, eigvalues.Length,0);
            for ( int i = 0; i < eigvalues.Length; ++i)
            {
                eigvalues[i] = evd.EigenValues.Real()[indices[i]];
                eigVectors.SetColumn(i, evd.EigenVectors.Column(indices[i]));
            }
            eigValues = DenseMatrix.OfDiagonalArray(eigvalues);
        }
        public Matrix<double> CreateGaussianKernel(Matrix<double> X, double beta, Matrix<double> Y = null)
        {
            if (Y == null) Y = X.Clone();
            Matrix<double> G = DenseMatrix.OfArray(new double[X.RowCount, Y.RowCount]);
            for( int i = 0; i < G.RowCount; ++i)
            {
                for ( int j = 0; j < G.ColumnCount; ++j)
                {
                    G[i, j] = Math.Exp(-Math.Pow((X.Row(i) - Y.Row(j)).Norm(2),2)/(2*beta));
                }
            }
            return G;
        }
    }
}
