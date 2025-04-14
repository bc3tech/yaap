# TBAAPI.V3Client.Model.Media
The `Media` object contains a reference for most any media associated with a team or event on TBA.
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Type** | **string** | String type of the media element. | 
**ForeignKey** | **string** | The key used to identify this media on the media site. | 
**Details** | [**Object**](.md) | If required, a JSON dict of additional media information. | [optional] 
**Preferred** | **bool** | True if the media is of high quality. | [optional] 
**DirectUrl** | **string** | Direct URL to the media. | [optional] 
**ViewUrl** | **string** | The URL that leads to the full web page for the media, if one exists. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

