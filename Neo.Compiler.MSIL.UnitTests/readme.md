can use json for testdefine

use json test like

`JsonTestTool.TestAllCaseInJson("./TestJsons/test_newarray.json")

a sample json blew:

`{
`  "tests": {
`    "newarray": {
`      "build": [ "./TestClasses/Contract1.cs" ],
`      "testmethod": "execute",
`      "entryscript": "./TestClasses/Contract1.cs",
`      "testparams": [
`        "(str)test",
`        []
`      ],
`      "testresult": "(bytes)01020304"
`    }
`  }
`}

tests   testcasename=> testcase

>testcase
>{
>	 build:array for sources
>    testmethod:"execute" use neovm to run avm. will have other testmethod in future.
>	 entryscript,the entry script
>    testparams:in execute mode,this is the params for contract call.
>    testresult:in execute mode,this is the expect result used for test.
>}