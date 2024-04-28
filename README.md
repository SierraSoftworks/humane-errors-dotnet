# Humane Errors
**Errors which make your users' lives easier**

This library provides support for integrating self-documenting error messages into your .NET applications,
enabling your users to easily identify the cause of a problem and take appropriate steps to resolve it.

When writing code, the person with the best sense of why something might fail and what to do about it is
invariably the person writing the code (at the time they're writing it). There's nothing worse than being
faced with a generic error message that lacks any useful context and requires you to dig through the original
source code trying to figure out what went wrong.

With Humane Errors, you can attach suggestions for your future self (or other users) directly to the exceptions
you throw, cutting out the guesswork and greatly reducing time to mitigate.

## Features
 - **No Dependencies** and a target framework of .NET Standard 2.0 means you can use this library in any .NET project with ease.
 - **Incremental Adoption** which does not modify the types of your existing exceptions, removing the need for changes to your existing flow control.

## Example

```csharp
using System.IO;

using SierraLib.HumaneErrors;

public void DoSomething()
{
    try
    {
        File.Open("file.txt", FileMode.Open);
    }
    catch (Exception e)
    {
        throw e.Humanize(
            failureMode: "We couldn't open 'file.txt' for reading.",
            suggestions: new [] {
                "Make sure the file exists.",
                "Make sure that you have permission to read the file."
            }
        );
    }
}

public static void Main(string[] args)
{
    try
    {
        DoSomething();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToHumaneString());
    }
}
```

Running this code will print something similar to the following:

```plaintext
System.IO.FileNotFoundException: We couldn't open 'file.txt' for reading. (The file could not be found)
   at /home/user/dev/your-project/Program.cs:line 12

Suggestions:
   - Make sure the file exists.
   - Make sure that you have permission to read the file.

Original Exception:
System.IO.FileNotFoundException: The file could not be found.
   at System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share, Int32 bufferSize, FileOptions options)
   at System.IO.FileStream..ctor(String path, FileMode mode)
   at System.IO.File.Open(String path, FileMode mode)
   at Program.DoSomething() in /home/user/dev/your-project/Program.cs:line 12
   at Program.Main(String[] args) in /home/user/dev/your-project/Program.cs:line 20
```