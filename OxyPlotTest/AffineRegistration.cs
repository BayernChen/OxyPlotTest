using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoherentPointDrift
{
    public  class AffineRegistration : EMRegistration
    {
        private Matrix<double> B;
        private Matrix<double> T;

        private Matrix<double> YPY;
        private Matrix<double> X_hat;
        private Matrix<double> A;

        public AffineRegistration(Matrix<double> Target , Matrix<double> Source, Matrix<double> b = null  , Matrix<double> t =null):base(Target , Source)
        {
            if (b != null && (base.DimOfTargetPoints == 2 || base.DimOfTargetPoints == 3) && b.Evd().EigenValues.Real().Minimum() > 0 && b.Evd().EigenValues.Imaginary().Maximum() == 0 && b.Evd().EigenValues.Imaginary().Minimum() == 0)
            {
                this.B = b;
            }
            else
            {
                this.B = DenseMatrix.CreateIdentity(base.DimOfTargetPoints);
            }
            this.T = (t != null ? t : DenseMatrix.Create(1, base.DimOfTargetPoints, 0));

            this.YPY = null;
            this.X_hat = null;
            this.A = null;
        }

        public override void UpdateTransform()
        {
            Matrix<double> muX = (base.PX.Transpose() * DenseMatrix.Create(base.PX.Transpose().ColumnCount, 1, 1)) / base.SumOfProbabilities;
            Matrix<double> muY = (base.SourcePoints.Transpose() * base.P1) / base.SumOfProbabilities;
            this.X_hat = base.TargetPoints - DenseMatrix.Create(base.TargetPoints.RowCount, 1, 1) * muX.Transpose();
            Matrix<double> y_Hat = base.SourcePoints - DenseMatrix.Create(base.SourcePoints.RowCount, 1, 1) * muY.Transpose();
            this.A = (this.X_hat.Transpose() * base.ProbabilitiesMatrix.Transpose() * y_Hat);
            this.YPY = (y_Hat.Transpose() * Matrix<double>.Build.DiagonalOfDiagonalVector(base.P1.Column(0)) * y_Hat).Inverse();
            this.B = this.A * this.YPY;
            this.T = (muX - this.B * muY).Transpose();
        }
        public override void TransformPointCloud(Matrix<double> Y = null)
        {
            if(Y == null)
            {
                base.TransformSourcePoints = base.SourcePoints*this.B.Transpose() + (DenseMatrix.Create(base.SourcePoints.RowCount, 1, 1) * this.T);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public override void UpdateVariance()
        {
            double missAlignmentPre = base.MissAlignment;
            double trAB = (this.A * this.B.Transpose()).Trace();
            double xPx = (this.X_hat.Transpose() * Matrix<double>.Build.DiagonalOfDiagonalVector(base.Pt1.Column(0)) * this.X_hat).Trace();
            double trBYPYP = (this.B * this.YPY * this.B).Trace();
            base.MissAlignment = (xPx -2*trAB + trBYPYP)/(2*base.Sigma2) + (base.DimOfTargetPoints*base.SumOfProbabilities/2)* Math.Log(base.Sigma2);
            base.Diff = Math.Abs(base.MissAlignment - missAlignmentPre);

            base.Sigma2 = (xPx - trAB) / (base.SumOfProbabilities * base.DimOfTargetPoints);
            if (base.Sigma2 <= 0) base.Sigma2 = base.Tolerance / 10;
        }
        public override List<object> GetRegistrationParameters()
        {
            Console.WriteLine("Transformation :");
            Console.WriteLine(this.B);
            Console.WriteLine("Translation : ");
            Console.WriteLine(this.T);
            List<object> parameter = new List<object>();
            parameter.Add(this.B) ;
            parameter.Add(this.T);
            return parameter;
        }
    }
}
