There are 3 "main" methods in the library for retrieving and updating sugar modules.

* GetEntry() - gets a single entry as a SugarEntry
* SetEntry() - updates a single SugarEntry and returns the id
* GetEntryList() - returns a list of sugar enteies as List of SugarEntry

A SugarEntry object is a basically a Dictionary<string, string> of key value pairs for properties of a given module. The SugarEntry object also contains properties for id and module.

One of the pains of the sugarsoap api is that it requires a string array of fields you wish to select or update. Some of the overrides in CandyWrapper do not require these field names. In the case that you do not supply the fields, the module is queried for all fields; this field list is then cached and passed to the sugarsoap method.

Please refer to the usage example https://github.com/samkuehn/candywrapper/wiki/Usage and the integration test for usage examples https://github.com/samkuehn/candywrapper/blob/master/src/CandyWrapperTests/IntegrationTests.cs.