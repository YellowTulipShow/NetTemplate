using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

using CommandParameterParse.Attributes;
using System.Text.RegularExpressions;

namespace CommandParameterParse.Test
{
    [TestClass]
    public class TestCommandParse : ICommandParseHelpPrint
    {
        [CommandParameterDescription("单元测试解析初始命令参数模型")]
        [CommandParameterDescription("第二行测试帮助文本")]
        private struct TestArgsModel
        {
            [AbbreviationName('r')]
            [AliasName("root")]
            [IsRequired()]
            [CommandParameterDescription("根目录地址")]
            public string RootDire;

            [IsRequired(false)]
            [CommandParameterDescription("模板地址")]
            public string TemplatePath { get; set; }

            [AbbreviationName('o')]
            [AliasName("output")]
            [CommandParameterDescription("输出路径")]
            public string OutputPath { get; set; }

            [AliasName("data")]
            [CommandParameterDescription("键值对数据配置")]
            [CommandParameterDescription("例如: --data key1=value1 key2=value2 ...")]
            public IDictionary<string, string> DataJSONPaths { get; set; }
        }

        [TestMethod]
        public void TestToData()
        {
            string[] args = new string[] {
                @"-r",
                @"d:\_code_test",
                @"--TemplatePath",
                @"template/Controller.cs",
                @"--output=d:\_result\code\UsersController.cs",
                @"--data",
                @"database=e:\db\admin.json",
                @"table=data/users.json",
            };
            ICommandParse<TestArgsModel> commandParse = new CommandParse<TestArgsModel>(this);
            commandParse.RegisterITypeHandle(new TypeHandles.TypeHandle_IDictionaryStringJoinString());
            commandParse.RegisterIParameterFormatHandle(new ParameterFormatHandles.KeyValueParameterFormatHandle());
            commandParse.OnExecute(args, m =>
            {
                Assert.AreEqual(@"d:\_code_test", m.RootDire);
                Assert.AreEqual(@"template/Controller.cs", m.TemplatePath);
                Assert.AreEqual(@"d:\_result\code\UsersController.cs", m.OutputPath);
                Assert.AreEqual(@"e:\db\admin.json", m.DataJSONPaths[@"database"]);
                Assert.AreEqual(@"data/users.json", m.DataJSONPaths[@"table"]);
            });
        }

        [TestMethod]
        public void TestHelp()
        {
            ICommandParse<TestArgsModel> commandParse = new CommandParse<TestArgsModel>(this);
            commandParse.OnExecute(new string[] { @"-h", @"--output=d:\_reser.cs", }, m => Assert.IsTrue(false));
            commandParse.OnExecute(new string[] { @"--help", @"--output=d:\_reler.cs", }, m => Assert.IsTrue(false));
        }

        public void Prints(string[] help_content)
        {
            Assert.IsNotNull(help_content);
            Assert.IsTrue(help_content.Length > 0);
        }
    }
}
