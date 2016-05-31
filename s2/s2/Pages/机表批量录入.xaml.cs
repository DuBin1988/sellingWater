using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using Com.Aote.ObjectTools;
using System.Net;
using System.Collections.Generic;
using System.Json;
using Com.Aote.Behaviors;

namespace Com.Aote.Pages
{
    public partial class 机表批量录入 : UserControl
    {
        PagedList list = new PagedList();
        public 机表批量录入()
        {
            // Required to initialize variables
            InitializeComponent();
        }

        #region saveButton_Click 保存按钮按下时
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            ui_handBusy.IsBusy = true;


            BaseObjectList list = daninfos.ItemsSource as BaseObjectList;

            List<GeneralObject> removed = new List<GeneralObject>();

            //对于每一条记录
            foreach (GeneralObject go in list)
            {
                //表具编号
                var userid = go.GetPropertyValue("f_userid");
                //抄表id
                var id = go.GetPropertyValue("id");
                // 抄表记录里的上期指数
                var lastinputnum = go.GetPropertyValue("lastinputgasnum");

                // 本次抄表指数
                var lastrecord = go.GetPropertyValue("lastrecord");

                // 抄表记录里的阶梯类型
                var stairType = go.GetPropertyValue("f_stairtype");
                // 用水量
                var oughtamount = go.GetPropertyValue("oughtamount");
                // 本期指数小于上期指数，不上传
                if (double.Parse(lastrecord.ToString()) < double.Parse(lastinputnum.ToString()))
                {
                    MessageBox.Show("表" + userid + "本期指数异常，请核查");
                    continue;
                }
                removed.Add(go);
                string sql = "update t_handplan set lastrecord=" + lastrecord + ",f_state= '"+ "待审核" + "',oughtamount= " + oughtamount + " where id=" + id;
                HQLAction action = new HQLAction();
                action.HQL = sql;
                action.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                action.Name = "t_handplan";
                action.Completed += action_Completed;
                action.Invoke();

            }
            foreach (GeneralObject go in removed)
            {
                list.Remove(go);
            }
        }

        private void action_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ui_handBusy.IsBusy = false;
        }
        #endregion

        private void countGas(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox box = sender as TextBox;
                GeneralObject go = box.DataContext as GeneralObject;

                //上期底数从数据对象取
                double lastinputgasnum = double.Parse(go.GetPropertyValue("lastinputgasnum").ToString());
                //由于焦点离开时，数据未传递到对象中，从界面取
                double lastrecord = double.Parse(box.Text);

                //设置水量
                double oughtamount = lastrecord - lastinputgasnum;
                go.SetPropertyValue("oughtamount", oughtamount, false);
            }
            catch (Exception ex)
            {
            }
        }

        #region 查询按钮处理过程
        // 查询按钮处理过程
        private void dansearchbutton_Click(object sender, RoutedEventArgs e)
        {
            ui_handBusy.IsBusy = true;
            // 掉用search对象的search方法，产生条件
            SearchObject search = daninfosearch.DataContext as SearchObject;
            search.Search();

            // 调用服务
            WebClientInfo wci = Application.Current.Resources["server"] as WebClientInfo;
            string uri = wci.BaseAddress + "/handcharge/download" + "?uuid=" + System.Guid.NewGuid().ToString();
            WebClient client = new WebClient();
            client.UploadStringCompleted += dansearch_UploadStringCompleted;
            client.UploadStringAsync(new Uri(uri), search.Condition);
        }

        void dansearch_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //有错误
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {

                //把数据转换成JSON
                JsonArray items = JsonValue.Parse(e.Result) as JsonArray;
                daninfos.ItemsSource = list;
                if (list.Size != 0)
                {
                    list.Clear();
                }
                foreach (JsonObject json in items)
                {
                    GeneralObject go = new GeneralObject();
                    try
                    {
                        go.EntityType = "t_handplan";



                        string f_userid = (string)json["f_userid"];//表具编号
                        go.SetPropertyValue("f_userid", f_userid, false);
                        string f_userinfoid = (string)json["f_userinfoid"];//用户编号
                        go.SetPropertyValue("f_userinfoid", f_userinfoid, false);
                        string f_handid = (string)json["f_handid"];//抄表编号
                        go.SetPropertyValue("f_handid", f_handid, false);
                        string f_meternumber = (string)json["f_meternumber"];//表钢印号
                        go.SetPropertyValue("f_meternumber", f_meternumber, false);
                        int id = (int)json["id"];//抄表记录id
                        go.SetPropertyValue("id", id, false);
                        string f_stairtype = (string)json["f_stairtype"];//阶梯类型
                        go.SetPropertyValue("f_stairtype", f_stairtype, false);
                        string f_extrawaterprice = (string)json["f_extrawaterprice"];//混合用水差价
                        go.SetPropertyValue("f_extrawaterprice", f_extrawaterprice, false);
                        string f_username = (string)json["f_username"];//用户名
                        go.SetPropertyValue("f_username", f_username, false);
                        string f_address = (string)json["f_address"];//地址
                        go.SetPropertyValue("f_address", f_address, false);
                        decimal lastinputgasnum = (decimal)json["lastinputgasnum"];//上期指数
                        go.SetPropertyValue("lastinputgasnum", lastinputgasnum, false);
                        list.Add(go);
                    }
                    catch (Exception ex)
                    {
                        ui_handBusy.IsBusy = false;
                        MessageBox.Show("用户:" + go.GetPropertyValue("f_userid") + "抄表数据有问题,请核查!");
                        MessageBox.Show(ex.ToString() + "" + go.GetPropertyValue("f_userid"));
                        return;
                    }
                }
            }
            ui_handBusy.IsBusy = false;
        }

        #endregion
    }
}