[![Build status](https://ci.appveyor.com/api/projects/status/8mk3us0d5stlkq47?svg=true)](https://ci.appveyor.com/project/badf00d/disposablefixer)
![](https://img.shields.io/appveyor/tests/badf00d/DisposableFixer.svg?style=flat)
![](https://img.shields.io/nuget/v/DisposableFixer.svg?style=flat)
![](https://img.shields.io/visual-studio-marketplace/v/DavidStormer.DisposableFixer.svg?style=flat&label=Visual%20Studio%20Markedplace)

# DisposableFixer
This is a small Visual Studio Extension that should identify and fix problems as memleaks while using IDispoables. You can download it via [Visual Studio Gallery](https://marketplace.visualstudio.com/items?itemName=DavidStormer.DisposableFixer) or using Visual Studio Extension Manager.

Here is a little Preview of the extension in action:

![Preview](https://github.com/BADF00D/DisposableFixer/blob/master/src/DisposableFixer.Vsix/Preview.png?raw=true "Preview")

This project is in early stage. Currently it has some known false positive and false negative results. 
Furthermore there are no code fixes available at the moment.

# How It works
In order to check for undisposed IDispoables, there is an Roslyn analyser that registers for
two syntax node actions:
* ObjectCreation - something like ```new MemoryStream()```
* InvocationExpression - something like ```CreateMemoryStream()```

Currently these both are considered the only possibilities where IDispoables can be created. 

Every time one of the actions is triggered, the static code analyzer starts analyzing. 
The first thing the analyzer tries to find out is, whether the given expression created
is actually an IDisposables. Otherwise the analysis is aborted. After that, the analyzer 
tries to find out, whether this IDisposable have to be disposed manually and if so, it 
checks if it is actually disposed, otherwise an diagnostics is reported.

# When is a IDisposables considered as disposed?
Generally this extension assumes, that an IDisposable has to be disposed, where it gets 
created. The class that created an IDisposable is responsible for destroying it. 

This sounds simple, but it's pretty complicated to detect, because this simple definition
is not satisfied by all your code. Even within the code assemblies there are exceptions,
that had to be detected correctly. In the following text, I want explain this in detail.

## When is an anonymous and local variable considered as disposed?
These are variables, where the result of the ObjectCreation or InvocationExpression is not 
stored in a named variable, where it is created. Example:

```csharp
public void SomeMethod(){
    new MemoryStream();
    //or
    var reader = new StreamReader(new MemoryStream());
    //or
    using(new MemoryStream()){}
}
``` 

Anonymous variables are considered disposed if:
* A Dispose is called (e.g. ```new MemoryStream().Dispose())```
* It is used in a using (e.g. ```using(new MemoryStream()))```
* It is used as a parameter in the construction of a tracking type
* It is used as parameter in a tracking method.
* The source is a InvocationExpression and the source is a tracking factory. (not yet implemented)

### WTF is that tracking stuff?
A tracking type is an IDisposable itself, that keeps track of the disposables given to this 
tracking type within the constructor. Example: System.IO.StreamReader.
If you call ```new StreamReader(new MemoryStream()).Dispose()``` the anonymous StreamReader 
instance and the anonymous MemoryStream instance are both disposed, because the StreamReader
keeps track of the given MemoryStream instance. 

These situation are detected within the extension, because there is a configuration file
that defines tracking types. Unfortunately this list is currently hard-coded an not yet 
editable by the user.

If you now notice, that there are certain constructor calls that does keep track of the 
given stream, then you are right. These situations are detected, too.

A tracking method is a method, that keeps track of an IDisposable. An Example is the 
following extension method from [Reactive Extensions](https://www.nuget.org/packages/System.Reactive/):
```csharp
public void AddTo<T>(this T disposable, ICollection<IDispoable> disposables) where T : IDispoable
``` 

A tracking factory is a class that created IDisposables for your and it keeps track of
these for you.   

## When is a field or property considered as disposed?
A field or property is considered disposed, if it gets disposed within a method
that is named Dispose and has no parameters.  

# Plans for the future

There should be some way to configure the extensions. There is lots of configuration hard-coded within the extensions:
* Tracking Types
* Tracking Methods
* Tracking Factory Methods
* Ignored Interfaces/Types
* Disposing Methods

Most of these pre-configured values are a good start for all projects. But sooner or later you will stuble upon a for instances tracking method of a private API you are using. These will unlikely became part of the extensions itself, but you should have a possiblity to configure these yourself.

Currenly there are three ideas:
* The addtion of annotations (like JetBrains.Annotations for ReSharper)
* Configurations per project via Addtional Files
* A mixture of both preceeding ideas

# Is there something that is not supported and will not be supported in future?
## Renaming
If you rename a variable and call Dispose on the renamed variable. E.g.
```csharp
var mem = new MemoryStream()
var mem2 = mem;
mem2.Dispose()
```  

# Some statisitics

|   |   |
|---|---|
|  Pull-Requests | ![](https://img.shields.io/github/issues-pr/badf00d/DisposableFixer.svg?style=flat) ![](https://img.shields.io/github/issues-pr-closed/badf00d/DisposableFixer.svg?style=flat) |
| Issues  | ![](https://img.shields.io/github/issues/badf00d/DisposableFixer.svg?style=flat) ![](https://img.shields.io/github/issues-closed/badf00d/DisposableFixer.svg?style=flat) ![](https://img.shields.io/github/issues/badf00d/DisposableFixer/bug.svg?colorB=red&label=bugs&style=flat) ![](https://img.shields.io/github/issues/badf00d/DisposableFixer/feature%20request.svg?label=feature%20request&style=flat) |
| Downloads  | ![Nuget](https://img.shields.io/nuget/dt/DisposableFixer.svg?label=nuget&color=lime) ![](https://img.shields.io/visual-studio-marketplace/d/DavidStormer.DisposableFixer.svg?label=Visual%20Studio%20Marketplace%20%28downloads%29) ![](https://img.shields.io/visual-studio-marketplace/i/DavidStormer.DisposableFixer.svg?label=Visual%20Studio%20Marketplace%20%28installs%29)|
| Contribution | ![GitHub last commit](https://img.shields.io/github/last-commit/Badf00d/DisposableFixer.svg) ![GitHub contributors](https://img.shields.io/github/contributors-anon/Badf00d/DisposableFixer.svg) | 


