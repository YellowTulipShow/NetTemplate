using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace CommandParameterParse.Test
{
    [TestClass]
    public class TestParseConverter
    {

        private struct TestArgsModel
        {
            public string RootDire;
            public string TemplatePath { get; set; }
            public string OutputPath { get; set; }
            public IDictionary<string, string> DataJSONPaths { get; set; }
        }

        [TestMethod]
        public void TestToData()
        {
            string[] args = new string[] {
                @"--root=d:\_code_test",
                @"--TemplatePath=template/Controller.cs",
                @"--output=d:\_result\code\UsersController.cs",
                @"--data:database=e:\db\admin.json",
                @"--data:table=data/users.json",
            };
            var parameterFormatParse = new ParameterFormatParse(new List<IParameterFormatHandle>()
            {
            });
            ParameterFormatResult[] parameter = parameterFormatParse.ExplainExecution(args);


            ITypeHandleLibrary handleLibrary = new TypeHandleLibrary();
            var parse = new ParseConverter<TestArgsModel>(handleLibrary);
            TestArgsModel m = parse.GenerateDataStruct();

            Assert.AreEqual(@"d:\_code_test", m.RootDire);
            Assert.AreEqual(@"template/Controller.cs", m.TemplatePath);
            Assert.AreEqual(@"d:\_result\code\UsersController.cs", m.OutputPath);
            Assert.AreEqual(@"e:\db\admin.json", m.DataJSONPaths[@"database"]);
            Assert.AreEqual(@"data/users.json", m.DataJSONPaths[@"table"]);
        }
    }
}
