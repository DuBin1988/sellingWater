using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Com.Aote.ObjectTools;
using Com.Aote.Behaviors;
using System.Linq;

namespace Com.Aote.Pages
{
	public partial class 档案变更 : UserControl
	{
		public 档案变更()
		{
			InitializeComponent();
		}
        private void save_Click(object sender, RoutedEventArgs e)
        {
            String sql="update t_handplan set f_username='"+ui_usernamechange.Text+"'"+
                ",f_inputtor='"+ui_inputtorchange.SelectedValue+"'"+
                ",f_phone='"+ui_phonechange.Text+"'"+
                ",f_address='"+ui_addresschange.Text+"'"+
                ",f_usertype='" + ui_usertypechange.SelectedValue + "'" +
                ",f_handarea='" +ui_handareachange.SelectedValue+ "'" +
                ",f_gasproperties='" + CoboxGasPro.SelectedValue + "'" +
                ",f_stairtype='" + CoboxStair.SelectedValue + "'" +
                ",f_extrawaterprice='"+ui_extrawaterpricechange.Text+"' where f_state='未抄表' and f_userinfoid='"+ui_userid.Text+"'";
            HQLAction action = new HQLAction();
            action.HQL = sql;
            action.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
            action.Name = "abc";
            action.Invoke();
            String sql1 = "update t_userfiles set f_username='" + ui_usernamechange.Text + "'" +
                ",f_inputtor='" + ui_inputtorchange.SelectedValue + "'" +
                ",f_phone='" + ui_phonechange.Text + "'" +
                ",f_address='" + ui_addresschange.Text + "'" +
                ",f_usertype='" + ui_usertypechange.SelectedValue + "'" +
                ",f_handarea='" + ui_handareachange.SelectedValue + "'" +
                ",f_gasproperties='" + CoboxGasPro.SelectedValue + "'" +
                ",f_stairtype='" + CoboxStair.SelectedValue + "'" +
                ",f_stair1amount='" + f_stair1amountchange.Text + "'" +
                ",f_stair1price='" + f_stair1pricechange.Text + "'" +
                ",f_stair2amount='" + f_stair2amountchange.Text + "'" +
                ",f_stair2price='" + f_stair2pricechange.Text + "'" +
                ",f_stair3amount='" + f_stair3amountchange.Text + "'" +
                ",f_stair3price='" + f_stair3pricechange.Text + "'" +
                ",f_stair4price='" + f_stair4pricechange.Text + "'" +
                ",f_stairmonths='" + f_stairmonthschange.Text + "'" +
                ",f_zongjiprice='" + f_zongjipricechange.Text + "'" +
                ",f_extrawaterprice='" + ui_extrawaterpricechange.Text + "' where f_userinfoid='" + ui_userid.Text + "'";
            HQLAction action1 = new HQLAction();
            action1.HQL = sql1;
            action1.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
            action1.Name = "aaa";
            action1.Invoke();
            //如果数据有误，页面提示
            //回调页面保存按钮功能
            SyncActionFactory save = (from p in loader.Res where p.Name.Equals("SaveAction") select p).First() as SyncActionFactory;
            save.Invoke();
            //updatehandplan.New();
        }
	}
}