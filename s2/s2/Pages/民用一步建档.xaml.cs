﻿using Com.Aote.Behaviors;
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
using System.Windows.Ink;
namespace Com.Aote.Pages
{
    public partial class 民用一步建档 : UserControl
    {
        GeneralObject obj = new GeneralObject();
        String userid = "";
        public 民用一步建档()
        {

            // Required to initialize variables
            InitializeComponent();
        }
        #region saveButton_Click 保存按钮按下时
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            ui_userfile.IsBusy = true;

            GeneralObject obj = userfile.DataContext as GeneralObject;
            userid = obj.GetPropertyValue("f_userid") + "";
            SyncActionFactory save = (from p in loader.Res where p.Name.Equals("SaveActionT") select p).First() as SyncActionFactory;
            save.Completed += save_Completed;
            save.Invoke();
        }

        void save_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            SyncActionFactory save = sender as SyncActionFactory;
            save.Completed -= save_Completed;

            if (userid != null)
            {
                //将产生的json串送后台服务进行处理
                WebClientInfo wci = Application.Current.Resources["server"] as WebClientInfo;
                string uri = wci.BaseAddress + "/iesgas/user/comand";
                WebClient client = new WebClient();
                client.UploadStringCompleted += client_UploadStringCompleted;
                client.UploadStringAsync(new Uri(uri), "[{customer_code:\"" + userid + "\"}]");
            }
            else
            {
                ui_userfile.IsBusy = false;
            }
        }

        void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            ui_userfile.IsBusy = false;
            if (e.Error == null)
            {
                //弹出错误信息
                if (e.Result != "")
                {
                    // MessageBox.Show(e.Result);
                }

            }
        }
        #endregion

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (null == f_userid.Text || "".Equals(f_userid.Text))
            {
                SyncActionFactory save2 = (from p in loader.Res where p.Name.Equals("getbianh") select p).First() as SyncActionFactory;
                save2.Invoke();
            }

        }

        private void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            //{m:Exp Str=tabletel.IsOld\=True}
           
        }
    }
}