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
using System.Net;
using System.Linq;

namespace Com.Aote.Pages
{
	public partial class 档案变更 : UserControl
	{
        String userid = "";
		public 档案变更()
		{
			InitializeComponent();
		}
        private void save_Click(object sender, RoutedEventArgs e)
        {
            String sql="update t_handplan set f_username='"+ui_usernamechange.Text+"'"+
                ",lastinputgasnum='"+ui_lastinputgasnumchange.Text+"'"+
                ",f_inputtor='"+ui_inputtorchange.SelectedValue+"'"+
                ",f_phone='"+ui_phonechange.Text+"'"+
                ",f_address='"+ui_addresschange.Text+"'"+
                ",f_usertype='"+ui_usertypechange.Text+"'"+
                ",f_handarea='" +ui_handareachange.SelectedValue+ "'" +
                ",f_gasproperties='" + CoboxGasPro.SelectedValue + "'" +
                ",f_stairtype='" + CoboxStair.SelectedValue + "'" +
                ",f_extrawaterprice='"+ui_extrawaterpricechange.Text+"'"+
                ",f_gasmeterstyle='"+ui_gasmeterstylechange.Text+"' where f_state='未抄表' and f_userid='"+ui_userid.Text+"'";
            HQLAction action = new HQLAction();
            action.HQL = sql;
            action.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
            action.Name = "abc";
            action.Invoke();
            //如果数据有误，页面提示
            //回调页面保存按钮功能
            SyncActionFactory save = (from p in loader.Res where p.Name.Equals("SaveAction") select p).First() as SyncActionFactory;
            save.Invoke();
            //updatehandplan.New();
        }

        #region saveButton_Click 过户保存按钮按下时
        private void save1_Click(object sender, RoutedEventArgs e)
        {
            ui_apply.IsBusy = true;

            GeneralObject obj = apply.DataContext as GeneralObject;
            userid = obj.GetPropertyValue("f_userid") + "";

            SyncActionFactory save = (from p in loader.Res where p.Name.Equals("t_SaveAction") select p).First() as SyncActionFactory;
            save.Invoke();

        }



       
        #endregion
    }
}