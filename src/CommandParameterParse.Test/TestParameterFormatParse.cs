using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Collections.Generic;

namespace CommandParameterParse.Test
{
    [TestClass]
    public class TestParameterFormatParse
    {
        [TestMethod]
        public void TestRender_AbbreviationParameterFormatHandle()
        {
            string[] args = new string[] {
                @"-q=张三q",
                @"-w='张三w'",
                @"-e=""张三e""",
                @"-r 张三r",
                @"-t '张三t'",
                @"-y ""张三y""",
            };
            IList<IParameterFormatHandle> formatHandles = new List<IParameterFormatHandle>
            {
                new ParameterFormatHandles.AbbreviationParameterFormatHandle(),
            };
            ParameterFormatResult[] parameters = ParameterFormatParse.Render(args, formatHandles);
            Assert.IsNotNull(parameters);
            Assert.AreEqual(6, parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterFormatResult param = parameters[i];
                Assert.IsNotNull(param);
                string key =
                    i == 0 ? "q" :
                    i == 1 ? "w" :
                    i == 2 ? "e" :
                    i == 3 ? "r" :
                    i == 4 ? "t" : "y";
                Assert.AreEqual(key, param.Name);
                Assert.IsNotNull(param.Contents);
                Assert.AreEqual(1, param.Contents.Length);
                Assert.AreEqual($"张三{key}", param.Contents[0]);
            }
        }

        [TestMethod]
        public void TestRender_HorizontalLineParameterFormatHandle()
        {
            string[] args = new string[] {
                @"--q=张三q",
                @"--w='张三w'",
                @"--e=""张三e""",
                @"--r 张三r",
                @"--t '张三t'",
                @"--y ""张三y""",
            };
            IList<IParameterFormatHandle> formatHandles = new List<IParameterFormatHandle>
            {
                new ParameterFormatHandles.HorizontalLineParameterFormatHandle(),
            };
            ParameterFormatResult[] parameters = ParameterFormatParse.Render(args, formatHandles);
            Assert.IsNotNull(parameters);
            Assert.AreEqual(6, parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterFormatResult param = parameters[i];
                Assert.IsNotNull(param);
                string key =
                    i == 0 ? "q" :
                    i == 1 ? "w" :
                    i == 2 ? "e" :
                    i == 3 ? "r" :
                    i == 4 ? "t" : "y";
                Assert.AreEqual(key, param.Name);
                Assert.IsNotNull(param.Contents);
                Assert.AreEqual(1, param.Contents.Length);
                Assert.AreEqual($"张三{key}", param.Contents[0]);
            }
        }

        [TestMethod]
        public void TestRender_KeyValueParameterFormatHandle()
        {
            string[] args = new string[] {
                @"--data",
                @"database=""e:\db\admin.json""",
                @"table='data/users.json'",
                @"info=users_info.json",
                @"--names",
                @"database:""e:\db\admin.json""",
                @"table:'data/users.json'",
                @"info:users_info.json",
            };
            IList<IParameterFormatHandle> formatHandles = new List<IParameterFormatHandle>
            {
                new ParameterFormatHandles.KeyValueParameterFormatHandle(),
            };
            ParameterFormatResult[] parameters = ParameterFormatParse.Render(args, formatHandles);
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterFormatResult param = parameters[i];
                Assert.IsNotNull(param);
                string key =
                    i == 0 ? "data" : "names";
                Assert.AreEqual(key, param.Name);
                Assert.IsNotNull(param.Contents);
                Assert.AreEqual(3, param.Contents.Length);
                Assert.AreEqual(@"database=e:\db\admin.json", param.Contents[0]);
                Assert.AreEqual(@"table=data/users.json", param.Contents[1]);
                Assert.AreEqual(@"info=users_info.json", param.Contents[2]);
            }
        }
    }
}
