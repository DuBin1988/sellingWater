using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using Com.Aote.ObjectTools;
using Com.Aote.Behaviors;
using System.Json;

namespace Com.Aote.Pages
{
    public partial class 新表投线 : UserControl
    {
        public 新表投线()
        {
            // Required to initialize variables
            InitializeComponent();
        }

        private void f_handno_TextChanged(object sender, TextChangedEventArgs e)
        {
            f_touxianno.Text = Coboxlist.SelectedValue.ToString() + "-" + f_handno.Text;
        }
    }
}