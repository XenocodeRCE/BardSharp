# Installation

## 1) BardAPI Python
```
$ pip install bardapi
```
```
$ pip install git+https://github.com/dsdanielpark/Bard-API.git
```

```python
import sys
from bardapi import Bard
from bardapi import BardCookies

if len(sys.argv) != 2:
    print("Usage: python main.py 'your_argument'")
    sys.exit(1)

argument = sys.argv[1]

cookie_dict = {
    %here%
}

bard = BardCookies(cookie_dict=cookie_dict)
print(bard.get_answer(argument)['content'])
```

The thing is, we will use a C# app to replace the `cookie_dict` value on the fly.

## 2) CockyGrabber

→ [CockyGrabber](https://github.com/MoistCoder/CockyGrabber)

```csharp
ChromeGrabber grabber = new ChromeGrabber(); // Create Grabber
var cookies = grabber.GetCookies(); // Collect all Cookies with GetCookies()

List<string> GoogleCookies = new List<string>();
string cookieChunk = "";
// Print the Hostname, Name, and Value of every cookie:
foreach (var cookie in cookies)
{
    if (cookie.Name.Contains("__Secure-1P")){ // sometimes you get rateLimited and need to wait 12h or switch account
        GoogleCookies.Add($"\"{cookie.Name}\" : \"{cookie.DecryptedValue}\",");
        cookieChunk = cookieChunk + Environment.NewLine + $"\"{cookie.Name}\" : \"{cookie.DecryptedValue}\",";
    }
}

cookieChunk = cookieChunk.Remove(cookieChunk.Length - 1);

string pythonSrc = @"
import sys
from bardapi import Bard
from bardapi import BardCookies

if len(sys.argv) != 2:
print(""Usage: python main.py 'your_argument'"")
sys.exit(1)

argument = sys.argv[1]

cookie_dict = {
%here%
}

bard = BardCookies(cookie_dict=cookie_dict)
print(bard.get_answer(argument)['content'])
";

string pythonSrcNew = pythonSrc.Replace("%here%", cookieChunk);
File.WriteAllText(PythonScriptPath, pythonSrcNew);

// Create a process to run the Python script
Process process = new Process();
process.StartInfo.FileName = "python"; // Use the "python" command
process.StartInfo.Arguments = $"{PythonScriptPath} \"{arguments}\"";
process.StartInfo.UseShellExecute = false;
process.StartInfo.RedirectStandardOutput = true;
process.StartInfo.RedirectStandardError = true;
process.StartInfo.CreateNoWindow = true;

// Start the process
process.Start();

// Read the output and error streams
string output = await process.StandardOutput.ReadToEndAsync();
string error = await process.StandardError.ReadToEndAsync();

// Wait for the process to exit
process.WaitForExit();

// Close the process
process.Close();// Create a process to run the Python script
```

And now you have :
A fully working c# app that takes an argument, pass it to the python script, return the data and reads it fine.
