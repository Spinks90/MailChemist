using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MailChemist.MjmlDotNet
{
    public class Class1
    {
        public static XDocument document = XDocument.Parse(@"<mjml>
                                                    <mj-head>
                                                        <mj-breakpoint width=""320px"" />
                                                    </mj-head>
                                                    <mj-body>
                                                        <mj-section>
                                                            <mj-column>
                                                                <mj-text font-size="" 20px"" color="" #F45E43"" font-family="" helvetica"">Hello World</mj-text>
                                                                <mj-divider border-color="" #F45E43""></mj-divider>
                                                            </mj-column>
                                                        </mj-section>
                                                    </mj-body>
                                                </mjml>");

        public string output;

        public static void Test()
        {
            var components = new List<IMjmlComponent>();

            foreach(var element in document.Elements().ToList())
            {
                var name = element.Name;

                if(string.Equals(name.LocalName, "mjml", StringComparison.OrdinalIgnoreCase))
                {
                    XElement repElement = XElement.Parse("</html>");
                    element.ReplaceWith(repElement);
                }

            }
        }
    }
}
