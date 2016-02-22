using Com.Aote.ObjectTools;
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
using Com.Aote.Controls;

namespace Com.Aote.Pages
{
    public partial class 撤停启用查询 : UserControl
    {
        public 撤停启用查询()
        {
            InitializeComponent();
        }

        private void zhikong_Click(object sender, RoutedEventArgs e)
        {
            ui_address.Text = "";
            ui_userid.Text = "";
            ui_username.Text = "";
            ui_whetherback.SelectedValue = "";
        }
    }
}
