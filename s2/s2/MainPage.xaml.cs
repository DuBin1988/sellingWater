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
using System.Collections;

namespace Com.Aote.Pages
{
    public partial class GasMainPage : UserControl
    {
        private int clickcount=0;
        public GasMainPage()
        {
            InitializeComponent();
            //gasmg.Loaded += gasmg_Loaded;
        }

        //void gasmg_Loaded(object sender, RoutedEventArgs e)
        //{
        //    gasmg.Loaded -= gasmg_Loaded;

        //    GeneralObject loginUser = (gasmg.Res[0] as CustomTypeHelper).FindResource("LoginUser") as GeneralObject;

        //     foreach (GeneralObject go in (loginUser.GetPropertyValue("functions")) as IEnumerable)
        //     {
        //         foreach (GeneralObject go2 in (IEnumerable)go.GetPropertyValue("children"))
        //         {
        //             if (go2.GetPropertyValue("name").Equals("综合查询"))
        //             {
        //                 //ObjectList sels = (from p in gasmg.Res where p.Name == "selected" select p).First() as ObjectList;
        //                 //sels.Add(go2);
        //                 break;
        //             }
        //         }
        //     }
        // }
            
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GeneralObject go = (GeneralObject)Application.Current.Resources["LoginUser"];
            go.GetPropertyValue("functions");
            clickcount = 0;
        }
		// tab关闭
		private void Button_Click1(object sender, RoutedEventArgs e)
        {
             Button b = sender as Button;
             if (this.tab.Items.Count > 1)
             {
                 //this.tab.Items.RemoveAt(Convert.ToInt32(b.Tag));
                 this.tab.Items.RemoveAt(Convert.ToInt32(b.Tag));
                 clickcount = 0;
             }
             else {
                 clickcount++;
             }
            if(clickcount>18)
             MessageBox.Show("这个，你是关闭不了的！你都点击了"+clickcount+"次了！");
        }
		
        // private void addPageToTab(object sender, RoutedEventArgs e)
        //{
        //    GeneralObject go = (GeneralObject)((Button)sender).DataContext;
        //    addPageToTab(go);
            
        //}

        private void addPageToTab(object sender, RoutedEventArgs e)
         {
             GeneralObject go = (GeneralObject)((Button)sender).DataContext;
             ObjectList sels = (from p in gasmg.Res where p.Name == "selected" select p).First() as ObjectList;
             if (sels.Contains(go))
             {
                 this.tab.SelectedIndex = go.IndexOf(sels);
             }
             else
             {
                 sels.Add(go);
                 this.tab.SelectedIndex = go.IndexOf(sels);
             }
         }


    }
}