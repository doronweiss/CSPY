using System;
using System.IO;
using Python.Runtime; // Make sure to install the 'pythonnet' NuGet package

namespace CSTester;

internal class Program {
  static void Main(string[] args) {
    // 1. Point to your local Python installation DLL
    // Update this string with your exact Python version and path!
    // Windows example: @"C:\Users\<User>\AppData\Local\Programs\Python\Python311\python311.dll"
    // Mac/Linux example: "/usr/local/lib/libpython3.11.dylib" or "libpython3.11.so"
    string user = Environment.UserName;
    Runtime.PythonDLL = $@"c:\Users\{user}\AppData\Local\Python\pythoncore-3.14-64\python314.dll";

    // 2. Resolve paths for the python folder
    // We find 'pyfolder' assuming it lives parallel to or outside your executable's execution directory
    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    string pyfolderPath = @"c:\Tries\CSPY\Pylib";

    Console.WriteLine($"Looking for Python script in: {pyfolderPath}");

    // 3. Fire up the Python engine
    PythonEngine.Initialize();

    try {
      // 4. Acquire the Global Interpreter Lock (GIL)
      using (Py.GIL()) {
        // 5. Inject 'pyfolder' into Python's sys.path so it knows where to look
        dynamic sys = Py.Import("sys");
        sys.path.append(pyfolderPath);

        // 6. Import your script file (pf.py -> "pf")
        dynamic pfScript = Py.Import("expmethods");

        // 7. Invoke the function and capture the output dynamically
        int num1 = 15;
        int num2 = 27;

        // PythonNET dynamically marshals the C# integers into Python ints and back
        dynamic pythonResult = pfScript.Add2(num1, num2);

        // 8. Convert the dynamic Python variable back into a standard C# int
        int finalResult = (int)pythonResult;

        Console.WriteLine($"\nSuccess!");
        Console.WriteLine($"Result returned from Python function 'add2({num1}, {num2})': {finalResult}");

        // person test
        dynamic personInstance = pfScript.person("Alice", 1995);
        dynamic resultFromFunction = pfScript.age(personInstance);
        Console.WriteLine($"Result from function: {resultFromFunction}"); // Outputs: 31

        // 4. Alternatively, you can also call the method on the object directly!
        dynamic resultFromMethod = personInstance.age();
        Console.WriteLine($"Result directly from method: {resultFromMethod}"); // Outputs: 31
                                                                               // 5. You can even read the object's properties directly from C#
        string name = personInstance.name;
        int birthYear = personInstance.byear;
        Console.WriteLine($"{name} was born in {birthYear}.");
      }
    } catch (PythonException ex) {
      // Catch native Python exceptions (Syntax errors, Type errors, missing files, etc.)
      Console.WriteLine($"Python Error: {ex.Message}");
    } finally {
      // 9. Shutdown the engine cleanly when your application ends
      PythonEngine.Shutdown();
    }
  }
}