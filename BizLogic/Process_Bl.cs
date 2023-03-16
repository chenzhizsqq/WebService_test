using Common.Entity;
using Dac.Implement;
using System.Data;
using System.Runtime.Remoting.Messaging;


namespace BizLogic
{
    public class Process_Bl
    {
        Process_Imp process_Imp = new Process_Imp();

        public string DataInteraction(userEntity userntity, string transType, string dealType)
        {
            string retmsg = "";
            try
            {
                // Logger.GetLogger().Publish("データベースに書き込み開始");
                if (transType == "Process")
                {
                    if (dealType == "Insert")
                    {
                        int i = process_Imp.InsertProcess(userntity);
                        if (i > 0)
                        {
                            retmsg = "失敗";

                            //Logger.GetLogger().Publish("データ更新終了：tag_uid=" + userntity.userID
                            //                                        + "|cereal_no=" + userntity.password);
                        }
                        else
                        {
                            retmsg = "成功";
                        }

                    }
                    else if (dealType == "Update")
                    {
                        int i = process_Imp.UpdateProcess(userntity);
                        if (i > 0)
                        {
                            retmsg = "MM00005";
                        }
                        else
                        {
                            retmsg = "EM00416";
                        }
                    }
                    else if (dealType == "Delete")
                    {
                        int i = process_Imp.DeleteProcess(userntity);
                        if (i > 0)
                        {
                            retmsg = "MM00006";
                        }
                        else
                        {
                            retmsg = "EM00417";
                        }
                    }
                }
            }
            catch
            {

                //Logger.GetLogger().Publish("データベースに書き込みエラー" + ex.Message.ToString());
                if (dealType == "Insert")
                {
                    retmsg = "";
                }
                else if (dealType == "Update")
                {
                    retmsg = "";

                }
                else if (dealType == "Delete")
                {
                    retmsg = "";
                }
            }
            //Logger.GetLogger().Publish("データベースに書き込み終了");
            return retmsg;
        }


        public DataSet DataSelect(userEntity userntity, string transType, string dealType)
        {

            string retmsg = "";

            DataSet i = process_Imp.GetProductInfo(userntity);
                if (i.Tables.Count > 0)
                {
                    retmsg = "MM00006";
                }
                else
                {
                    retmsg = "EM00417";
                }
            return i;
        }

     }
}
