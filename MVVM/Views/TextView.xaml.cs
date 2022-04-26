using System;
using System.Collections.Generic;
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

namespace PhotoEditorNet.MVVM.Views
{
    /// <summary>
    /// Interaction logic for TextView.xaml
    /// </summary>
    public partial class TextView : UserControl
    {

        public ICollection<FontFamily> FontCollection = Fonts.SystemFontFamilies;
      
        public FontFamily selectedFontFamily;
        private static readonly double[] CommonlyUsedFontSizes =
        {
            3.0, 4.0, 5.0, 6.0, 6.5,
            7.0, 7.5, 8.0, 8.5, 9.0,
            9.5, 10.0, 10.5, 11.0, 11.5,
            12.0, 12.5, 13.0, 13.5, 14.0,
            15.0, 16.0, 17.0, 18.0, 19.0,
            20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0,
            40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
            80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
        };

        public TextView()
        {
            InitializeComponent();
        }

        private void FontChooser_Initialized(object sender, EventArgs e)
        {
            FillFontComboBox(FontChooser);
        }

        public void FillFontComboBox(ComboBox comboBoxFonts)
        {
            // Enumerate the current set of system fonts,
            // and fill the combo box with the names of the fonts.
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                comboBoxFonts.Items.Add(fontFamily.Source);
            }

            comboBoxFonts.SelectedIndex = 0;
        }

        private void FontSizeChooser_Initialized(object sender, EventArgs e)
        {
            foreach(var value in CommonlyUsedFontSizes)
            {
                FontSizeChooser.Items.Add(value);
            }
            FontSizeChooser.SelectedIndex = 11;
        }


        private void TypeFaceChooser_Initialized(object sender, EventArgs e)
        {
            TypeFaceChooserFunc(TypeFaceChooser);
        }


        public void TypeFaceChooserFunc(ComboBox cb)
        {
            var family = selectedFontFamily;
            if (family != null)
            {
                var faceCollection = family.GetTypefaces();

                //var items = new TypefaceListItem[faceCollection.Count];

                //var i = 0;

                //foreach (var face in faceCollection)
                //{
                //    items[i++] = new TypefaceListItem(face);
                //}

                //Array.Sort(items);

                foreach (var face in faceCollection)
                {
                    cb.Items.Add(face);
                }
            }
        }

        private void FontChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedFontFamily = FontChooser.SelectedItem as FontFamily;
            
            TypeFaceChooserFunc(TypeFaceChooser);
        }

        private void FontFace_Initialized(object sender, EventArgs e)
        {
            var family = selectedFontFamily;
            if (family != null)
            {
                var faceCollection = family.GetTypefaces();
                
                //var items = new TypefaceListItem[faceCollection.Count];

                //var i = 0;

                //foreach (var face in faceCollection)
                //{
                //    items[i++] = new TypefaceListItem(face);
                //}

                //Array.Sort(items);

                foreach (var face in faceCollection)
                {
                    FontFace.Items.Add(face);
                }
            }
        }
    }
}
