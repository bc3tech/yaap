# TBAAPI.V3Client.Model.Team
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Key** | **string** | TBA team key with the format &#x60;frcXXXX&#x60; with &#x60;XXXX&#x60; representing the team number. | 
**TeamNumber** | **int** | Official team number issued by FIRST. | 
**Nickname** | **string** | Team nickname provided by FIRST. | [optional] 
**Name** | **string** | Official long name registered with FIRST. | 
**SchoolName** | **string** | Name of team school or affilited group registered with FIRST. | [optional] 
**City** | **string** | City of team derived from parsing the address registered with FIRST. | [optional] 
**StateProv** | **string** | State of team derived from parsing the address registered with FIRST. | [optional] 
**Country** | **string** | Country of team derived from parsing the address registered with FIRST. | [optional] 
**Address** | **string** | Will be NULL, for future development. | [optional] 
**PostalCode** | **string** | Postal code from the team address. | [optional] 
**GmapsPlaceId** | **string** | Will be NULL, for future development. | [optional] 
**GmapsUrl** | **string** | Will be NULL, for future development. | [optional] 
**Lat** | **double** | Will be NULL, for future development. | [optional] 
**Lng** | **double** | Will be NULL, for future development. | [optional] 
**LocationName** | **string** | Will be NULL, for future development. | [optional] 
**Website** | **string** | Official website associated with the team. | [optional] 
**RookieYear** | **int** | First year the team officially competed. | [optional] 
**Motto** | **string** | Team&#39;s motto as provided by FIRST. This field is deprecated and will return null - will be removed at end-of-season in 2019. | [optional] 
**HomeChampionship** | [**Object**](.md) | Location of the team&#39;s home championship each year as a key-value pair. The year (as a string) is the key, and the city is the value. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

