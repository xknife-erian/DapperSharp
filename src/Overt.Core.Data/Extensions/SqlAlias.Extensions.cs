using Overt.Core.Data.Expressions;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Overt.Core.Data
{
    /// <summary>
    /// Sql别名
    /// </summary>
    public static class SqlAliasExtensions
    {
        private static ConcurrentDictionary<string, string> ColumnMap = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 参数前缀
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string ParamPrefix(this DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.GteSqlServer2012:
                    return "@";
                case DatabaseType.MySql:
                    return "?";
                case DatabaseType.SQLite:
                    return "@";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 获取添加左右标记 防止有关键字作为字段名/表名
        /// </summary>
        /// <param name="column"></param>
        /// <param name="sqlGenerate"></param>
        /// <returns></returns>
        public static string ParamSql(this string column, SqlGenerate sqlGenerate)
        {
            return column.ParamSql(sqlGenerate?.DatabaseType, sqlGenerate?.EntityType);
        }

        /// <summary>
        /// 获取添加左右标记 防止有关键字作为字段名/表名
        /// </summary>
        /// <param name="column"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string ParamSql<TEntity>(this string column, DatabaseType? dbType)
        {
            return column.ParamSql(dbType, typeof(TEntity));
        }

        /// <summary>
        /// 获取添加左右标记 防止有关键字作为字段名/表名
        /// </summary>
        /// <param name="column"></param>
        /// <param name="dbType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ParamSql(this string column, DatabaseType? dbType, Type type)
        {
            var key = $"{type?.FullName}_{column}";
            column = ColumnMap.GetOrAdd(key, k =>
            {
                var pis = type.GetProperties();
                var pi = pis.FirstOrDefault(oo => oo.Name == column);
                if (pi == null)
                    return column;

                var attribute = pi.GetAttribute<ColumnAttribute>();
                if (attribute == null || string.IsNullOrEmpty(attribute.Name))
                    return column;

                return attribute.Name;
            });
            return column.ParamSql(dbType);
        }

        /// <summary>
        /// 获取添加左右标记 防止有关键字作为字段名/表名
        /// </summary>
        /// <param name="columnOrTable"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string ParamSql(this string columnOrTable, DatabaseType? dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.GteSqlServer2012:
                    if (columnOrTable.StartsWith("["))
                        return columnOrTable;
                    return $"[{columnOrTable}]";
                case DatabaseType.MySql:
                    if (columnOrTable.StartsWith("`"))
                        return columnOrTable;
                    return $"`{columnOrTable}`";
                case DatabaseType.SQLite:
                    if (columnOrTable.StartsWith("`"))
                        return columnOrTable;
                    return $"`{columnOrTable}`";
                default:
                    return columnOrTable;
            }
        }

        /// <summary>
        /// 获取最后一次Insert
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string SelectLastIdentity(this DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.GteSqlServer2012:
                    return " select @@Identity";
                case DatabaseType.MySql:
                    return " select LAST_INSERT_ID();";
                case DatabaseType.SQLite:
                    return " select last_insert_rowid();";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 是否存在表
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string ExistTableSql(this DatabaseType dbType, string dbName, string tableName)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.GteSqlServer2012:
                    return $"select count(1) from sys.tables where name='{tableName}' and type = 'u'";
                case DatabaseType.MySql:
                    return $"select count(1) from information_schema.tables where table_schema = '{dbName}' and table_name = '{tableName}'";
                case DatabaseType.SQLite:
                    return $"select count(1) from sqlite_master where type = 'table' and name='{tableName}'";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// 是否存在字段
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string ExistFieldSql(this DatabaseType dbType, string dbName, string tableName, string fieldName)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.GteSqlServer2012:
                    return $"select count(1) sys.columns where object_id = object_id('{tableName}') and name='{fieldName}'";
                case DatabaseType.MySql:
                    return $"select count(1) from information_schema.columns where table_schema = '{dbName}' and table_name  = '{tableName}' and column_name = '{fieldName}'";
                case DatabaseType.SQLite:
                    return $"select * from sqlite_master where name='{tableName}' and sql like '%{fieldName}%';";
                default: return string.Empty;
            }
        }
    }
}
