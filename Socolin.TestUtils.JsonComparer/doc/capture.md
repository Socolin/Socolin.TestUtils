## Capture a value

It's possible to capture some values when a value is compared with a *capture object*, a callback will be called.

This can be used in functional testing to easily capture a value from a JSON and then use this value later.

For example to compare and extract the `id` field of the following JSON

```json
{
  "id": 4
}
```

You should use the following *capture object*

```json
{
  "id": {
    "__capture": {
      "name": "someCaptureName",
      "type": "integer"
    }
  }
}
```

### Example

#### Code

```cs
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault(((captureName, token) => {
  Console.WriteLine($"Captured value: name={captureName} token={token}");
}));


const string expectedJson = @"{""a"":{""__capture"":{""name"": ""some-name"", ""type"":""integer""}}, ""b"":""abc""}";
const string actualJson = @"{""a"":42, ""b"":""abc""}";

var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

### Output

```
Captured value: name=some-name token=42
No differences found
```

### Note

If there is any errors, to improve the readability of the output, the capture objects are replaced by their matching value in the expectedJToken.

#### Example

##### Code

```cs
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault(((captureName, token) => {
  Console.WriteLine($"Captured value: name={captureName} token={token}");
}));

const string expectedJson = @"{""a"":{""__capture"":{""name"": ""some-name"", ""type"":""integer""}}, ""b"":""abc""}";
const string actualJson = @"{""a"":42, ""b"":""def""}";


var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

##### Output

```diff
Captured value: name=some-name token=42
Given json does not match expected one:
  - b: Invalid value, expected 'abc' but found 'def'

--- expected
+++ actual
 {
   "a": 42,
-  "b": "abc"
+  "b": "def"
 }
```

## Capture using regex

To match and capture value of field id


```json
{
  "id": "B6E73AF8-BDB9-41B2-BB77-28575B08A28C"
}
```

```json
{
  "id": {
    "__capture": {
      "name": "some-global-capture-name",
      "regex": "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$"
    }
  }
}
```

### Example

#### Code

```cs
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault(((captureName, token) => {
    Console.WriteLine($"Captured value: name={captureName} token={token}");
}));

var expectedJson = JToken.Parse(@"{""a"":{""__capture"":{""name"": ""some-global-capture-name"", ""regex"":""^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$""}}}");
var actualJson = JToken.Parse(@"{""a"":""B6E73AF8-BDB9-41B2-BB77-28575B08A28C""}");

var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```
Captured value: name=some-global-capture-name token=B6E73AF8-BDB9-41B2-BB77-28575B08A28C
No differences found
```


## Capture using regex capture groups

### Example 1

#### Code

```cs
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault(((captureName, token) => {
    Console.WriteLine($"Captured value: name={captureName} token={token}");
}));

var expectedJson = JToken.Parse(@"{""a"":{""__capture"":{""name"": ""some-global-capture-name"", ""regex"":""(?<localCapture>bcd)""}}, ""b"":""def""}");
var actualJson = JToken.Parse(@"{""a"":""abcdef"", ""b"":""def""}");

var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```
Captured value: name=some-global-capture-name token=abcdef
Captured value: name=localCapture token=bcd
No differences found
```

### Example 2

#### Code

```cs
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault(((captureName, token) => {
    Console.WriteLine($"Captured value: name={captureName} token={token}");
}));

var expectedJson = JToken.Parse(@"{""a"":{""__capture"":{""regex"":""(?<localCapture>bcd)""}}, ""b"":""def""}");
var actualJson = JToken.Parse(@"{""a"":""abcdef"", ""b"":""def""}");

var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```
Captured value: name=localCapture token=bcd
No differences found
```
