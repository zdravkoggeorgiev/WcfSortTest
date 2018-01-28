using System;
using System.IO;
using System.ServiceModel;

namespace WcfSortTest
{
    /// <summary>
    /// Service to store, sort and retrieve string streams.
    /// Can have multiple streams at once, distinguished by UID.
    /// Can have multiple calls to same stream to add new data.
    /// </summary>
    [ServiceContract]
    public interface ISortingService
    {
        /// <summary>
        /// Begins a client session and returns a guid (globally unique identifier) for the stream.
        /// Multiple streams can be opened at once.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Guid BeginStream();

        /// <summary>
        /// Sends some text upstream; clients can call this API multiple times for the length of a session
        /// </summary>
        /// <param name="streamGuid">Already owned guid, retrieved by BeginStream(). </param>
        /// <param name="text">Array of strings to be sorted. </param>
        [OperationContract]
        void PutStreamData(Guid streamGuid, string[] text);

        /// <summary>
        /// Streams back the data accumulated so far, sorted ascending lexicographically, treating each line of text as a single word.
        /// Can be called multiple times.
        /// </summary>
        /// <param name="streamGuid">Already owned guid, retrieved by BeginStream(). </param>
        /// <returns></returns>
        [OperationContract]
        Stream GetSortedStream(Guid streamGuid);

        /// <summary>
        /// Ends a client session and closes the stream; subsequent calls to retrieve the stream data must fail.
        /// </summary>
        /// <param name="streamGuid">Already owned guid, retrieved by BeginStream(). </param>
        [OperationContract]
        void EndStream(Guid streamGuid);
    }
}
