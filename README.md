[![Build status](https://ci.appveyor.com/api/projects/status/8mk3us0d5stlkq47?svg=true)](https://ci.appveyor.com/project/badf00d/disposablefixer)

# DisposableFixer
This is a small Visual Studio Extension that should identify and fix problems as memleaks while using IDispoables.

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


# Is there something that is not supported and will not be supported in future?
## Renaming
If you rename a variable and call Dispose on the renamed variable. E.g.
```csharp
var mem = new MemoryStream()
var mem2 = mem;
mem2.Dispose()
```  
