using System;
using System.Data.SqlClient;

namespace AuthDomain.Dal
{
    static class Helper
    {
        public static object ToSqlNullable<T>(T value)
        {
            if (value == null)
                return DBNull.Value;
            else
                return value;
        }

        public static T GetObjectOrNull<T>(this SqlDataReader sqlDataReader, string columnName) where T : class
        {
            var columnValue = sqlDataReader[columnName];
            return GetObjectOrNull<T>(columnValue);
        }

        public static T GetObjectOrNull<T>(object value) where T : class
        {
            return (value is DBNull) ? (T)null : (T)value;
        }

        public static Nullable<T> GetValueOrNull<T>(this SqlDataReader sqlDataReader, string columnName) where T : struct
        {
            var columnValue = sqlDataReader[columnName];
            return (columnValue is DBNull) ? (Nullable<T>)null : (T)columnValue;
        }
    }
}
