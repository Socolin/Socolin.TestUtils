## 1.16

- Support array of string/number for `__partialArray`

## 1.15.1

- Support regex alias on `__capture` too

## 1.15

- Add support for alias when comparing with regex. This reduce copy/paste of common regexes, like when checking date.
- Update dependencies

## 1.14

- Rework colorization:
  - It's now possible to disable the color even when it was enabled with `NO_COLOR` (https://no-color.org/)
  - Support customization of the colors
  - Support colorizing the json in the diff output

## 1.13

- Fix comparing with large int value (int64)

## 1.12

- Add new `__partialArray` to compare array. See the [documentation](doc/partial.md) for more details

## 1.11

- Add new `__match` version that allow to compare string while ignoring some characters (useful for stuff like `\r\n` `\n` for test running on linux and windows)

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


## 1.10.2

- Enable [sourcelink](https://github.com/dotnet/sourcelink)

## 1.10
_
- When given json is invalid, throw an exception that include the json that show what the error is instead of describing it

## 1.9

- Output is now colorful ! (Compatible with Rider test runner). Set `useColor` parameter to `true` or with the NUnit extension, just add `.WithColoredOutput()`

## 1.8

- Add a new option on the comparer to be able to ignore some fields using a lambda to identify them

## 1.7

- Copy `NGitDiff` in the project so we can compile it using netstandard and fix warnings when building with .NET Core

## 1.6

- Add new option in `__match` to check if a number is within a range

## 1.5

- Add new object `__partial` to partially match an object
- Fix comparing date

## 1.4

- Support regex in capture object, allow capturing partial value

## 1.3

- Add NUnit extensions (new nuget) to make this comparer simple to use
- Add new option in `__match` object to match object by types

## 1.2

- When comparing with matching object, replace `__match` object with the actual value when it matches. This reduce noise in output.

## 1.1

- Fix output on windows.
- Allow to match some fields of the json with a regex

## 1.0

- Initial JSON Comparer version.
- Compare 2 json and list differences
- Allow to capture some field to be store in a context
- Nice readable output with a diff between actual and expected JSON
