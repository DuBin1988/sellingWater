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

namespace Com.Aote.Pages
{
	public partial class 表档案信息 : UserControl
	{
        SearchObject userSearch = new SearchObject();

        PagedList userList = new PagedList();

		public 表档案信息()
		{
			// Required to initialize variables
			InitializeComponent();

            daninfosearch.DataContext = userSearch;

            daninfos.ItemsSource = userList;
		}
        int pageIndex = 0;
        private void dansearchbutton_Click(object sender, RoutedEventArgs e)
        {
            ui_busys.IsBusy = true;
            userSearch.Search();
            userList.WebClientInfo = Application.Current.Resources["dbclient"] as WebClientInfo;
            userList.LoadOnPathChanged = false;
            string sql = "select f_userinfoid,f_userid,f_meternumber,f_username,f_usertype,f_gaswatchbrand,f_metertype," +
                "lastinputgasnum,f_phone,f_gasproperties,f_stairtype,f_yytdepa," +
                "f_metergasnums,f_metertitles,f_gasmeterstyle,f_gasmeteraccomodations," +
                "f_userstate,f_finallybought,f_weizhi,f_cumulativepurchase,f_filiale,f_yytoper," +
                "CONVERT(varchar(12), f_finabuygasdate, 111 ) f_finabuygasdate," +
                "CONVERT(varchar(12), f_yytdate, 111 ) f_yytdate,f_gasmetermanufacturers," +
                "f_aliasname from t_userfiles " +
                "where " + userSearch.Condition + " order by id";
            userList.LoadOnPathChanged = false;
            userList.Path = "sql";
            userList.SumHQL = "select f_userinfoid,f_userid,f_meternumber,f_username,f_usertype,f_gaswatchbrand,f_metertype," +
                "lastinputgasnum,f_phone,f_gasproperties,f_stairtype,f_filiale,f_yytdepa," +
                "f_metergasnums,f_metertitles,f_gasmeterstyle,f_gasmeteraccomodations," +
                "f_userstate,f_finallybought,f_weizhi,f_cumulativepurchase,f_yytoper," +
                "CONVERT(varchar(12), f_finabuygasdate, 111 ) f_finabuygasdate,f_anzhuanguser,"+
                "CONVERT(varchar(12), f_yytdate, 111 ) f_yytdate,f_gasmetermanufacturers,"+
                "f_aliasname from t_userfiles " +
                "where "+userSearch.Condition+"";
            userList.HQL = sql;
            userList.PageSize = pager2.PageSize;
            userList.SumNames = ",";
            userList.DataLoaded += setList_DataLoaded;
            userList.PageIndex = pageIndex;
            userList.Load();
        }

        private void setList_DataLoaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if(userList.Size!=0){
                double zhye = 0;
                foreach(GeneralObject go in userList){
                    if (go.GetPropertyValue("f_zhye") == null) {
                        go.SetPropertyValue("f_zhye",0.0,false);
                       }
                    zhye += Double.Parse(go.GetPropertyValue("f_zhye").ToString());
                }
                ui_sumzhye.Text = "用户余额："+zhye.ToString();
            }
            ui_busys.IsBusy = false;
        }
        private void ui_pager_PageIndexChanged(object sender, EventArgs e)
        {
            userList.PageIndex = pager2.PageIndex;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            detail.Visibility = Visibility.Visible;
        }
	}
}