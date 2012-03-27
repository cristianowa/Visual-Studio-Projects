using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace Graphs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            gen_graph();
         //   exFunc();
        }

        Double[] vector = new Double[1];
        string[] labels = { "1", "2", "3", "4", "5" };
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            List<Double> list = new List<Double>();
           // if(checkBox1.Checked)
                list.Add(Convert.ToDouble(textBox1.Text));
           // if(checkBox2.Checked)
                list.Add(Convert.ToDouble(textBox2.Text));
           // if(checkBox3.Checked)
                list.Add(Convert.ToDouble(textBox3.Text));
          //  if(checkBox4.Checked)
                list.Add(Convert.ToDouble(textBox4.Text));
          //  if(checkBox5.Checked)
                list.Add(Convert.ToDouble(textBox5.Text));
            vector = new Double[list.Count];
            for(int i = 0;i<list.Count;i++)
                vector[i] = list[i];
        }
        ZedGraphControl graph;
        private void gen_graph()
        {
            graph = new ZedGraphControl();
            graph.Location = new Point(10, 10);
            graph.Height = 350;
            graph.Width = 350;
            this.Controls.Add(graph);
        }

        void updateGraph()
        {

            graph.GraphPane.AddPieSlices(vector, labels);
           
            graph.AxisChange();
            
        }


        void exFunc()
        {
            GraphPane myPane = graph.GraphPane;
  
            // Set the Titles
            myPane.Title.Text = "My Test Bar Graph";
            myPane.XAxis.Title.Text = "Label";
            myPane.YAxis.Title.Text = "My Y Axis";

            // Make up some random data points
            string[] labels = { "Panther", "Lion", "Cheetah", 
                      "Cougar", "Tiger", "Leopard" };
            double[] y = { 100, 115, 75, 22, 98, 40 };
            double[] y2 = { 90, 100, 95, 35, 80, 35 };
            double[] y3 = { 80, 110, 65, 15, 54, 67 };
            double[] y4 = { 120, 125, 100, 40, 105, 75 };

            // Generate a red bar with "Curve 1" in the legend
            BarItem myBar = myPane.AddBar("Curve 1", null, y,
                                                        Color.Red);
            myBar.Bar.Fill = new Fill(Color.Red, Color.White,
                                                        Color.Red);

            // Generate a blue bar with "Curve 2" in the legend
            myBar = myPane.AddBar("Curve 2", null, y2, Color.Blue);
            myBar.Bar.Fill = new Fill(Color.Blue, Color.White,
                                                        Color.Blue);

            // Generate a green bar with "Curve 3" in the legend
            myBar = myPane.AddBar("Curve 3", null, y3, Color.Green);
            myBar.Bar.Fill = new Fill(Color.Green, Color.White,
                                                        Color.Green);

            // Generate a black line with "Curve 4" in the legend
            LineItem myCurve = myPane.AddCurve("Curve 4",
                  null, y4, Color.Black, SymbolType.Circle);
            myCurve.Line.Fill = new Fill(Color.White,
                                  Color.LightSkyBlue, -45F);

            // Fix up the curve attributes a little
            myCurve.Symbol.Size = 8.0F;
            myCurve.Symbol.Fill = new Fill(Color.White);
            myCurve.Line.Width = 2.0F;

            // Draw the X tics between the labels instead of 
            // at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;

            // Set the XAxis labels
            myPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Fill the Axis and Pane backgrounds
            myPane.Chart.Fill = new Fill(Color.White,
                  Color.FromArgb(255, 255, 166), 90F);
            myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            graph.AxisChange();
        }
    }
}
