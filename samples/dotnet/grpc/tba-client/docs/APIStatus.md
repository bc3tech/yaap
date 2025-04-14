# TBAAPI.V3Client.Model.APIStatus
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CurrentSeason** | **int** | Year of the current FRC season. | 
**MaxSeason** | **int** | Maximum FRC season year for valid queries. | 
**IsDatafeedDown** | **bool** | True if the entire FMS API provided by FIRST is down. | 
**DownEvents** | **List&lt;string&gt;** | An array of strings containing event keys of any active events that are no longer updating. | 
**Ios** | [**APIStatusAppVersion**](APIStatusAppVersion.md) |  | 
**Android** | [**APIStatusAppVersion**](APIStatusAppVersion.md) |  | 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

