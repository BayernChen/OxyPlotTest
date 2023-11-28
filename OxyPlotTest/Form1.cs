using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Data.Text;
using CoherentPointDrift;
using System.Reflection.Emit;
using System.Security.Policy;
using System.IO;

namespace OxyPlotTest
{
    public partial class Form1 : Form
    {
        private Random rand = new Random(0);
        private double[] RandomWalk(int points = 5, double start = 100, double mult = 50)
        {
            // return an array of difting random numbers
            double[] values = new double[points];
            values[0] = start;
            for (int i = 1; i < points; i++)
                values[i] = values[i - 1] + (rand.NextDouble() - .5) * mult;
            return values;
        }
        private double[,] Outlier(int points = 5, double start = 100, double mult = 50)
        {
            // return an array of difting random numbers
            double[,] values = new double[points,2];
        
            for (int i = 0; i < points; i++)
            {
                values[i, 0] = start + rand.NextDouble() * mult;
                values[i, 1] = start + rand.NextDouble() * mult;
            }
            return values;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int pointCount = 1_000;
            double[] xs1 = RandomWalk(pointCount);
            double[] ys1 = RandomWalk(pointCount);
            double[] xs2 = RandomWalk(pointCount);
            double[] ys2 = RandomWalk(pointCount);

            // create lines and fill them with data points
            var line1 = new OxyPlot.Series.LineSeries()
            {
                Title = $"Series 1",
                Color = OxyPlot.OxyColors.Blue,
                StrokeThickness = 1,
                MarkerSize = 2,
                MarkerType = OxyPlot.MarkerType.Circle
            };

            var line2 = new OxyPlot.Series.LineSeries()
            {
                Title = $"Series 2",
                Color = OxyPlot.OxyColors.Red,
                StrokeThickness = 1,
                MarkerSize = 2,
                MarkerType = OxyPlot.MarkerType.Circle
            };

            for (int i = 0; i < pointCount; i++)
            {
                line1.Points.Add(new OxyPlot.DataPoint(xs1[i], ys1[i]));
                line2.Points.Add(new OxyPlot.DataPoint(xs2[i], ys2[i]));
            }

            // create the model and add the lines to it
            var model = new OxyPlot.PlotModel
            {
                Title = $"Scatter Plot ({pointCount:N0} points each)"
            };
            model.Series.Add(line1);
            model.Series.Add(line2);

            // load the model into the user control
            plotView1.Model = model;
           
            
        }

        private  void button1_Click(object sender, EventArgs e)
        {
            if (reg != null) reg.Register(visuialize);

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    if (ICP != null) ICP.Iteration(visuialize);
                    break;
                case 1:
                    if (reg != null) reg.Register(visuialize);
                    break;
                case 2:
                    if (reg != null) reg.Register(visuialize);
                    break;
                case 3:
                    if (reg != null) reg.Register(visuialize);
                    break;
                case 4:
                    if(reg != null)
                    {
                        List<object> RTS = reg.Register(visuialize);
                        Matrix<double> Z = (double)RTS[2]*Y * ((Matrix<double>)RTS[0]).Transpose() + DenseMatrix.Create(Y.RowCount, 1, 1) * (Matrix<double>)RTS[1];
                        ICP = new IterativeClosestPoints(X, Y);
                        ICP.CaculateRotationTranslate(Y.Transpose(), Z.Transpose());
                        Y = Y * (ICP.GetRotation.Transpose()) + DenseMatrix.Create(Y.RowCount, 1, 1) * ICP.GetTranslate;
                        ICP = new IterativeClosestPoints(X, Y);
                        ICP.Iteration(visuialize);
                        reg = null;
                        
                    }
                    break;
                case 5:
                    if(reg != null)
                    {
                        List<object> RTS = reg.Register(visuialize);
                        Matrix<double> Z = Y * ((Matrix<double>)RTS[0]).Transpose() + DenseMatrix.Create(Y.RowCount, 1, 1) * (Matrix<double>)RTS[1];
                        ICP = new IterativeClosestPoints(X, Y);
                        ICP.CaculateRotationTranslate(Y.Transpose(), Z.Transpose());
                        Y = Y * (ICP.GetRotation.Transpose()) + DenseMatrix.Create(Y.RowCount, 1, 1) * ICP.GetTranslate;
                        ICP = new IterativeClosestPoints(X , Y) ;
                        ICP.Iteration(visuialize);
                        reg = null;
                    }
                    break;

            }


        }

        private bool visuialize(Dictionary<string , object> iter)
        {
            label1.Text = "Iteration : " + iter["iteration"].ToString() + "\nError : " + iter["error"].ToString();
            label1.Refresh();
            scatterSeriesSource.Points.Clear();
            scatterSeriesTarget.Points.Clear();
            for (int i = 0; i < ((Matrix<double>)iter["X"]).RowCount; i++)
            {
                this.scatterSeriesTarget.Points.Add(new ScatterPoint(((Matrix<double>)iter["X"])[i, 0], ((Matrix<double>)iter["X"])[i, 1]));
            }
            for (int j = 0; j < ((Matrix<double>)iter["Y"]).RowCount; ++j)
            {
                this.scatterSeriesSource.Points.Add(new ScatterPoint(((Matrix<double>)iter["Y"])[j, 0], ((Matrix<double>)iter["Y"])[j, 1]));
            }
            plotView1.Refresh();
            System.Threading.Thread.Sleep(100);
            return true;
        }
        PlotModel model;
        ScatterSeries scatterSeriesTarget;
        ScatterSeries scatterSeriesSource;
      
        EMRegistration reg;
        IterativeClosestPoints ICP;
        Matrix<double> X, Y;
        private void button2_Click(object sender, EventArgs e)
        {
            
            //選擇點雲資料
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select Target Points set ";
            ofd.InitialDirectory = Directory.GetParent( Directory.GetCurrentDirectory()).Parent.FullName + @"\testData" ;
            ofd.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|All files (*.*)|*.*";
            ofd.ShowDialog();
            string url = ofd.FileName;
            if (url == "") url = ofd.InitialDirectory +@"\fish_target1.txt";
             X = DelimitedReader.Read<double>(url);
            double theta = Double.Parse(txtTheta.Text)*Math.PI/180;// 旋轉大小
            Matrix<double> R = DenseMatrix.OfArray(new double[2, 2]
            {
                {Math.Cos(theta) , -Math.Sin(theta) },
                {Math.Sin(theta) , Math.Cos(theta) }
            });
            Matrix<double> t = DenseMatrix.OfArray(new double[1,2 ]
            {
                { Double.Parse(txtDeltaX.Text), Double.Parse(txtDeltaY.Text)} //設定平移大小
            });

            ofd.Title = "Select Source Points set ";
            ofd.ShowDialog();
            url = ofd.FileName;
            if (url == "") url = ofd.InitialDirectory +@"\fish_target.txt";

             Y = DelimitedReader.Read<double>(url);
            
            Matrix<double> Z = DenseMatrix.OfArray(Outlier(int.Parse(txtNumOfOutlier.Text) , -2 , 4)); //異常點數量
            Y = Y * R + DenseMatrix.Create(Y.RowCount, 1, 1) * t;
            X = (X.Transpose().Append(Z.Transpose())).Transpose();
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    this.ICP = new IterativeClosestPoints(X, Y);
                    this.model = new PlotModel { Title = "ICP Algorithm " };
                    break;
                case 1:
                    this.reg = new RigidRegistration(X, Y);
                    this.model = new PlotModel { Title = "CPD(Rigid) Algorithm " };
                    break;
                case 2:
                    this.reg = new AffineRegistration(X, Y);
                    this.model = new PlotModel { Title = "CPD(Affine) Algorithm " };
                    break;
                case 3:
                    this.reg = new DeformableRegistration(X , Y );
                    this.model = new PlotModel { Title = "CPD(Deformable) Algorithm " };
                    break;
                case 4:
                    this.reg = new RigidRegistration(X, Y);
                    this.model = new PlotModel { Title = "CPD(Rigid)+ICP Algorithm " };
                    break;
                case 5:
                    this.reg = new  AffineRegistration(X, Y); ;
                    this.model = new PlotModel { Title = "CPD(Affine)+ICP Algorithm " };
                    break;
                default:
                    this.model = new PlotModel { Title = "XY Scatter" };
                    break;
            }
            

            
            this.scatterSeriesTarget = new ScatterSeries() { MarkerFill = OxyColors.Blue, MarkerType = MarkerType.Circle };
            this.scatterSeriesSource = new ScatterSeries() { MarkerFill = OxyColors.Red, MarkerType = MarkerType.Circle };
            for (int i = 0; i < X.RowCount; i++)
            {
                this.scatterSeriesTarget.Points.Add(new ScatterPoint(X[i,0], X[i,1]));
            }
            for( int j = 0;  j < Y.RowCount; ++j)
            {
                this.scatterSeriesSource.Points.Add(new ScatterPoint(Y[j, 0], Y[j, 1]));
            }
            
            model.Series.Add(this.scatterSeriesTarget);
            model.Series.Add(this.scatterSeriesSource);
            plotView1.Model = model;
            plotView1.Refresh();
        }

       
    }
}
