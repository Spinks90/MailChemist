using MailChemist.Core.Attributes;
using MailChemist.Core.Interfaces;
using MailChemist.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace MailChemist.UnitTests
{
    [TestClass]
    public class MailChemistTests
    {
        [MailChemistModel]
        public class PersonModel : DefaultModel
        {
            public PersonModel(string firstName, [CallerMemberName] string lastName = null) : base(firstName, lastName)
            {
            }
        }

        [MailChemistModel]
        public class DefaultModel : IDefaultModel
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public DefaultModel(string firstName, [CallerMemberName] string lastName = null)
            {
                FirstName = firstName;
                LastName = lastName;
            }
        }

        public class TestModel
        {
            public DefaultModel Test { get; set; } = new DefaultModel("Ben", "Smith");
        }

        public interface IDefaultModel
        {
            string FirstName { get; set; }

            string LastName { get; set; }
        }

        /// <summary>
        /// Setup the types and filters but only problem is can't unregister them.
        /// </summary>
        public MailChemistTests()
        {
            //MailChemist.RegisterGlobalTypes();
            //MailChemist.RegisterGlobalFilters();
        }

        [TestMethod]
        public void ValidateErrorsTryGenerateFluidTest()
        {
            string content = @"<mjml>
                              <mj-body>
                                <mj-section>
                                  <mj-column>
                                    <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ ; }}</mj-text>
                                  </mj-column>
                                </mj-section>
                              </mj-body>
                            </mjml>";

            var model = new PersonModel("Paul");

            var mailChemist = new MailChemist();
            mailChemist.TryGenerateFluid(content, model, out var result, out var errors);

            Assert.IsNotNull(errors);
        }

        [TestMethod]
        public void ValidSimpleStrongTypeTryGenerateFluidTest()
        {
            var model = new PersonModel("Paul", "Spinks");

            string content = @"{{ Model.FirstName }} {{ Model.LastName }}";

            var mailChemist = new MailChemist();
            mailChemist.TryGenerateFluid(content, model, out var fluid, out var errors);

            Assert.IsTrue(string.IsNullOrEmpty(errors));
            Assert.AreEqual("Paul Spinks", fluid);
        }

        [TestMethod]
        public void ValidSimpleStrongTypeGenerateFluidTest()
        {
            var model = new PersonModel("Paul", "Spinks");

            string content = @"{{ Model.FirstName }} {{ Model.LastName }}";

            var mailChemist = new MailChemist();
            string fluid = mailChemist.GenerateFluid(content, model);

            Assert.AreEqual("Paul Spinks", fluid);
        }


        [TestMethod]
        public void NullEmailContentProviderExceptionTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MailChemist(null));
        }

        [TestMethod]
        public void NullFileEmailContentProviderExceptionTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MailChemist(new FileEmailContentProvider(null)));
        }

        [TestMethod]
        public void ValidFileEmailContentProviderTryTest()
        {
            var model = new
            {
                FirstName = "Paul"
            };

            var mailChemist = new MailChemist(new FileEmailContentProvider("Templates"));

            if(mailChemist.TryGenerate("Test.mc", model, out var result, out var errors, true) == false)
                Assert.IsTrue(false, "The fluid didn't compile.");
        }

        [TestMethod]
        public void ValidFileEmailContentProviderTest()
        {
            var model = new
            {
                FirstName = "Paul"
            };

            var mailChemist = new MailChemist(new FileEmailContentProvider("Templates"));

            if (mailChemist.TryGenerate("Test.mc", model, out var result, out var errors, true) == false)
                Assert.IsTrue(false, "The fluid didn't compile.");
        }

        [TestMethod]
        public void UnauthorisedMjmlEmailContentProviderTryTest()
        {
            string mjml = @"<mjml>
                              <mj-body>
                                <mj-section>
                                  <mj-column>
                                    <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ Model.FirstName }}</mj-text>
                                  </mj-column>
                                </mj-section>
                              </mj-body>
                            </mjml>";

            var mailChemist = new MailChemist("", "");

            var result = mailChemist.TryGenerateContent(mjml, out var content);

            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void UnauthorisedMjmlEmailContentProviderTest()
        {
            string mjml = @"<mjml>
                              <mj-body>
                                <mj-section>
                                  <mj-column>
                                    <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ Model.FirstName }}</mj-text>
                                  </mj-column>
                                </mj-section>
                              </mj-body>
                            </mjml>";

            var mailChemist = new MailChemist("", "");

            var result = mailChemist.GenerateContent(mjml);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test()
        {
            string mjml = @"<mjml>
                              <mj-body>
                                <mj-section>
                                  <mj-column>
                                    <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ Model.Test.FirstName }}</mj-text>
                                  </mj-column>
                                </mj-section>
                              </mj-body>
                            </mjml>";

            var model = new TestModel();

            var mailChemist = new MailChemist();
            var result = mailChemist.TryGenerateFluid(mjml, model, out var fluid, out var errors, registerType: true);


        }
    }
}