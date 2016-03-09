package com.aote.rs;

import java.util.List;

import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.PUT;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.WebApplicationException;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.CoreConnectionPNames;
import org.apache.http.util.EntityUtils;
import org.apache.log4j.Logger;
import org.codehaus.jettison.json.JSONArray;
import org.codehaus.jettison.json.JSONException;
import org.codehaus.jettison.json.JSONObject;
import org.hibernate.HibernateException;
import org.hibernate.SQLQuery;
import org.hibernate.Session;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.orm.hibernate3.HibernateCallback;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.stereotype.Component;


@Path("reiesgas")
@Component
public class iesGas_return {
	static Logger log = Logger.getLogger(IndoorInspectService.class);

	@Autowired
	private HibernateTemplate hibernateTemplate;
	
	String hostc="http://60.208.16.56:61620/ws/rs/";

	/**
	 * execute sql in hibernate
	 * @param sql
	 */
	private void execSQL(final String sql) {
        hibernateTemplate.execute(new HibernateCallback() {
            public Object doInHibernate(Session session)
                    throws HibernateException {
                session.createSQLQuery(sql).executeUpdate();
                return null;
            }
        });		
	}
	
	/**
	 * execute sql in hibernate
	 * @param sql 返回影响的条数
	 */
	private Object execSQLnum(final String sql) {
		return hibernateTemplate.execute(new HibernateCallback() {
			 public Object doInHibernate(Session session)
	                    throws HibernateException {
				 return session.createSQLQuery(sql).executeUpdate();
	            }
        });
	}
	
	// syncFile
	@Path("syncFile")
	@POST
	@Produces("application/json")
	public String syncFile(String Obj){
		log.debug("抄表系统上传表具状态：" + Obj);
		try {
			JSONArray rows=new JSONArray(Obj);
			JSONArray rerows=new JSONArray();
			for(int i=0;i<rows.length();i++){
				JSONObject row = rows.getJSONObject(i);
				String f_userid = row.getString("customer_code");
				String type = row.getString("type");
				rerows.put(new JSONObject("{\"type\":\""+type+"\",\"customer_code\":\""+f_userid+"\",\"returnvalue\":\"0\"}"));
			}	
			return rerows.toString();
		} catch (JSONException e) {
			return "[]";
		}
	}
	
	// customerStateChange
		@Path("customerStateChange")
		@POST
		@Produces("application/json")
		public String customerStateChange(String Obj){
			log.debug("抄表系统上传表具状态：" + Obj);
			try {
				JSONArray rows=new JSONArray(Obj);
				JSONArray rerows=new JSONArray();
				for(int i=0;i<rows.length();i++){
					JSONObject row = rows.getJSONObject(i);
					String f_userid = row.getString("customer_code");
					String type = row.getString("type");
					rerows.put(new JSONObject("{\"type\":\""+type+"\",\"customer_code\":\""+f_userid+"\",\"returnvalue\":\"0\"}"));
				}	
				return rerows.toString();
			} catch (JSONException e) {
				return "[]";
			}
		}
		
		// chargesMeter
				@Path("chargesMeter")
				@POST
				@Produces("application/json")
				public String chargesMeter(String Obj){
					log.debug("抄表系统上传表具状态：" + Obj);
					try {
						JSONArray rows=new JSONArray(Obj);
						JSONArray rerows=new JSONArray();
						for(int i=0;i<rows.length();i++){
							JSONObject row = rows.getJSONObject(i);
							String f_userid = row.getString("customer_code");
							String mode = row.getString("mode");
							rerows.put(new JSONObject("{\"mode\":\""+mode+"\",\"customer_code\":\""+f_userid+"\",\"returnvalue\":\"0\"}"));
						}	
						return rerows.toString();
					} catch (JSONException e) {
						return "[]";
					}
				}

				// priceChange
				@Path("priceChange")
				@POST
				@Produces("application/json")
				public String priceChange(String Obj){
					log.debug("抄表系统上传表具状态：" + Obj);
					try {
						JSONArray rows=new JSONArray(Obj);
						JSONArray rerows=new JSONArray();
						for(int i=0;i<rows.length();i++){
							JSONObject row = rows.getJSONObject(i);
							String f_userid = row.getString("f_stairtype");
							String f_stairtype = row.getString("f_stairtype");
							rerows.put(new JSONObject("{\"f_stairtype\":\""+f_stairtype+"\",\"customer_code\":\""+f_userid+"\",\"returnvalue\":\"0\"}"));
						}	
						return rerows.toString();
					} catch (JSONException e) {
						return "[]";
					}
				}
				
				// valveControl
				@Path("valveControl")
				@POST
				@Produces("application/json")
				public String valveControl(String Obj){
					log.debug("抄表系统上传表具状态：" + Obj);
					try {
						JSONArray rows=new JSONArray(Obj);
						JSONArray rerows=new JSONArray();
						for(int i=0;i<rows.length();i++){
							JSONObject row = rows.getJSONObject(i);
							String f_userid = row.getString("customer_code");
							String type = row.getString("type");
							rerows.put(new JSONObject("{\"type\":\""+type+"\",\"customer_code\":\""+f_userid+"\",\"returnvalue\":\"0\"}"));
						}	
						return rerows.toString();
					} catch (JSONException e) {
						return "[]";
					}
				}
}
