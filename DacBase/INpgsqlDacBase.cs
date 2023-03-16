using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DacBase
{
    public interface INpgsqlDacBase
    {
        void Dispose();

        /// <summary>
        /// データベースの事務を起動
        /// </summary>
        /// <remarks></remarks>
        void BeginTrans();

        /// <summary>
        /// データベース事務Commit
        /// </summary>
        /// <remarks></remarks>
        void CommitTrans();

        /// <summary>
        /// 異常発生、事務中のデータが初期化状態に戻る
        /// </summary>
        /// <remarks></remarks>
        void RollbackTrans();

        void test(DataTable dt);

        /// <summary>
        ///データにデータ一致の場合、System.Data.DataSet.DataTableに指定範囲の行の内容を追加或は更新
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <param name="typedTable"></param>
        /// <remarks></remarks>
        void Fill(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType, DataTable typedTable);

        /// <summary>
        /// データにデータ一致の場合、System.Data.DataSet.DataTableに指定範囲の行を取得
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <returns>System.Data.DataSet （正常にデータを取得）</returns>
        /// <remarks></remarks>
        DataSet ExcuteQuery(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType);

        /// <summary>
        ///   データにデータ一致の場合、System.Data.DataSet.DataTableに指定範囲の行の内容を追加或は更新
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <returns>System.Data.DataSetが正常に追加或は更新された行。</returns>
        /// <remarks></remarks>
        int ExecuteNonQuery(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType);

        /// <summary>
        /// 検索実施、.NET Frameworkの初期行列のデータタイプの結果に戻って、残りの行または列を無視
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <returns>.NET Framework 固定のデータタイプ結果を最初の行と列に設定</returns>
        /// <remarks>結果が空の場合、結果のREF CURSORの場合、null参照。</remarks>
        object ExecuteScalar(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType);

        /// <summary>
        /// 検索実施、 .NET Frameworkデータタイプに戻る結果を取得
        /// </summary>
        /// <param name="strCmdText">SQL命令</param>
        /// <param name="sqlParameters">SQLパラメータ</param>
        /// <param name="cmdType">SQL命令種類</param>
        /// <returns>System.Data.SQLClient.SQLDataReader obj類。</returns>
        /// <remarks></remarks>
        NpgsqlDataReader ExecuteReader(string strCmdText, NpgsqlParameter[] sqlParameters, CommandType cmdType);

        /// <summary>
        /// 複数のSQL分を実行し、データベース事務を実装。
        /// </summary>
        /// <param name="SQLStringList">SQL分のハッシュテーブル（keyはsql分、valueはこの分のSqlParameter[]）</param>
        void ExecuteSqlTran(Hashtable SQLStringList);
    }
}
