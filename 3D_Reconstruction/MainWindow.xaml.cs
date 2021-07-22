using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.Windows.Media;
using STL_Tools;
using System.Windows.Media.Imaging;
using System.Drawing;

/// Косячная разметка приложения (необходимо сделать привязку элементов)
namespace _3D_Reconstruction
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model3D device;
        private ModelVisual3D device3D = new ModelVisual3D();
        public MainWindow()
        {
            InitializeComponent();
        }
        private Model3D Display3d(string model)
        {
            try
            {
                viewPort3d.RotateGesture = new MouseGesture(MouseAction.RightClick);
                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Silver));
                ModelImporter import = new ModelImporter();
                import.DefaultMaterial = material;
                device = import.Load(model);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            return device;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            viewPort3d.Children.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                STLReader stlReader = new STLReader(openFileDialog.FileName);
                device = SetModel(openFileDialog.FileName);
                TranslateTransform3D myTranslate = new TranslateTransform3D(0, 0, 0);
                device.Transform = myTranslate;

                device3D.Content = device;

                viewPort3d.Children.Add(device3D);
                viewPort3d.Children.Add(new SunLight());
                viewPort3d.ZoomExtents();
                viewPort3d.SetView(new Point3D(0,0,10), new Vector3D(0, 0, -1),
                    new Vector3D(0, 1, 0), 1000);
                viewPort3d.CameraController.AddZoomForce(-0.3);
            }
        }
        private static Model3DGroup SetModel(string filePath)
        {
            ModelImporter modellImporter = new ModelImporter();
            var model = modellImporter.Load(filePath);
            GeometryModel3D geometryModel3D = model.Children[0] as GeometryModel3D;
            DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
            geometryModel3D.Material = material;
            geometryModel3D.BackMaterial = material;
            Model3DGroup model3D = new Model3DGroup();
            model3D.Children.Add(geometryModel3D);
            return model3D;
        }

        private void Quit(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        // Костыль для перехода от bitmap (winforms) к bitmap (wpf()
        Bitmap bitmap = new Bitmap(1, 1);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            #region Фильтры для открытия изображений
            openDialog.Filter = "Image files (" +
                "*.BMP, " +
                "*.JPG, " +
                "*.GIF, " +
                "*.TIF, " +
                "*.PNG, " +
                "*.ICO, " +
                "*.EMF, " +
                "*.WMF)|" +
                "*.bmp;" +
                "*.jpg;" +
                "*.gif; " +
                "*.tif; " +
                "*.png; " +
                "*.ico; " +
                "*.emf; " +
                "*.wmf";
            #endregion

            if (openDialog.ShowDialog() == true)
            {
                Img.Source = new BitmapImage(new Uri(openDialog.FileName));
                bitmap = new Bitmap(openDialog.FileName);
            }

            #region Отделение фона от объекта
            System.Drawing.Color crl = bitmap.GetPixel(1, 1);
            bitmap.MakeTransparent(crl);
            #endregion

            PositionObj position = new PositionObj();
            var info = position.CalculateInfo(bitmap);

            #region Вывод информации
            TextBlock.Text= "Координаты центра: "+Environment.NewLine+
                +info.CenterX +
                " : "
                + info.CenterY + Environment.NewLine +
                " M 1:1 = "
                + info.M11 + Environment.NewLine +
                " M 2:0 = "
                + info.M20 + Environment.NewLine +
                " M 0:2 = "
                + info.M02 + Environment.NewLine +
                " Угол поворота:  "
                + info.Angle;
            #endregion
        }

        private void Reconstruction_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

