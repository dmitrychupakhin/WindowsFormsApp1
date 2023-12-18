using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            draw_picture_box_image();
        }

        private bool isAddNewNodeClick = false;
        private bool isAddNewArcClick = false;
        private bool isAddNewArcActive = false;
        private bool isAddNewEdgeClick = false;
        private bool isAddNewEdgeActive = false;
        private bool isRemoveButtonClick = false;

        private Button activeButton = null;

        private List<Arc> arcs = new List<Arc>();
        private List<Edge> edges = new List<Edge>();
        private List<Node> nodes = new List<Node>();

        Node firstNode = null;
        Node secondNode = null;

        private Node currentNode;

        class Arc
        {
            public Node Node1;
            public Node Node2;

            public void Draw(Graphics graphics)
            {
                if (Node1 == Node2)
                {
                    // Рисуйте петлю вокруг узла
                    int x = Node1.Center.X - Node1.Radius;
                    int y = Node1.Center.Y - Node1.Radius;
                    int width = 2 * 40;
                    int height = 2 * 40;

                    graphics.DrawEllipse(Pens.Black, x, y, width, height);
                    DrawArrow(graphics, Pens.Black, Node1.Center, Node2.Center);
                }
                else
                {
                    // Рисуйте линию между двумя узлами
                    graphics.DrawLine(Pens.Black, Node1.Center, Node2.Center);

                    DrawArrow(graphics, Pens.Black, Node1.Center, Node2.Center);
                }

            }
            private void DrawArrow(Graphics g, Pen pen, Point p1, Point p2)
            {
                // Определите координаты для стрелки
                float angle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
                float arrowLength = 40; // Длина стрелки

                PointF arrowPoint1 = new PointF(p2.X - arrowLength * (float)Math.Cos(angle - Math.PI / 6),
                                                p2.Y - arrowLength * (float)Math.Sin(angle - Math.PI / 6));
                PointF arrowPoint2 = new PointF(p2.X - arrowLength * (float)Math.Cos(angle + Math.PI / 6),
                                                p2.Y - arrowLength * (float)Math.Sin(angle + Math.PI / 6));

                // Рисуйте линию и стрелку
                g.DrawLine(pen, p1, p2);
                g.FillPolygon(new SolidBrush(pen.Color), new PointF[] { p2, arrowPoint1, arrowPoint2 });
            }
        }
        

        class Edge
        {
            public Node Node1;
            public Node Node2;

            public void Draw(Graphics graphics)
            {
                if (Node1 == Node2)
                {
                    // Рисуйте петлю вокруг узла
                    int x = Node1.Center.X - Node1.Radius;
                    int y = Node1.Center.Y - Node1.Radius;
                    int width = 2 * 40;
                    int height = 2 * 40;

                    graphics.DrawEllipse(Pens.Black, x, y, width, height);
                }
                else
                {
                    // Рисуйте линию между двумя узлами
                    graphics.DrawLine(Pens.Black, Node1.Center, Node2.Center);
                }

            }
        }

        class Node
        {
            public int Number = 0;
            public Point Center { get; set; }
            public int Radius { get; set; }

            public Node(Point center, int radius, int number)
            {
                Center = center;
                Radius = radius;
                Number = number;
            }
            public bool IsClickInRadius(Point clickPoint)
            {
                // Вычислите расстояние между точкой клика и центром
                double distance = Math.Sqrt(Math.Pow(clickPoint.X - Center.X, 2) + Math.Pow(clickPoint.Y - Center.Y, 2));

                // Проверьте, находится ли точка в радиусе
                return distance <= Radius;
            }
            public void Draw(Graphics graphics)
            {
                Pen pen = new Pen(Color.Black);
                Font font = new Font("Arial", 12);
                Brush brush = Brushes.Black;

                int diameter = Radius * 2;
                int x = Center.X - Radius;
                int y = Center.Y - Radius;

                int ellipseX = Center.X - Radius;
                int ellipseY = Center.Y - Radius;
                int ellipseWidth = 2 * Radius;
                int ellipseHeight = 2 * Radius;

                graphics.FillEllipse(Brushes.White, ellipseX, ellipseY, ellipseWidth, ellipseHeight);

                // Рисуем круг
                graphics.DrawEllipse(pen, x, y, diameter, diameter);

                // Создаем прямоугольник, ограничивающий круг
                RectangleF rectangle = new RectangleF(x, y, diameter, diameter);

                // Создаем объект форматирования строки
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                // Рисуем текст внутри круга
                graphics.DrawString(Number.ToString(), font, brush, rectangle, stringFormat);
            }
        }

        private void draw_picture_box_image()
        {
            Bitmap emptyBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            // Используйте Graphics для рисования пустого изображения
            using (Graphics g = Graphics.FromImage(emptyBitmap))
            {
                g.Clear(Color.White); // Установите цвет фона, если необходимо
                Pen gridPen = new Pen(Color.Gray);

                // Задайте размер ячейки сетки (например, 20x20 пикселей)
                int cellSize = 20;

                // Рисуйте горизонтальные линии сетки
                for (int y = 0; y < pictureBox1.Height; y += cellSize)
                {
                    g.DrawLine(gridPen, 0, y, pictureBox1.Width, y);
                }

                // Рисуйте вертикальные линии сетки
                for (int x = 0; x < pictureBox1.Width; x += cellSize)
                {
                    g.DrawLine(gridPen, x, 0, x, pictureBox1.Height);
                }
                foreach (var edge in edges)
                {
                    edge.Draw(g);
                }
                foreach (var arc in arcs)
                {
                    arc.Draw(g);
                }
                // Рисуйте узлы поверх сетки
                foreach (var node in nodes)
                {
                    node.Draw(g);
                }
            }

            label2.Text = "Узлов: " + nodes.Count;
            label4.Text = "Ребра: " + edges.Count;
            label5.Text = "Дуги: " + arcs.Count;
            // Установите очищенное изображение в PictureBox
            pictureBox1.Image = emptyBitmap;

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {

            if (isAddNewNodeClick)
            {
                MouseEventArgs mouseArgs = e as MouseEventArgs;
                Point clickPoint = mouseArgs.Location;

                List<Node> sortedList = nodes.OrderBy(node => node.Number).ToList();

                int current_number = 0;
                foreach (var node in sortedList)
                {
                    if (node.Number != current_number)
                    {
                        break;
                    }
                    else
                    {
                        current_number += 1;
                    }
                }

                // Создаем объект узла с координатами клика и произвольным радиусом
                Node new_node = new Node(clickPoint, 20, current_number);

                nodes.Add(new_node);

                draw_picture_box_image();
            }
            else if (isRemoveButtonClick)
            {
                MouseEventArgs mouseArgs = e as MouseEventArgs;
                Point clickPoint = mouseArgs.Location;

                Node nodeToRemove = nodes.FirstOrDefault(node => node.IsClickInRadius(clickPoint));

                if (nodeToRemove != null)
                {
                    // Если найден узел в радиусе клика, удалите его из коллекции
                    nodes.Remove(nodeToRemove);
                }

                List<Edge> edgesToRemove = edges.Where(edge => edge.Node1 == nodeToRemove || edge.Node2 == nodeToRemove).ToList();
                foreach (Edge edge in edgesToRemove)
                {
                    edges.Remove(edge);
                }

                List<Arc> arcsToRemove = arcs.Where(arc => arc.Node1 == nodeToRemove || arc.Node2 == nodeToRemove).ToList();
                foreach (Arc arc in arcsToRemove)
                {
                     arcs.Remove(arc);
                }

                draw_picture_box_image();
            }
            else if (isAddNewEdgeClick)
            {
                MouseEventArgs mouseArgs = e as MouseEventArgs;
                Point clickPoint = mouseArgs.Location;
                if (isAddNewEdgeActive == false)
                {
                    firstNode = nodes.FirstOrDefault(node => node.IsClickInRadius(clickPoint));
                    if (firstNode != null)
                    {
                        isAddNewEdgeActive = true;
                    }
                }
                else
                {
                    secondNode = nodes.FirstOrDefault(node => node.IsClickInRadius(clickPoint));
                    if (secondNode != null)
                    {
                        Edge edge = new Edge();
                        edge.Node1 = firstNode;
                        edge.Node2 = secondNode;
                        edges.Add(edge);
                        draw_picture_box_image();
                        isAddNewEdgeActive = false;
                    }
                }
            }
            else if (isAddNewArcClick)
            {
                MouseEventArgs mouseArgs = e as MouseEventArgs;
                Point clickPoint = mouseArgs.Location;
                if (isAddNewArcActive == false)
                {
                    firstNode = nodes.FirstOrDefault(node => node.IsClickInRadius(clickPoint));
                    if (firstNode != null)
                    {
                        isAddNewArcActive = true;
                    }
                }
                else
                {
                    secondNode = nodes.FirstOrDefault(node => node.IsClickInRadius(clickPoint));
                    if (secondNode != null)
                    {
                        Arc arc = new Arc();
                        arc.Node1 = firstNode;
                        arc.Node2 = secondNode;
                        arcs.Add(arc);
                        draw_picture_box_image();
                        isAddNewArcActive = false;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (activeButton != null && (Button)sender != activeButton)
            {
                activeButton.PerformClick();
            }

            isAddNewNodeClick = !isAddNewNodeClick;
            button1.BackColor = isAddNewNodeClick ? Color.Coral : SystemColors.Control;
            activeButton = button1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (activeButton != null && (Button)sender != activeButton)
            {
                activeButton.PerformClick();
            }
            isRemoveButtonClick = !isRemoveButtonClick;
            button4.BackColor = isRemoveButtonClick ? Color.Coral : SystemColors.Control;
            activeButton = button4;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (activeButton != null && (Button)sender != activeButton)
            {
                activeButton.PerformClick();
            }
            isAddNewEdgeClick = !isAddNewEdgeClick;
            button2.BackColor = isAddNewEdgeClick ? Color.Coral : SystemColors.Control;
            activeButton = button2;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (activeButton != null && (Button)sender != activeButton)
            {
                activeButton.PerformClick();
            }
            isAddNewArcClick = !isAddNewArcClick;
            button3.BackColor = isAddNewArcClick ? Color.Coral : SystemColors.Control;

            activeButton = button3;
        }
    }
}
