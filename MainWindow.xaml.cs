using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Forms = System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using ControlzEx.Standard;
using System.Configuration;

namespace NDI_Telestrator
{
    public partial class MainWindow : MetroWindow
    {
        Double test = 1;
        private object client;

        private void requestNDI(object caller, object args)
        {
            ndi.requestFrameUpdate();
        }
        public static Color FromName(String name)
        {
            var color_props = typeof(Colors).GetProperties();
            foreach (var c in color_props)
                if (name.Equals(c.Name, StringComparison.OrdinalIgnoreCase))
                    return (Color)c.GetValue(new Color(), null);
            return Colors.Transparent;
        }
        static async Task GetVmix(String url)
        {

            var client = new HttpClient();

            var result = await client.GetAsync(url);
            Console.WriteLine(result);
        }
        public MainWindow()
        {
            InitializeComponent();
            InkControls.whiteboard = theWhiteboard;
            optionsDialogue.whiteboard = theWhiteboard;
            optionsDialogue.background = theBackground;

            theWhiteboard.SetPenColour(FromName(ConfigurationManager.AppSettings["Default_Pen_Color"]));
            theWhiteboard.SetPenThickness(double.Parse(ConfigurationManager.AppSettings["Default_Pen_Size"]));


            // Send background updates every 250ms
            System.Windows.Threading.DispatcherTimer backgroundUpdateTimer = new System.Windows.Threading.DispatcherTimer();
            backgroundUpdateTimer.Interval = TimeSpan.FromMilliseconds(250);
            backgroundUpdateTimer.Tick += requestNDI;

            // Send canvas updates every 10ms
            System.Windows.Threading.DispatcherTimer canvasUpdateTimer = new System.Windows.Threading.DispatcherTimer();
            canvasUpdateTimer.Interval = TimeSpan.FromMilliseconds(10);
            canvasUpdateTimer.Tick += requestNDI;

            // Switch from the canvas update timer to the background update timer after 1 second
            System.Windows.Threading.DispatcherTimer backgroundUpdateTransitionTimer = new System.Windows.Threading.DispatcherTimer();
            backgroundUpdateTransitionTimer.Interval = TimeSpan.FromSeconds(1);
            backgroundUpdateTransitionTimer.Tick += (a, b) =>
            {
                backgroundUpdateTransitionTimer.Stop(); // Turn off after the tick
                backgroundUpdateTimer.Start();
                canvasUpdateTimer.Stop();
            };

            theWhiteboard.GotMouseCapture += (a, b) =>
            {
                backgroundUpdateTimer.Stop();
                canvasUpdateTimer.Start();
            };

            theWhiteboard.LostMouseCapture += (a, b) => backgroundUpdateTransitionTimer.Start();

            backgroundUpdateTimer.Start();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {


                case Key.Z:
                    if ((Forms.Control.ModifierKeys & Forms.Keys.Control) == Forms.Keys.Control)
                    {
                        if ((Forms.Control.ModifierKeys & Forms.Keys.Shift) == Forms.Keys.Shift)
                        {
                            theWhiteboard.Redo();
                        }
                        else
                        {
                            theWhiteboard.Undo();
                        }
                    }
                    break;

                case Key.Space:
                    theWhiteboard.Clear();
                    break;
                case Key.Back:
                    theWhiteboard.Clear();
                    break;

                case Key.Up:
                    test = test + 3.0;
                    theWhiteboard.SetPenThickness(test);
                    break;

                case Key.Down:
                    if (test >= 2.0)
                    {
                        test = test - 3.0;
                        theWhiteboard.SetPenThickness(test);
                        break;
                    }
                    else
                    {
                        break;
                    }

                case Key.R:

                    theWhiteboard.SetPenColour(Colors.Red);

                    break;

                case Key.G:

                    theWhiteboard.SetPenColour(Colors.Green);

                    break;
                case Key.B:

                    theWhiteboard.SetPenColour(Colors.Blue);

                    break;
                case Key.P:

                    theWhiteboard.SetPenColour(Colors.Purple);

                    break;
                case Key.Y:

                    theWhiteboard.SetPenColour(Colors.Yellow);

                    break;


                case Key.D1:
                    String play = ConfigurationManager.AppSettings["Play-Pause"];
                    GetVmix(play);
                    Console.WriteLine("Play-Pause");
                    theWhiteboard.Clear();
                    break;

                case Key.D2:
                    GetVmix(ConfigurationManager.AppSettings["Reverse"]);
                    Console.WriteLine("Reverse");
                    break;
            }
        }

        #region Toolbar Events
        private void Btn_Screenshot_Click(object sender, RoutedEventArgs e)
        {
            InkControls.Btn_Screenshot_Click(sender, e);
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            InkControls.clearWhiteboard();

        }

        private void Btn_Undo_Click(object sender, RoutedEventArgs e)
        {
            InkControls.undo();
        }

        private void Btn_Redo_Click(object sender, RoutedEventArgs e)
        {
            InkControls.redo();
        }
        private void Btn_Options_Click(object sender, RoutedEventArgs e)
        {
            optionsDialogue.IsOpen = !optionsDialogue.IsOpen;
        }

        private void onClrPickPen(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null) InkControls.setPenColour((Color)e.NewValue);
        }

        private void onClrPickBackground(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null) InkControls.setBackgroundColour(new SolidColorBrush((Color)e.NewValue));
        }

        public ICommand handleSelectThickness
        {
            get => new SimpleCommand(o => InkControls.setPenThickness(double.Parse((string)o)));
        }

        #endregion

    }
}