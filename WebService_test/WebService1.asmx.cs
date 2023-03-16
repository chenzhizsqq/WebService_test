using mtrrecv.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Common;
using Common.Entity;
using Dac;
using DacBase;
using BizLogic;
using System.Data;
using Newtonsoft.Json;
using System.Web.UI;
using JsonUtils;

namespace WebService_test
{
    public class User
    {
        public string userID { get; set; }
        public string Age { get; set; }
    }
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // 写入数据库
        //public void insertIDInfo(userEntity entity, string transType, string dealType)
         public void insertIDInfo(string json)
        {

            jsonEntity jsonEntity = new jsonEntity();
            jsonEntity = JsonConvert.DeserializeObject<jsonEntity>("json");

            userEntity entity = new userEntity();
            entity.userID = jsonEntity.userID;
            entity.password = jsonEntity.password;
            string transType = jsonEntity.transType;
            string dealType = jsonEntity.dealType;


            try
            {

                Logger.GetLogger().Publish("データベースに書き込み開http://localhost:51052/WebService1.asmx.cs始");
                Process_Bl process_Bl = new Process_Bl();
                string retmsg = process_Bl.DataInteraction(entity, transType, dealType);
                Logger.GetLogger().Publish(retmsg + "データ更新終了：user_id=" + entity.userID
                                                                + "| password=" + entity.password);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().Publish("データベースに書き込みエラー" + ex.Message.ToString());
                throw ex;
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SelectInfo(string json)
        {
          

            userEntity user = new userEntity();
            user = JsonConvert.DeserializeObject<userEntity>(json);

            userEntity entity = new userEntity();
            entity.userID = user.userID;
            string transType = user.transType;
            string dealType = user.dealType;

            string password;
            try
            {
                Logger.GetLogger().Publish("データベースに書き込み開http://localhost:51052/WebService1.asmx.cs始");
                Process_Bl process_Bl = new Process_Bl();
                DataSet userData = process_Bl.DataSelect(entity, transType, dealType);
                password =  userData.Tables[0].Rows[0][0].ToString();
                Logger.GetLogger().Publish("データ更新終了：user_id=" + entity.userID
                                                                + "| password=" + entity.password);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().Publish("データベースに書き込みエラー" + ex.Message.ToString());
                throw ex;
            }

            entity.password = user.password;

            string pas = JsonConvert.SerializeObject(entity);

            return pas;

        }




    }
}
