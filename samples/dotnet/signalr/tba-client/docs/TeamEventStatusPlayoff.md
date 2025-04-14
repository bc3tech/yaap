# TBAAPI.V3Client.Model.TeamEventStatusPlayoff
Playoff status for this team, may be null if the team did not make playoffs, or playoffs have not begun.
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Level** | **string** | The highest playoff level the team reached. | [optional] 
**CurrentLevelRecord** | [**WLTRecord**](WLTRecord.md) |  | [optional] 
**Record** | [**WLTRecord**](WLTRecord.md) |  | [optional] 
**Status** | **string** | Current competition status for the playoffs. | [optional] 
**PlayoffAverage** | **int** | The average match score during playoffs. Year specific. May be null if not relevant for a given year. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

