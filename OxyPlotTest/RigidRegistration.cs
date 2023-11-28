using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.ComponentModel.Design;

namespace CoherentPointDrift
{
    public class RigidRegistration :EMRegistration
    {
        private Matrix<double> Rotation;
        private Matrix<double> Translate;
        private Matrix<double> A;
        private Matrix<double> x_Hat;

        private double Scale;
        private double YPY;
        private bool S;
        public RigidRegistration(Matrix<double> Target , Matrix<double> Source , Matrix<double> rotation = null , Matrix<double> translate = null , double scale = 0 , bool s = true ):base(Target , Source)
        {

            if(rotation != null && (base.DimOfTargetPoints == 2 || base.DimOfTargetPoints == 3) && rotation.Evd().EigenValues.Real().Minimum() > 0 && rotation.Evd().EigenValues.Imaginary().Maximum() == 0 && rotation.Evd().EigenValues.Imaginary().Minimum()==0)
            {
                this.Rotation = rotation;
            }
            else
            {
                this.Rotation = DenseMatrix.CreateIdentity(base.DimOfTargetPoints);
            }
            this.Translate = (translate != null ? translate : DenseMatrix.Create(1, base.DimOfTargetPoints, 0));
            this.Scale = ( scale <= 0 ? 1 : scale);
            this.S = s;
        }
        public override void UpdateTransform()
        {
            double ScalePre = this.Scale;
            Matrix<double> TranslatePre = this.Translate;
            Matrix<double> RotationPre = this.Rotation;


            Matrix<double> muX = (base.PX.Transpose() * DenseMatrix.Create(base.PX.Transpose().ColumnCount, 1, 1))/base.SumOfProbabilities;
            Matrix<double> muY = (base.SourcePoints.Transpose() * base.P1) / base.SumOfProbabilities;
            this.x_Hat = base.TargetPoints - DenseMatrix.Create(base.TargetPoints.RowCount, 1, 1) * muX.Transpose();
            Matrix<double> y_Hat = base.SourcePoints - DenseMatrix.Create(base.SourcePoints.RowCount, 1, 1) * muY.Transpose();
            this.YPY = (y_Hat.Transpose() * Matrix<double>.Build.DiagonalOfDiagonalVector(base.P1.Column(0)) * y_Hat).Trace();
            this.A = this.x_Hat.Transpose() * base.ProbabilitiesMatrix.Transpose() * y_Hat;
            Svd<double> singular = A.Svd();
            Matrix<double> C = DenseMatrix.CreateIdentity(base.DimOfTargetPoints);
            C[C.RowCount - 1, C.ColumnCount - 1] = (singular.U * singular.VT).Determinant();
            this.Rotation = singular.U * C * singular.VT;
            if (S) this.Scale = (this.A.Transpose() * this.Rotation).Trace() / this.YPY;
            this.Translate = (muX - this.Rotation * muY * this.Scale).Transpose();

            base.Diff = Math.Abs(this.Scale - ScalePre);
            TranslatePre = TranslatePre - this.Translate;
            RotationPre = RotationPre - this.Rotation;
            for( int i = 0; i < TranslatePre.RowCount; ++i)
            {
                base.Diff += Math.Abs(TranslatePre[i, 0]);
            }
            for( int i = 0; i < RotationPre.RowCount; ++i)
            {
                for (int j = 0; j < TranslatePre.ColumnCount; ++j) base.Diff += Math.Abs(RotationPre[i, j]);
            }
        }
        public override void TransformPointCloud(Matrix<double> Y = null )
        {
            if ( Y == null)
            {
                base.TransformSourcePoints = this.Scale * base.SourcePoints * this.Rotation.Transpose() + (DenseMatrix.Create(base.SourcePoints.RowCount, 1, 1) * this.Translate);
            }
            else
            {
                throw new NotImplementedException();
            }
            
        }
        public override void UpdateVariance()
        {
            double missAlignmentPre = base.MissAlignment;
            double trAR = (this.A.Transpose() * this.Rotation).Trace();
            base.MissAlignment = ((this.x_Hat.Transpose() * Matrix<double>.Build.DiagonalOfDiagonalVector(base.Pt1.Column(0)) * this.x_Hat).Trace() - 2 * this.Scale * trAR + Math.Pow(this.Scale, 2) * this.YPY) / (2 * base.Sigma2) + (base.DimOfTargetPoints * base.SumOfProbabilities / 2) * Math.Log(base.Sigma2);
            base.Diff = Math.Abs(base.MissAlignment - missAlignmentPre);

            base.Sigma2 = ((this.x_Hat.Transpose() * Matrix<double>.Build.DiagonalOfDiagonalVector(base.Pt1.Column(0)) * this.x_Hat).Trace() - this.Scale * trAR) / (base.SumOfProbabilities * base.DimOfTargetPoints) ;
            if (base.Sigma2 <= 0) base.Sigma2 = base.Tolerance / 10;
            
        }
        public override List<object> GetRegistrationParameters()
        {

            Console.WriteLine("Rotation :");
            Console.WriteLine(this.Rotation);
            Console.WriteLine("Translate : ");
            Console.WriteLine(this.Translate);
            Console.WriteLine("Scale : ");
            Console.WriteLine(this.Scale);
            List<object> parameters = new List<object>();
            parameters.Add(this.Rotation);
            parameters.Add(this.Translate);
            parameters.Add(this.Scale);
            return parameters;
            //throw new NotImplementedException();
        }
    }
}
