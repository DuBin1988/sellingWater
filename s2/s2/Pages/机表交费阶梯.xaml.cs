using Com.Aote.ObjectTools;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Json;
using System.Collections.Generic;
using Com.Aote.Behaviors;
using Com.Aote.Behaviors;
using Com.Aote.Utils;

namespace Com.Aote.Pages
{
    public partial class 机表交费阶梯 : UserControl
    {
        //欠费list
        int count = 0;
        //开阀用userid
        string iesid = "null";
        public 机表交费阶梯()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ui_userid.Focus();
        }

        private void ui_userid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                kbsellgasbusy.IsBusy = true;
                busy.IsBusy = true;

                string f_userid = ui_userid.Text;

                WebClientInfo wci = Application.Current.Resources["server"] as WebClientInfo;
                string uri = wci.BaseAddress + "/sell/bill/" + f_userid + "?uuid=" + System.Guid.NewGuid().ToString();

                WebClient client = new WebClient();
                client.DownloadStringCompleted += userfiles_DownloadStringCompleted;
                client.DownloadStringAsync(new Uri(uri));

                shoukuan.Focus();
            }
        }
	
 
         #region 选中新用户后的处理过程
        private void userfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //取当前选中项的用户编号，传递到后台取数据
            GeneralObject go = userfiles.SelectedItem as GeneralObject;
            if (go == null)
            {
                return;
            }

            kbsellgasbusy.IsBusy = true;
            busy.IsBusy = true;

            string f_userid = go.GetPropertyValue("f_userid").ToString();

            WebClientInfo wci = Application.Current.Resources["server"] as WebClientInfo;
            string uri = wci.BaseAddress + "/sell/bill/" + f_userid + "?uuid=" + System.Guid.NewGuid().ToString();

            WebClient client = new WebClient();
            client.DownloadStringCompleted += userfiles_DownloadStringCompleted;
            client.DownloadStringAsync(new Uri(uri));
        }

        void userfiles_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            kbsellgasbusy.IsBusy = false;
            busy.IsBusy = false;

            if (e.Error != null)
            {
                MessageBox.Show("未找到用户的表具信息，请去表具建档！");
                return;
            }

            //把数据转换成JSON
            JsonObject item = JsonValue.Parse(e.Result) as JsonObject;

            //把用户数据写到交费界面上
            ui_username.Text = (string)item["f_username"];
            ui_usertype.Text = (String)item["f_usertype"];
            ui_districtname.Text = (String)item["f_districtname"];
            ui_gasproperties.Text = (String)item["f_gasproperties"];
            ui_stairpricetype.Text = (String)item["f_stairtype"];
            zhye.Text = item["f_zhye"].ToString();
            ui_address.Text = (String)item["f_address"];
            //ui_gaspricetype.Text = (String)item["f_gaspricetype"];
            ui_userid.Text = item["infoid"].ToString();

            ui_dibaohu.IsChecked = item["f_dibaohu"].ToString().Equals("1");
            ui_userstate.Text = (String)item["f_userstate"];
            ui_paytype.Text = (String)item["f_payment"];
            // ui_gasprice.Text = item["f_gasprice"].ToString();
            //折子行号
            zhe.Text = item["f_zherownum"].ToString();


            //把欠费数据插入到欠费表中
            BaseObjectList list = dataGrid1.ItemsSource as BaseObjectList;
            if (list != null)
            {
                list.Clear();
            }

            // 当前正在处理的表号
            String currentId = "";
            // 总的上期指数
            decimal lastnum = 0;
            // 总气量
            decimal gasSum = 0;
            // 总气费
            decimal feeSum = 0;
            // 总附加费用
            decimal extrafeeSum = 0;
            // 总基本用水金额
            decimal f_feeSum = 0;
            //总的违约金
            decimal zhinajinAll = 0;
            //余额
            decimal f_zhye = decimal.Parse(item["f_zhye"].ToString());


            JsonArray bills = item["f_hands"] as JsonArray;
            foreach (JsonObject json in bills)
            {
                GeneralObject go = new GeneralObject();
                go.EntityType = "t_handplan";

                //默认选中
                go.IsChecked = true;

                //上期指数
                decimal lastinputgasnum = (decimal)json["lastinputgasnum"];
                go.SetPropertyValue("lastinputgasnum", lastinputgasnum, false);
                string f_userid = (string)json["f_userid"];
                go.SetPropertyValue("f_userid", f_userid, false);
                // 如果表号变了
                if (!f_userid.Equals(currentId))
                {
                    currentId = f_userid;
                    lastnum += lastinputgasnum;
                }

                //计算总金额
                decimal fee = (decimal)json["f_fee"];
                go.SetPropertyValue("f_fee", fee, false);
                feeSum += fee;
                //计算总附加费用
                decimal fujiafee = (decimal)json["extrazjfee"];
                go.SetPropertyValue("extrazjfee", fujiafee, false);
                extrafeeSum += fujiafee;
                //计算总基本用水金额
                decimal shuifee = (decimal)json["oughtfee"];
                go.SetPropertyValue("oughtfee",shuifee, false);
                f_feeSum += shuifee;
                // 计算总气量
                decimal oughtamount = (decimal)json["oughtamount"];
                gasSum += oughtamount;
                go.SetPropertyValue("oughtamount", oughtamount, false);
                //计算总违约金
                decimal f_zhinajin = (decimal)json["f_zhinajin"];
                zhinajinAll += f_zhinajin;
                go.SetPropertyValue("f_zhinajin", f_zhinajin, false);

                go.SetPropertyValue("lastinputdate", DateTime.Parse(json["lastinputdate"]), false);
                go.SetPropertyValue("lastrecord", (decimal)json["lastrecord"], false);
                go.SetPropertyValue("f_endjfdate", DateTime.Parse(json["f_endjfdate"]), false);
                go.SetPropertyValue("f_zhinajintianshu", (int)json["days"], false);
                go.SetPropertyValue("f_network", (string)json["f_network"], false);
                go.SetPropertyValue("f_operator", (string)json["f_operator"], false);
                go.SetPropertyValue("f_inputdate", DateTime.Parse(json["f_inputdate"]), false);
                go.SetPropertyValue("f_userid", (string)json["f_userid"], false);
                go.SetPropertyValue("jianshuiliang", (string)json["jianshuiliang"], false);

                go.SetPropertyValue("f_stair1amount", (decimal)json["f_stair1amount"], false);
                go.SetPropertyValue("f_stair1price", (decimal)json["f_stair1price"], false);
                go.SetPropertyValue("f_stair1fee", (decimal)json["f_stair1fee"], false);

                go.SetPropertyValue("f_stair2amount", (decimal)json["f_stair2amount"], false);
                go.SetPropertyValue("f_stair2price", (decimal)json["f_stair2price"], false);
                go.SetPropertyValue("f_stair2fee", (decimal)json["f_stair2fee"], false);

                go.SetPropertyValue("f_stair3amount", (decimal)json["f_stair3amount"], false);
                go.SetPropertyValue("f_stair3price", (decimal)json["f_stair3price"], false);
                go.SetPropertyValue("f_stair3fee", (decimal)json["f_stair3fee"], false);

                go.SetPropertyValue("f_stair4amount", (decimal)json["f_stair4amount"], false);
                go.SetPropertyValue("f_stair4price", (decimal)json["f_stair4price"], false);
                go.SetPropertyValue("f_stair4fee", (decimal)json["f_stair4fee"], false);

                list.Add(go);
            }

            // 计算出来的总气量等放到用户界面上
            ui_pregas.Text = gasSum.ToString("0.#");//总气量
            ui_lastinputgasnum.Text = lastnum.ToString("0.#");//总上期底数
            ui_lastrecord.Text = (lastnum + gasSum).ToString("0.#");//总本期底数
            ui_zhinajin.Text = zhinajinAll.ToString("0.##");//总违约金
            ui_linshizhinajin.Text = zhinajinAll.ToString("0.##");//违约金
            decimal f_totalcost = feeSum - f_zhye + zhinajinAll > 0 ? feeSum - f_zhye + zhinajinAll : 0;
            ui_totalcost.Text = f_totalcost.ToString("0.##");//应缴金额
            decimal f_benqizhye = (decimal)(f_zhye - feeSum - zhinajinAll > 0 ? f_zhye - feeSum - zhinajinAll : 0);
            ui_benqizhye.Text = f_benqizhye.ToString("0.##");//本期结余
            shoukuan.Text = f_totalcost.ToString("0.##");
            ui_preamount.Text = f_feeSum.ToString("0.##");//总基本水费金额
            ui_extrafee.Text = extrafeeSum.ToString("0.##");//总附加费用金额

        }
        #endregion
        #region SaveClick 提交按钮操作过程
        // 提交数据到后台服务器
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            //保存前获取欠费表信息和表编号
            count = (dataGrid1.ItemsSource as ObjectList).Count();
            if (count > 0)
            {
                ObjectList handList = dataGrid1.ItemsSource as ObjectList;
                GeneralObject go = handList.First();
                iesid = go.GetPropertyValue("f_userid").ToString();
            };
            //如果数据有错，提示不能保存
            GeneralObject kbfee = kbfee1.DataContext as GeneralObject;

            if (kbfee.HasErrors)
            {
                MessageBox.Show("输入数据有错，请检查！");
                return;
            }
            GeneralObject loginUser = (GeneralObject)FrameworkElementExtension.FindResource(this, "LoginUser");
            if (loginUser == null)
            {
                MessageBox.Show("无法获取当前登陆用户信息,请重新登陆后操作!");
                return;
            }
            string loginid = (string)loginUser.GetPropertyValue("id");
            //显示正在工作
            busy.IsBusy = true;
            //发票号
            string f_invoicenum = kbfee.GetPropertyValue("f_invoicenum") + "";
            if (string.IsNullOrEmpty(f_invoicenum))
            {
                f_invoicenum = "0";
            }

            //获取基础地址
            WebClientInfo wci = (WebClientInfo)Application.Current.Resources["server"];

            // 提交
            string str = wci.BaseAddress + "/sell/" + ui_userid.Text + "/" + shoukuan.Text + "/"
                + ui_zhinajin.Text + "/" + f_payment.SelectedValue + "/" + loginid + "?uuid=" + System.Guid.NewGuid().ToString();

            Uri uri = new Uri(str);
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            client.DownloadStringAsync(uri);
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            busy.IsBusy = false;
            //把数据转换成JSON
            JsonObject item = JsonValue.Parse(e.Result) as JsonObject;
            // 没有出错
           if (e.Error == null)
            {
                GeneralObject printobj = aofengprint.DataContext as GeneralObject;
                printobj.FromJson(item);
                //string date = (string)item["f_deliverydate"];
                //ui_day.Text = date;
                //保存发票信息
                GeneralObject fpinfosobj = (GeneralObject)(from r in loader.Res where r.Name.Equals("fpinfosobj") select r).First();
                fpinfosobj.SetPropertyValue("f_fapiaostatue", "已用", true);
                fpinfosobj.Save();
                // 调用打印
                MessageBoxResult mbr = MessageBox.Show("是否打印", "提示", MessageBoxButton.OKCancel);
                if (mbr == MessageBoxResult.OK)
                {
                    if (ui_usertype.Text == "民用")
                    {
                        //把数据转换成JSON
                        JsonObject item = JsonValue.Parse(e.Result) as JsonObject;
                        ui_stair1price.Text = item["f_stair1price"].ToString();
                        ui_stair1fee.Text = item["f_stair1fee"].ToString();
                        ui_stair1amount.Text = item["f_stair1amount"].ToString();
                        ui_stair2price.Text = item["f_stair2price"].ToString();
                        ui_stair2fee.Text = item["f_stair2fee"].ToString();
                        ui_stair2amount.Text = item["f_stair2amount"].ToString();
                        ui_stair3price.Text = item["f_stair3price"].ToString();
                        ui_stair3fee.Text = item["f_stair3fee"].ToString();
                        ui_stair3amount.Text = item["f_stair3amount"].ToString();


                        print5.TipPrint();
                        print5.Completed += print_Completed;
                    }
                    else
                    {
                        print.TipPrint();
                        print.Completed += print_Completed;
                    }
                    ui_userid.Focus();

                }
                else
                {
                    GeneralObject kbfee = (GeneralObject)(from r in loader.Res where r.Name.Equals("kbfee") select r).First();
                    kbfee.New();
                    ui_userid.Focus();
                }
               //交费开阀
               if (count > 0)
               {
                //更新阀门控制状态
                   string sql = "update t_userfiles set f_operate_zl='启用', f_returnvalueoperate=" + "'1' " +
                                " where f_userid='" + iesid + "'";
                   HQLAction action = new HQLAction();
                   action.HQL = sql;
                   action.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
                   action.Name = "t_userfiles";
                   action.Completed += action_Completed;
                   action.Invoke();
               }
            }
            else
            {
                // 提示出错
                MessageBox.Show("连接服务器失败，请重试！如果继续失败，请联系系统管理员。");
                // 清除界面数据
                GeneralObject kbfee = (GeneralObject)(from r in loader.Res where r.Name.Equals("kbfee") select r).First();
                kbfee.New();
                ui_userid.Focus();
            }

           
        }
        #endregion

        private void action_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            HQLAction action = sender as HQLAction;
            action.Completed -= action_Completed;

            if (e.Error == null)
            {
                //将产生的json串送后台服务进行处理
                WebClientInfo wci = Application.Current.Resources["server"] as WebClientInfo;
                string uri = wci.BaseAddress + "/iesgas/table/comand";
                WebClient client = new WebClient();
                client.UploadStringAsync(new Uri(uri), "[{\"search\":\"f_userid like '" + iesid + "'\"}]");

            }

        }

        private void Compute()
        {
            // 取出余额
            decimal f_zhye = decimal.Parse(zhye.Text);
            // 拿余额+实缴金额算未交费抄表记录是否应该交费
            decimal money = decimal.Parse(shoukuan.Text);
            decimal zhinajin = decimal.Parse(ui_zhinajin.Text);
            decimal total = f_zhye + money - zhinajin;

            // 存放所有扣费的表号
            List<string> userids = new List<string>();

            // 总的上期指数
            decimal lastnum = 0m;
            // 总气量
            decimal gasSum = 0m;
            // 总气费
            decimal feeSum = 0m;

            // 先收有违约金的欠费，有违约金的必须收
            foreach (GeneralObject map in dataGrid1.ItemsSource)
            {
                String f_userid = (String)map.GetPropertyValue("f_userid");

                // 取出应交金额
                decimal f_fee = decimal.Parse(map.GetPropertyValue("f_fee").ToString());
                //取出违约金
                decimal f_zhinajin = decimal.Parse(map.GetPropertyValue("f_zhinajin").ToString());
                //取出违约金天数
                int f_zhinajintianshu = int.Parse(map.GetPropertyValue("f_zhinajintianshu").ToString());

                // 超过交费期限，必须选取
                if (f_zhinajintianshu > 0)
                {
                    total -= f_fee;

                    // 当前表号没有处理过，上期指数增加
                    if (!userids.Contains(f_userid))
                    {
                        userids.Add(f_userid);

                        decimal lastinputgasnum = decimal.Parse(map.GetPropertyValue("lastinputgasnum").ToString());
                        lastnum += lastinputgasnum;
                    }

                    // 气量相加
                    decimal gas = decimal.Parse(map.GetPropertyValue("oughtamount").ToString());
                    gasSum += gas;

                    // 气费相加
                    feeSum += f_fee;

                    // 修改为选中
                    map.IsChecked = true;
                }
                else
                {
                    // 修改为未选中，避免开始清除所有选中项
                    map.IsChecked = false;

                }
            }

            // 当前正在处理的表号
            String currentId = "";
            // 当前表是否可以继续扣减
            bool canSub = true;

            foreach (GeneralObject map in dataGrid1.ItemsSource)
            {
                // 排除掉超期的
                int f_zhinajintianshu = int.Parse(map.GetPropertyValue("f_zhinajintianshu").ToString());
                if (f_zhinajintianshu > 0)
                {
                    continue;
                }

                String f_userid = (String)map.GetPropertyValue("f_userid");

                // 取出应交金额
                decimal f_fee = decimal.Parse(map.GetPropertyValue("f_fee").ToString());

                // 如果表号变了，继续可以扣减
                if (!f_userid.Equals(currentId))
                {
                    canSub = true;
                    currentId = f_userid;
                }

                // 当前表没有终止，够交，扣除，交费记录变为已交
                if (total >= f_fee && canSub)
                {
                    total = total - f_fee;

                    // 当前表号没有处理过，上期指数增加
                    if (!userids.Contains(f_userid))
                    {
                        userids.Add(f_userid);

                        decimal lastinputgasnum = decimal.Parse(map.GetPropertyValue("lastinputgasnum").ToString());
                        lastnum += lastinputgasnum;
                    }

                    // 气量相加
                    decimal gas = decimal.Parse(map.GetPropertyValue("oughtamount").ToString());
                    gasSum += gas;

                    // 气费相加
                    feeSum += f_fee;

                    // 修改为选中
                    map.IsChecked = true;

                }
                else
                {
                    // 当前表不能继续扣减
                    canSub = false;

                    // 修改为未选中
                     map.IsChecked = false;
                }
            }

            //把计算结果放到界面上
            ui_lastinputgasnum.Text = lastnum.ToString("0.#");
            ui_lastrecord.Text = (lastnum + gasSum).ToString("0.#");
            ui_pregas.Text = gasSum.ToString("0.#");
            //ui_preamount.Text = feeSum.ToString("0.##");
            //应交金额=气费-上期结余
            decimal f_totalcost = (feeSum - f_zhye + zhinajin) > 0 ? (feeSum - f_zhye + zhinajin) : 0;
            ui_totalcost.Text = f_totalcost.ToString("0.##");
            //本期结余=上期结余+实收-气费
            decimal f_benqizhye = f_zhye + money - feeSum - zhinajin;
            ui_benqizhye.Text = f_benqizhye.ToString("0.##");

        }

        // 清除界面上的数据
        private void Clear()
        {
            // 清除交费内容
            GeneralObject kbfee = (GeneralObject)(from r in loader.Res where r.Name.Equals("kbfee") select r).First();
            kbfee.New();

            // 清除欠费信息
            BaseObjectList list = dataGrid1.ItemsSource as BaseObjectList;
            list.Clear();

            // 当前选中用户为空
            userfiles.SelectedItem = null;
        }
         #region 收款输入后的处理过程
        private void shoukuan_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Compute();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
        #region 违约金修改后的处理过程
        private void ui_zhinajin_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Compute();
                ui_benqizhye.Text = 0.ToString("0.##");
                shoukuan.Text = ui_totalcost.Text;
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        private void cancle_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void print_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Clear();
        }

        private void ui_userid_LostFocus(object sender, RoutedEventArgs e)
        {
            //kbsellgasbusy.IsBusy = true;
            //busy.IsBusy = true;

            //string f_userid = ui_userid.Text;

            //WebClientInfo wci = Application.Current.Resources["server"] as WebClientInfo;
            //string uri = wci.BaseAddress + "/sell/bill/" + f_userid + "?uuid=" + System.Guid.NewGuid().ToString();

            //WebClient client = new WebClient();
            //client.DownloadStringCompleted += userfiles_DownloadStringCompleted;
            //client.DownloadStringAsync(new Uri(uri));
        }
        //#region SaveClick 提交按钮操作过程
        //// 提交数据到后台服务器
        //private void SaveClick(object sender, RoutedEventArgs e)
        //{
        //    //显示正在工作
        //    busy.IsBusy = true;
        //    //取出登陆用户id，后台根据id查找登陆用户放入分公司等信息
        //    GeneralObject loginUser = (GeneralObject)FrameworkElementExtension.FindResource(this, "LoginUser");
        //    if (loginUser == null)
        //    {
        //        MessageBox.Show("无法获取当前登陆用户信息,请重新登陆后操作!");
        //        return;
        //    }
        //    string loginid = (string)loginUser.GetPropertyValue("id");
        //    //获取基础地址
        //    WebClientInfo wci = (WebClientInfo)Application.Current.Resources["server"];
        //    // 提交
        //    string str = wci.BaseAddress + "/sell/" + ui_userid.Text + "/" + shoukuan.Text + "/"
        //        + ui_zhinajin.Text + "/" + f_payment.SelectedValue + "/" + loginid;
        //    Uri uri = new Uri(str);
        //    WebClient client = new WebClient();
        //    client.DownloadStringCompleted += client_DownloadStringCompleted;
        //    client.DownloadStringAsync(uri);
        //}

        //void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    busy.IsBusy = false;

        //    // 没有出错
        //    if (e.Error == null)
        //    {
        //        // 调用打印
        //        MessageBoxResult mbr = MessageBox.Show("是否打印", "提示", MessageBoxButton.OKCancel);
        //        if (mbr == MessageBoxResult.OK)
        //        {
        //            print.Print();
        //            print.Completed += print_Completed;

        //        }
        //        else
        //        {
        //            GeneralObject kbfee = (GeneralObject)(from r in loader.Res where r.Name.Equals("kbfee") select r).First();
        //            kbfee.New();
        //        }

        //    }
        //    else
        //    {
        //        // 提示出错
        //        MessageBox.Show("连接服务器失败，请重试！如果继续失败，请联系系统管理员。");
        //        // 清除界面数据
        //        GeneralObject kbfee = (GeneralObject)(from r in loader.Res where r.Name.Equals("kbfee") select r).First();
        //        kbfee.New();
        //    }
        //}

        //private void print_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    GeneralObject kbfee = (GeneralObject)(from r in loader.Res where r.Name.Equals("kbfee") select r).First();
        //    kbfee.New();
        //}
        //#endregion

    }

}
