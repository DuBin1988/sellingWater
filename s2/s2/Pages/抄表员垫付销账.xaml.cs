using Com.Aote.ObjectTools;
using Com.Aote.Utils;
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
using System.Json;
using Com.Aote.Behaviors;
namespace Com.Aote.Pages
{
    public partial class 抄表员垫付销账 : UserControl
    {
        public 抄表员垫付销账()
        {
            // Required to initialize variables
            InitializeComponent();
        }

        //批量打印
        //private void printButton_Click(object sender, RoutedEventArgs e)
        //{
        //    PagedObjectList all = new PagedObjectList();
        //    all.EntityType = "t_userfiles";
        //    //获取页面对象
        //    ObjectList printobj = this.daninfos.ItemsSource as ObjectList;
        //    if (printobj == null)
        //        return;
        //    IEnumerator<GeneralObject> printEnum = printobj.GetEnumerator();
        //    int count = 0;
        //    while (printEnum.MoveNext())
        //    {
        //        GeneralObject go = printEnum.Current as GeneralObject;
        //        if (go.IsChecked)
        //        {
        //            count = count + 1;
        //            all.Add(go);
        //        }
        //    }
        //    all.Count = count;
        //    hmcprint.List = all;
        //    hmcprint.PageRow = 1;
        //    hmcprint.PrintOneNoLoad();
        //}

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string bz = beizhu.Text;
            DateTime displacedate = DateTime.Today;
            ObjectList sells = this.daninfos.ItemsSource as ObjectList;
            if (sells == null)
                return;
            IEnumerator<GeneralObject> sell = sells.GetEnumerator();
            while (sell.MoveNext())
            {
                GeneralObject go = sell.Current as GeneralObject;
                if (go.IsChecked)
                {
                    string sql = "update t_sellinggas set f_whetherdianfu='0',f_displacedetails ='" + bz + "',f_displacedate='" + displacedate + "'  where id = " + go.GetPropertyValue("id");
                    HQLAction action = new HQLAction();
                    action.HQL = sql;
                    action.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                    action.Name = "abc";
                    action.Invoke();
                }
            }
        }

        private void qx_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox allCheck = (CheckBox)sender;
            IEnumerator<GeneralObject> enumer = ((ObjectList)this.daninfos.ItemsSource).GetEnumerator();
            while (enumer.MoveNext())
            {
                GeneralObject one = (GeneralObject)enumer.Current;
                if (allCheck.IsChecked == true)
                {
                    one.IsChecked = true;
                }
                else
                {
                    one.IsChecked = false;
                }
            }





            //if(allCheck.IsChecked == true)
            //{
            //    while (enumer.MoveNext())
            //    {
            //        GeneralObject one = (GeneralObject)enumer.Current;
            //        one.IsChecked = true;
            //    }
            //}
            //else if(allCheck.IsChecked == false)
            //{
            //    while (enumer.MoveNext())
            //    {
            //        GeneralObject one = (GeneralObject)enumer.Current;
            //        one.IsChecked = false;
            //     }
            //}     
        }
    }
}