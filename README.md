# MailChemist 

MailChemist is a combination of two technologies (MJML, and Fluid&#46;NET) to dynamically generate beautiful responsive emails driven by templates. 

## Content Providers
An email content provider is a high-level class that handles preparing the email template content for fluid Generater using e.g. Web API, file and SQL. Currently MailChemist supports two content providers:
* MJML API
* File

## Getting started

### Basic usage example
Basic usage using the MJML API and Fluid using an anonymous model.
```csharp
var model = new 
{
    FirstName = "Paul"
};

string content = @"<mjml><mj-body>{{ Model.FirstName }}</mj-body></mjml>";

var mailChemist = new MailChemistEngine(MailChemistMjmlRenderer.MjmlDotNet);
mailChemist.Render(content, model);
```

> RegisterType is required to be set to true since it's an anonymous type. This should be set to false, if you intend on using the `MailChemistModel` attribute which would be cached.

### Usage without having to register the type each call

```csharp
[MailChemistModel]
public class PersonModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}
```

> If your using nested types, you will need to decorate them with `MailChemistModel` too.

You will only need to call `MailChemist.RegisterGlobalTypes()` once before calling `Render()` or `RenderFluid()`.

### Usage using file content provider

If you want a higher performance by using already Generated MJML you can use the `FileContentProvider` without the overhead of connecting to an external website to Generate the MJML.
```csharp
var contentProvider = new FileContentProvider("Email\\Assets");
var mailChemistEngine = new MailChemistEngine(MailChemistMjmlRenderer.MjmlDotNet, contentProvider)

mailChemist.Render(content, model);
```
`Test.mc` would contain the pre-Generated MJML with Fluid&#46;NET templating.


## Fluid&#46;NET Filters

MailChemist utilises Fluid.NET under the hood because of this; You can leverage [custom filter functions](https://github.com/sebastienros/fluid#adding-custom-filters) in your templates. MailChemist has out of the box fluid filters which can be registered by running `MailChemist.RegisterGlobalFilter()`.

### IsLessThanZeroAddClassFilter

`IsLessThanZeroAddClassFilter` can be used like:
```html
<div class="{{ Model.Profit | IsLessThanZeroAddClass:'redText' }}">{{ Model.Profit }}</div>
```

If `Profit` was lower than zero then it would generate:
```html
<div class="redText">-1</div>
```

## Appreciations

Thanks to the following great projects that made MailChemist possible:

- [Fluid.NET](https://github.com/sebastienros/fluid)
- [MJML](https://github.com/mjmlio/mjml)
- [MJML.NET](https://github.com/SebastianStehle/mjml-net)
