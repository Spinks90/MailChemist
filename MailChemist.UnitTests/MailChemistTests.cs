using MailChemist.Core;
using MailChemist.Exceptions;
using MailChemist.UnitTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace MailChemist.UnitTests;

[TestClass]
public class MailChemistTests
{
    private readonly MailChemistEngine _sut;

    /// <summary>
    /// Setup the types and filters but only problem is can't unregister them.
    /// </summary>
    public MailChemistTests()
    {
        _sut = new MailChemistEngine(MailChemistMjmlRenderer.MjmlDotNet);

        //MailChemist.RegisterGlobalTypes();
        //MailChemist.RegisterGlobalFilters();
    }

    [TestMethod]
    public void Test()
    {
        var model = new PersonsModel();
        model.PersonX = new PersonX() { Id = 1, Name = "DUDE" };
        model.ListPeople = new List<Person>()
        {
            new Person() { Id = 1, Name = "Paul Spinks" },
            new Person() { Id = 2, Name = "Shannon Ball" }
        };

        string content = @"<mjml>
                                      <mj-body>
                                        <mj-section>
                                          <mj-column>
                                            <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ PersonX.Name }}</mj-text>
                                            

                                            {% for person in Model.ListPeople %}
                                                {{ person.Name }}
                                            {% endfor %}
                                          </mj-column>
                                        </mj-section>
                                      </mj-body>
                                    </mjml>";

        content = @"<mjml>
  <mj-body background-color=""#ccd3e0"" font-size=""13px"">
    <mj-section background-color=""#fff"" padding-bottom=""20px"" padding-top=""20px"">
      <mj-column width=""100%"">
        <mj-image src=""https://via.placeholder.com/450"" alt="""" align=""center"" border=""none"" width=""100px"" padding-left=""0px"" padding-right=""0px"" padding-bottom=""10px"" padding-top=""10px""></mj-image>
        <mj-image src=""https://via.placeholder.com/450"" alt="""" align=""center"" border=""none"" width=""200px"" padding-left=""0px"" padding-right=""0px"" padding-bottom=""0px"" padding-top=""0""></mj-image>
      </mj-column>
    </mj-section>
    <mj-section background-color=""#356cc7"" padding-bottom=""0px"" padding-top=""0"">
      <mj-column width=""100%"">
        <mj-text align=""center"" font-size=""13px"" color=""#ABCDEA"" font-family=""Ubuntu, Helvetica, Arial, sans-serif"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""18px"" padding-top=""28px"">HELLO
          <p style=""font-size:16px; color:white"">[[FirstName]]</p>
        </mj-text>
      </mj-column>
    </mj-section>
    <mj-section background-color=""#356cc7"" padding-bottom=""5px"" padding-top=""0"">
      <mj-column width=""100%"">
        <mj-divider border-color=""#ffffff"" border-width=""2px"" border-style=""solid"" padding-left=""20px"" padding-right=""20px"" padding-bottom=""0px"" padding-top=""0""></mj-divider>
        <mj-text align=""center"" color=""#FFF"" font-size=""13px"" font-family=""Helvetica"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""28px"" padding-top=""28px""><span style=""font-size:20px; font-weight:bold"">Thank you very much for your purchase.</span>
          <br />
          <span style=""font-size:15px"">Please find the receipt below.</span>
        </mj-text>
      </mj-column>
    </mj-section>
    <mj-section background-color=""#568feb"" padding-bottom=""15px"">
      <mj-column>
        <mj-text align=""center"" color=""#FFF"" font-size=""15px"" font-family=""Ubuntu, Helvetica, Arial, sans-serif"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""0px""><strong>Order Number</strong></mj-text>
        <mj-text align=""center"" color=""#FFF"" font-size=""13px"" font-family=""Helvetica"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""20px"" padding-top=""10px"">{{ PersonX.Name }}</mj-text>
      </mj-column>
      <mj-column>
        <mj-text align=""center"" color=""#FFF"" font-size=""15px"" font-family=""Ubuntu, Helvetica, Arial, sans-serif"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""0px""><strong>Order Date</strong></mj-text>
        <mj-text align=""center"" color=""#FFF"" font-size=""13px"" font-family=""Helvetica"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""20px"" padding-top=""10px"">[[OrderDate]]</mj-text>
      </mj-column>
      <mj-column>
        <mj-text align=""center"" color=""#FFF"" font-size=""15px"" font-family=""Ubuntu, Helvetica, Arial, sans-serif"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""0px""><strong>Total Price</strong></mj-text>
        <mj-text align=""center"" color=""#FFF"" font-size=""13px"" font-family=""Helvetica"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""20px"" padding-top=""10px"">[[TotalPrice]]</mj-text>
      </mj-column>
    </mj-section>
    <mj-section background-color=""#356CC7"" padding-bottom=""20px"" padding-top=""20px"">
      <mj-column width=""50%"">
        <mj-button background-color=""#ffae00"" color=""#FFF"" font-size=""14px"" align=""center"" font-weight=""bold"" border=""none"" padding=""15px 30px"" border-radius=""10px"" href=""https://mjml.io"" font-family=""Helvetica"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""10px"">Download Receipt</mj-button>
      </mj-column>
      <mj-column width=""50%"">
        <mj-button background-color=""#ffae00"" color=""#FFF"" font-size=""14px"" align=""center"" font-weight=""bold"" border=""none"" padding=""15px 30px"" border-radius=""10px"" href=""https://mjml.io"" font-family=""Helvetica"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""12px"">Track My Order</mj-button>
      </mj-column>
    </mj-section>
    <mj-section background-color=""#356cc7"" padding-bottom=""5px"" padding-top=""0"">
      <mj-column width=""100%"">
        <mj-divider border-color=""#ffffff"" border-width=""2px"" border-style=""solid"" padding-left=""20px"" padding-right=""20px"" padding-bottom=""0px"" padding-top=""0""></mj-divider>
        <mj-text align=""center"" color=""#FFF"" font-size=""15px"" font-family=""Helvetica"" padding-left=""25px"" padding-right=""25px"" padding-bottom=""20px"" padding-top=""20px"">Best,
          <br />
          <span style=""font-size:15px"">The [[CompanyName]] Team</span>
        </mj-text>
      </mj-column>
    </mj-section>
  </mj-body>
</mjml>";

        var _globalTypes = new HashSet<int>();
        var a = new List<int>() { 1, 2, 3 };
        var b = new List<int>() {  3, 4, 6 };

        _globalTypes.UnionWith(a);
        _globalTypes.UnionWith(b);






        var yolo = new MailChemistEngine(MailChemistMjmlRenderer.MjmlApi);


        try
        {
            var lol = yolo.Render(content, model, registerType: true);

        }
        catch(MailChemistGetContentException mcgcex)
        {
            if(mcgcex.InnerException is FileNotFoundException fnfex)
            {
                throw;
            }
            throw;
        }
        catch(Exception ex)
        {
            throw;
        }

        //var wtf = _sut.RenderFluid(content, model, registerType: true);

        //var lol = _sut.GenerateMjmlDotNet(content);
    }


    //    [TestMethod]
    //    public void ValidateComplexTypeFluidTest()
    //    {
    //        string content = @"<mjml>
    //                          <mj-body>
    //                            <mj-section>
    //                              <mj-column>
    //                                <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica""></mj-text>
    //                                {{ PersonX.Name }}

    //                                {% for person in Model.People %}
    //                                    {{ person.Name }}
    //                                {% endfor %}
    //                              </mj-column>
    //                            </mj-section>
    //                          </mj-body>
    //                        </mjml>";

    //        var model = new PersonsModel();
    //        model.PersonX = new PersonX() { Id = 1, Name = "DUDE" };
    //        //model.People = new List<Person>()
    //        //{ 
    //        //    new Person() { Id = 1, Name = "Paul Spinks" },
    //        //    new Person() { Id = 2, Name = "Shannon Ball" }
    //        //};

    //        var mailChemist = new MailChemist();
    //        mailChemist.TryGenerateFluid(content, model, out var result, out var errors);

    //        Assert.IsNotNull(errors);
    //    }

    //    [TestMethod]
    //    public void ValidateErrorsTryGenerateFluidTest()
    //    {
    //        string content = @"<mjml>
    //                          <mj-body>
    //                            <mj-section>
    //                              <mj-column>
    //                                <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ ; }}</mj-text>
    //                              </mj-column>
    //                            </mj-section>
    //                          </mj-body>
    //                        </mjml>";

    //        var model = new PersonModel("Paul");

    //        var mailChemist = new MailChemist();
    //        mailChemist.TryGenerateFluid(content, model, out var result, out var errors);

    //        Assert.IsNotNull(errors);
    //    }

    //    [TestMethod]
    //    public void ValidSimpleStrongTypeTryGenerateFluidTest()
    //    {
    //        var model = new PersonModel("Paul", "Spinks");

    //        string content = @"{{ Model.FirstName }} {{ Model.LastName }}";

    //        var mailChemist = new MailChemist();
    //        mailChemist.TryGenerateFluid(content, model, out var fluid, out var errors);

    //        Assert.IsTrue(string.IsNullOrEmpty(errors));
    //        Assert.AreEqual("Paul Spinks", fluid);
    //    }

    //    [TestMethod]
    //    public void ValidSimpleStrongTypeGenerateFluidTest()
    //    {
    //        var model = new PersonModel("Paul", "Spinks");

    //        string content = @"{{ Model.FirstName }} {{ Model.LastName }}";

    //        var mailChemist = new MailChemist();
    //        string fluid = mailChemist.GenerateFluid(content, model);

    //        Assert.AreEqual("Paul Spinks", fluid);
    //    }


    //    [TestMethod]
    //    public void NullEmailContentProviderExceptionTest()
    //    {
    //        Assert.ThrowsException<ArgumentNullException>(() => new MailChemist(null), "Expected ArgumentNullException to be thrown");
    //    }

    //    [TestMethod]
    //    public void NullFileEmailContentProviderExceptionTest()
    //    {
    //        Assert.ThrowsException<ArgumentNullException>(() => new MailChemist(new FileMjmlContentProvider(null)), "Expected ArgumentNullException to be thrown");
    //    }

    //    [TestMethod]
    //    public void ValidFileEmailContentProviderTryTest()
    //    {
    //        var model = new
    //        {
    //            FirstName = "Paul"
    //        };

    //        var mailChemist = new MailChemist(new FileMjmlContentProvider("Templates"));

    //        if(mailChemist.TryGenerate("Test.mc", model, out var result, out var errors, true) == false)
    //            Assert.IsTrue(false, "The fluid didn't compile.");
    //    }

    //    [TestMethod]
    //    public void ValidFileEmailContentProviderTest()
    //    {
    //        var model = new
    //        {
    //            FirstName = "Paul"
    //        };

    //        var mailChemist = new MailChemist(new FileMjmlContentProvider("Templates"));

    //        if (mailChemist.TryGenerate("Test.mc", model, out var result, out var errors, true) == false)
    //            Assert.IsTrue(false, "The fluid didn't compile.");
    //    }

    //    [TestMethod]
    //    public void UnauthorisedMjmlEmailContentProviderTryTest()
    //    {
    //        string mjml = @"<mjml>
    //                          <mj-body>
    //                            <mj-section>
    //                              <mj-column>
    //                                <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ Model.FirstName }}</mj-text>
    //                              </mj-column>
    //                            </mj-section>
    //                          </mj-body>
    //                        </mjml>";

    //        var mailChemist = new MailChemist("", "");

    //        var result = mailChemist.TryGenerateContent(mjml, out var content);

    //        Assert.IsFalse(result);
    //    }

    //    [TestMethod]
    //    public void UnauthorisedMjmlEmailContentProviderTest()
    //    {
    //        string mjml = @"<mjml>
    //                          <mj-body>
    //                            <mj-section>
    //                              <mj-column>
    //                                <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ Model.FirstName }}</mj-text>
    //                              </mj-column>
    //                            </mj-section>
    //                          </mj-body>
    //                        </mjml>";

    //        var mailChemist = new MailChemist("", "");

    //        var result = mailChemist.GenerateContent(mjml);

    //        Assert.IsNotNull(result);
    //    }

    //    [TestMethod]
    //    public void Test()
    //    {
    //        string mjml = @"<mjml>
    //                          <mj-body>
    //                            <mj-section>
    //                              <mj-column>
    //                                <mj-text font-size=""20px"" color=""#F45E43"" font-family=""helvetica"">{{ Model.Test.FirstName }}</mj-text>
    //                              </mj-column>
    //                            </mj-section>
    //                          </mj-body>
    //                        </mjml>";

    //        var model = new TestModel();

    //        var mailChemist = new MailChemist();
    //        var result = mailChemist.TryGenerateFluid(mjml, model, out var fluid, out var errors, registerType: true);

    //        Assert.IsNotNull(result);
    //        Assert.IsTrue(fluid.Contains(model.Test.FirstName));
    //    }
}