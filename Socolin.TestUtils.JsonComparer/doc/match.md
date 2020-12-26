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
<tr><td colspan="2">
No differences found
</td></tr>
</table>
