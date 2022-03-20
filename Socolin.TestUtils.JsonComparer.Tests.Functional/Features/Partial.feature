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
