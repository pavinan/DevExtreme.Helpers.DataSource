using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Bharat.DevExtreme.Helpers.DataSource.Tests
{
    [TestClass]
    public class WhereClauseBuilderTests
    {
        private WhereClauseBuilder Builder = new WhereClauseBuilder();

        [TestMethod]
        public void Test1()
        {
            var json = "[\"CustomerID\",\"=\",\"ALFKI\"]";

            ParseAndTest(json);
        }

        [TestMethod]
        public void Test2()
        {
            var json = "[[\"CustomerID\",\"=\",\"ALFKI\"],\"or\",[\"CustomerID\",\"=\",\"ANATR\"]]";

            ParseAndTest(json);
        }

        [TestMethod]
        public void Test3()
        {
            var json = "[\"!\",[[\"CustomerID\",\"=\",\"ALFKI\"],\"or\",[\"CustomerID\",\"=\",\"ANATR\"]]]";

            ParseAndTest(json);
        }

        [TestMethod]
        public void Test4()
        {
            var json = "[[\"!\",[[\"CustomerID\",\"=\",\"ALFKI\"],\"or\",[\"CustomerID\",\"=\",\"ANATR\"]]],\"and\",[\"!\",[[\"ShipCountry\",\"=\",\"Argentina\"],\"or\",[\"ShipCountry\",\"=\",\"Austria\"]]]]";

            ParseAndTest(json);
        }

        [TestMethod]
        public void Test5()
        {
            var json = "[[\"ShipCountry\",\"contains\",\"i\"],\"and\",[\"!\",[[\"CustomerID\",\"=\",\"ALFKI\"],\"or\",[\"CustomerID\",\"=\",\"ANATR\"]]],\"and\",[\"!\",[[\"ShipCountry\",\"=\",\"Argentina\"],\"or\",[\"ShipCountry\",\"=\",\"Austria\"]]]]";

            ParseAndTest(json);
        }

        [TestMethod]
        public void Test6()
        {
            var json = "[[\"ShipCountry\",\"contains\",\"i\"],\"and\",[\"!\",[[\"CustomerID\",\"=\",\"ALFKI\"],\"or\",[\"CustomerID\",\"=\",\"ANATR\"]]],\"and\",[[[\"OrderDate\",\">=\",\"1996-07-08T00:00:00\"],\"and\",[\"OrderDate\",\"<\",\"1996-07-09T00:00:00\"]],\"or\",[[\"OrderDate\",\">=\",\"1996-07-11T00:00:00\"],\"and\",[\"OrderDate\",\"<\",\"1996-07-12T00:00:00\"]],\"or\",[[\"OrderDate\",\">=\",\"1996-07-12T00:00:00\"],\"and\",[\"OrderDate\",\"<\",\"1996-07-13T00:00:00\"]],\"or\",[[\"OrderDate\",\">=\",\"1996-07-15T00:00:00\"],\"and\",[\"OrderDate\",\"<\",\"1996-07-16T00:00:00\"]],\"or\",[[\"OrderDate\",\">=\",\"1996-07-18T00:00:00\"],\"and\",[\"OrderDate\",\"<\",\"1996-07-19T00:00:00\"]],\"or\",[[\"OrderDate\",\">=\",\"1996-07-19T00:00:00\"],\"and\",[\"OrderDate\",\"<\",\"1996-07-20T00:00:00\"]],\"or\",[[\"OrderDate\",\">=\",\"1996-07-26T00:00:00\"],\"and\",[\"OrderDate\",\"<\",\"1996-07-27T00:00:00\"]]],\"and\",[\"!\",[[\"ShipCountry\",\"=\",\"Argentina\"],\"or\",[\"ShipCountry\",\"=\",\"Austria\"]]]]";

            ParseAndTest(json);
        }
        

        [TestMethod]
        public void Test7()
        {
            var json = "[[\"col1\", \"=\", 45], [\"col2\", \"=\", true]]";

            ParseAndTest(json);
        }

        public void ParseAndTest(string json)
        {
            var jarray = JArray.Parse(json);

            Console.WriteLine(jarray.ToString(Formatting.None));
            Console.WriteLine();

            var sql = Builder.BuildFor(jarray);

            var errors = new List<string>();

            var isValid = IsSQLQueryValid(sql, out errors);

            errors.ForEach(x =>
            {
                Console.WriteLine(x);
            });


            Assert.IsTrue(isValid);
        }


        public bool IsSQLQueryValid(string sql, out List<string> errors)
        {
            sql = "select * from emp where 1=1 and " + sql;

            Console.WriteLine(sql);

            errors = new List<string>();
            TSql140Parser parser = new TSql140Parser(false);
            TSqlFragment fragment;
            IList<ParseError> parseErrors;

            using (TextReader reader = new StringReader(sql))
            {
                fragment = parser.Parse(reader, out parseErrors);
                if (parseErrors != null && parseErrors.Count > 0)
                {
                    errors = parseErrors.Select(e => e.Message).ToList();
                    return false;
                }
            }
            return true;
        }

        private WhereClauseBuilder GetBuilder()
        {
            return new WhereClauseBuilder();
        }
    }
}
