Feature: Match

    Scenario: Can successfully compare using match object with regex
        Given the following JSON
        """
        {
          "a": "42",
          "b": "abc"
        }
        """
        And the expected JSON
        """
        {
            "a": {
                "__match": {
                    "regex": "\\d+"
                }
            },
            "b":"abc"
        }
        """
        When comparing both JSON
        Then the 2 json matched

    Scenario: Can detect diff when comparing using match object with regex
        Given the following JSON
        """
        {
          "a": "abc",
          "b": "abc"
        }
        """
        And the expected JSON
        """
        {
            "a": {
                "__match": {
                    "regex": "\\d+"
                }
            },
            "b": "abc"
        }
        """
        When comparing both JSON
        Then the json did not match with the following output
        """
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
        """

    Scenario: When __match object successfully match object, do not display it in diff when other property is not equal
        Given the following JSON
        """
        {
          "a": "abc",
          "b": "def"
        }
        """
        And the expected JSON
        """
        {
            "a": {
                "__match": {
                    "regex": ".+"
                }
            },
            "b": "abc"
        }
        """
        When comparing both JSON
        Then the json did not match with the following output
        """
        Given json does not match expected one:
          - b: Invalid value, expected 'abc' but found 'def'

        --- expected
        +++ actual
         {
           "a": "abc",
        -  "b": "abc"
        +  "b": "def"
         }
        """

    Scenario: Can compare using regex aliases
        Given the regex ""[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}(.[0-9]+)?(Z|\\+00:00)"" has been registered as an alias with the name "DateIso8601Utc"
        Given the following JSON
        """
        {
          "a": "2022-12-24T19:59:45Z",
          "b": "abc"
        }
        """
        And the expected JSON
        """
        {
            "a": {
                "__match": {
                    "regexAlias": "DateIso8601Utc"
                }
            },
            "b":"abc"
        }
        """
        When comparing both JSON
        Then the 2 json matched
