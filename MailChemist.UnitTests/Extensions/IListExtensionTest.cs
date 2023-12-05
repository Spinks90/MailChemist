//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Collections.Concurrent;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace MailChemist.UnitTests.Extensions
//{
//    [TestClass]
//    public class IListExtensionTest
//    {
//        [TestMethod]
//        public void ArgumentNullExceptionSourceTest()
//        {
//            var test = new List<string>();

//            Assert.ThrowsException<ArgumentNullException>(() => test.AddRange(null));
//        }

//        //[TestMethod]
//        //public void ArgumentNullExceptionTargetTest()
//        //{
//        //    IList<string> test = null;

//        //    Assert.ThrowsException<ArgumentNullException>(() => test.AddRange(null));
//        //}

//        //[TestMethod]
//        //public void ValidateNoneIListTest()
//        //{
//        //    var strings = new string[] { "Hello", "World" };
//        //    var test = new ArraySegment<string>(strings);
    
//        //    var retVal = new List<string>();

//        //    retVal.AddRange(test);

//        //    for(int i = 0; i < retVal.Count; i++)
//        //        Assert.Equal(strings[i], retVal[i]);
//        //}

//        //[TestMethod]
//        //public void ValidateListTest()
//        //{
//        //    var test = new List<string>() { "Hello", "World" };

//        //    IList<string> retVal = new List<string>();

//        //    retVal.AddRange(test);

//        //    for (int i = 0; i < retVal.Count; i++)
//        //        Assert.AreEqual(test[i], retVal[i]);
//        //}
//    }
//}
