using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

using CommandParameterParse.Attributes;
using System.Text.RegularExpressions;

namespace CommandParameterParse.Test
{
    [TestClass]
    public class TestParseConverter
    {
        private struct TestArgsModel
        {
            [AbbreviationName("root")]
            public string RootDire;

            public string TemplatePath { get; set; }

            [AbbreviationName("output")]
            public string OutputPath { get; set; }

            [AbbreviationName("data")]
            public IDictionary<string, string> DataJSONPaths { get; set; }
        }

        private class DictType : ITypeHandle<IDictionary<string, string>>
        {
            public Type Identification() => typeof(IDictionary<string, string>);
            public IDictionary<string, string> To(string[] strs)
            {
                IDictionary<string, string> dict = new Dictionary<string, string>();
                foreach (string str in strs)
                {
                    var match = Regex.Match(str, @"^([a-zA-Z]+)=([^\n\r]+)$");
                    if (!match.Success)
                        continue;
                    string key = match.Groups[1].Value;
                    string value = match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(key))
                        dict[key] = value;
                }
                return dict;
            }
            object ITypeHandle.To(string[] strs) => To(strs);
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
            ICommandParse<TestArgsModel> commandParse = new CommandParse<TestArgsModel>();
            commandParse.RegisterITypeHandle(new DictType());
            commandParse.OnExecute(args, m =>
            {
                Assert.AreEqual(@"d:\_code_test", m.RootDire);
                Assert.AreEqual(@"template/Controller.cs", m.TemplatePath);
                Assert.AreEqual(@"d:\_result\code\UsersController.cs", m.OutputPath);
                Assert.AreEqual(@"e:\db\admin.json", m.DataJSONPaths[@"database"]);
                Assert.AreEqual(@"data/users.json", m.DataJSONPaths[@"table"]);
            });
        }
    }
}
