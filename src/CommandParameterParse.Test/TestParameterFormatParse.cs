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
                @"-q=����q",
                @"-w='����w'",
                @"-e=""����e""",
                @"-r ����r",
                @"-t '����t'",
                @"-y ""����y""",
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
                Assert.AreEqual($"����{key}", param.Contents[0]);
            }
        }

        [TestMethod]
        public void TestRender_HorizontalLineParameterFormatHandle()
        {
            string[] args = new string[] {
                @"--qname=����qname",
                @"--wname='����wname'",
                @"--ename=""����ename""",
                @"--rname ����rname",
                @"--tname '����tname'",
                @"--yname ""����yname""",
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
                    i == 0 ? "qname" :
                    i == 1 ? "wname" :
                    i == 2 ? "ename" :
                    i == 3 ? "rname" :
                    i == 4 ? "tname" : "yname";
                Assert.AreEqual(key, param.Name);
                Assert.IsNotNull(param.Contents);
                Assert.AreEqual(1, param.Contents.Length);
                Assert.AreEqual($"����{key}", param.Contents[0]);
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


        [TestMethod]
        public void TestRender_MergeTotal()
        {
            string[] args = new string[] {
                @"-q=����q",
                @"-w='����w'",
                @"-e=""����e""",
                @"-r ����r",
                @"-t '����t'",
                @"-y ""����y""",

                @"--qname=����qname",
                @"--wname='����wname'",
                @"--ename=""����ename""",
                @"--rname ����rname",
                @"--tname '����tname'",
                @"--yname ""����yname""",

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
                new ParameterFormatHandles.AbbreviationParameterFormatHandle(),
                new ParameterFormatHandles.HorizontalLineParameterFormatHandle(),
                new ParameterFormatHandles.KeyValueParameterFormatHandle(),
            };
            ParameterFormatResult[] parameters = ParameterFormatParse.Render(args, formatHandles);
            var dict_param = parameters.ToDictionary(b => b.Name);
        }
    }
}
