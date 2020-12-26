## Match a string using a regex


<table>
<tr><td>Actual</td><td>Expected</td></tr>
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



## Match using type

It's possible to compare a field and just check if the type is what expected, like `string`, `integer`...


<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr><td>


```json
{
  "compareMeByType": 42
}
```

</td>
<td>

```json
{
  "compareMeByType": {
    "__match": {
      "type": "integer"
    }
  }
}
```

</td></tr>
</table>

## Match an integer using a range


<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr><td>


```json
{
  "compareMeInRange": 42
}
```

</td>
<td>

```json
{
  "compareMeInRange": {
    "__match": {
      "range": [40, 45]
    }
  }
}
```

</td></tr>
</table>

## Match a string ignoring some characters

<table>
<tr><td>Actual</td><td>Expected</td></tr>
<tr><td>


```json
{
  "a": "Hello\r\nWorld"
}
```

</td>
<td>

```json
{
  "a": {"__match":{
    "value": "Hello\nWorld",
    "ignoredCharacters": "\r"
  }}
}
```

</td></tr>
</table>


