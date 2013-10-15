using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;

namespace SmartEntity
{
    internal static class SqlServerHelper
    {
        private static readonly string CONNECTION_STRING = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        
        /// <summary>
        /// Add parameters to cmd
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        private static void PreParameters(SqlCommand cmd, List<SqlParameter> parameters)
        {
            if (parameters.Count > 0)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if ((parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput) && parameter.Value == null)
                        parameter.Value = DBNull.Value;
                    else if (parameter.Value == null || parameter.Value.ToString() == string.Empty)
                        parameter.Value = DBNull.Value;
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        
        /// <summary>
        /// ExcuteReader
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static SqlDataReader ExcuteReader(string sqlStr, List<SqlParameter> parameters)
        {
            SqlConnection connection = new SqlConnection(CONNECTION_STRING);
            SqlCommand cmd = new SqlCommand(sqlStr, connection);
            SqlDataReader dr;
            try
            {
                connection.Open();
                PreParameters(cmd, parameters);
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                connection.Close();
                throw new Exception(ex.Message);
            }
            return dr;
        }
        
        /// <summary>
        /// ExcuteSql
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExcuteSql(string sqlStr, List<SqlParameter> parameters)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, connection))
                {
                    try
                    {
                        connection.Open();
                        PreParameters(cmd, parameters);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
        
        /// <summary>
        /// GetSingle
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int GetSingle(string sqlStr, List<SqlParameter> parameters)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, connection))
                {
                    try
                    {
                        connection.Open();
                        PreParameters(cmd, parameters);
                        int rows;
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if (obj.Equals(null) || obj.Equals(DBNull.Value))
                            rows = 0;
                        else
                            rows = Convert.ToInt32(obj);
                        return rows;
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
    }
}
