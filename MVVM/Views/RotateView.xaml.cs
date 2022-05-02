using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PhotoEditorNet;

namespace PhotoEditorNet.MVVM.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private Bitmap beforeEdit;
        private Bitmap afterEdit;
        MainWindow window2;
       

        public HomeView()
        {
            InitializeComponent();
            window2 = Application.Current.Windows
    .Cast<Window>()
    .FirstOrDefault(window => window is MainWindow) as MainWindow;
            //beforeEdit = window2.EditedImage;
            
        }

        private void Rotate90Left_Click(object sender, RoutedEventArgs e)
        {
            //afterEdit = beforeEdit.RotateFlip(RotateFlipType.Rotate270FlipNone);
            //window2.EditedImage = beforeEdit;
        }
    }
}
