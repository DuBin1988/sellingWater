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
using System.Windows.Navigation;
using Com.Aote.Marks;
using System.Collections;
using s2.Pages;

namespace Com.Aote.Pages
{
    public partial class 用水量减免 : UserControl
    {
        int id = 0;
        ObjectList list = null;
        GeneralObject oo = null;
        public 用水量减免()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //拿出datagrid所选数据
            GeneralObject go = handUserUnits.SelectedItem as GeneralObject;
            //拿出页面数据上下文
            GeneralObject updatehandplan = handUserUnit.DataContext as GeneralObject;
            //新建对象，往t_yongshuiliang插入数据
            GeneralObject obj = new GeneralObject();
            try
            {
                obj.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                obj.EntityType = "t_yongshuiliang";
                obj.SetPropertyValue("f_userid", ui_userid.Text, false);
                obj.SetPropertyValue("f_username", ui_username.Text, false);
                obj.SetPropertyValue("f_address", ui_address.Text, false);
                obj.SetPropertyValue("oughtfee", decimal.Parse(go.GetPropertyValue("oughtfee").ToString()), false);
                obj.SetPropertyValue("newoughtfee", decimal.Parse(ui_oughtfee.Text), false);
                obj.SetPropertyValue("f_updatenote", ui_updatenote.Text, false);
                obj.SetPropertyValue("lastinputdate", go.GetPropertyValue("lastinputdate"), false);
                obj.SetPropertyValue("newlastinputdate", ui_lastinputdate.SelectedDate, false);
                obj.SetPropertyValue("oughtamount", decimal.Parse(go.GetPropertyValue("oughtamount").ToString()), false);
                obj.SetPropertyValue("newoughtamount", decimal.Parse(ui_oughtamount.Text), false);
                obj.SetPropertyValue("extrazjfee", decimal.Parse(go.GetPropertyValue("extrazjfee").ToString()), false);
                obj.SetPropertyValue("newextrazjfee", decimal.Parse(ui_f_extraprices.Text), false);
                obj.SetPropertyValue("f_fee", decimal.Parse(go.GetPropertyValue("f_fee").ToString()), false);
                obj.SetPropertyValue("f_newfee", decimal.Parse(ui_f_fee.Text), false);
                obj.SetPropertyValue("f_jianshuiliang", decimal.Parse(ui_jianamount.Text), false);
                obj.SetPropertyValue("f_handplanoperator", ui_handplanoperator.Text, false);
                obj.SetPropertyValue("f_handplandate", ui_handplandate.SelectedDate, false);
                obj.Name = "t_yongshuiliang";
                obj.Save();
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
            //取表具编号
            string userid = ui_userid.Text;
            string id = go.GetPropertyValue("id").ToString();
            int amount = int.Parse(ui_oughtamount.Text);
            int jianamount = int.Parse(ui_jianamount.Text);
            int pregas = amount - jianamount;
            string sql3 = "update t_extraprices set f_pregas= " + pregas + ",f_extrafee = f_extraprices*(" + pregas + ") where parentid=" + id;
            HQLAction action3 = new HQLAction();
            action3.HQL = sql3;
            action3.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
            action3.Name = "handplan1";
            action3.Invoke();

            string sql = "update t_handplan set oughtamount= " + pregas + ",extrazjfee =(select sum(f_extrafee) from t_extraprices where parentid='" + id + "') where id=" + id;
            HQLAction action = new HQLAction();
            action.HQL = sql;
            action.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
            action.Name = "t_handplan";
            action.Invoke();


            string f_stair1amount = oo.GetPropertyValue("f_stair1amount").ToString();
            string f_stair1fee = oo.GetPropertyValue("f_stair1fee").ToString();
            string f_stair2amount = oo.GetPropertyValue("f_stair2amount").ToString();
            string f_stair2fee = oo.GetPropertyValue("f_stair2fee").ToString();
            string f_stair3amount = oo.GetPropertyValue("f_stair3amount").ToString();
            string f_stair3fee = oo.GetPropertyValue("f_stair3fee").ToString();
            string f_stair4amount = oo.GetPropertyValue("f_stair4amount").ToString();
            string f_stair4fee = oo.GetPropertyValue("f_stair4fee").ToString();
            string f_allamont = oo.GetPropertyValue("f_allamont").ToString();
            string oughtfee = oo.GetPropertyValue("oughtfee").ToString();
            string sql2 = "update t_handplan set f_stair1amount= " + f_stair1amount + ",f_stair1fee= " + f_stair1fee +
         ",f_stair2amount= " + f_stair2amount + ",f_stair2fee= " + f_stair2fee +
         ",f_stair3amount= " + f_stair3amount + ",f_stair3fee= " + f_stair3fee +
         ",f_stair4amount= " + f_stair4amount + ",f_stair4fee= " + f_stair4fee +
         ",f_allamont= " + f_allamont + ",oughtfee= " + oughtfee + ",f_fee = (oughtfee + extrazjfee) where id=" + id;
            HQLAction action2 = new HQLAction();
            action2.HQL = sql2;
            action2.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
            action2.Name = "t_handplan";
            action2.Invoke();
            //如果数据有误，页面提示
            //回调页面保存按钮功能
            BatchExcuteAction save = (from p in loader.Res where p.Name.Equals("SaveAction") select p).First() as BatchExcuteAction;
            save.Invoke();
            PagedObjectList save1 = (from p in loader.Res where p.Name.Equals("personlist") select p).First() as PagedObjectList;
            save1.IsOld = true;
            updatehandplan.New();
        }

        int idx = 0; 
        //鼠标离开时，计算阶梯水价
        private void ui_jianamount_LostFocus(object sender, RoutedEventArgs e)
        {
                //拿出datagrid所选数据
                GeneralObject go = handUserUnits.SelectedItem as GeneralObject;
                //取用户编号
                string userinfoid = ui_userinfoid.Text;
                //计算水量
                int amount = int.Parse(ui_oughtamount.Text);
                int jianamount = int.Parse(ui_jianamount.Text);
                int pregas = amount - jianamount;
                ui_oughtamount.Text = pregas.ToString();
                if(pregas < 0)
                {
                    MessageBox.Show("抄表指数录入错误或减免水量过大");
                    return;
                }

                string sql1 = "update t_handplan set f_state= '" + "待审核" + "' where f_userinfoid=" + userinfoid + "";
                HQLAction action1 = new HQLAction();
                action1.HQL = sql1;
                action1.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                action1.Name = "handplan";
                action1.Invoke();

                list = new ObjectList();
                list.LoadOnPathChanged = false;
                list.EntityType = "t_handplan";
                list.Path = "from t_handplan where f_userinfoid=" + userinfoid + "and f_state= '" + "待审核" + "'";
                list.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                list.Completed += LoadBuildingAndOthers_Completed;
                list.Load();
                
        }

        public void LoadBuildingAndOthers_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            for (int i = 0; i < list.Count;i++ )
            {
                oo = list[i] as GeneralObject;
                string userid = oo.GetPropertyValue("f_userid").ToString();
                int pregas = int.Parse(oo.GetPropertyValue("oughtamount").ToString());
                //转换日期为文本lastinputdate
                String date = ((DateTime)oo.GetPropertyValue("lastinputdate")).ToString("yyyyMMdd");

                WebClientInfo wci = (WebClientInfo)Application.Current.Resources["server"];
                string str = wci.BaseAddress + "/handcharge/num/" + userid + "/" + pregas + "/" + date;
                Uri uri = new Uri(str);
                WebClient client = new WebClient();
                client.DownloadStringCompleted += (o, a) =>
                {
                    //更新数据
                    JsonObject items = JsonValue.Parse(a.Result) as JsonObject;
                    oo.SetPropertyValue("f_stair1amount", items["f_stair1amount"].ToString(), false);
                    oo.SetPropertyValue("f_stair1fee", items["f_stair1fee"].ToString(), false);
                    oo.SetPropertyValue("f_stair2amount", items["f_stair2amount"].ToString(), false);
                    oo.SetPropertyValue("f_stair2fee", items["f_stair2fee"].ToString(), false);
                    oo.SetPropertyValue("f_stair3amount", items["f_stair3amount"].ToString(), false);
                    oo.SetPropertyValue("f_stair3fee", items["f_stair3fee"].ToString(), false);
                    oo.SetPropertyValue("f_stair4amount", items["f_stair4amount"].ToString(), false);
                    oo.SetPropertyValue("f_stair4fee", items["f_stair4fee"].ToString(), false);
                    oo.SetPropertyValue("f_allamont", items["f_allamont"].ToString(), false);
                    oo.SetPropertyValue("oughtfee", items["f_chargenum"].ToString(), false);
                };
                client.DownloadStringAsync(uri);
            }
            //拿出datagrid所选数据
            GeneralObject go = handUserUnits.SelectedItem as GeneralObject;
            string id = go.GetPropertyValue("id").ToString();
            //拿出该条抄表记录的附加费用单价总和
            double extrazj = double.Parse(go.GetPropertyValue("extrazj").ToString());
            double gas = double.Parse(ui_oughtamount.Text.ToString()) - double.Parse(ui_jianamount.Text.ToString());
            if (id.Equals(oo.GetPropertyValue(id).ToString())){
                ui_oughtfee.Text = oo.GetPropertyValue("oughtfee").ToString();
                ui_f_extraprices.Text = (extrazj * gas).ToString();
                ui_f_fee.Text = ui_oughtfee.Text + ui_f_extraprices.Text;
            }
        }
    }
}