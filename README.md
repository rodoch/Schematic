# Schematic

Schematic is an [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) framework for building customised content management systems. It aims to:

- Improve developer productivity by providing ready-made, opt-in utilities for common content management requirements
- Integrate seamlessly with applications built using a domain-driven design approach
- Provide first-class localisation support for multilingual applications
- Ensure that all aspects of the framework allow for extensibilty, overriding and modification

It offers an end-to-end solution, helping you write your business logic and making it easy to compose a user interface (UI) for all kinds of editorial tasks. Schematic does not try to reinvent the wheel, or make you learn a new framework from scratch: it allows you to utilise the
tools already available in .NET Core — like MVC model binding and validation, string localisers, etc. — and leverages the power of [web components](https://developer.mozilla.org/en-US/docs/Web/Web_Components) to help you build a modern, standards-compliant UI.

**Note:** This is a very early-stage release. There will be many breaking changes and new features added before we reach version 1.0. The information below offers more of a high-level overview than in-depth instructions. Comprehensive documentation will be added with the v0.3 release.

## Contents

- [Use case](#the-use-case-for-schematic)
- [What it is not](#what-it-is-not)
- [Architecture](#architecture)
- [Concepts](#concepts)
  - [Resources](#resources)
  - [Repeatables](#repeatables)
  - [Contexts](#contexts)
  - [Assets](#assets)
  - [Users](#users)
- [Roadmap](#roadmap)

## The use case for Schematic

It's important to use tried-and-tested, well-supported tools whenever you can. If one of the pre-fabricated content management system (CMS) solutions out there — like [WordPress](https://wordpress.org/), [Ghost](https://ghost.org/), or [Perch](https://grabaperch.com/) — meets all of your development needs for a particular project then you should absolutely choose one of those tools. For every other time, there's Schematic. Consider the scenarios below:

**Scenario 1:** You often work with an off-the-shelf CMS like WordPress and it allows you to achieve 80-90% of what you need to do. However, in many situations you find yourself limited because you're tied to particular database structure, the CMS doesn't play nice with your clean, domain-driven design architecture or you can't integrate another type of data store. Maybe there's a string somewhere you just can't localise using one of the available plugins. You find yourself solving these issues with workarounds that are hacky and brittle. This is the kind of development that makes a system hard to maintain, causes entropy in a code base over time and eventually leads to malfunctions and a poorer user experience.

**Scenario 2:** It's clear from the project requirements that one of the pre-fabricated CMSs out there would be too constraining to use and you need to provide a custom solution. However, this might mean building non-trivial utilities from scratch for things like user authentication and management, front-end user interface (UI) and editor tools, or else you might copy and paste code from a previous project. These tasks may well be repetitive, a poor use of your time, and create room for error.

If either of these scenarios sound familiar, then you'll appreciate why Schematic might help you build your next application.

## What it is not

All software design decisions are the result of trade-offs and it's important to be clear about the limitations of any tool. Here are some points to keep in mind as regards Schematic:

- Schematic is concerned with back-end content management *only*. It does not generate front-end templates à la WordPress — it is your responsibility to build any public-facing presentation layer. The assumption here is that if your requirements dictate that you need to build a custom CMS you are likely to find pre-made templating solutions too restrictive. This is also liberating, as Schematic doesn't care how your data will be presented. You can use it to build invoicing software as easily as you can use it for a website backend.
- For its jumping-off point, Schematic assumes that you plan to use a [domain-driven design](https://en.wikipedia.org/wiki/Domain-driven_design) approach, to a greater or lesser extent, in building your application. This has some implications. For example, by default Schematic enforces a [repository pattern](https://deviq.com/repository-pattern/) to manage your data access logic. It is absolutely possible, and relatively easy, to override these default patterns by creating your own controllers for situations where you need to implement a different approach. However, if you think you will mostly be fighting against the default patterns then you're not going to see the productivity benefits that Schematic brings as standard.

## Architecture

The Schematic framework is highly modular. The various aspects of the CMS framework are separated into multiple NuGet packages. This makes it easy to maintain and add new features to existing solutions. The main project, which you can clone from this repo, is extremely lightweight. For example, the interfaces that describe user management and authorisation actions are contained in the **Schematic.Identity** dependency. A controller that implements these actions is contained in **Schematic.Core.Mvc** and the default views are in **Schematic.Core.Razor**. A user repositiory implemented in [SQLite](https://www.sqlite.org) is housed in the **Schematic.BaseInfrastructure.Sqlite** package. When you start up the main project these facilities just work.

When you wish to implement your own features, it is easy to override the default options. All of the constituent dependencies are made available on an open-source basis for your reference:

| Package/GitHub Repo | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- |
| [Schematic.Core](https://github.com/rodoch/Schematic.Core) | N/A | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Schematic.Core.svg)](https://www.nuget.org/packages/Schematic.Core/) |  [![NuGet](https://img.shields.io/nuget/dt/Schematic.Core.svg)](https://www.nuget.org/packages/Schematic.Core/) |
| [Schematic.Core.Mvc](https://github.com/rodoch/Schematic.Core.Mvc) | N/A | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Schematic.Core.Mvc.svg)](https://www.nuget.org/packages/Schematic.Core.Mvc/) |  [![NuGet](https://img.shields.io/nuget/dt/Schematic.Core.Mvc.svg)](https://www.nuget.org/packages/Schematic.Core.Mvc/) |
| [Schematic.Core.Razor](https://github.com/rodoch/Schematic.Core.Razor) | N/A | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Schematic.Core.Razor.svg)](https://www.nuget.org/packages/Schematic.Core.Razor/) |  [![NuGet](https://img.shields.io/nuget/dt/Schematic.Core.Razor.svg)](https://www.nuget.org/packages/Schematic.Core.Razor/) |
| [Schematic.Identity](https://github.com/rodoch/Schematic.Identity) | N/A | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Schematic.Identity.svg)](https://www.nuget.org/packages/Schematic.Identity/) |  [![NuGet](https://img.shields.io/nuget/dt/Schematic.Identity.svg)](https://www.nuget.org/packages/Schematic.Identity/) |
| [Schematic.BaseInfrastructure](https://github.com/rodoch/Schematic.BaseInfrastructure) | N/A | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Schematic.BaseInfrastructure.svg)](https://www.nuget.org/packages/Schematic.BaseInfrastructure/) |  [![NuGet](https://img.shields.io/nuget/dt/Schematic.BaseInfrastructure.svg)](https://www.nuget.org/packages/Schematic.BaseInfrastructure/) |

There are also implementation-specific packages. Only a SQLite implementation is provided currently, but Schematic will also support SQL Server soon:

| Package/GitHub Repo  | Data store | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- | --------- |
| [Schematic.BaseInfrastructure.Sqlite](https://github.com/rodoch/Schematic.BaseInfrastructure.Sqlite) | [SQLite](https://www.sqlite.org) | N/A | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Schematic.BaseInfrastructure.Sqlite.svg)](https://www.nuget.org/packages/Schematic.BaseInfrastructure.Sqlite/) |  [![NuGet](https://img.shields.io/nuget/dt/Schematic.BaseInfrastructure.Sqlite.svg)](https://www.nuget.org/packages/Schematic.BaseInfrastructure.Sqlite/) |

The Schematic UI is built with [Stencil-compiled](https://stenciljs.com/) web components. The components package will be made available with the v0.2 release via NPM:

| Package/GitHub Repo | NPM Stable | NPM Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- |
| [schematic-components](https://github.com/rodoch/schematic-components) | N/A | N/A | N/A |

## Concepts

Schematic is architected around a few key generic concepts.

### Resources

A **Resource** is an object or entity that you make available for editing in your Schematic-based CMS. Your resources can exist either within the Schematic namespace or in any linked assembly. Resources can be 'pure' entities, perhaps included in a linked core class library, or encapsulated in a view model or similar. Any instantiable C# class can be a resource: you just need to add the `SchematicResource` attribute.

```csharp
[SchematicResource]
public class Book
{
    public string Title { get; set; }
    public List<Author> Authors { get; set; }
    public List<Publisher> Publishers { get; set; }
    public string ISBN { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}
```

On application startup, Schematic canvasses all linked assemblies for public types annotated with the `SchematicResource` attribute and automatically creates the following HTTP endpoints:

- `/en-GB/resource/book/` (Explorer view)
- `/en-GB/resource/book/create`
- `/en-GB/resource/book/read`
- `/en-GB/resource/book/update`
- `/en-GB/resource/book/delete`
- `/en-GB/resource/book/schema` (returns a JSON schema for the resource)

(The URL template is `<CURRENT-UI-CULTURE>/resource/<RESOURCE-NAME>/<OPTIONAL-ENDPOINT>`.)

All you need to then do is write the editor UI using standard MVC practices and aided by Schematic's composable web component library, and implement your data persistence logic in a resource repository. Schematic uses its own generic `ResourceController` to generate the business logic and CRUD methods for you.

There are many more aspects of the **Resource** concept we could discuss, but a final point worth noting for now is that resources can be given additional levels of access protection by use of the `SchematicAuthorize` attribute. This is in addition to the standard user authentication and authorisation that happens when someone signs in to a Schematic-based CMS. In the example below only users granted the *Editor* role within this particular application will have access to the `Book` resource. You get to define the roles and/or priviliges employed by the application.

```csharp
[SchematicResource]
[SchematicAuthorize(Role = "Editor")]
public class Book
{
    public string Title { get; set; }
    public List<Author> Authors { get; set; }
    public List<Publisher> Publishers { get; set; }
    public string ISBN { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}
```

### Repeatables

In real-world applications the entities you edit as resources will not always be flat objects but may contain complex classes as sub-objects of a resource. **Repeatables** offer an easy solution for situations where a resource may hold an indefinite number (i.e. none or one or many) of instances of these sub-objects. Take, for example, the `Book` class above. It may have one or more authors. Annotating the `Author` class with a `SchematicRepeatable` attribute allows you to easily provide an *Add new author* button and form fieldset in your editor UI:

```csharp
[SchematicResource]
[SchematicRepeatable]
public class Author
{
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    public string Biography { get; set; }
}
```

It is possible for a class to be both a **Resource** *and* a **Repeatable**, as in the case of the `Author` type above. This means that it can be used as a repeatable within a *Book* editor but that you can also have a separate interface when you wish to manage the authors in isolation.

### Contexts

Sometimes you need more than just simple CRUD functions or you require more complex validation than can be perfomed using validation attributes alone. **Contexts** offer hooks into the `ResourceController`'s new/create/read/update/delete methods: allowing you to add validation and processing logic to specific resources. Context logic is applied before the resource is persisted to your data store.

### Assets

An **Asset** represents file metadata. It holds information such as file names, content types and creation dates for the files you upload. In special cases where particular file types need additional metadata, special asset types are derived from the `Asset` base class. For example, the `ImageAsset` class contains all of the standard asset metadata plus image-specific information such as the image width and height. In all standard, supported implementations Schematic creates database tables by default to store this metadata as well as asset controllers to manage the files.

### Users

An easily-understood concept, a **User** represents a person who uses your Schematic-based CMS. It holds metadata such as a password and contact information, as well as a unique identifier such as an e-mail address or username. You can override the default `User` class and create your own to suit specific needs, but all **User** classes must implement the `ISchematicUser` interface (found in the [Schematic.Identity](#architecture) NuGet package) in order to be able to work with the application.

## Roadmap

This is a very early-stage release of the Schematic CMS framework. Planned development over the coming months include:

- Sample projects and step-by-step documentation
- A major rewrite of the UI components to improve design and user/developer experience. While the C# code has already been refactored numerous times and the API is quite stable, the UI has not yet been refined to the same extent.
- Better exception handling and error reporting.
- Unit and integration tests for the various packages.
- Move towards a more event-driven architecture. Right now, for example, when you create a new user this calls a *Create* action on the user controller which, if successful, then calls a *EmailSender* service that sends the new user an invitation e-mail. It would be better for extensibility purposes if the *Create* action simply raised an event on completion that other services, such as a the e-mail service, could then hook into.
- Additional editors: right now, Schematic comes with support for the [Quill](https://quilljs.com/) rich-text editor. It is proposed to add support for the [SimpleMDE](https://simplemde.com/) Markdown editor and [Xonomy](http://www.lexiconista.com/xonomy/) XML editor.
- Test with Entity Framework and generic repositories. To date, all implementations have used handwritten SQL queries and this use case remains untried.