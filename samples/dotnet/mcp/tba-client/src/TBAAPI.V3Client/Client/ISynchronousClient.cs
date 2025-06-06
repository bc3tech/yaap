/* 
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).    A `User-Agent` header may need to be set to prevent a 403 Unauthorized error.
 *
 * The version of the OpenAPI document: 3.8.0
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TBAAPI.V3Client.Client;

/// <summary>
/// Contract for Synchronous RESTful API interactions.
/// 
/// This interface allows consumers to provide a custom API accessor client.
/// </summary>
public interface ISynchronousClient
{
    /// <summary>
    /// Executes a blocking call to some <paramref name="path"/> using the GET http verb.
    /// </summary>
    /// <param name="path">The relative path to invoke.</param>
    /// <param name="options">The request parameters to pass along to the client.</param>
    /// <param name="configuration">Per-request configurable settings.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The response data, decorated with <see cref="ApiResponse{T}"/></returns>
    ApiResponse<T> Get<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null);

    /// <summary>
    /// Executes a blocking call to some <paramref name="path"/> using the POST http verb.
    /// </summary>
    /// <param name="path">The relative path to invoke.</param>
    /// <param name="options">The request parameters to pass along to the client.</param>
    /// <param name="configuration">Per-request configurable settings.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The response data, decorated with <see cref="ApiResponse{T}"/></returns>
    ApiResponse<T> Post<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null);

    /// <summary>
    /// Executes a blocking call to some <paramref name="path"/> using the PUT http verb.
    /// </summary>
    /// <param name="path">The relative path to invoke.</param>
    /// <param name="options">The request parameters to pass along to the client.</param>
    /// <param name="configuration">Per-request configurable settings.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The response data, decorated with <see cref="ApiResponse{T}"/></returns>
    ApiResponse<T> Put<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null);

    /// <summary>
    /// Executes a blocking call to some <paramref name="path"/> using the DELETE http verb.
    /// </summary>
    /// <param name="path">The relative path to invoke.</param>
    /// <param name="options">The request parameters to pass along to the client.</param>
    /// <param name="configuration">Per-request configurable settings.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The response data, decorated with <see cref="ApiResponse{T}"/></returns>
    ApiResponse<T> Delete<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null);

    /// <summary>
    /// Executes a blocking call to some <paramref name="path"/> using the HEAD http verb.
    /// </summary>
    /// <param name="path">The relative path to invoke.</param>
    /// <param name="options">The request parameters to pass along to the client.</param>
    /// <param name="configuration">Per-request configurable settings.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The response data, decorated with <see cref="ApiResponse{T}"/></returns>
    ApiResponse<T> Head<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null);

    /// <summary>
    /// Executes a blocking call to some <paramref name="path"/> using the OPTIONS http verb.
    /// </summary>
    /// <param name="path">The relative path to invoke.</param>
    /// <param name="options">The request parameters to pass along to the client.</param>
    /// <param name="configuration">Per-request configurable settings.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The response data, decorated with <see cref="ApiResponse{T}"/></returns>
    ApiResponse<T> Options<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null);

    /// <summary>
    /// Executes a blocking call to some <paramref name="path"/> using the PATCH http verb.
    /// </summary>
    /// <param name="path">The relative path to invoke.</param>
    /// <param name="options">The request parameters to pass along to the client.</param>
    /// <param name="configuration">Per-request configurable settings.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>The response data, decorated with <see cref="ApiResponse{T}"/></returns>
    ApiResponse<T> Patch<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null);
}