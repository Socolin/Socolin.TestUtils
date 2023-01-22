Feature: Partial

    Scenario: Can successfully compare using partial object
        Given the following JSON
        """
        {
            "a": {
                "tested": "123",
                "ignored": "42"
            },
            "b":"abc"
        }
        """
        And the expected JSON
        """
        {
            "a":{
                "__partial":{
                    "tested": "123"
                }
            },
            "b":"abc"
        }
        """
        When comparing both JSON
        Then the 2 json matched

    Scenario: Can detect missing properties when using partial object
        Given the following JSON
        """
        {
            "a": {
                "tested": "123",
                "ignored": "42"
            },
            "b":"abc"
        }
        """
        And the expected JSON
        """
        {
            "a":{
                "__partial":{
                    "tested": "123",
                    "missing": "123"
                }
            },
            "b":"abc"
        }
        """
        When comparing both JSON
        Then the json did not match with the following output
        """
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
        """

    Scenario: Can successfully compare using partial array of values
        Given the following JSON
        """
        {
          "array": [
            "a",
            "b",
            "c"
          ]
        }
        """
        And the expected JSON
        """
        {
            "array":{
                "__partialArray": {"array": [
                    "a", "b"
                ]}
            }
        }
        """
        When comparing both JSON
        Then the 2 json matched


    Scenario: Can detect missing value when using partial array
        Given the following JSON
        """
        {
          "array1": [
            "a",
            "b",
            "c"
          ],
          "array2": [
            "a"
          ]
        }
        """
        And the expected JSON
        """
        {
            "array1":{
                "__partialArray": {"array": [
                    "a", "b"
                ]}
            },
            "array2": {
                "__partialArray": {"array": [
                    "a", "b"
                ]}
            }
        }
        """
        When comparing both JSON
        Then the json did not match with the following output
        """
        Given json does not match expected one:
          - array2: Missing element identified by the key b

        --- expected
        +++ actual
         {
           "array1": [
             "a",
             "b"
           ],
           "array2": [
        -    "a",
        -    "b"
        +    "a"
           ]
         }
        """
