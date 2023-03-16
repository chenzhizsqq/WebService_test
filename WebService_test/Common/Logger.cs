using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mtrrecv.common
{
    /// <summary>
    /// 日志输出工具类
    /// </summary>
    public class Logger
    {
        private static Logger instance;
        private string _path;
        private string _outputFlag;
        private string _fileNamePrefix;

        /// <summary>
        /// 取得Logger对象
        /// </summary>
        /// <returns></returns>
        public static Logger GetLogger()
        {
            if (instance == null)
            {
                instance = new Logger();
            }
            return instance;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Logger()
        {
            this._path = ConfigurationUtil.GetSingleTagSection("logSettings", "Path");
            this._fileNamePrefix = ConfigurationUtil.GetSingleTagSection("logSettings", "FileNamePrefix");
            this._outputFlag = ConfigurationUtil.GetSingleTagSection("logSettings", "OutputFlag");

        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="logInfo"></param>
        /// <param name="args"></param>
        public void Publish(String logInfo, params string[] args)
        {
            WriteFile(this.Format(logInfo, args));
        }

        /// <summary>
        /// 输出日志（包含异常内容）
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="logInfo"></param>
        /// <param name="args"></param>
        public void Publish(Exception exception, String logInfo, params string[] args)
        {
            WriteFile(this.Format(exception, logInfo, args));
        }

        /// <summary>
        /// 追加日志内容
        /// </summary>
        /// <param name="content"></param>
        protected void WriteFile(String content)
        {
            if ("1".Equals(this._outputFlag))
            {
                if (!System.IO.Directory.Exists(this._path))
                {
                    // 建立目录
                    System.IO.Directory.CreateDirectory(this._path);
                }
                String logFile = String.Concat(this._path, this._fileNamePrefix, ".log");
                System.IO.File.AppendAllLines(logFile, new String[] { content }, Encoding.UTF8);

            }
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="logInfo"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected string Format(String logInfo, params string[] args)
        {
            return this.Format(null, logInfo, args);
        }

        /// <summary>
        /// 格式化（Core）
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="logInfo"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string Format(Exception exception, String logInfo, params string[] args)
        {
            string value;
            if (args != null)
            {
                value = string.Format(null, logInfo, args);
            }
            else
            {
                value = logInfo;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", null));
            stringBuilder.Append(' ');
            stringBuilder.Append(value);
            if (exception != null)
            {
                this.SetExceptionToBuilder(stringBuilder, exception);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 输出Exception
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="ex"></param>
        private void SetExceptionToBuilder(StringBuilder sb, Exception ex)
        {
            sb.Append(Environment.NewLine);
            sb.Append(ex.GetType().FullName);
            sb.Append(Environment.NewLine);
            sb.Append(ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append(ex.Source);
            sb.Append(Environment.NewLine);
            sb.Append(ex.StackTrace);
            sb.Append(Environment.NewLine);
            sb.Append("-----");
            if (ex.InnerException != null)
            {
                this.SetExceptionToBuilder(sb, ex.InnerException);
            }
        }

        /// <summary>
        /// 删除log文件
        /// </summary>
        public void DeleteLogFile()
        {
            string sFile = this._path + this._fileNamePrefix + ".log";
            if (System.IO.File.Exists(sFile))
            {
                // 删除目录
                System.IO.File.Delete(sFile);
            }
        }
    }

}
