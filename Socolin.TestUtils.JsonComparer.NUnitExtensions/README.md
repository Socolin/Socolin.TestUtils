Extensions of NUnit to easily use Socolin.TestUtils.JsonComparer

## Usage

### Code

```cs
const string expectedJson = @"{
    ""a"":1,
    ""b"":""abc""
}";
const string actualJson = @"{
    ""a"":42,
    ""b"":""abc""
}";

Assert.That(actualJson, Is.JsonEquivalent(expectedJson));
```

### Output

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
