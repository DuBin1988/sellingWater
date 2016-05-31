﻿using System;
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
	public partial class 抄表册生成 : UserControl
	{
        public 抄表册生成()
		{
			// Required to initialize variables
			InitializeComponent();
		}

        private void f_listname_MouseEnter(object sender, MouseEventArgs e)
        {
            string date = handdate.Text;
            string area = handarea.SelectedValue.ToString();
            string code1 = code.Text;
            string handplan = date + area + code1;
            listid.Text = handplan;
        }
	}
}