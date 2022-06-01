using System;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using static System.Math;
using System.Collections.Generic;
using System.Drawing;

namespace freepglut
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_RGBA);
            Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-25, 25, -25, 25);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }

        private void Graph()
        {
            if (int.TryParse(N.Text, out int n))
            {
               
                double stepN = 2 * PI / n;
                List<double[]> listMid = new List<double[]>();

                for (int i = 0; i < n; i++)
                    listMid.Add(new double[] { 15 * Cos(i * stepN), 15 * Sin(i * stepN) });

                List<int[]> dots = new List<int[]>();
                var lines = richTextBox1.Text.Split('\n');
                foreach (var l in lines)
                {
                    string[] d = l.Split(' ');
                    dots.Add(new int[] { int.Parse(d[0]), int.Parse(d[1]), int.Parse(d[2])});
                }

                richTextBox2.Clear();

                List<int> way = Way(dots, int.Parse(startVer.Text), int.Parse(endVer.Text), n);
                if (way[0] < int.MaxValue / 10)
                {
                    richTextBox2.Text += "Min way = " + way[0] + "\n";
                    if (way.Count > 2)
                    {
                        for (int j = 1; j < way.Count - 1; j++)
                            richTextBox2.Text += way[j] + " -> ";
                        richTextBox2.Text += way[way.Count - 1];
                    }
                }
                else
                    richTextBox2.Text += "Min way = NoWay\n";

                bool check;
                for (int i = 0; i < listMid.Count; i++)
                {
                    check = true;
                    for (int j = 1; j < way.Count; j++)
                    {
                        if(i == way[j] - 1)
                        {
                            Gl.glColor3d(1, 0, 1);
                            GetCircle(listMid[i], 40);
                            GetNumMid(listMid[i], (i + 1).ToString());
                            check = false;
                            break;
                        }
                    }
                    if(check)
                    {
                        Gl.glColor3d(1, 0, 0);
                        GetCircle(listMid[i], 40);
                        Gl.glColor3d(0, 0, 1);
                        GetNumMid(listMid[i], (i + 1).ToString());
                    }
                }

                for (int i = 0; i < dots.Count; i++)
                {
                    check = true;
                    for (int j = 1; j < way.Count - 1; j++)
                    {
                        if (dots[i][0] == way[j] && dots[i][1] == way[j + 1])
                        {
                            Gl.glColor3d(1, 0, 1);
                            GetLine(listMid, dots[i][0], dots[i][1], dots[i][2]);
                            check = false;
                            break;
                        }
                    }
                    if (check)
                    {
                        Gl.glColor3d(1, 0, 0);
                        GetLine(listMid, dots[i][0], dots[i][1], dots[i][2]);
                    }
                }
            }
        }

        private void GetLine(List<double[]> listMid, int k1, int k2, int w)
        {
            Gl.glPushMatrix();
            if (k1 > listMid.Count || k2 > listMid.Count)
                return;

            double[] ver1 = listMid[k1 - 1];
            double[] ver2 = listMid[k2 - 1];

            double norm = Sqrt(Pow(ver1[0] - ver2[0], 2) + Pow(ver1[1] - ver2[1], 2));
            double[] a = { ver1[0] + 2 * (ver2[0] - ver1[0]) / norm, ver1[1] + 2 * (ver2[1] - ver1[1]) / norm };
            double[] b = { ver2[0] - 2 * (ver2[0] - ver1[0]) / norm, ver2[1] - 2 * (ver2[1] - ver1[1]) / norm };

            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(a[0], a[1]);
            Gl.glVertex2d(b[0], b[1]);
            Gl.glEnd();

            norm = Sqrt(Pow(b[0] - a[0], 2) + Pow(b[1] - a[1], 2));

            //cтрелки
            Gl.glPushMatrix();
            Gl.glTranslated(b[0], b[1], 0);
            Gl.glRotated(Tan((b[0] - a[0]) / (b[1] - b[0])) * PI / 180 - 160, 0, 0, 1);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(0, 0);
            Gl.glVertex2d(1.5 * (b[0] - a[0]) / norm, 1.5 * (b[1] - a[1]) / norm);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(b[0], b[1], 0);
            Gl.glRotated(Tan((b[0] - a[0]) / (b[1] - b[0])) * PI / 180 - 200, 0, 0, 1);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2d(0, 0);
            Gl.glVertex2d(1.5 * (b[0] - a[0]) / norm, 1.5 * (b[1] - a[1]) / norm);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glColor3d(0, 0, 1);
            GetStr(a, b, w.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Graph();
            AnT.Invalidate();
        }

        private void GetCircle(double[] a, int m)
        {
            double stepM = 2 * PI / m;

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int j = 0; j <= m; j++)
                Gl.glVertex2d(a[0] + 2 * Cos(j * stepM), a[1] + 2 * Sin(j * stepM));
            Gl.glEnd();
        }
        private void GetStr(double[] a, double[] b, string str)
        {
            double[] pos = new double[] { (b[0] + a[0]) / 2, (b[1] + a[1]) / 2 };
            Gl.glPushMatrix();//
            Gl.glTranslated(-0.7, -0.5, 0);
            Gl.glRasterPos2d(pos[0], pos[1]);
            Glut.glutBitmapString(Glut.GLUT_BITMAP_TIMES_ROMAN_24, str);
            Gl.glPopMatrix();//
        }

        private void GetNumMid(double[] pos, string str)
        {
            Gl.glPushMatrix();//
            Gl.glTranslated(-0.5, -0.5, 0);
            Gl.glRasterPos2d(pos[0], pos[1]);
            Glut.glutBitmapString(Glut.GLUT_BITMAP_TIMES_ROMAN_24, str);
            Gl.glPopMatrix();//
        }

        private List<int> Way(List<int[]> dots, int startVer, int endVer, int n)
        {
            int[] d = new int[n];
            for (int i = 0; i < n; i++)
                d[i] = int.MaxValue / 2;

            d[startVer - 1] = 0;

            List<List<int>> way = new List<List<int>>(n);
            for (int j = 0; j < n; j++)
            {
                way.Add(new List<int>());
                way[j].Add(startVer);
            }
            for (int j = 0; j < n - 1; j++)
            {
                for (int k = 0; k < dots.Count; k++)
                    if (d[dots[k][1] - 1] > d[dots[k][0] - 1] + dots[k][2])
                    {
                        d[dots[k][1] - 1] = d[dots[k][0] - 1] + dots[k][2];
                        way[dots[k][1] - 1] = new List<int>(way[dots[k][0] - 1]);
                        way[dots[k][1] - 1].Add(dots[k][1]);
                    }
            }

            List<int> res = new List<int>();
            res.Add(d[endVer - 1]);
            for (int i = 0; i < way[endVer - 1].Count; i++)
                res.Add(way[endVer - 1][i]);

            return res;
        }
    }
}
