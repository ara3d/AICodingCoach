# AI Coding Coach

An interactive C# programming teaching tool with AI assistant.

The AI Coding Coach has been built as a submission for 
[The Microsoft .NET 8 Global Hack](https://github.com/microsoft/Hack-Together-DotNet).

# About

The AI Coding Coach integrates a syntax color text editor, a C# compiler, 
a drawing canvas, and an interface to GPT-4 API designed to help people learn to code. 

# Code Example

The compiler automatically compiles C# code as it is entered into the editor.
If a class has a `Draw` metod which accepts a single parameter of type 
[`DrawingContext`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.drawingcontext).  

For example:

```csharp
using System.Windows;
using System.Windows.Media;

public class GradientExample
{
 
    public void Draw(DrawingContext context)
    {
        var gradientBrush = new LinearGradientBrush();
        gradientBrush.StartPoint = new Point(0, 0); 
        gradientBrush.EndPoint = new Point(1, 1);   
        gradientBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.0)); 
        gradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));  
        context.DrawRectangle(gradientBrush, null, new Rect(50, 50, 300, 200));
    }
}
```

# Prompts 

The GPT Chat API is provided with a system prompt to guide it to produce 
code that can be executed by the compiler. 

If you right click in the prompt window, the context menu provides a list of 
predefined prompts you can choose from. 

If you highlight text in the code editor, you can right click and choose from 
the context menu an option to ask Chat GPT to explain the code for you. 

# Building the Code

Compiling the source code for AICodingCoach as-is requires that you first 
clone the [Ara3D repository](https://github.com/ara3d/ara3d) so that both repositories 
share the same parent folder. If you clone the Ara3D repository into a different 
location you will have to update the AICodingCoach project manually so that it can find
the project references. 

# Inspiration and References

* [SignalChat](https://github.com/MeshackMusundi/SignalChat) by Meshack Musundi
* [Demystifying Retrieval Augmented Generation with .NET](https://devblogs.microsoft.com/dotnet/demystifying-retrieval-augmented-generation-with-dotnet/) by Stephen Toub
* [EZRX Scripting for Grasshopper](https://github.com/EZ-Script/EZRX-Scripting) by EZRX Team for AECTech NY 2023 Hackathon
* [OpenAI for C#/.NET](https://rogerpincombe.com/openai-dotnet-api) by Roger Pincombe
* [Revit.ScriptCS](https://github.com/sridharbaldava/Revit.ScriptCS) by Sridhar Baldava
* [RoslynPad](https://github.com/roslynpad/roslynpad) by Eli Arbel.
* [AvalonEdit](https://github.com/icsharpcode/AvalonEdit) by Daniel Grunwald 
* [Ara 3D libraries](https://github.com/ara3d/ara3d) by Ara 3D.
 
