# Json Comparer

[![Nuget](https://img.shields.io/nuget/v/Socolin.TestUtils.JsonComparer)](https://www.nuget.org/packages/Socolin.RabbitMQ.Client)

This library provides a simple way to compare two [JSON](https://www.json.org/), with an easily readable output to find what differs.

Features:
  - Compare JSON and returns a list of errors.
  - Compare a value using a regex
  - Compare a number to match a range
  - Compare a value ignoring some fields
  - Compare partial objects
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
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJson, actualJson, errors, useColor: true));
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

## Special compare with `__match`

It's possible to compare a field with an "approximate" value using regex, range or other fuzzy compare method ([more example](doc/match.md)).

This is done using a special object `__match` like this

<table>
<tr><td>

```json
{
    "field": "something"
}
```

</td>
<td>

```json
{
  "field": {"__match": {
    "regex": "[a-z]+"
  }}
}
```

</td></tr>
</table>

You can find more detailed `__match` [example here](doc/match.md)

### Note

When using `Compare(JToken expected, JToken actual)` if there is any errors, to improve the readability of the output, the *match objects* are replaced by their matching value in the expectedJToken.

This is **NOT** done when using `Compare(string expectedJson, string actualJson)`

#### Example

So when comparing with those 2 json

<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr>
<td>

```json
{
  "a": "abc",
  "b": "def"
}
```
</td>
<td>

```json
{
  "a":{ "__match":{
    "regex": ".+"
  }},
  "b":"abc"
}
```

</td>
</tr>
</table>

With this code

```cs
var expectedJToken = JToken.Parse(expecteJsonAsString);
var actualJToken = JToken.Parse(actualJsonAsString);
var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var errors = jsonComparer.Compare(expectedJToken, actualJToken);
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
```

Then the output will be 

```diff
 {
   "a": 42,
-  "b": "abc"
+  "b": "def"
 }
```

Instead of this

```diff
 {
+  "a": 42,
-  "a":{
-  "__match":{
-    "regex": ".+"
-  }
-  "b": "abc"
+  "b": "def"
 }
```

## Capture a value

It's possible to capture some values when a value is compared with a *capture object*, a callback will be called.

This can be used in functional testing to easily capture a value from a JSON and then use this value later.

See [this documentation](doc/capture.md)


## Partial compare

If you want to compare a large json, but only test a small set you can use `__partial` object.

For example, in the following json, to compare only the key `compareMe` and ignore `doNotCompareMe1` and `doNotCompareMe2`

### Example 1

<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr>
<td>

```json
{
  "compareMe": "something",
  "doNotCompareMe1": "something",
  "doNotCompareMe2": "something"
}
```
</td>
<td>

```json
{
  "__partial": {
    "compareMe": "something"
  }
}
```

</td>
</tr>
</table>

### Example 2


<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr>
<td>

```json
{
  "a": {
    "tested": "123",
    "ignored": "42"
  },
  "b":"abc"
}
```
</td>
<td>

```json
{
  "a":{
    "__partial":{
      "tested": "123",
      "missing": "123"
    }
  },
  "b":"abc"
}
```

</td>
</tr>
<tr>
<td colspan="2">


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

</td>
</tr>
</table>


## Ignoring properties

During comparison it's possible to ignore some fields. For that you need to give a function that will be called
for each field to know if they need to be ignored or not, like in the following example.

Ignored fields are removed from the `JObject`s so when the diff is displayed using `GetReadableMessage` the ignored
field are not polluting the output

### Example

#### Example 1

```cs
const string expectedJson = @"{
    ""a"":{
        ""b"": ""ignore-me-12"",
        ""notInActual"": ""ignore-me-12"",
        ""c"": ""compare-me"",
    },
    ""d"": ""compare-me""
}";
const string actualJson = @"{
    ""a"":{
        ""b"": ""ignore-me-45"",
        ""notInExpected"": ""ignore-me-45"",
        ""c"": ""compare-me"",
    },
    ""d"": ""compare-me""
}";

var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var expectedJToken = JToken.Parse(expectedJson);
var actualJToken = JToken.Parse(actualJson);
var errors = jsonComparer.Compare(expectedJToken, actualJToken, new JsonComparisonOptions
{
    IgnoreFields = (fieldPath, fieldName) => fieldPath == "a.b" || fieldName == "notInExpected" || fieldName == "a.notInActual"
});
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
```

#### Example 2

```cs
const string expectedJson = @"{
    ""a"":{
        ""b"": ""ignore-me-12"",
        ""notInActual"": ""ignore-me-12"",
        ""c"": ""compare-me"",
    },
    ""d"": ""compare-me""
}";
const string actualJson = @"{
    ""a"":{
        ""b"": ""ignore-me-45"",
        ""notInExpected"": ""ignore-me-45"",
        ""c"": ""compare-me"",
    },
    ""d"": ""compare-me""
}";

var jsonComparer = TestUtils.JsonComparer.JsonComparer.GetDefault();
var expectedJToken = JToken.Parse(expectedJson);
var actualJToken = JToken.Parse(actualJson);
var errors = jsonComparer.Compare(expectedJToken, actualJToken, new JsonComparisonOptions
{
    IgnoreFields = (fieldPath, fieldName) => fieldName == "notInExpected"
                                             || fieldName == "notInActual"
});
Console.WriteLine(JsonComparerOutputFormatter.GetReadableMessage(expectedJToken, actualJToken, errors));
```

```
No differences found
```

```diff
Given json does not match expected one:
  - a.b: Invalid value, expected 'ignore-me-12' but found 'ignore-me-45'

--- expected
+++ actual
 {
   "a": {
-    "b": "ignore-me-12",
+    "b": "ignore-me-45",
     "c": "compare-me"
   },
   "d": "compare-me"
 }
```
