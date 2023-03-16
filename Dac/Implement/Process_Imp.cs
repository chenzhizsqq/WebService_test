using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Common.Entity;
using Dac.Interface;
using DacBase;

namespace Dac.Implement
{
    public class Process_Imp : NpgsqlDacBase, IProcess
    {
        /// <summary>
        /// 新規
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int InsertProcess(userEntity entity)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();
            stringBuilder.Append(@"
                        INSERT INTO m_user ( user_id, password, insert_time)
                        VALUES
	                        ( @user_id, @password, current_timestamp );
                                ");
            sqlParameters.Add(new NpgsqlParameter(@"user_id", entity.userID));
            sqlParameters.Add(new NpgsqlParameter(@"password", entity.password));
            return ExecuteNonQuery(stringBuilder.ToString(), sqlParameters.ToArray(), CommandType.Text);
        }

        public DataSet GetProductInfo(userEntity entity)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();
            stringBuilder.Append(@"
SELECT
	password
FROM
	m_user
WHERE
    user_id = @user_id
            ");
            sqlParameters.Add(new NpgsqlParameter(@"user_id", entity.userID));
            return ExcuteQuery(stringBuilder.ToString(), sqlParameters.ToArray(), CommandType.Text);
        }

        public int UpdateProcess(userEntity entity)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();
            stringBuilder.Append(@"
UPDATE m_user 
SET 
password = @process_name

WHERE
	user_id = @user_id
            ");
            sqlParameters.Add(new NpgsqlParameter(@"user_id", entity.userID));
            sqlParameters.Add(new NpgsqlParameter(@"password", entity.password));
            return ExecuteNonQuery(stringBuilder.ToString(), sqlParameters.ToArray(), CommandType.Text);
        }

        public int DeleteProcess(userEntity entity)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<NpgsqlParameter> sqlParameters = new List<NpgsqlParameter>();
            stringBuilder.Append(@"
DELETE m_user 
WHERE
	user_id = @user_id
            ");
            sqlParameters.Add(new NpgsqlParameter(@"user_id", entity.userID));
            return ExecuteNonQuery(stringBuilder.ToString(), sqlParameters.ToArray(), CommandType.Text);
        }
    }
}
