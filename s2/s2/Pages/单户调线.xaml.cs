using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Com.Aote.Pages
{
    public partial class 单户调线 : UserControl
    {
        public 单户调线()
        {
            // Required to initialize variables
            InitializeComponent();
        }
        private void f_newhandno_TextChanged(object sender, TextChangedEventArgs e)
        {
            f_newtiaoxianno.Text = Coboxlist.SelectedValue.ToString() + f_newhandno.Text;
        }
    }
}