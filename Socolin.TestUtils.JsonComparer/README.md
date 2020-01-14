# Json Comparer

This library provides a simple way to compare two [JSON](https://www.json.org/), with an easily readable output to find what differs.

Features:
  - Compare JSON and returns a list of errors.
  - Compare a value using a regex
  - Capture a value

NuGet: https://www.nuget.org/packages/Socolin.TestUtils.JsonComparer/

-------

## Simple compare

### Example

#### Code

```cs
const string expectedJson = @"{
    ""a"":1,
    ""b"":""abc""
}";
const string actualJson = @"{
    ""a"":42,
    ""b"":""abc""
}";
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```diff
Given json does not match expected one:
  - a: Invalid value, expected '1' but found '42'

--- expected
+++ actual
 {
-  "a": 1,
+  "a": 42,
   "b": "abc"
 }
```

-------


## Compare using regex

It's possible to compare strings with a regex, using a custom object with a property `__match`.

For example, in the following json, to compare the key `compareMeWithARegex` with the `[a-z]+`

```json
{
  "compareMeWithARegex": "something"
}
```

The expected json given to the comparer should be


```json
{
  "compareMeWithARegex": {
    "__match": {
      "regex": "[a-z]+"
    }
  }
}
```


### Example 1

#### Code

```cs
Console.WriteLine("==== MatchExample.Test1 ==== ");
const string expectedJson = @"{
    ""a"":{
        ""__match"":{
            ""regex"": ""\\d+""
        }
    },
    ""b"":""abc""
}";
const string actualJson = @"{
    ""a"":""42"",
    ""b"":""abc""
}";
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```diff
No differences found
```

### Example 2

#### Code


```cs
const string expectedJson = @"{
    ""a"":{
        ""__match"":{
            ""regex"": ""\\d+""
        }
    },
    ""b"":""abc""
}";
const string actualJson = @"{
    ""a"":""abc"",
    ""b"":""abc""
}";
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```diff
Given json does not match expected one:
  - a: Invalid value, 'abc' should match regex '\d+'

--- expected
+++ actual
 {
-  "a": {
-    "__match": {
-      "regex": "\\d+"
-    }
-  },
+  "a": "abc",
   "b": "abc"
 }
```


### Note

If there is any errors, to improve the readability of the output, the *match objects* are replaced by their matching value in the expectedJToken.

#### Example

##### Code

```cs
var expectedJToken = JToken.Parse(@"{
    ""a"":{
        ""__match"":{
            ""regex"": "".+""
        }
    },
    ""b"":""abc""
}");
var actualJToken = JToken.Parse(@"{
    ""a"":""abc"",
    ""b"":""def""
}");
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJToken, actualJToken);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
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

-------

## Compare using type


It's possible to compare object and expect to be of a certain type, without looking at the value, using a custom object with a property `__match`.

For example, in the following json, to verify the value associated to the key `compareMeByType`  is an integer

```json
{
  "compareMeByType": 42
}
```

The expected json given to the comparer should be


```json
{
  "compareMeByType": {
    "__match": {
      "type": "integer"
    }
  }
}
```


### Example

#### Code

```cs
Console.WriteLine("==== MatchExample.Test4 ==== ");
const string expectedJson = @"{
    ""a"":{
        ""__match"":{
            ""type"": ""integer""
        }
    },
    ""b"":""abc""
}";
const string actualJson = @"{
    ""a"":42,
    ""b"":""abc""
}";
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```diff
No differences found
```

-------

## Compare using range


It's possible to use a range to compare a numeric value, using a custom object with a property `__match`.

For example, in the following json, to verify the value associated to the key `compareMeInRange` is in a given range

```json
{
  "compareMeByType": 42
}
```

The expected json given to the comparer should be


```json
{
  "compareMeInRange": {
    "__match": {
      "range": [40, 45]
    }
  }
}
```


### Example

#### Code

```cs
const string expectedJson = @"{
    ""a"":{
        ""__match"":{
            ""range"": [95, 105]
        }
    },
    ""b"": {
        ""__match"":{
            ""range"": [95, 105]
        }
    }
}";
const string actualJson = @"{
    ""a"": 95,
    ""b"": 105
}";
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```diff
No differences found
```

-------

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

## Partial compare

If you want to compare a large json, but only test a small set you can use `__partial` object.

For example, in the following json, to compare only the key `compareMe` and ignore `doNotCompareMe1` and `doNotCompareMe2`

```json
{
  "compareMe": "something",
  "doNotCompareMe1": "something",
  "doNotCompareMe2": "something"
}
```

The expected json given to the comparer should be

```json
{
  "__partial": {
    "compareMe": "something"
  }
}
```

### Example


#### Example 1
```cs
const string expectedJson = @"{
    ""a"":{
        ""__partial"":{
            ""tested"": ""123""
        }
    },
    ""b"":""abc""
}";
const string actualJson = @"{
    ""a"": {
        ""tested"": ""123"",
        ""ignored"": ""42""
    },
    ""b"":""abc""
}";

var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJson, actualJson);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors));
```

#### Output

```
No differences found
```

#### Example 2

```cs

const string expectedJson = @"{
    ""a"":{
        ""__partial"":{
            ""tested"": ""123"",
            ""missing"": ""123""
        }
    },
    ""b"":""abc""
}";
const string actualJson = @"{
    ""a"": {
        ""tested"": ""123"",
        ""ignored"": ""42""
    },
    ""b"":""abc""
}";

var expectedJToken = JToken.Parse(expectedJson);
var actualJToken = JToken.Parse(actualJson);

var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJToken, actualJToken);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
```


#### Output

```diff
Given json does not match expected one:
  - : Missing property `missing`

--- expected
+++ actual
 {
   "a": {
-    "missing": "123",
     "tested": "123"
   },
   "b": "abc"
 }
```
