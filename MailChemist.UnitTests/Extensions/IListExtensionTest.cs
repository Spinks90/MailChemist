using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MailChemist.Extensions;
using System.Collections.Concurrent;

namespace MailChemist.UnitTests.Extensions
{
    public class IListExtensionTest
    {
        [Fact]
        public void ArgumentNullExceptionSourceTest()
        {
            IList<string> test = new List<string>();

            Assert.Throws<ArgumentNullException>(() => test.AddRange(null));
        }

        [Fact]
        public void ArgumentNullExceptionTargetTest()
        {
            IList<string> test = null;

            Assert.Throws<ArgumentNullException>(() => test.AddRange(null));
        }

        //[Fact]
        //public void ValidateNoneIListTest()
        //{
        //    var strings = new string[] { "Hello", "World" };
        //    var test = new ArraySegment<string>(strings);
    
        //    var retVal = new List<string>();

        //    retVal.AddRange(test);

        //    for(int i = 0; i < retVal.Count; i++)
        //        Assert.Equal(strings[i], retVal[i]);
        //}

        [Fact]
        public void ValidateListTest()
        {
            var test = new List<string>() { "Hello", "World" };

            IList<string> retVal = new List<string>();

            retVal.AddRange(test);

            for (int i = 0; i < retVal.Count; i++)
                Assert.Equal(test[i], retVal[i]);
        }
    }
}
