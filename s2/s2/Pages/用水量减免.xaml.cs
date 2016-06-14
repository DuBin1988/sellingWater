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
        ObjectList list = null;
        List<GeneralObject> handlist = new List<GeneralObject>();
     
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
                obj.SetPropertyValue("oughtfee", double.Parse(go.GetPropertyValue("oughtfee").ToString()), false);
                obj.SetPropertyValue("newoughtfee", double.Parse(ui_oughtfee.Text), false);
                obj.SetPropertyValue("f_updatenote", ui_updatenote.Text, false);
                obj.SetPropertyValue("lastinputdate", go.GetPropertyValue("lastinputdate"), false);
                obj.SetPropertyValue("newlastinputdate", ui_lastinputdate.SelectedDate, false);
                obj.SetPropertyValue("oughtamount", double.Parse(go.GetPropertyValue("oughtamount").ToString()), false);
                obj.SetPropertyValue("newoughtamount", double.Parse(ui_oughtamount.Text), false);
                obj.SetPropertyValue("extrazjfee", double.Parse(go.GetPropertyValue("extrazjfee").ToString()), false);
                obj.SetPropertyValue("newextrazjfee", double.Parse(ui_f_extraprices.Text), false);
                obj.SetPropertyValue("f_fee", double.Parse(go.GetPropertyValue("f_fee").ToString()), false);
                obj.SetPropertyValue("f_newfee", double.Parse(ui_f_fee.Text), false);
                obj.SetPropertyValue("f_jianshuiliang", double.Parse(ui_jianamount.Text), false);
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
            int pregas = int.Parse(ui_oughtamount.Text);
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

            foreach(GeneralObject ss in handlist)
            {
                string handid = ss.GetPropertyValue("id").ToString();
                double f_stair1amount = double.Parse(ss.GetPropertyValue("f_stair1amount").ToString());
                double f_stair1fee = double.Parse(ss.GetPropertyValue("f_stair1fee").ToString());
                double f_stair2amount = double.Parse(ss.GetPropertyValue("f_stair2amount").ToString());
                double f_stair2fee = double.Parse(ss.GetPropertyValue("f_stair2fee").ToString());
                double f_stair3amount = double.Parse(ss.GetPropertyValue("f_stair3amount").ToString());
                double f_stair3fee = double.Parse(ss.GetPropertyValue("f_stair3fee").ToString());
                double f_stair4amount = double.Parse(ss.GetPropertyValue("f_stair4amount").ToString());
                double f_stair4fee = double.Parse(ss.GetPropertyValue("f_stair4fee").ToString());
                double f_allamont = double.Parse(ss.GetPropertyValue("f_allamont").ToString());
                double oughtfee = double.Parse(ss.GetPropertyValue("oughtfee").ToString());
                string sql2 = "update t_handplan set f_stair1amount= " + f_stair1amount + ",f_stair1fee= " + f_stair1fee +
         ",f_stair2amount= " + f_stair2amount + ",f_stair2fee= " + f_stair2fee +
         ",f_stair3amount= " + f_stair3amount + ",f_stair3fee= " + f_stair3fee +
         ",f_stair4amount= " + f_stair4amount + ",f_stair4fee= " + f_stair4fee +
         ",f_allamont= " + f_allamont + ",oughtfee= " + oughtfee + ",f_fee = extrazjfee + " + oughtfee + " where id=" + handid;
                HQLAction action2 = new HQLAction();
                action2.HQL = sql2;
                action2.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                action2.Name = "t_handplan";
                action2.Invoke();
            } 
            //如果数据有误，页面提示
            //回调页面保存按钮功能
            BatchExcuteAction save = (from p in loader.Res where p.Name.Equals("SaveAction") select p).First() as BatchExcuteAction;
            save.Invoke();
            PagedObjectList save1 = (from p in loader.Res where p.Name.Equals("personlist") select p).First() as PagedObjectList;
            save1.IsOld = true;
            updatehandplan.New();
        }

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

                string sql1 = "update t_handplan set f_state= '" + "待审核" + "' where f_userinfoid=" + userinfoid + " and oughtfee!=null and shifoujiaofei = '否' and lastinputdate>=" + ui_lastinputdate.Text;
                HQLAction action1 = new HQLAction();
                action1.HQL = sql1;
                action1.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                action1.Name = "handplan";
                action1.Invoke();

                list = new ObjectList();
                list.LoadOnPathChanged = false;
                list.EntityType = "t_handplan";
                list.Path = "from t_handplan where f_userinfoid=" + userinfoid + "and f_state= '" + "待审核" + "' and oughtfee!=null and shifoujiaofei = '否' and lastinputdate>=" + ui_lastinputdate.Text;
                list.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                list.Completed += LoadBuildingAndOthers_Completed;
                list.Load();
                
        }

        public void LoadBuildingAndOthers_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //拿出datagrid所选数据
            GeneralObject go = handUserUnits.SelectedItem as GeneralObject;
            //拿出该条抄表记录的附加费用单价总和
            double extrazj = (double)(go.GetPropertyValue("extrazj"));
            double gas = double.Parse(ui_oughtamount.Text.ToString());
            for (int i = 0; i < list.Count;i++ )
            {
                GeneralObject at = new GeneralObject();
                at.EntityType = "t_handplan";
                GeneralObject  oo = list[i] as GeneralObject;
                string id = oo.GetPropertyValue("id").ToString();
                double pregas = 0;
                if (go.Equals(oo))
                {
                    pregas = double.Parse(gas.ToString());
                }
                else {
                    pregas = double.Parse(oo.GetPropertyValue("oughtamount").ToString());
                }
                string userid = oo.GetPropertyValue("f_userid").ToString();
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
                    try { 
                        at.SetPropertyValue("id", id, false);
                        at.SetPropertyValue("f_stair1amount", (double)items["f_stair1amount"], false);
                        at.SetPropertyValue("f_stair1fee", (double)items["f_stair1fee"], false);
                        at.SetPropertyValue("f_stair2amount", (double)items["f_stair2amount"], false);
                        at.SetPropertyValue("f_stair2fee", (double)items["f_stair2fee"], false);
                        at.SetPropertyValue("f_stair3amount", (double)items["f_stair3amount"], false);
                        at.SetPropertyValue("f_stair3fee", (double)items["f_stair3fee"], false);
                        at.SetPropertyValue("f_stair4amount", (double)items["f_stair4amount"], false);
                        at.SetPropertyValue("f_stair4fee", (double)items["f_stair4fee"], false);
                        at.SetPropertyValue("f_allamont", (double)items["f_allamont"], false);
                        at.SetPropertyValue("oughtfee", (double)items["f_chargenum"], false);
                        handlist.Add(at);
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                        if (go.Equals(oo))
                           {
                                double oughtfee = double.Parse(at.GetPropertyValue("oughtfee").ToString());
                                ui_oughtfee.Text = oughtfee.ToString();
                                double extrafee = extrazj * gas;
                                ui_f_extraprices.Text = extrafee.ToString();
                                ui_f_fee.Text = (oughtfee + extrafee).ToString();
                            }
                };
                client.DownloadStringAsync(uri);


                string sql1 = "update t_handplan set f_state= '" + "已抄表" + "' where id=" + id + "";
                HQLAction action1 = new HQLAction();
                action1.HQL = sql1;
                action1.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                action1.Name = "handplan";
                action1.Invoke();
            }
        }
    }
}