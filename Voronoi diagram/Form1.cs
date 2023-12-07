using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Voronoi_diagram
{
	public enum Mode
	{
		INPUT,
		OUTPUT,
		MANUAL
	}

	public partial class Form1 : Form
	{
		private String choseFilePath = String.Empty;
		private Point origin;
		Graphics g;
		private SolidBrush pointBursh = new SolidBrush(Color.Black);
		private SolidBrush polygonBursh = new SolidBrush(Color.FromArgb(123, 142, 249, 243));
		private Pen linePen = new Pen(Color.Black, 0.5f);
		private Pen clearLinePen = new Pen(Color.White, 3f);
		private Pen hyperPlanePen = new Pen(Color.Blue, 0.5f);
		private Pen polygonPen = new Pen(Color.Green, 1f);
		private Pen[] convexHullPen = { new Pen(Color.Orange, 1f), new Pen(Color.Yellow, 1f) };
		private Pen[] voronoiPen = { new Pen(Color.Brown, 1f), new Pen(Color.Tomato, 1f) };
		int voronoiNum = 0;
		private Point[] allNodes;
		private Edge[] edges = new Edge[100];
		private int nodeNum = 0, edgeNum = 0;
		private Mode mode = Mode.INPUT;
		String[] contentRows;
		private int dataSetIndex = 0;
		StreamWriter sw;
		bool mergeOrNot = false;

		public Form1()
		{
			InitializeComponent();

			origin = new Point(0, 600);
			g = paintArea.CreateGraphics();
		}

		private void fileButton_Click(object sender, EventArgs e)
		{
			openFileDialog1.InitialDirectory = "c:\\";
			openFileDialog1.Title = "選擇要開啟的文字檔案";
			openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			openFileDialog1.RestoreDirectory = true;

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				choseFilePath = openFileDialog1.FileName;

				var fileStream = openFileDialog1.OpenFile();
				using (StreamReader reader = new StreamReader(fileStream))
				{
					textBox1.Text = reader.ReadToEnd();
					contentRows = textBox1.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
				}
				runButton.Enabled = true;
				dataSetIndex = 0;
			}

		}

		private void runButton_Click(object sender, EventArgs e)
		{
			//drawLineWithSlope(new Point(40, 400), -10, 1,false,false,true);
			g.Clear(Color.White);

			//read input
			contentRows = textBox1.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
			voronoiNum = 0;
			switch (mode)
			{
				case Mode.INPUT:
					edgeNum = 0;
					if (readInput() == false)
					{
						mousePosLabel.Text = "input Error!";
					}
					else
					{
						DaC(allNodes,false);
					}
					sw.Close();
					break;
				case Mode.MANUAL:
					sw = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\Output.txt");
					allNodes = subArray(allNodes, 0, nodeNum - 1);
					allNodes = writePointAndLexicalSort(allNodes.Length, allNodes);
					DaC(allNodes, false);
					sw.Close();
					break;
				case Mode.OUTPUT:
					drawFileOutput();
					break;
			}

		}

		private void paintArea_MouseMove(object sender, MouseEventArgs e)
		{
			Point p = coordinateConverter(new Point(e.X, e.Y));
			mousePosLabel.Text = p.X.ToString() + "," + p.Y.ToString();
		}

		private void clearButton_Click(object sender, EventArgs e)
		{
			reset();
		}

		private void stepButton_Click(object sender, EventArgs e)
		{
			g.Clear(Color.White);

			//read input
			contentRows = textBox1.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
			voronoiNum = 0;
			switch (mode)
			{
				case Mode.INPUT:
					if (mergeOrNot == false)
					{
						edgeNum = 0;
						if (readInput() == false)
						{
							mousePosLabel.Text = "input Error!";
						}
						else
						{
							DaC(allNodes, true);
						}
						sw.Close();
						mergeOrNot = true;
					}
					else 
					{
						g.Clear(Color.White);
						drawVoronoiByNodeAndEdge(allNodes, subArray(edges, 0, edgeNum - 1));
						mergeOrNot = false;
					}
					
					break;
			}

		}

		private void paintArea_MouseClick(object sender, MouseEventArgs e)
		{
			if (mode == Mode.MANUAL)
			{
				Point p = coordinateConverter(new Point(e.X, e.Y));
				drawPoint(coordinateConverter(p));
				allNodes[nodeNum] = p;
				nodeNum++;
			}
		}

		private void modeButton_Click(object sender, EventArgs e)
		{
			if (mode == Mode.INPUT)
			{
				mode = Mode.OUTPUT;
				modeButton.BackColor = Color.FromArgb(131, 119, 209);
				modeButton.Text = "output mode";
			}
			else if (mode == Mode.MANUAL)
			{
				mode = Mode.INPUT;
				modeButton.BackColor = Color.FromArgb(142, 249, 243);
				modeButton.Text = "input mode";
			}
			else if (mode == Mode.OUTPUT)
			{
				mode = Mode.MANUAL;
				modeButton.BackColor = Color.FromArgb(94, 252, 141);
				modeButton.Text = "manual mode";
				reset();
				allNodes = new Point[100];
			}
		}

		private void reset()
		{
			g.Clear(Color.White);
			dataSetIndex = 0;
			runButton.Enabled = true;
			nodeNum = 0;
			edgeNum = 0;
			
		}

		private void test()
		{

			Pen pen = new Pen(Color.Black, 1);
			//g.DrawLine(pen, 0, 600, 600, 0);left bottom to right top
			g.DrawLine(pen, coordinateConverter(new Point(0, 0)), coordinateConverter(new Point(100, 300)));
			//debugLabel.Text = "O:" + origin.X.ToString() + "," + origin.Y.ToString();
		}

		private Point getMidpoint(Point p1, Point p2)
		{
			return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
		}

		private void DaC(Point[] nodes,bool step)
		{
			Point[] Lnodes, Rnodes;

			if (nodes.Length > 3)
			{
				Lnodes = subArray(nodes, 0, nodes.Length / 2 - 1);
				Rnodes = subArray(nodes, nodes.Length / 2, nodes.Length - 1);
				DaC(Lnodes,false);
				DaC(Rnodes,false);
				merge(Lnodes, Rnodes,step);

			}
			else
			{
				drawVoronoiInThreePoint(nodes, voronoiPen[voronoiNum]);
				voronoiNum++;
			}

		}

		private void merge(Point[] LconvexHull, Point[] RconvexHull, bool step = false)
		{
			Point upperPointL = new Point(0, 0);
			Point upperPointR = new Point(0, 0);
			Point lowerPointL = new Point(0, 0);
			Point lowerPointR = new Point(0, 0);
			int LCHRightestindex = 0;
			int RCHLeftestindex = 0;
			int upperPointLIndex, lowerPointLIndex;
			int upperPointRIndex, lowerPointRIndex;
			int temp = 0, max = 0;
			int Rnext = 0, Lnext = 0;
			int hyperPlaneNum = 0;
			int counter = 0;
			List<Point> convexHull = new List<Point>();

			RconvexHull = counterClockwiseSortPoint(RconvexHull);
			LconvexHull = counterClockwiseSortPoint(LconvexHull);


			for (int i = 0; i < LconvexHull.Length; i++)
			{
				if (LconvexHull[i].X > max)
				{
					temp = i;
					max = LconvexHull[i].X;
				}
			}
			LCHRightestindex = temp;
			findAndDrawConvexHull(LconvexHull,convexHullPen[0]);
			findAndDrawConvexHull(RconvexHull, convexHullPen[1]);

			
			Lnext = LCHRightestindex + 1;
			Rnext = RCHLeftestindex - 1;

			// find upper
			while (true)
			{
				if (Lnext >= LconvexHull.Length)
				{
					Lnext = 0;
				}
				if (Rnext < 0)
				{
					Rnext = RconvexHull.Length - 1;
				}
				if (counterclockwise(LconvexHull[LCHRightestindex], RconvexHull[RCHLeftestindex], RconvexHull[Rnext]) == true)
				{
					RCHLeftestindex = Rnext;
					Rnext--;
				}
				else if (counterclockwise(RconvexHull[RCHLeftestindex], LconvexHull[LCHRightestindex], LconvexHull[Lnext]) == false)
				{
					LCHRightestindex = Lnext;
					Lnext++;
					counter++;
					if (counter == LconvexHull.Length)
					{
						upperPointL = LconvexHull[LCHRightestindex];
						upperPointR = RconvexHull[RCHLeftestindex];
						upperPointLIndex = LCHRightestindex;
						upperPointRIndex = RCHLeftestindex;
						break;
					}
				}
				else
				{
					upperPointL = LconvexHull[LCHRightestindex];
					upperPointR = RconvexHull[RCHLeftestindex];
					upperPointLIndex = LCHRightestindex;
					upperPointRIndex = RCHLeftestindex;
					break;
				}
			}

			RCHLeftestindex = 0;
			LCHRightestindex = temp;

			Lnext = LCHRightestindex - 1;
			Rnext = RCHLeftestindex + 1;
			counter = 0;
			while (true)
			{
				if (Rnext >= RconvexHull.Length)
				{
					Rnext = 0;
				}
				if (Lnext < 0)
				{
					Lnext = LconvexHull.Length - 1;
				}
				if (counterclockwise(LconvexHull[LCHRightestindex], RconvexHull[RCHLeftestindex], RconvexHull[Rnext]) == false)
				{
					RCHLeftestindex = Rnext;
					Rnext++;
					counter++;
					if (counter == RconvexHull.Length)
					{
						lowerPointL = LconvexHull[LCHRightestindex];
						lowerPointR = RconvexHull[RCHLeftestindex];
						lowerPointLIndex = LCHRightestindex;
						lowerPointRIndex = RCHLeftestindex;
						break;
					}
				}
				else if (counterclockwise(RconvexHull[RCHLeftestindex], LconvexHull[LCHRightestindex], LconvexHull[Lnext]) == true)
				{
					LCHRightestindex = Lnext;
					Lnext--;
				}
				else
				{
					lowerPointL = LconvexHull[LCHRightestindex];
					lowerPointR = RconvexHull[RCHLeftestindex];
					lowerPointLIndex = LCHRightestindex;
					lowerPointRIndex = RCHLeftestindex;
					break;
				}
			}

			//g.DrawLine(hyperPlanePen,coordinateConverter(upperPointL),coordinateConverter(upperPointR));

			//g.DrawLine(hyperPlanePen, coordinateConverter(lowerPointL), coordinateConverter(lowerPointR));

			temp = upperPointLIndex;
			while (true)
			{
				convexHull.Add(LconvexHull[temp]);
				if (temp == lowerPointLIndex)
				{
					break;
				}
				temp++;
				if (temp >= LconvexHull.Length)
				{
					temp = 0;
				}
			}

			temp = lowerPointRIndex;
			while (true)
			{
				convexHull.Add(RconvexHull[temp]);
				if (temp == upperPointRIndex)
				{
					break;
				}
				temp++;
				if (temp >= RconvexHull.Length)
				{
					temp = 0;
				}
			}

			g.FillPolygon(polygonBursh, coordinateConverter(convexHull.ToArray()));





			Edge tempHyperplane;
			Edge[] hyperplane = new Edge[50];
			
			Point? intersectionPoint;
			Point lastIntersectionPoint = new Point(0,0);
			tempHyperplane = getHyperPlane(getMidpoint(upperPointL, upperPointR), upperPointL, upperPointR, upperPointR.X - upperPointL.X, upperPointR.Y - upperPointL.Y, true);
			//tempEdge = drawHyperPlane(getMidpoint(lowerPointL, lowerPointR), lowerPointL, lowerPointR, lowerPointR.X - lowerPointL.X, lowerPointR.Y - lowerPointL.Y, false);
			int ylimit = 10000;
			int index = 0;
			int hyperplaneNum = 0;
			temp = 0;
			int lasttemp = -1;
			max = 0;
			Point lastVector = new Point(0, 0);
			Point currentVector = new Point(0, 0);
			Point leftNode = upperPointL;
			Point rightNode = upperPointR;
			bool findIntersection = false;
			
			
			bool over = false;

			while (true)
			{
				max = 0;
				index = 0;
				findIntersection = false;
				while (true)
				{
					intersectionPoint = intersection(tempHyperplane, edges[index]);
					if (intersectionPoint != null)
					{
						if (((Point)intersectionPoint).Y > max && index != lasttemp)
						{
							if (((Point)intersectionPoint).Y < ylimit)
							{
								max = ((Point)intersectionPoint).Y;
								temp = index;
								findIntersection = true;
							}
							else if(((Point)intersectionPoint).Y == ylimit)
							{
								max = ((Point)intersectionPoint).Y;
								temp = index;
								findIntersection = true;
							}
							
						}
					}
					index++;
					if (index >= edgeNum) break;
				}

				if (findIntersection == true)
				{
					hyperPlaneNum++;
					intersectionPoint = intersection(tempHyperplane, edges[temp]);
					//inside = true;
					//drawPoint(coordinateConverter((Point)intersectionPoint));
					if (tempHyperplane.points[0].Y > tempHyperplane.points[1].Y)
					{
						tempHyperplane.points[1] = (Point)intersectionPoint;
					}
					else if (tempHyperplane.points[0].Y < tempHyperplane.points[1].Y)
					{
						tempHyperplane.points[0] = (Point)intersectionPoint;
						tempHyperplane.swapPoint();
					}
					else 
					{
						if (tempHyperplane.points[0] == lastIntersectionPoint)
							tempHyperplane.points[1] = (Point)intersectionPoint;
						else
							tempHyperplane.points[0] = (Point)intersectionPoint;
					}

					ylimit = ((Point)intersectionPoint).Y;

					if (leftNode == edges[temp].n1)
					{
						leftNode = edges[temp].n2;
					}
					else if (leftNode == edges[temp].n2)
					{
						leftNode = edges[temp].n1;
					}
					else if (rightNode == edges[temp].n1)
					{
						rightNode = edges[temp].n2;
					}
					else if (rightNode == edges[temp].n2)
					{
						rightNode = edges[temp].n1;
					}

					if (leftNode == lowerPointL && rightNode == lowerPointR && tempHyperplane.points[0] == tempHyperplane.points[1])
					{
						tempHyperplane = getHyperPlane(tempHyperplane.points[0], lowerPointL, lowerPointR, lowerPointR.X - lowerPointL.X, lowerPointR.Y - lowerPointL.Y, false);
						over = true;
						findIntersection = false;
					}
					lastIntersectionPoint = (Point)intersectionPoint;
				}

				g.DrawLine(hyperPlanePen, coordinateConverter(tempHyperplane.points[0]), coordinateConverter(tempHyperplane.points[1]));
				if (tempHyperplane.points[1].Y > tempHyperplane.points[0].Y)
				{
					tempHyperplane.swapPoint();
				}

				currentVector.X = tempHyperplane.points[1].X - tempHyperplane.points[0].X;
				currentVector.Y = tempHyperplane.points[1].Y - tempHyperplane.points[0].Y;
				


				if (hyperPlaneNum > 1 && tempHyperplane.points[0]!= tempHyperplane.points[1] && over==false)
				{
					if (counterclockwise(lastVector, currentVector) == false)
					{
						if (counterclockwise(currentVector, new Point(edges[lasttemp].points[0].X - tempHyperplane.points[0].X, edges[lasttemp].points[0].Y - tempHyperplane.points[0].Y)) == false)
						{
							if (step == false)
							{
								g.DrawLine(clearLinePen, coordinateConverter(tempHyperplane.points[0]), coordinateConverter(edges[lasttemp].points[0]));
							}
							edges[lasttemp].points[0] = tempHyperplane.points[0];
						}
						else {
							if (step == false)
							{
								g.DrawLine(clearLinePen, coordinateConverter(tempHyperplane.points[0]), coordinateConverter(edges[lasttemp].points[1]));
							}
							edges[lasttemp].points[1] = tempHyperplane.points[0];
						}
					}
					else
					{
						if (counterclockwise(currentVector, new Point(edges[lasttemp].points[0].X - tempHyperplane.points[0].X, edges[lasttemp].points[0].Y - tempHyperplane.points[0].Y)) == true)
						{
							if (step == false)
							{
								g.DrawLine(clearLinePen, coordinateConverter(tempHyperplane.points[0]), coordinateConverter(edges[lasttemp].points[0]));
							}
							edges[lasttemp].points[0] = tempHyperplane.points[0];
						}
						else 
						{
							if (step == false)
							{
								g.DrawLine(clearLinePen, coordinateConverter(tempHyperplane.points[0]), coordinateConverter(edges[lasttemp].points[1]));
							}
							edges[lasttemp].points[1] = tempHyperplane.points[0];
						}
					}
				}
				lastVector.X = currentVector.X;
				lastVector.Y = currentVector.Y;

				hyperplane[hyperplaneNum] = tempHyperplane;
				hyperplaneNum++;

				if (findIntersection == false)
				{
					for (int i = 0; i < hyperplaneNum; i++)
					{
						edges[edgeNum] = hyperplane[i];
						edgeNum++;
					}
					break;
				}
				else
					tempHyperplane = getHyperPlane((Point)intersectionPoint, leftNode, rightNode, rightNode.X - leftNode.X, rightNode.Y - leftNode.Y, false);


				lasttemp = temp;
			}

			if (step == false)
			{
				g.Clear(Color.White);
				drawVoronoiByNodeAndEdge(allNodes, subArray(edges, 0, edgeNum - 1));
			}
				

			//g.DrawLine(hyperPlanePen, coordinateConverter(tempHyperplane.points[0]), coordinateConverter(tempHyperplane.points[1]));

			//g.DrawLine(linePen, coordinateConverter(leftNode), coordinateConverter(rightNode));



		}

		private Edge getHyperPlane(Point point, Point n1, Point n2, int xChange, int yChange, bool startOrNot)
		{
			Point leftPoint = new Point(0, 0);
			Point rightPoint = new Point(0, 0);
			int temp = xChange;
			xChange = yChange;
			yChange = -temp;

			if (xChange != 0 && yChange != 0)
			{
				double slope = (double)yChange / (double)xChange;
				if (point.X < 0)
				{
					if (xChange > 0)
					{
						leftPoint.X = point.X - 100 * xChange;
						leftPoint.Y = point.Y - 100 * yChange;
					}
					else
					{
						leftPoint.X = point.X + 100 * xChange;
						leftPoint.Y = point.Y + 100 * yChange;
					}
				}
				else if ((point.Y - (point.X * slope) > 600 || point.Y - (point.X * slope) < 0))
				{
					if (slope > 0)
					{
						leftPoint.X = point.X - (int)((double)point.Y / slope);
						leftPoint.Y = 0;
					}
					else if (slope < 0)
					{
						leftPoint.X = point.X - (int)((double)(point.Y - 600) / slope);
						leftPoint.Y = 600;
					}
				}
				else
				{
					leftPoint.X = 0;
					leftPoint.Y = (int)(point.Y - (point.X * slope));
				}

				if (point.X > 600)
				{
					if (xChange > 0)
					{
						rightPoint.X = point.X + 100 * xChange;
						rightPoint.Y = point.X + 100 * yChange;
					}
					else
					{
						rightPoint.X = point.X - 100 * xChange;
						rightPoint.Y = point.X - 100 * yChange;
					}
				}
				else if ((point.Y + ((600 - point.X) * slope) > 600 || point.Y + ((600 - point.X) * slope) < 0))
				{
					if (slope > 0)
					{

						rightPoint.X = point.X + (int)((double)(600 - point.Y) / slope);
						rightPoint.Y = 600;
					}
					else if (slope < 0)
					{
						rightPoint.X = point.X + (int)((double)-point.Y / slope);
						rightPoint.Y = 0;
					}
				}
				else
				{
					rightPoint.X = 600;
					rightPoint.Y = (int)(point.Y + ((600 - point.X) * slope));
				}

			}
			

			if (startOrNot == true)
			{
				if (xChange == 0)
				{
					return new Edge(new Point(point.X, 600), new Point(point.X, 0), n1, n2, false);
				}
				else if (yChange == 0)
				{
					return new Edge(new Point(0, point.Y), new Point(600, point.Y), n1, n2, false);
				}
				else if (leftPoint.Y >= rightPoint.Y)
				{
					//g.DrawLine(hyperPlanePen, coordinateConverter(leftPoint), coordinateConverter(point));
					return new Edge(leftPoint, rightPoint, n1, n2, false);
				}
				else
				{
					//g.DrawLine(hyperPlanePen, coordinateConverter(point), coordinateConverter(rightPoint));
					return new Edge(rightPoint, leftPoint, n1, n2, false);
				}

			}
			else
			{
				if (xChange == 0)
				{
					return new Edge(point, new Point(point.X, 0), n1, n2, false);
				}
				else if (yChange == 0)
				{
					return new Edge(point, new Point(600, point.Y), n1, n2, false);
				}
				else if (leftPoint.Y <= rightPoint.Y)
				{
					//g.DrawLine(hyperPlanePen, coordinateConverter(leftPoint), coordinateConverter(point));
					return new Edge(leftPoint, point, n1, n2, false);
				}
				else
				{
					//g.DrawLine(hyperPlanePen, coordinateConverter(point), coordinateConverter(rightPoint));
					return new Edge(point, rightPoint, n1, n2, false);
				}

			}
		}


		private void drawLineWithSlope(Pen linePen,Point n1, Point n2, Point point, int xChange, int yChange, bool drawMidLine = false, bool complete = true)
		{
			// point uses normal coordinate:right is postive for x, up is postive for y
			// drawMidLine = true : draw the midline, so change xChange and yChange for vector and slope used in draw line

			Point leftPoint = new Point(0, 0);
			Point rightPoint = new Point(0, 0);


			// convert vector to normal vector
			if (drawMidLine == true)
			{
				int temp = xChange;
				xChange = yChange;
				yChange = -temp;
			}


			// get leftest point and rightest point
			if (xChange != 0 && yChange != 0)
			{
				double slope = (double)yChange / (double)xChange;
				if (point.X < 0)
				{
					if (xChange > 0)
					{
						leftPoint.X = point.X - 10 * xChange;
						leftPoint.Y = point.Y - 10 * yChange;
					}
					else
					{
						leftPoint.X = point.X + 10 * xChange;
						leftPoint.Y = point.Y + 10 * yChange;
					}
				}
				else if ((point.Y - (point.X * slope) > 600 || point.Y - (point.X * slope) < 0))
				{
					if (slope > 0)
					{
						leftPoint.X = point.X - (int)((double)point.Y / slope);
						leftPoint.Y = 0;
					}
					else if (slope < 0)
					{
						leftPoint.X = point.X - (int)((double)(point.Y - 600) / slope);
						leftPoint.Y = 600;
					}
				}
				else
				{
					leftPoint.X = 0;
					leftPoint.Y = (int)(point.Y - (point.X * slope));
				}

				if (point.X > 600)
				{
					if (xChange > 0)
					{
						rightPoint.X = point.X + 10 * xChange;
						rightPoint.Y = point.X + 10 * yChange;
					}
					else
					{
						rightPoint.X = point.X - 10 * xChange;
						rightPoint.Y = point.X - 10 * yChange;
					}
				}
				else if ((point.Y + ((600 - point.X) * slope) > 600 || point.Y + ((600 - point.X) * slope) < 0))
				{
					if (slope > 0)
					{

						rightPoint.X = point.X + (int)((double)(600 - point.Y) / slope);
						rightPoint.Y = 600;
					}
					else if (slope < 0)
					{
						rightPoint.X = point.X + (int)((double)-point.Y / slope);
						rightPoint.Y = 0;
					}
				}
				else
				{
					rightPoint.X = 600;
					rightPoint.Y = (int)(point.Y + ((600 - point.X) * slope));
				}

			}
			else if (xChange == 0 && yChange == 0)
			{
				return;
			}


			// draw the complete line or just drqw the line from point to left or right according to the vector
			if (complete == true)
			{
				if (xChange == 0)
				{
					g.DrawLine(linePen, coordinateConverter(new Point(point.X, 0)), coordinateConverter(new Point(point.X, 600)));
					edges[edgeNum] = new Edge(new Point(point.X, 0), new Point(point.X, 600), n1, n2);
				}
				else if (yChange == 0)
				{
					g.DrawLine(linePen, coordinateConverter(new Point(0, point.Y)), coordinateConverter(new Point(600, point.Y)));
					edges[edgeNum] = new Edge(new Point(0, point.Y), new Point(600, point.Y), n1, n2);
				}
				else
				{
					g.DrawLine(linePen, coordinateConverter(leftPoint), coordinateConverter(rightPoint));
					edges[edgeNum] = new Edge(leftPoint, rightPoint, n1, n2);
				}
			}
			else
			{
				if (xChange == 0)
				{
					if (yChange > 0)
					{
						g.DrawLine(linePen, coordinateConverter(point), coordinateConverter(new Point(point.X, 600)));
						edges[edgeNum] = new Edge(point, new Point(point.X, 600), n1, n2);
					}
					else
					{
						g.DrawLine(linePen, coordinateConverter(new Point(point.X, 0)), coordinateConverter(point));
						edges[edgeNum] = new Edge(new Point(point.X, 0), point, n1, n2);
					}
				}
				else if (yChange == 0)
				{
					if (xChange > 0)
					{
						g.DrawLine(linePen, coordinateConverter(point), coordinateConverter(new Point(600, point.Y)));
						edges[edgeNum] = new Edge(point, new Point(600, point.Y), n1, n2);
					}
					else
					{
						g.DrawLine(linePen, coordinateConverter(new Point(0, point.Y)), coordinateConverter(point));
						edges[edgeNum] = new Edge(new Point(0, point.Y), point, n1, n2);
					}
				}
				else
				{
					if (xChange > 0)
					{
						g.DrawLine(linePen, coordinateConverter(point), coordinateConverter(rightPoint));
						edges[edgeNum] = new Edge(point, rightPoint, n1, n2);
					}
					else
					{
						g.DrawLine(linePen, coordinateConverter(leftPoint), coordinateConverter(point));
						edges[edgeNum] = new Edge(leftPoint, point, n1, n2);
					}
				}
			}

			edgeNum++;

			//g.DrawLine(linePen, CoordinateConverter(leftPoint), CoordinateConverter(rightPoint));

			//Point[] temp12 = { new Point(10, 100), new Point(25, 210), new Point(90, 300), new Point(50, 80) };
			//g.FillPolygon(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), temp12);
			// get rightest point


			//drawPoint(new Point(10, 100));
		}

		private bool readInput()
		{
			String[] elements;


			try
			{
				while (true)
				{
					if (contentRows[dataSetIndex][0] == '#')
						dataSetIndex++;
					else
					{
						nodeNum = Int32.Parse(contentRows[dataSetIndex]);
						break;
					}
				}
			}
			catch (IndexOutOfRangeException)
			{
				mousePosLabel.Text = "End";
				runButton.Enabled = false;
				//sw.Close();
				return false;
			}
			catch
			{
				if (contentRows[dataSetIndex][0] == '#')
					mousePosLabel.Text = "Error";
				//sw.Close();
				return false;
			}

			if (nodeNum == 0)
			{
				//sw.Close();
				return false;
			}


			allNodes = new Point[nodeNum];
			for (int i = 1; i <= nodeNum; i++)
			{
				elements = contentRows[dataSetIndex + i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
				allNodes[i - 1].X = Int32.Parse(elements[0]);
				allNodes[i - 1].Y = Int32.Parse(elements[1]);
			}
			dataSetIndex += nodeNum + 1;

			sw = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\Output.txt");
			allNodes = writePointAndLexicalSort(allNodes.Length, allNodes);


			return true;

		}


		private void drawVoronoiInThreePoint(Point[] nodes, Pen pen)
		{
			//10
			//283 200
			//535 240

			Point circumcentre = new Point(0, 0); // the center of all point
			Point midPoint = new Point(0, 0); // the center of the two  chosen point
			bool allOnSameLineFlag = false;




			for (int i = 0; i < nodes.Length; i++)
			{
				circumcentre.X += nodes[i].X;
				circumcentre.Y += nodes[i].Y;
				drawPoint(coordinateConverter(nodes[i]));
			}




			// 3 points on the same line
			if (nodes.Length == 3)
			{


				if (nodes[0].X == nodes[1].X && nodes[1].X == nodes[2].X)
				{
					allOnSameLineFlag = true;
					midPoint.X = nodes[0].X;
					for (int i = 0; i < 2; i++)
					{
						midPoint.Y = (nodes[i].Y + nodes[i + 1].Y) / 2;
						drawLineWithSlope(pen, nodes[i], nodes[i + 1], midPoint, 0, nodes[i].Y - nodes[i + 1].Y, true, true);
					}

				}
				else if (nodes[0].Y == nodes[1].Y && nodes[1].Y == nodes[2].Y)
				{
					allOnSameLineFlag = true;
					midPoint.Y = nodes[0].Y;
					for (int i = 0; i < 2; i++)
					{
						midPoint.X = (nodes[i].X + nodes[i + 1].X) / 2;
						drawLineWithSlope(pen, nodes[i], nodes[i + 1], midPoint, nodes[i].X - nodes[i + 1].X, 0, true, true);
					}
				}
				else if (((double)nodes[0].Y - (double)nodes[1].Y) / ((double)nodes[1].Y - (double)nodes[2].Y) == ((double)nodes[0].X - (double)nodes[1].X) / ((double)nodes[1].X - (double)nodes[2].X))
				{
					allOnSameLineFlag = true;
					for (int i = 0; i < 2; i++)
					{
						midPoint.X = (nodes[i].X + nodes[i + 1].X) / 2;
						midPoint.Y = (nodes[i].Y + nodes[i + 1].Y) / 2;
						drawLineWithSlope(pen, nodes[i], nodes[i + 1], midPoint, nodes[i].X - nodes[i + 1].X, nodes[i].Y - nodes[i + 1].Y, true, true);
					}
				}

			}


			if (nodes.Length == 2)
			{
				circumcentre.X /= 2;
				circumcentre.Y /= 2;
			}
			else if (allOnSameLineFlag == false)
			{
				int d1 = (nodes[1].X * nodes[1].X + nodes[1].Y * nodes[1].Y) - (nodes[0].X * nodes[0].X + nodes[0].Y * nodes[0].Y);
				int d2 = (nodes[2].X * nodes[2].X + nodes[2].Y * nodes[2].Y) - (nodes[1].X * nodes[1].X + nodes[1].Y * nodes[1].Y);
				int fm = 2 * ((nodes[2].Y - nodes[1].Y) * (nodes[1].X - nodes[0].X) - (nodes[1].Y - nodes[0].Y) * (nodes[2].X - nodes[1].X));
				circumcentre.X = ((nodes[2].Y - nodes[1].Y) * d1 - (nodes[1].Y - nodes[0].Y) * d2) / fm;
				circumcentre.Y = ((nodes[1].X - nodes[0].X) * d2 - (nodes[2].X - nodes[1].X) * d1) / fm;
				nodes = counterClockwiseSortPoint(nodes);
			}

			if (nodes.Length == 2)
			{
				drawLineWithSlope(pen, nodes[0], nodes[1], circumcentre, nodes[0].X - nodes[1].X, nodes[0].Y - nodes[1].Y, true);
			}
			else
			{
				for (int i = 0; i < nodes.Length && allOnSameLineFlag == false; i++)
				{
					if (i < nodes.Length - 1)
					{
						drawLineWithSlope(pen, nodes[i], nodes[i + 1], circumcentre, nodes[i + 1].X - nodes[i].X, nodes[i + 1].Y - nodes[i].Y, true, false);
					}
					else
					{
						drawLineWithSlope(pen, nodes[0], nodes[i], circumcentre, nodes[0].X - nodes[i].X, nodes[0].Y - nodes[i].Y, true, false);
					}
				}
			}

			writeEdge();


		}

		private void findAndDrawConvexHull(Point[] nodes, Pen pen )
		{
			if (nodes.Length == 2)
			{
				g.DrawLine(pen, coordinateConverter(nodes[0]), coordinateConverter(nodes[1]));
			}
			else
			{
				g.DrawPolygon(pen, coordinateConverter(nodes));
			}

		}

		private void drawVoronoiByNodeAndEdge(Point[] nodes,Edge[] edges)
		{
			foreach (Point node in nodes)
			{
				drawPoint(coordinateConverter(node));
			}
			foreach (Edge edge in edges)
			{
				g.DrawLine(linePen, coordinateConverter(edge.points[0]), coordinateConverter(edge.points[1]));
			}
		}

		private void drawFileOutput()
		{
			//P 283 200
			//E 0 34 193 161		
			String[] elements;

			for (int i = 0; i < contentRows.Length; i++)
			{
				elements = contentRows[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
				switch (elements[0])
				{

					case "E":
						g.DrawLine(linePen, coordinateConverter(new Point(Int32.Parse(elements[1]), Int32.Parse(elements[2]))), coordinateConverter(new Point(Int32.Parse(elements[3]), Int32.Parse(elements[4]))));
						break;
					case "P":
						drawPoint(coordinateConverter(new Point(Int32.Parse(elements[1]), Int32.Parse(elements[2]))));
						break;
				}
			}
		}


		private void drawPoint(Point p)
		{
			Rectangle rect = new Rectangle(p.X - 2, p.Y - 2, 5, 5);
			g.FillRectangle(pointBursh, rect);

		}

		private Point coordinateConverter(Point point)
		// covert between normal coordinate and paintArea coordinate
		{
			return new Point(point.X, 600 - point.Y);
		}

		private Point[] coordinateConverter(Point[] points)
		{
			Point[] result = new Point[points.Length];
			for (int i = 0; i < points.Length; i++)
				result[i] = coordinateConverter(points[i]);
			return  result;
		}


		private int cheakArea(Point midPoint, Point relatPoint, Point otherPoint)
		{
			if (pointDistanceSquare(midPoint, relatPoint) < pointDistanceSquare(midPoint, otherPoint))
				return 1;
			else if (pointDistanceSquare(midPoint, relatPoint) > pointDistanceSquare(midPoint, otherPoint))
				return 0;
			else
				return 2;
		}

		private int pointDistanceSquare(Point p1, Point p2)
		{
			return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
		}

		

		private Point[] counterClockwiseSortPoint(Point[] nodes)
		{
			Point temp = new Point(0, 0);
			Point[] vector = new Point[3];
			Point[] result = (Point[])nodes.Clone();

			if (nodes.Length < 3)
			{
				return result;
			}

			for (int i = 1; i < 3; i++)
			{
				vector[i-1].X = result[i].X - result[0].X;
				vector[i-1].Y = result[i].Y - result[0].Y;
			}
			if (counterclockwise(vector[0], vector[1])==false)
			{
				temp = result[1];
				result[1] = result[2];
				result[2] = temp;
			}
			return result;

		}

		private Point[]  writePointAndLexicalSort(int len, Point[] nodes)
		{
			Point temp = new Point(0, 0);
			for (int i = len - 1; i > 0; i--)
			{
				for (int j = 0; j < i; j++)
				{
					if (nodes[j].X > nodes[j + 1].X)
					{
						temp = nodes[j];
						nodes[j] = nodes[j + 1];
						nodes[j + 1] = temp;
					}
					else if (nodes[j].X == nodes[j + 1].X)
					{
						if (nodes[j].Y > nodes[j + 1].Y)
						{
							temp = nodes[j];
							nodes[j] = nodes[j + 1];
							nodes[j + 1] = temp;
						}
					}
				}
			}
			for (int i = 0; i < len; i++)
			{
				sw.WriteLine("P " + nodes[i].X.ToString() + " " + nodes[i].Y.ToString());
			}
			return nodes;
		}

		
		private void writeEdge()
		{
			Edge temp = new Edge(new Point(0, 0), new Point(0, 0), null, null); ;
			for (int i = edgeNum - 1; i > 0; i--)
			{
				for (int j = 0; j < i; j++)
				{
					if (edges[j].points[0].X > edges[j + 1].points[0].X)
					{
						temp = edges[j];
						edges[j] = edges[j + 1];
						edges[j + 1] = temp;
					}
					else if (edges[j].points[0].X == edges[j + 1].points[0].X)
					{
						if (edges[j].points[0].Y > edges[j + 1].points[0].Y)
						{
							temp = edges[j];
							edges[j] = edges[j + 1];
							edges[j + 1] = temp;
						}
						else if (edges[j].points[0].Y == edges[j + 1].points[0].Y)
						{
							if (edges[j].points[1].X > edges[j + 1].points[1].X)
							{
								temp = edges[j];
								edges[j] = edges[j + 1];
								edges[j + 1] = temp;
							}
							else if (edges[j].points[1].X == edges[j + 1].points[1].X)
							{
								if (edges[j].points[1].Y > edges[j + 1].points[1].Y)
								{
									temp = edges[j];
									edges[j] = edges[j + 1];
									edges[j + 1] = temp;
								}
							}
						}
					}
				}
			}

			for(int i=0; i< edgeNum; i++)
			{
				sw.WriteLine(edges[i].getString());
			}

			
		}

		private Point[] subArray(Point[] points,int startIndex,int endIndex)
		{
			int length = endIndex - startIndex + 1;
			Point[] result = new Point[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = points[startIndex + i];
			}
			return result;
		}

		private Edge[] subArray(Edge[] edges, int startIndex, int endIndex)
		{
			int length = endIndex - startIndex + 1;
			Edge[] result = new Edge[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = edges[startIndex + i];
			}
			return result;
		}

		private bool counterclockwise(Point v1, Point v2)
		{
			// v1 to v2 is counterclockwise or not
			if (v1.X * v2.Y - v1.Y * v2.X > 0)
				return true;
			else
				return false;
		}

		private bool counterclockwise(Point p1, Point p2,Point p3)
		{
			if (crossProduct(p1, p2,  p3) > 0)
				return true;
			else
				return false;
		}

		private int crossProduct(Point v1, Point v2)
		{
			return v1.X * v2.Y - v1.Y * v2.X;
		}

		private int crossProduct(Point p1, Point p2, Point p3)
		{
			Point v1 = new Point(p2.X - p1.X, p2.Y - p1.Y);
			Point v2 = new Point(p3.X - p1.X, p3.Y - p1.Y);
			return v1.X * v2.Y - v1.Y * v2.X;
		}

		Point? intersection(Point a1, Point a2, Point b1, Point b2,bool limit=true)
		{
			Point v1 = new Point(a2.X - a1.X, a2.Y - a1.Y);
			Point v2 = new Point(b2.X - b1.X, b2.Y - b1.Y);
			Point v3 = new Point(b1.X - a1.X, b1.Y - a1.Y);
			int AmaxX = a1.X;
			int AminX = a1.X;
			int BmaxX = b1.X;
			int BminX = b1.X;
			Point intersectionPoint;
			if (crossProduct(v1, v2) == 0)
				return null;
			intersectionPoint = new Point(a1.X + v1.X * crossProduct(v3, v2) / crossProduct(v1, v2), a1.Y + v1.Y * crossProduct(v3, v2) / crossProduct(v1, v2));



			if (limit == true)
			{
				if (a1.X > a2.X)
				{
					AminX = a2.X;
				}
				else 
				{ 
					AmaxX = a2.X;
				}
				if (b1.X > b2.X)
				{
					BminX = b2.X;
				}
				else 
				{
					BmaxX = b2.X;
				}

				if (AminX <= intersectionPoint.X && intersectionPoint.X <= AmaxX && BminX <= intersectionPoint.X && intersectionPoint.X <= BmaxX)
					return intersectionPoint;
				else return null;
			}
			else
			{
				return intersectionPoint;
			}
			
		}

		Point? intersection(Edge e1, Edge e2, bool limit = true)
		{
			return intersection(e1.points[0], e1.points[1], e2.points[0], e2.points[1], limit);
		}


	}

	class Edge
	{
		public Point[] points = new Point[2];
		public Point n1, n2;
		public Edge(Point start,Point end,Point? n1,Point? n2, bool lexicalFlag = true)
		{
			points[0] = start;
			points[1] = end;
			if(n1 != null)
				this.n1 =(Point) n1;
			if (n2 != null)
				this.n2 =(Point) n2;
			if (lexicalFlag)
				sort();
		}

		public void swapPoint()
		{
			Point temp;
			temp = points[0];
			points[0] = points[1];
			points[1] = temp;
		}

		private void sort()
		{
			Point temp;
			if (points[0].X > points[1].X)
			{
				temp = points[0];
				points[0] = points[1];
				points[1] = temp;
			}
			else if (points[0].X == points[1].X)
			{
				if (points[0].Y > points[1].Y)
				{
					temp = points[0];
					points[0] = points[1];
					points[1] = temp;
				}
			}
			if (n1.X > n2.X)
			{
				temp = n1;
				n1 = n2;
				n2 = temp;
			}
			else if (n1.X == n2.X)
			{
				if (n1.Y > n2.Y)
				{
					temp = n1;
					n1 = n2;
					n2 = temp;
				}
			}
		}

		public String getString()
		{
			return "E " + points[0].X.ToString() + " " + points[0].Y.ToString() + " " + points[1].X.ToString() + " " + points[1].Y.ToString();
		}

	}

	
}
