using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Const;

namespace DacBase
{
    public class NpgsqlDacBase : INpgsqlDacBase
    {
        //private readonly string _ConnectionString = CommConst.SQL_CNN_STRING;
        private const string C_CNN_STRING_KEY = "SqlCNNString";
        //private readonly string _ConnectionString = ConfigurationManager.AppSettings[C_CNN_STRING_KEY];
        private readonly string _ConnectionString = System.Configuration.ConfigurationManager.AppSettings[C_CNN_STRING_KEY].ToString();
        //Logアドレス
        //private string _logPath = CommConst.WEB_LOG_PATH;
        //private int _timeOut = CommConst.DB_TIME_OUT;
        //private string _logPath = ConfigurationManager.AppSettings["WebLogPath"].ToString();
        private int _timeOut = Converts.ToInt(ConfigurationManager.AppSettings["DbTimeOut"].ToString());

        private NpgsqlConnection _Connection;
        private NpgsqlCommand _Command;
        private NpgsqlDataAdapter _Adapter;
        private NpgsqlTransaction _Trans;

        /// <summary>
        /// ログファイル
        /// </summary>
        private string path = ConfigurationManager.AppSettings["sqlPath"].ToString();
        private bool isNeedLog = ConfigConst.ISNEEDLOG;

        /// <summary>
        /// Construct 関数
        /// </summary>
        /// <remarks></remarks>
        public NpgsqlDacBase()
        {
            _Connection = new NpgsqlConnection(_ConnectionString);
            _Command = new NpgsqlCommand();
            _Command.Connection = _Connection;

            _Command.CommandTimeout = _timeOut;
            _Adapter = new NpgsqlDataAdapter();
            _Adapter.SelectCommand = _Command;
            _Adapter.SelectCommand.CommandTimeout = _timeOut;
        }

        public void Dispose()
        {
            if (_Trans != null)
            {
                _Trans.Dispose();
                _Trans = null;
            }

            if (_Adapter != null)
            {
                _Adapter.Dispose();
                _Adapter = null;
            }

            if (_Command != null)
            {
                _Command.Dispose();
                _Command = null;
            }

            if (_Connection != null)
            {
                if (_Connection.State != ConnectionState.Closed)
                {
                    _Connection.Close();
                }
                _Connection.Dispose();
                _Connection = null;
            }
        }

        public NpgsqlDacBase(NpgsqlConnection con, NpgsqlCommand com, NpgsqlDataAdapter adp, NpgsqlTransaction trans)
        {
            _Connection = con;
            _Command = com;
            _Adapter = adp;
            _Trans = trans;
        }

        #region 属性取得
        protected NpgsqlConnection GetConnection
        {
            get
            {
                return _Connection;
            }
        }

        protected NpgsqlCommand GetCommand
        {
            get
            {
                return _Command;
            }
        }

        protected NpgsqlDataAdapter GetAdapter
        {
            get
            {
                return _Adapter;
            }
        }

        #endregion

        /// <summary>
        /// データベース事務を起動
        /// </summary>
        /// <remarks></remarks>
        public void BeginTrans()
        {
            if ((_Connection.State != ConnectionState.Open))
            {
                _Connection.Open();
            }
            _Trans = _Connection.BeginTransaction();
        }

        /// <summary>
        /// データベース事務Commit
        /// </summary>
        /// <remarks></remarks>
        public void CommitTrans()
        {
            if (_Trans != null)
            {
                _Trans.Commit();
                _Trans.Dispose();
                _Trans = null;
            }
        }

        /// <summary>
        /// 異常発生、事務中のデータが初期化状態に戻る
        /// </summary>
        /// <remarks></remarks>
        public void RollbackTrans()
        {
            if (_Trans != null)
            {
                _Trans.Rollback();
                _Trans.Dispose();
                _Trans = null;
            }
        }

        /// <summary>
        /// 事務
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>
        /// BLL層使用方法
        /// </remarks>
        public NpgsqlTransaction Transaction
        {
            get
            {
                return _Trans;
            }
            set
            {
                _Trans = value;
            }
        }

        public NpgsqlConnection Connection
        {
            get
            {
                return _Connection;
            }
        }

        public NpgsqlCommand Command
        {
            get
            {
                return _Command;
            }
        }

        public NpgsqlDataAdapter Adapter
        {
            get
            {
                return _Adapter;
            }
        }


        public void test(DataTable dt)
        {
            NpgsqlCommandBuilder cmb = new NpgsqlCommandBuilder(_Adapter);
            _Adapter.Update(dt);
        }

        /// <summary>
        /// SQL命令準備
        /// </summary>
        /// <param name="cmd">SQL命令</param>
        /// <param name="conn">SQL接続</param>
        /// <param name="trans">SQLの事務</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <param name="cmdText">SQL文</param>
        /// <param name="cmdParms">SQLパラメータ</param>
        /// <remarks></remarks>
        private void PrepareCommand(NpgsqlCommand cmd, NpgsqlConnection conn, NpgsqlTransaction trans, CommandType cmdType, string cmdText, NpgsqlParameter[] cmdParms)
        {
            try
            {
                if ((conn.State != ConnectionState.Open))
                {
                    conn.Open();
                }
                cmd.Connection = conn;

                System.IO.StreamWriter writer = null;

                if (isNeedLog)
                {
                    writer = new System.IO.StreamWriter(path, true);
                    writer.WriteLine(cmdText.Replace("  ", " "));
                    writer.WriteLine();
                }

                cmd.CommandText = cmdText;
                if ((trans != null))
                {
                    cmd.Connection = trans.Connection;
                    cmd.Transaction = trans;
                }
                cmd.CommandType = cmdType;

                cmd.Parameters.Clear();
                if (cmdParms != null)
                {
                    foreach (NpgsqlParameter parm in cmdParms)
                    {
                        if (parm == null) break;
                        cmd.Parameters.Add(parm);

                        if (isNeedLog)
                        {
                            writer.WriteLine(parm.ParameterName + "=>" + parm.Value);
                        }
                    }
                }

                if (isNeedLog)
                {
                    writer.WriteLine();
                    writer.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///データにデータ一致の場合、System.Data.DataSet.DataTableに指定範囲の行の内容を追加或は更新
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <param name="typedTable"></param>
        /// <remarks></remarks>
        public void Fill(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType, DataTable typedTable)
        {
            try
            {
                PrepareCommand(_Command, _Connection, _Trans, cmdType, strCmdText, sqlParameters);
                _Adapter.Fill(typedTable);
            }
            catch
            {
                throw;
            }
            finally
            {
                if ((_Trans == null) && (_Connection.State == ConnectionState.Open))
                {
                    _Connection.Close();
                    //_Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// データにデータ一致の場合、System.Data.DataSet.DataTableに指定範囲の行を取得
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <returns>System.Data.DataSet （正常にデータを取得）</returns>
        /// <remarks></remarks>
        public DataSet ExcuteQuery(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType)
        {
            DataSet _dataset = new DataSet();
            try
            {
                PrepareCommand(_Command, _Connection, _Trans, cmdType, strCmdText, sqlParameters);
                _Adapter.Fill(_dataset);
                return _dataset;
            }
            catch
            {
                throw;
            }
            finally
            {
                if ((_Trans == null) && (_Connection.State == ConnectionState.Open))
                {
                    _Connection.Close();
                    //_Connection.Dispose();
                }
            }
        }

        /// <summary>
        ///   データにデータ一致の場合、System.Data.DataSet.DataTableに指定範囲の行の内容を追加或は更新
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <returns>System.Data.DataSetが正常に追加又は更新された行。</returns>
        /// <remarks></remarks>
        public int ExecuteNonQuery(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType)
        {
            try
            {
                PrepareCommand(_Command, _Connection, _Trans, cmdType, strCmdText, sqlParameters);
                return _Command.ExecuteNonQuery();
            }
            catch
            {

                string strErr = string.Empty;
                if (sqlParameters != null)
                {
                    for (int i = 0; i < sqlParameters.Length; i++)
                    {
                        if (strErr == string.Empty)
                        {
                            strErr = sqlParameters[0].ToString() + "=" + sqlParameters[0].Value.ToString();
                        }
                        else
                        {
                            if (sqlParameters[i].Value != null)
                                strErr = strErr + "," + sqlParameters[i].ToString() + "=" + sqlParameters[i].Value.ToString();
                            else
                                strErr = strErr + "," + sqlParameters[i].ToString() + "= null";
                        }
                    }
                }
                //Log.WriteLog(strCmdText + strErr, CommConst.LOG_ERROR);
                throw;
            }
            finally
            {
                if ((_Trans == null) && (_Connection.State == ConnectionState.Open))
                {
                    _Connection.Close();
                    //_Connection.Dispose();
                }
            }
        }



        /// <summary>
        /// 検索実施、.NET Framework初期行列のデータタイプの結果に戻って、残りの行或は列を無視
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <returns>.NET Framework 固定のデータタイプの結果を最初の行と列を設定</returns>
        /// <remarks>結果は空の場合、結果のREF CURSOR の場合、null参照。</remarks>
        public object ExecuteScalar(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType)
        {
            try
            {
                PrepareCommand(_Command, _Connection, _Trans, cmdType, strCmdText, sqlParameters);
                return _Command.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                if ((_Trans == null) && (_Connection.State == ConnectionState.Open))
                {
                    _Connection.Close();
                    //_Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// 検索実施、.NET Frameworkデータタイプに戻る結果を取得
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <returns>System.Data.SQLClient.SQLDataReader obj類。</returns>
        /// <remarks></remarks>
        public NpgsqlDataReader ExecuteReader(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType)
        {
            try
            {
                PrepareCommand(_Command, _Connection, _Trans, cmdType, strCmdText, sqlParameters);
                return (NpgsqlDataReader)_Command.ExecuteReader();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 複数のSQL分を実行し、データベース事務を実装する。
        /// </summary>
        /// <param name="SQLStringList">SQL文のハッシュテーブル（keyはsql文、valueはこの文の SqlParameter[]）</param>
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            try
            {
                //サーキュレーション
                foreach (DictionaryEntry myDE in SQLStringList)
                {
                    string strCmdText = myDE.Key.ToString();
                    CommandType cmdType = CommandType.Text;
                    NpgsqlParameter[] sqlParameters = (NpgsqlParameter[])myDE.Value;
                    PrepareCommand(_Command, _Connection, _Trans, cmdType, strCmdText, sqlParameters);
                    _Command.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if ((_Trans == null) && (_Connection.State == ConnectionState.Open))
                {
                    _Connection.Close();
                    //_Connection.Dispose();
                }
            }
        }


        /// <summary>
        ///一括更新データ
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <param name="typedTable"></param>
        /// <remarks></remarks>
        public void BatchUpdate(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType, DataTable typedTable)
        {
            try
            {
                PrepareCommand(_Command, _Connection, _Trans, cmdType, strCmdText, sqlParameters);
                _Adapter.UpdateCommand = _Command;
                _Adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;
                _Adapter.UpdateBatchSize = 0;
                _Adapter.Update(typedTable);
            }
            catch
            {
                throw;
            }
            finally
            {
                if ((_Trans == null) && (_Connection.State == ConnectionState.Open))
                {
                    _Connection.Close();
                }
            }
        }

        /// <summary>
        ///一括データ挿入
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <param name="typedTable"></param>
        /// <remarks></remarks>
        public void BatchInsert(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType, DataTable typedTable)
        {
            try
            {
                PrepareCommand(_Command, _Connection, _Trans, cmdType, strCmdText, sqlParameters);
                _Adapter.InsertCommand = _Command;
                _Adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
                _Adapter.UpdateBatchSize = 1000;
                _Adapter.Update(typedTable);
            }
            catch
            {
                throw;
            }
            finally
            {
                if ((_Trans == null) && (_Connection.State == ConnectionState.Open))
                {
                    _Connection.Close();
                }
            }
        }
    }
}

