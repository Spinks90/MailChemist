using MailChemist.Core.Attributes;
using MailChemist.Core.Interfaces;
using MailChemist.Providers;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace MailChemist.UnitTests
{
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
        public abstract class DefaultModel : IDefaultModel
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public DefaultModel(string firstName, [CallerMemberName] string lastName = null)
            {
                FirstName = firstName;
                LastName = lastName;
            }
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
            MailChemist.RegisterGlobalTypes();
            MailChemist.RegisterGlobalFilters();
        }

        [Fact]
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

            Assert.NotEmpty(errors);
        }

        [Fact]
        public void ValidSimpleStrongTypeTryGenerateFluidTest()
        {
            var model = new PersonModel("Paul", "Spinks");

            string content = @"{{ Model.FirstName }} {{ Model.LastName }}";

            var mailChemist = new MailChemist();
            mailChemist.TryGenerateFluid(content, model, out var fluid, out var errors);

            Assert.Empty(errors);
            Assert.Equal("Paul Spinks", fluid);
        }

        [Fact]
        public void ValidSimpleStrongTypeGenerateFluidTest()
        {
            var model = new PersonModel("Paul", "Spinks");

            string content = @"{{ Model.FirstName }} {{ Model.LastName }}";

            var mailChemist = new MailChemist();
            string fluid = mailChemist.GenerateFluid(content, model);

            Assert.Equal("Paul Spinks", fluid);
        }


        [Fact]
        public void NullEmailContentProviderExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new MailChemist(null));
        }

        [Fact]
        public void NullFileEmailContentProviderExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new MailChemist(new FileEmailContentProvider(null)));
        }

        [Fact]
        public void ValidFileEmailContentProviderTryTest()
        {
            var model = new
            {
                FirstName = "Paul"
            };

            var mailChemist = new MailChemist(new FileEmailContentProvider("Templates"));

            if(mailChemist.TryGenerate("Test.mc", model, out var result, out var errors, true) == false)
                Assert.True(false, "The fluid didn't compile.");
        }

        [Fact]
        public void ValidFileEmailContentProviderTest()
        {
            var model = new
            {
                FirstName = "Paul"
            };

            var mailChemist = new MailChemist(new FileEmailContentProvider("Templates"));

            if (mailChemist.TryGenerate("Test.mc", model, out var result, out var errors, true) == false)
                Assert.True(false, "The fluid didn't compile.");
        }

        [Fact]
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

            Assert.False(result);
        }
        
        [Fact]
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

            Assert.NotNull(result);
        }
    }
}