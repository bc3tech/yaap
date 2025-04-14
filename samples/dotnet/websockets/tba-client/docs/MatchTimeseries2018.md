# TBAAPI.V3Client.Model.MatchTimeseries2018
Timeseries data for the 2018 game *FIRST* POWER UP. *WARNING:* This is *not* official data, and is subject to a significant possibility of error, or missing data. Do not rely on this data for any purpose. In fact, pretend we made it up. *WARNING:* This model is currently under active development and may change at any time, including in breaking ways.
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**EventKey** | **string** | TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event. | [optional] 
**MatchId** | **string** | Match ID consisting of the level, match number, and set number, eg &#x60;qm45&#x60; or &#x60;f1m1&#x60;. | [optional] 
**Mode** | **string** | Current mode of play, can be &#x60;pre_match&#x60;, &#x60;auto&#x60;, &#x60;telop&#x60;, or &#x60;post_match&#x60;. | [optional] 
**Play** | **int** |  | [optional] 
**TimeRemaining** | **int** | Amount of time remaining in the match, only valid during &#x60;auto&#x60; and &#x60;teleop&#x60; modes. | [optional] 
**BlueAutoQuest** | **int** | 1 if the blue alliance is credited with the AUTO QUEST, 0 if not. | [optional] 
**BlueBoostCount** | **int** | Number of POWER CUBES in the BOOST section of the blue alliance VAULT. | [optional] 
**BlueBoostPlayed** | **int** | Returns 1 if the blue alliance BOOST was played, or 0 if not played. | [optional] 
**BlueCurrentPowerup** | **string** | Name of the current blue alliance POWER UP being played, or &#x60;null&#x60;. | [optional] 
**BlueFaceTheBoss** | **int** | 1 if the blue alliance is credited with FACING THE BOSS, 0 if not. | [optional] 
**BlueForceCount** | **int** | Number of POWER CUBES in the FORCE section of the blue alliance VAULT. | [optional] 
**BlueForcePlayed** | **int** | Returns 1 if the blue alliance FORCE was played, or 0 if not played. | [optional] 
**BlueLevitateCount** | **int** | Number of POWER CUBES in the LEVITATE section of the blue alliance VAULT. | [optional] 
**BlueLevitatePlayed** | **int** | Returns 1 if the blue alliance LEVITATE was played, or 0 if not played. | [optional] 
**BluePowerupTimeRemaining** | **string** | Number of seconds remaining in the blue alliance POWER UP time, or 0 if none is active. | [optional] 
**BlueScaleOwned** | **int** | 1 if the blue alliance owns the SCALE, 0 if not. | [optional] 
**BlueScore** | **int** | Current score for the blue alliance. | [optional] 
**BlueSwitchOwned** | **int** | 1 if the blue alliance owns their SWITCH, 0 if not. | [optional] 
**RedAutoQuest** | **int** | 1 if the red alliance is credited with the AUTO QUEST, 0 if not. | [optional] 
**RedBoostCount** | **int** | Number of POWER CUBES in the BOOST section of the red alliance VAULT. | [optional] 
**RedBoostPlayed** | **int** | Returns 1 if the red alliance BOOST was played, or 0 if not played. | [optional] 
**RedCurrentPowerup** | **string** | Name of the current red alliance POWER UP being played, or &#x60;null&#x60;. | [optional] 
**RedFaceTheBoss** | **int** | 1 if the red alliance is credited with FACING THE BOSS, 0 if not. | [optional] 
**RedForceCount** | **int** | Number of POWER CUBES in the FORCE section of the red alliance VAULT. | [optional] 
**RedForcePlayed** | **int** | Returns 1 if the red alliance FORCE was played, or 0 if not played. | [optional] 
**RedLevitateCount** | **int** | Number of POWER CUBES in the LEVITATE section of the red alliance VAULT. | [optional] 
**RedLevitatePlayed** | **int** | Returns 1 if the red alliance LEVITATE was played, or 0 if not played. | [optional] 
**RedPowerupTimeRemaining** | **string** | Number of seconds remaining in the red alliance POWER UP time, or 0 if none is active. | [optional] 
**RedScaleOwned** | **int** | 1 if the red alliance owns the SCALE, 0 if not. | [optional] 
**RedScore** | **int** | Current score for the red alliance. | [optional] 
**RedSwitchOwned** | **int** | 1 if the red alliance owns their SWITCH, 0 if not. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

