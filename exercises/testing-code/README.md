# Exercise 3: Testing the Translation Workflow

During this exercise, you will

- Run a unit test provided for the `TranslateTerm` Activity
- Develop and run your own unit test for the `TranslateTerm` Activity
- Write assertions for a Workflow test
- Uncover, diagnose, and fix a bug in the Workflow Definition
- Observe the time-skipping feature in the Workflow test environment

Make your changes to the code in the `practice` subdirectory (look for `TODO` comments that will guide you to where you should make changes to the code). If you need a hint or want to verify your changes, look at the complete version in the `solution` subdirectory.

## Part A: Running a Test

We have provided a unit test for the `TranslateTerm` Activity to get you started. 

This test verifies that the Activity correctly translates the term "Hello" to German. Take a moment to study the test, which you'll find in the [Tests/ActivityTest.cs](./practice/Tests/ActivityTest.cs) file. 

Since the test runs the Activity, which in turn calls the microservice to do the translation, you'll begin by starting that.

1. Open a new terminal and run: 
`cd exercises/testing-code/practice/Web` and `dotnet run`

2. Open another terminal and run the test: 
`cd exercises/testing-code/practice/Tests`
and 
`dotnet test`

## Part B: Write and Run Another Test for the Activity

Now it's time to develop and run your own unit test, this time verifying that the Activity correctly supports the translation of a different word in a different language.

1. Edit the [Tests/ActivityTest.cs](./practice/Tests/ActivityTest.cs) file
2. Copy the `TestSuccessfulTranslateActivityHelloGerman` function, renaming the new function as `TestSuccessfulTranslateActivityGoodbyeLatvian`
3. Change the term for the input from `Hello` to `Goodbye`
4. Change the language code for the input from `de` (German) to `lv` (Latvian)
5. Assert that translation returned by the Activity is `ardievu`

## Part C: Test the Activity with Invalid Input

In addition to verifying that your code behaves correctly when used as you intended, it is sometimes also helpful to verify its behavior with unexpected input. The example below does this, testing that the Activity returns the appropriate error when called with an invalid language code.

Take a moment to study this code, and then continue with the following steps:

```csharp
public async Task TestFailedTranslateActivityBadLanguageCodeAsync()
{
    var env = new ActivityEnvironment();
    var input = new Activities.TranslateTermInput("Hello", "xq");

    var activities = new Activities(Client);

    Task<Activities.TranslateTermOutput> ActAsync() => env.RunAsync(() => activities.TranslateTermAsync(input));

    var exception = await Assert.ThrowsAsync<HttpRequestException>(ActAsync);
    Assert.Contains("Response status code does not indicate success", exception.Message);
}
```

1. Copy the entire `TestFailedTranslateActivityBadLanguageCode` function provided above and paste it at the bottom of the [Tests/ActivityTest.cs](./practice/Tests/ActivityTest.cs) file
2. Save the changes
3. Run `cd exercises/testing-code/practice/Tests`
and `dotnet test` again to run this new test, in addition to the others

## Part D: Test a Workflow Definition

1. Edit the [Tests/WorkflowTest.cs](./practice/Tests/WorkflowTest.cs) file
2. Remove the Skip parameter from the Fact attribute to ensure the test runs.
3. Add assertions for the following conditions
   - The `HelloMessage` field in the result is `bonjour, Pierre`
   - The `GoodbyeMessage` field in the result is `au revoir, Pierre`
4. Save your changes
5. Run `cd exercises/testing-code/practice/Tests`
and `dotnet test` again to run this new test. This will fail, due to a bug in the Workflow Definition.
6. Find and fix the bug in the Workflow Definition
7. Run the `dotnet test` command again to verify that you fixed the bug

There are two things to note about this test.

First, the test completes in under a second, even though the Workflow Definition contains a `DelayAsync` call that adds a 10-second delay to the Workflow Execution. This is because of the time-skipping feature provided by the test environment.

Second, calls to `AddActivity` indicate that the Activity Definitions are executed as part of this Workflow test. As you learned, you can test your Workflow Definition in isolation from the Activity implementations by using mocks. The optional exercise that follows provides an opportunity to try this for yourself.

## Part E (Optional) Using Mock Activities in a Workflow Test

If you have time and would like an additional challenge, continue with the following steps.

1. Make a copy of the existing Workflow test file named `WorkflowMockTest.cs`
2. Edit the `WorkflowMockTest.cs` file
3. Rename the test function to `TestSuccessfulTranslationWithMock`
5. Replace the registration of the `TranslateTerm` Activity with our new mock
6. Save your changes
7. Run `cd exercises/testing-code/practice/Tests` and `dotnet test` to run the tests
