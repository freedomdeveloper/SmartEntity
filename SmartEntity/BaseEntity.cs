#region About Me
/*
 *Anthor: zxd 
 *Email: sher-lock@qq.com 
 *Last modify time: 2013-10-14
 */
#endregion
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;

namespace SmartEntity
{
    public class BaseEntity<T> where T: new()
    {
        #region static property

        private static Type _Type;

        private static PropertyInfo[] _PiArray;

        private static string _TableName;

        private static Dictionary<string, int> _ColumnDict = new Dictionary<string, int>();

        private static Dictionary<string, int> _PrimaryKeyDict = new Dictionary<string, int>();

        private static Dictionary<string, int> _ForeignKeyDict = new Dictionary<string, int>();

        #endregion

        #region static constructed function

        static BaseEntity()
        {
            //Get Type
            _Type = typeof(T);

            //Get PropertyInfo[]
            _PiArray = _Type.GetProperties();

            //Get TableName
            TableAttribute tableAttr = Attribute.GetCustomAttribute(_Type, typeof(TableAttribute)) as TableAttribute;
            if (tableAttr == null) 
                throw new Exception("You have to add TableAttribute to current Entity.");
            else
                _TableName = tableAttr.Name;

            //Get Dictionary
            for (int i = 0; i < _PiArray.Length; i++)
            {
                ColumnAttribute columnAttr = Attribute.GetCustomAttribute(_PiArray[i], typeof(ColumnAttribute)) as ColumnAttribute;
                if (columnAttr != null)
                    _ColumnDict.Add(columnAttr.Name, i);

                PrimaryKeyAttribute pkAttr = Attribute.GetCustomAttribute(_PiArray[i], typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;
                if (pkAttr != null)
                    _PrimaryKeyDict.Add(pkAttr.Name, i);

                ForeignKeyAttribute fkAttr = Attribute.GetCustomAttribute(_PiArray[i], typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;
                if (fkAttr != null)
                    _ForeignKeyDict.Add(fkAttr.Name, i);
            }

            if (_PrimaryKeyDict.Count == 0)
                throw new Exception("Please check out your primary key setting.");
        }

        #endregion

        #region private function
        
        /// <summary>
        /// Create PrimaryKey WHERE part string
        /// </summary>
        /// <returns></returns>
        private string BulidPrimaryKeyParameterStr()
        {
            string result = string.Empty;
            StringBuilder pkParameterStr = new StringBuilder();
            foreach(string pk in _PrimaryKeyDict.Keys)
                pkParameterStr.Append(string.Format("{0}=@{0} AND",pk));
            result = pkParameterStr.ToString();
            return result.Substring(0,result.Length - 3);
        }

        /// <summary>
        /// Add Primary key to parameters
        /// </summary>
        /// <param name="parameters"></param>
        private void AddPrimaryKeyParameter(List<SqlParameter> parameters)
        {
            foreach (var pk in _PrimaryKeyDict)
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = string.Format("@{0}", pk.Key),
                    Value = _PiArray[pk.Value].GetValue(this, null),
                };
                parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Add Foreign key to parameters
        /// </summary>
        /// <param name="parameters"></param>
        private void AddForeignKeyParameter(List<SqlParameter> parameters)
        {
            foreach (var fk in _ForeignKeyDict)
            {
                object o = _PiArray[fk.Value].GetValue(this, null);
                Type t = o.GetType();
                PropertyInfo[] piArray = t.GetProperties();
                int pkIndex = GetPrimaryKeyAttritubeIndex(piArray);

                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = fk.Key,
                    Value = piArray[pkIndex].GetValue(o,null),
                };
                parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Add column to parameters
        /// </summary>
        /// <param name="parameters"></param>
        private void AddColumnParameter(List<SqlParameter> parameters)
        {
            foreach (var column in _ColumnDict)
            {
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = column.Key,
                    Value = _PiArray[column.Value].GetValue(this,null),
                };
                parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Get the primary key of the foreign key object
        /// </summary>
        /// <param name="piArray"></param>
        /// <returns></returns>
        private int GetPrimaryKeyAttritubeIndex(PropertyInfo[] piArray)
        {
            for (int i = 0; i < piArray.Length; i++)
            {
                PrimaryKeyAttribute pkAttr = Attribute.GetCustomAttribute(piArray[i], typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;
                if (pkAttr != null)
                    return i;
            }
            return -1;
        }

        #endregion

        #region data operation API

        /// <summary>
        /// Get exists primary key count
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            int result;
            string sqlStr = string.Format(@"SELECT COUNT(*)
                                              FROM {0}
                                             WHERE {1}",_TableName,BulidPrimaryKeyParameterStr());
            List<SqlParameter> parameters = new List<SqlParameter>();
            AddPrimaryKeyParameter(parameters);
            result = SqlServerHelper.GetSingle(sqlStr, parameters);
            return result;
        }

        /// <summary>
        /// Delete by primary key
        /// </summary>
        public void Delete()
        {
            string sqlStr = string.Format(@"DELETE
                                              FROM {0}
                                             WHERE {1}", _TableName, BulidPrimaryKeyParameterStr());
            List<SqlParameter> parameters = new List<SqlParameter>();
            AddPrimaryKeyParameter(parameters);
            SqlServerHelper.ExcuteSql(sqlStr, parameters);
        }

        /// <summary>
        /// Get real data by primary key
        /// </summary>
        /// <returns></returns>
        public bool Retrieve()
        {
            //Retrieve success sign
            bool status = false;
            string sqlStr = string.Format(@"SELECT *
                                              FROM {0}
                                             WHERE {1}", _TableName, BulidPrimaryKeyParameterStr());
            List<SqlParameter> parameters = new List<SqlParameter>();
            AddPrimaryKeyParameter(parameters);
            using (SqlDataReader dr = SqlServerHelper.ExcuteReader(sqlStr, parameters))
            {
                if (dr.HasRows)
                {
                    status = true; 
                    while (dr.Read())
                    {
                        foreach (var column in _ColumnDict)
                        {
                            if(dr[column.Key] != DBNull.Value)
                                _PiArray[column.Value].SetValue(this, dr[column.Key], null);
                        }
                        foreach (var fk in _ForeignKeyDict)
                        {
                            object o = _PiArray[fk.Value].GetValue(this, null);
                            Type t = o.GetType();
                            PropertyInfo[] piArray = t.GetProperties();
                            int pkIndex = GetPrimaryKeyAttritubeIndex(piArray);
                            piArray[pkIndex].SetValue(o, dr[fk.Key], null);
                        }
                    }
                }
            }
            return status;
        }

        /// <summary>
        /// Add data 
        /// </summary>
        public void Add()
        {
            StringBuilder columnStrBulieder = new StringBuilder();
            StringBuilder parameterStrBulider = new StringBuilder();
            string columnStr;
            string parameterStr;

            foreach (string fk in _ForeignKeyDict.Keys)
            {
                columnStrBulieder.Append(string.Format("{0},", fk));
                parameterStrBulider.Append(string.Format("@{0},",fk));
            }
            foreach (string column in _ColumnDict.Keys)
            {
                columnStrBulieder.Append(string.Format("{0},", column));
                parameterStrBulider.Append(string.Format("@{0},", column));           
            }

            columnStr = columnStrBulieder.ToString(0, columnStrBulieder.Length - 1);
            parameterStr = parameterStrBulider.ToString(0, parameterStrBulider.Length - 1);

            string sqlStr = string.Format(@"INSERT INTO {0}
                                            ({1})
                                            VALUES
                                            ({2})", _TableName, columnStr, parameterStr);
            List<SqlParameter> parameters = new List<SqlParameter>();
            AddForeignKeyParameter(parameters);
            AddColumnParameter(parameters);
            SqlServerHelper.ExcuteSql(sqlStr, parameters);
        }

        /// <summary>
        /// Update data by primary key
        /// </summary>
        public void Update()
        {
            StringBuilder parameterStrBulider = new StringBuilder();
            string parameterStr;

            foreach (string column in _ColumnDict.Keys)
                parameterStrBulider.Append(string.Format("{0}=@{0},", column));
            foreach (string fk in _ForeignKeyDict.Keys)
                parameterStrBulider.Append(string.Format("{0}=@{0},", fk));

            parameterStr = parameterStrBulider.ToString(0, parameterStrBulider.Length - 1);

            string sqlStr = string.Format(@"UPDATE {0}
                                               SET {1}
                                             WHERE {2}", _TableName, parameterStr, BulidPrimaryKeyParameterStr());
            List<SqlParameter> parameters = new List<SqlParameter>();
            AddPrimaryKeyParameter(parameters);
            AddForeignKeyParameter(parameters);
            AddColumnParameter(parameters);
            SqlServerHelper.ExcuteSql(sqlStr, parameters);
        }

        #endregion
    }
}
