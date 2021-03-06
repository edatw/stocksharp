﻿namespace StockSharp.Algo.Export.Database.DbProviders
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Ecng.Common;
	using Ecng.Xaml.Database;

	internal class SQLiteDbProvider : BaseDbProvider
	{
		public SQLiteDbProvider(DatabaseConnectionPair connection)
			: base(connection)
		{
		}

		public override void InsertBatch(Table table, IEnumerable<IDictionary<string, object>> parameters)
		{
			using (var trans = Database.BeginBatch())
			{
				using (var connection = Database.CreateConnection())
				{
					foreach (var value in parameters)
					{
						using (var command = CreateCommand(connection, CreateInsertSqlString(table, value), value.ToDictionary(par => "@" + par.Key, par => par.Value)))
							command.ExecuteNonQuery();
					}
				}

				trans.Commit();
			}
		}

		private static string CreateInsertSqlString(Table table, IDictionary<string, object> parameters)
		{
			var sb = new StringBuilder();
			sb.Append("INSERT OR IGNORE INTO ");
			sb.Append(table.Name);
			sb.Append(" (");
			foreach (var par in parameters)
			{
				sb.Append("[");
				sb.Append(par.Key);
				sb.Append("],");
			}
			sb.Remove(sb.Length - 1, 1);
			sb.Append(") VALUES (");
			foreach (var par in parameters)
			{
				sb.Append("@");
				sb.Append(par.Key);
				sb.Append(",");
			}
			sb.Remove(sb.Length - 1, 1);
			sb.AppendLine(")");
			return sb.ToString();
		}

		protected override string CreatePrimaryKeyString(IEnumerable<ColumnDescription> columns)
		{
			var str = columns.Select(c => "[{0}]".Put(c.Name)).Join(",");

			if (str.IsEmpty())
				return null;

			return "UNIQUE (" + str + ")";
		}

		protected override string CreateIsTableExistsString(Table table)
		{
			return "SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'".Put(table.Name);
		}

		protected override string GetDbType(Type t, object restriction)
		{
			//anothar:SQLite не поддерживает datetime2, а только datetime то есть округляет до трех знаков в миллисекундах-нам не подходит.
			if (t == typeof(DateTime))
				return "TEXT";
			if (t == typeof(DateTimeOffset))
				return "TEXT";
			if (t == typeof(bool))
				return "boolean";

			return base.GetDbType(t, restriction);
		}
	}
}