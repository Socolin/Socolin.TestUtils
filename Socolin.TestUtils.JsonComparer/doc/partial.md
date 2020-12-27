## Partial compare

### Comparing objects

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

### Comparing arrays of object

To compare array of object, for now the only supported way is by specifying a key used to match the object. The reason is that
the compare does not know how to match objects otherwise.
So it can only partially compare array of object like the following example where the key would be `id`

The order of elements are ignored when comparing partial array


<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr>
<td>

```json
[
  {
    "id": 1,
    "name": "name1"
  },
  {
    "id": 2,
    "name": "name2"
  }
]
```
</td>
<td>

```json
{
  "field": {
    "__partialArray":{"key": "id", "array": [
      {
        "id": 1,
        "name": "name1"
      },
      {
        "id": 2,
        "name": "name2"
      }
    ]}
  }
}
```

</td>
</tr>
</table>


Example: Show invalid element

<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr>
<td>

```json
{
  "field": [
  {
    "id": 30,
    "name": "name0"
  },
  {
    "id": 42,
    "name": "name1"
  },
  {
    "id": 44,
    "name": "name2"
  }
]
}
```
</td>
<td>

```json
{
  "field": {
    "__partialArray":{"key": "id", "array": [
      {
        "id": 30,
        "name": "name0"
      },
      {
        "id": 42,
        "name": "name2"
      }
    ]}
  }
}
```

</td>
</tr>
<tr>
<td colspan="2">


```diff
Given json does not match expected one: 
  - name: Invalid value, expected 'name2' but found 'name1'

--- expected
+++ actual
 {
   "field": [
   {
     "id": 30,
     "name": "name0"
   },
   {
     "id": 42,
-    "name": "name2"
+    "name": "name1"
   }
   ]
 }
```

</td>
</tr>
</table>

------

Example: Show missing elements

<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr>
<td>

```json
{
  "field": [
    {
      "id": 30,
      "name": "name0"
    },
    {
      "id": 44,
      "name": "name2"
    }
  ]
}
```
</td>
<td>

```json
{
  "field": {
    "__partialArray":{"key": "id", "array": [
      {
        "id": 30,
        "name": "name0"
      },
      {
        "id": 42,
        "name": "name2"
      }
    ]}
  }
}
```

</td>
</tr>
<tr>
<td colspan="2">


```diff
Given json does not match expected one: 
  - field: Missing element identified by the key 42

--- expected
+++ actual
 {
   "field": [
     {
       "id": 30,
       "name": "name0"
-    },
-    {
-      "id": 42,
-      "name": "name2"
     }
   ]
 }
```

</td>
</tr>
</table>

### Comparing arrays of values (int or string)

**Not implemented yet** (Will do when someone need this)

```json
["a", "b", "c"]
```

```json
[1, 2, 3]
```
