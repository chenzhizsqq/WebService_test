using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mtrrecv.common
{
    internal static class ConfigurationUtil
    {
        public static string GetValue(string strKey)
        {
            return ConfigurationManager.AppSettings[strKey].ToString();
        }

        public static readonly string CONNSTR = ConfigurationManager.AppSettings["SqlCNNString"].ToString();

        /// <summary>
        /// 取指定Tag与属性的值
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string GetSingleTagSection(string sectionName, string attributeName)
        {
            Hashtable section = ConfigurationUtil.GetSection<Hashtable>(sectionName);
            if (section != null && section[attributeName] != null)
            {
                return (string)section[attributeName];
            }
            return String.Empty;
        }

        /// <summary>   
        /// 取指定Tag的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static T GetSection<T>(string sectionName)
        {
            object section = ConfigurationManager.GetSection(sectionName);
            if (section != null && (section is T))
            {
                return (T)((object)section);

            }
            return (T)((object)null);


        }


    }
}
