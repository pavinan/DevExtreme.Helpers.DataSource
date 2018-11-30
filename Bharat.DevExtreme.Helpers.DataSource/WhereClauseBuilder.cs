﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bharat.DevExtreme.Helpers.DataSource
{
    public class WhereClauseBuilder
    {
        private ISQLColumnProvider columnProvider;

        public WhereClauseBuilder()
        {
            this.columnProvider = new DefaultSQLColumnProvider();
        }

        public WhereClauseBuilder(ISQLColumnProvider sqlColumnProvider)
        {
            this.columnProvider = sqlColumnProvider;
        }

        public static WhereClauseBuilder CreateDefault()
        {
            return new WhereClauseBuilder();
        }

        public string BuildFor(JArray jsonArray)
        {
            var sb = new StringBuilder();

            this.ParseJArrayConditions(jsonArray, sb);

            string query = sb.ToString();

            return query;
        }

        private void ParseJArrayConditions(JArray jsonArray, StringBuilder query)
        {
            if (jsonArray.Count == 2)
            {
                // [["col1", "=", 45], ["col2", "=", true]]
                if (jsonArray[0].Type == JTokenType.Array)
                {
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        JArray jsonItem = jsonArray[i] as JArray;

                        this.ParseJArrayConditions(jsonItem, query);

                        if (i + 1 != jsonArray.Count)
                        {
                            query.Append(" and ");
                        }

                    }

                }
                else if (jsonArray[0].Type == JTokenType.String && jsonArray[0].ToString() == "!")
                {
                    // ["!", ["col", "=", true]]
                    // or 
                    //["!", [["col", "=", true], "or", ["col", "=", true]]]
                    JArray jArray = jsonArray[1] as JArray;

                    NegateJsonArray(jArray);

                    this.ParseJArrayConditions(jArray, query);
                }
                else
                {
                    var ex = new Exception("Unknown 2d array");

                    ex.Data.Add("2djarray", jsonArray);
                    throw ex;
                }
            }
            else if (jsonArray[0].Type == JTokenType.String)
            {
                //// ["col", "condition", "value"]

                var columnName = jsonArray[0].ToString();
                var condition = jsonArray[1].ToString();
                var value = jsonArray[2].ToString();

                var convertedValue = this.columnProvider.GetDBValueConversion(columnName, condition, value);

                var databaseColumnName = this.columnProvider.GetDBColumnName(columnName);

                if (condition == "contains")
                {
                    query.Append($" {databaseColumnName} like {convertedValue} ");
                }
                else if (condition == "not contains")
                {
                    query.Append($" {databaseColumnName} not like {convertedValue} ");
                }
                else
                {
                    ThrowIfUnknownCondition(condition);
                    query.Append($" {databaseColumnName} {condition} {convertedValue} ");
                }
            }
            else if (jsonArray[0].Type == JTokenType.Array)
            {
                query.Append(" ( ");

                foreach (JToken jsonItem in jsonArray)
                {
                    if (jsonItem.Type == JTokenType.Array)
                    {
                        this.ParseJArrayConditions((JArray)jsonItem, query);
                    }
                    else if (jsonItem.Type == JTokenType.String)
                    {
                        query.Append($" {jsonItem} ");
                    }
                }

                query.Append(" ) ");
            }
            else
            {
                throw new Exception($"Unknown data type in json array.");
            }
        }

        private void NegateJsonArray(JArray jarray)
        {
            if (jarray[0].Type == JTokenType.String)
            {
                var negatedValue = NegateCondition(jarray[1].Value<string>());
                jarray[1].Replace(negatedValue);
            }
            else
            {
                for (int i = 0; i < jarray.Count; i++)
                {
                    JToken jToken = jarray[i];

                    if (jToken.Type == JTokenType.Array)
                    {
                        NegateJsonArray(jToken as JArray);
                    }
                    else
                    {
                        var negatedValue = NegateCondition(jToken.Value<string>());
                        jToken.Replace(negatedValue);
                    }

                }
            }
        }

        private string NegateCondition(string condition)
        {
            switch (condition)
            {
                case "contains":
                    return "not contains";
                case "=":
                    return "!=";
                case "<":
                    return ">";
                case ">":
                    return "<";
                case "<=":
                    return ">=";
                case ">=":
                    return "<=";
                case "and":
                    return "or";
                case "or":
                    return "and";
                default:
                    throw new Exception($"Unknown condition {condition}");
            }
        }

        private void ThrowIfUnknownCondition(string condition)
        {
            switch (condition)
            {
                case "=":
                case "!=":
                case "<":
                case ">":
                case "<=":
                case ">=":
                    break;
                default:
                    throw new Exception($"Unknown condition {condition}");
            }
        }

    }
}
