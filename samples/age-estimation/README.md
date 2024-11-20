# Age Estimation Workflow
This example provides a Workflow that can estimate someone's age, 
based on their given name. It uses a single Activity implementation, 
which calls a remote API to retrieve this estimation.

To run this sample, first, start the Worker:

```
dotnet run --project Worker
```

In addition to the Worker, it also includes a file `Client/Program.cs` 
that you can use to run the Workflow (pass a name to use as input 
on the command line when invoking it). 

```
doetnet run --project Client Betty
```

This will output a message with the name and estimated age:

```
Betty has an estimated age of 77
```

This queries an external API which randomizes the age so the output might not be exactly the same.

Additionally, this example provides tests for the Workflow 
and Activity code.