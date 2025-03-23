using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using System.Numerics;
using LasersNMirrors.Core;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.Linq;

namespace Presentation
{
    using LNMGrid = LasersNMirrors.Core.Grid;
    
    public partial class MainWindow : Window
    {
        private const UInt32 cell_size = 65; // Size of each cell in the grid
        private LNMGrid game_grid;
        private Dictionary<Ellipse, Laser> ellipse_laser_map;

        public MainWindow()
        {
            InitializeComponent();
            game_grid = GridFactory.Create("riddle.json"); //new LNMGrid(10, 10);// Example grid with width and height of 10
            ellipse_laser_map = new Dictionary<Ellipse, Laser>();
            Redraw();
        }

        internal void DrawCells()
        {
            UInt32 horizontal_side_length = game_grid.Width  * cell_size;
            UInt32 vertical_side_length   = game_grid.Height * cell_size;

            UInt32 horizontal_center = (UInt32)(ActualWidth  / 2);
            UInt32 vertical_center   = (UInt32)(ActualHeight / 2);

            Vector2 start_corner = new Vector2
                (horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);

            for(UInt32 x = 0; x < game_grid.Width; x++)
            {
                for (UInt32 y = 0; y < game_grid.Height; y++)
                {
                    var cell = game_grid.GetCell(new Vector2(x, y));

                    Rectangle rect = new Rectangle
                    {
                        Width = cell_size,
                        Height = cell_size,
                        Stroke = Brushes.White,
                        StrokeThickness = 1,
                        Fill = Brushes.Transparent
                    };
                    rect.MouseLeftButtonDown += RectMouseLeftButtonDown;
                    rect.MouseRightButtonDown += RectMouseRightButtonDown;

                    var left = start_corner.X + x * cell_size;
                    var top = start_corner.Y + y * cell_size;
                    var bottom = top + cell_size;
                    var right = left + cell_size;
                    Canvas.SetLeft(rect, left);
                    Canvas.SetTop(rect, top);

                    if (cell.Mirror != null)
                    {
                        if(cell.Mirror.Type == MirrorType.SLASH)
                        {
                            Line line = new Line
                            {
                                X1 = left,
                                Y1 = bottom,
                                X2 = right,
                                Y2 = top,
                                Stroke = Brushes.LightBlue,
                                StrokeThickness = 4,
                            };
                            GridCanvas.Children.Add(line);
                        }
                        else
                        {
                            Line line = new Line
                            {
                                X1 = left,
                                Y1 = top,
                                X2 = right,
                                Y2 = bottom,
                                Stroke = Brushes.LightBlue,
                                StrokeThickness = 4,
                            };
                            GridCanvas.Children.Add(line);
                        }
                    }

                    GridCanvas.Children.Add(rect);
                }
            }
        }

        internal void DrawDots()
        {
            UInt32 horizontal_side_length = game_grid.Width * cell_size;
            UInt32 vertical_side_length = game_grid.Height * cell_size;

            UInt32 horizontal_center = (UInt32)(ActualWidth / 2);
            UInt32 vertical_center = (UInt32)(ActualHeight / 2);

            Vector2 start_corner = new Vector2 (horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(-5.0f * cell_size / 8.0f, 3.0f * cell_size / 8.0f);

            for (UInt32 i = 0; i < game_grid.Height; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Stroke = Brushes.White,
                    Width = cell_size / 4,
                    Height = cell_size / 4,
                    Fill = game_grid.left_lasers[(int)i].Active ? Brushes.White : Brushes.Gray,
                };
                ellipse.MouseLeftButtonDown += DotMouseLeftButtonDown;
                Canvas.SetLeft(ellipse, start_corner.X);
                Canvas.SetTop(ellipse, start_corner.Y);

                GridCanvas.Children.Add(ellipse);
                start_corner += new Vector2(0.0f, cell_size);

                ellipse_laser_map.Add(ellipse, game_grid.left_lasers[(int)i]);
            }

            start_corner = new Vector2(horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(horizontal_side_length, 0.0f);
            start_corner += new Vector2(3.0f * cell_size / 8.0f, 3.0f * cell_size / 8.0f);

            for (UInt32 i = 0; i < game_grid.Height; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Stroke = Brushes.White,
                    Width = cell_size / 4,
                    Height = cell_size / 4,
                    Fill = game_grid.right_lasers[(int)i].Active ? Brushes.White : Brushes.Gray,
                };
                ellipse.MouseLeftButtonDown += DotMouseLeftButtonDown;
                Canvas.SetLeft(ellipse, start_corner.X);
                Canvas.SetTop(ellipse, start_corner.Y);
                
                GridCanvas.Children.Add(ellipse);
                start_corner += new Vector2(0.0f, cell_size);

                ellipse_laser_map.Add(ellipse, game_grid.right_lasers[(int)i]);
            }

            start_corner = new Vector2(horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(3.0f * cell_size / 8.0f, -5.0f * cell_size / 8.0f);

            for (UInt32 i = 0; i < game_grid.Width; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Stroke = Brushes.White,
                    Width = cell_size / 4,
                    Height = cell_size / 4,
                    Fill = game_grid.top_lasers[(int)i].Active ? Brushes.White : Brushes.Gray,
                };
                ellipse.MouseLeftButtonDown += DotMouseLeftButtonDown;
                Canvas.SetLeft(ellipse, start_corner.X);
                Canvas.SetTop(ellipse, start_corner.Y);

                GridCanvas.Children.Add(ellipse);
                start_corner += new Vector2(cell_size, 0.0f);

                ellipse_laser_map.Add(ellipse, game_grid.top_lasers[(int)i]);
            }

            start_corner = new Vector2(horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(0.0f, vertical_side_length);
            start_corner += new Vector2(3.0f * cell_size / 8.0f, 3.0f * cell_size / 8.0f);

            for (UInt32 i = 0; i < game_grid.Width; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Stroke = Brushes.White,
                    Width = cell_size / 4,
                    Height = cell_size / 4,
                    Fill = game_grid.bottom_lasers[(int)i].Active ? Brushes.White : Brushes.Gray,
                };
                ellipse.MouseLeftButtonDown += DotMouseLeftButtonDown;
                Canvas.SetLeft(ellipse, start_corner.X);
                Canvas.SetTop(ellipse, start_corner.Y);

                GridCanvas.Children.Add(ellipse);
                start_corner += new Vector2(cell_size, 0.0f);

                ellipse_laser_map.Add(ellipse, game_grid.bottom_lasers[(int)i]);
            }
        }

        internal void DrawNumbers()
        {
            UInt32 horizontal_side_length = game_grid.Width * cell_size;
            UInt32 vertical_side_length = game_grid.Height * cell_size;

            UInt32 horizontal_center = (UInt32)(ActualWidth / 2);
            UInt32 vertical_center = (UInt32)(ActualHeight / 2);

            Vector2 start_corner = new Vector2(horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(-16.0f * cell_size / 8.0f, 2.0f * cell_size / 8.0f);

            for (UInt32 i = 0; i < game_grid.Height; i++)
            {
                Brush brush = Brushes.Transparent;

                if (game_grid.left_numbers[(int)i].Number != 0)
                {
                    if (game_grid.left_lasers[(int)i].Solved)
                    {
                        brush = Brushes.GreenYellow;
                    }
                    else
                    {
                        brush = Brushes.Red;
                    }
                }

                TextBox text_box = new TextBox
                {
                    Width = cell_size,
                    Height = cell_size / 2,
                    Text = game_grid.left_numbers[(int)i].Number.ToString(),
                    Foreground = brush,
                    IsReadOnly = game_grid.left_numbers[(int)i].Constant,
                    FontSize = 20,
                    TextAlignment = TextAlignment.Center,
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    FontWeight = game_grid.left_numbers[(int)i].Constant ? FontWeights.Bold : FontWeights.Normal,
                    TextDecorations = game_grid.left_numbers[(int)i].Constant ? TextDecorations.Underline : null,

                };

                Canvas.SetLeft(text_box, start_corner.X);
                Canvas.SetTop(text_box, start_corner.Y);

                GridCanvas.Children.Add(text_box);
                start_corner += new Vector2(0.0f, cell_size);
            }

            start_corner = new Vector2(horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(horizontal_side_length, 0.0f);
            start_corner += new Vector2(8.0f * cell_size / 8.0f, 2.0f * cell_size / 8.0f);

            for (UInt32 i = 0; i < game_grid.Height; i++)
            {
                Brush brush = Brushes.Transparent;

                if (game_grid.right_numbers[(int)i].Number != 0)
                {
                    if (game_grid.right_lasers[(int)i].Solved)
                    {
                        brush = Brushes.GreenYellow;
                    }
                    else
                    {
                        brush = Brushes.Red;
                    }
                }

                TextBox text_box = new TextBox
                {
                    Width = cell_size,
                    Height = cell_size / 2,
                    Text = game_grid.right_numbers[(int)i].Number.ToString(),
                    Foreground = brush,
                    IsReadOnly = game_grid.right_numbers[(int)i].Constant,
                    FontSize = 20,
                    TextAlignment = TextAlignment.Center,
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    FontWeight =      game_grid.right_numbers[(int)i].Constant ? FontWeights.Bold : FontWeights.Normal,
                    TextDecorations = game_grid.right_numbers[(int)i].Constant ? TextDecorations.Underline : null,

                };

                Canvas.SetLeft(text_box, start_corner.X);
                Canvas.SetTop(text_box, start_corner.Y);

                GridCanvas.Children.Add(text_box);
                start_corner += new Vector2(0.0f, cell_size);
            }

            start_corner = new Vector2(horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(0.0f, -10.0f * cell_size / 8.0f);

            for (UInt32 i = 0; i < game_grid.Width; i++)
            {
                Brush brush = Brushes.Transparent;

                if (game_grid.top_numbers[(int)i].Number != 0)
                {
                    if (game_grid.top_lasers[(int)i].Solved)
                    {
                        brush = Brushes.GreenYellow;
                    }
                    else
                    {
                        brush = Brushes.Red;
                    }
                }

                TextBox text_box = new TextBox
                {
                    Width = cell_size,
                    Height = cell_size / 2,
                    Text = game_grid.top_numbers[(int)i].Number.ToString(),
                    Foreground = brush,
                    IsReadOnly = game_grid.top_numbers[(int)i].Constant,
                    FontSize = 20,
                    TextAlignment = TextAlignment.Center,
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    FontWeight = game_grid.top_numbers[(int)i].Constant ? FontWeights.Bold : FontWeights.Normal,
                    TextDecorations = game_grid.top_numbers[(int)i].Constant ? TextDecorations.Underline : null,
                };

                Canvas.SetLeft(text_box, start_corner.X);
                Canvas.SetTop(text_box, start_corner.Y);

                GridCanvas.Children.Add(text_box);
                start_corner += new Vector2(cell_size, 0.0f);
            }

            start_corner = new Vector2(horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(0.0f, vertical_side_length);
            start_corner += new Vector2(0.0f, 7.0f * cell_size / 8.0f);

            for (UInt32 i = 0; i < game_grid.Width; i++)
            {
                Brush brush = Brushes.Transparent;

                if (game_grid.bottom_numbers[(int)i].Number != 0)
                {
                    if (game_grid.bottom_lasers[(int)i].Solved)
                    {
                        brush = Brushes.GreenYellow;
                    }
                    else
                    {
                        brush = Brushes.Red;
                    }
                }

                TextBox text_box = new TextBox
                {
                    Width = cell_size,
                    Height = cell_size / 2,
                    Text = game_grid.bottom_numbers[(int)i].Number.ToString(),
                    Foreground = brush,
                    IsReadOnly = game_grid.bottom_numbers[(int)i].Constant,
                    FontSize = 20,
                    TextAlignment = TextAlignment.Center,
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    FontWeight = game_grid.bottom_numbers[(int)i].Constant ? FontWeights.Bold : FontWeights.Normal,
                    TextDecorations = game_grid.bottom_numbers[(int)i].Constant ? TextDecorations.Underline : null,
                };

                Canvas.SetLeft(text_box, start_corner.X);
                Canvas.SetTop(text_box, start_corner.Y);

                GridCanvas.Children.Add(text_box);
                start_corner += new Vector2(cell_size, 0.0f);
            }
        }

        internal void DrawLaser(Laser laser)
        {
            UInt32 horizontal_side_length = game_grid.Width * cell_size;
            UInt32 vertical_side_length = game_grid.Height * cell_size;

            UInt32 horizontal_center = (UInt32)(ActualWidth / 2);
            UInt32 vertical_center = (UInt32)(ActualHeight / 2);

            Vector2 start_corner = new Vector2
                (horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);
            start_corner += new Vector2(cell_size / 2, cell_size / 2);

            var path = laser.Path;
            var a = path.ElementAt(1);
            for (int i = 0; i < path.Count - 1; i++)
            {
                var first = path.ElementAt(i);
                var second = path.ElementAt(i + 1);

                var start = new Vector(start_corner.X + cell_size * first.X, start_corner.Y + cell_size * first.Y);
                var end = new Vector(start_corner.X + cell_size * second.X, start_corner.Y + cell_size * second.Y);

                Line line = new Line
                {
                    X1 = start.X,
                    Y1 = start.Y,
                    X2 = end.X,
                    Y2 = end.Y,
                    Stroke = laser.Solved ? Brushes.GreenYellow : Brushes.Red,
                    StrokeThickness = 3,
                };
                GridCanvas.Children.Add(line);
            }
        }
        
        internal void DrawLasers()
        {
            foreach (var laser in game_grid.left_lasers)
            {
                if (!laser.Active) continue;
                DrawLaser(laser);
            }

            foreach (var laser in game_grid.right_lasers)
            {
                if (!laser.Active) continue;
                DrawLaser(laser);
            }

            foreach (var laser in game_grid.top_lasers)
            {
                if (!laser.Active) continue;
                DrawLaser(laser);
            }

            foreach (var laser in game_grid.bottom_lasers)
            {
                if (!laser.Active) continue;
                DrawLaser(laser);
            }


        }

        private Tuple<UInt32, UInt32> GetXYFromRect(Rectangle rectangle)
        {
            UInt32 horizontal_side_length = game_grid.Width * cell_size;
            UInt32 vertical_side_length = game_grid.Height * cell_size;

            UInt32 horizontal_center = (UInt32)(ActualWidth / 2);
            UInt32 vertical_center = (UInt32)(ActualHeight / 2);

            Vector2 start_corner = new Vector2
                (horizontal_center - horizontal_side_length / 2, vertical_center - vertical_side_length / 2);

            var left = Canvas.GetLeft(rectangle);
            var top = Canvas.GetTop(rectangle);

            left -= start_corner.X;
            left /= cell_size;

            top -= start_corner.Y;
            top /= cell_size;

            return new Tuple<UInt32, UInt32>((UInt32)left, (UInt32)top);
        }

        public void DotMouseLeftButtonDown(object sender, EventArgs _)
        {
            Ellipse ellipse = (Ellipse)sender;
            Laser laser = null;
            ellipse_laser_map.TryGetValue(ellipse, out laser);
            laser.Active = !laser.Active;
            if (laser.Active)
            {
                //recalculate
            }
            Redraw();
        }

        public void RectMouseLeftButtonDown(object sender, EventArgs _)
        {
            Rectangle rectangle = (Rectangle)sender;
            var (x, y) = GetXYFromRect(rectangle);
            var mirror = game_grid.GetCell(x, y).Mirror;

            if (mirror != null && mirror.Type == MirrorType.SLASH)
            {
                game_grid.ClearMirror(x, y);
            }
            else
            {
                game_grid.AddMirror(x, y, MirrorType.SLASH);
            }
            Redraw();
        }

        public void RectMouseRightButtonDown(object sender, EventArgs _)
        {
            Rectangle rectangle = (Rectangle)sender;
            var (x, y) = GetXYFromRect(rectangle);
            var mirror = game_grid.GetCell(x, y).Mirror;

            if (mirror != null && mirror.Type == MirrorType.BACKSLASH)
            {
                game_grid.ClearMirror(x, y);
            }
            else
            {
                game_grid.AddMirror(x, y, MirrorType.BACKSLASH);
            }
            Redraw();
        }

        private void Redraw()
        {
            game_grid.CleanNumbers();
            game_grid.CalculateLasers();
            Console.WriteLine($"Puzzle solved: {game_grid.IsValid()}");
            ellipse_laser_map.Clear();
            GridCanvas.Children.Clear();
            DrawCells();
            DrawLasers();
            DrawDots();
            DrawNumbers();
        }
    
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Redraw();
        }
    }
}