using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public class Converts
    {
        private const string C_KEY = @"H):j@p7>";

        #region Object⇒string
        /// <summary>
        //stringタイプに変換
        /// </summary>
        /// <param name="value">変換必要の値</param>
        /// <param name="Params">NULLの場合、戻り値</param>
        /// <returns>変換された値</returns>
        public static string ToStr(object value, params string[] Params)
        {
            if (value == null || value == DBNull.Value)
            {
                if (Params.Length == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return Params[0];
                }
            }

            return value.ToString().Trim();
        }
        #endregion
        /// <summary>
        /// 値を整形に変換し、DBNULLの場合、0に戻る
        /// </summary>
        /// <param name="value">変換必要の値</param>
        /// <param name="Params">デフォルト値</param>
        /// <returns>変換された値</returns>
        public static int ToInt(object value, params int[] Params)
        {
            try
            {
                if (value == null || value == DBNull.Value || !IsNumeric(value.ToString()))
                {
                    if (Params.Length == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return Params[0];
                    }
                }
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 日付検査
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDate(string strValue)
        {
            try
            {
                if (strValue.Length < 8)
                {
                    return false;
                }
                if (Convert.ToDateTime(strValue) == DateTime.MinValue)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 数値検査
        /// </summary>
        /// <param name="strValue">数値</param>
        /// <returns>検査結果</returns>
        public static bool IsNumeric(string strValue)
        {
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(strValue);
        }
    }
}
