Feature: Simple json

    Scenario: Can compare simple json
        Given the following JSON
        """
        {
            "a": 42,
            "b": "abc"
        }
        """
        And the expected JSON
        """
        {
            "a": 1,
            "b": "abc"
        }
        """
        When comparing both JSON
        Then the json did not match with the following output
        """
        Given json does not match expected one:
          - a: Invalid value, expected '1' but found '42'

        --- expected
        +++ actual
         {
        -  "a": 1,
        +  "a": 42,
           "b": "abc"
         }
        """

    Scenario: Can compare json with big int
        Given the following JSON
        """
        {
            "a": 5362689768,
        }
        """
        And the expected JSON
        """
        {
            "a": 5362689768
        }
        """
        When comparing both JSON
        Then the 2 json matched
