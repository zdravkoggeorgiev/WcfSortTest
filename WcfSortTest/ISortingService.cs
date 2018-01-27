using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfSortTest
{
    [ServiceContract]
    public interface ISortingService
    {
        /// <summary>
        /// Begins a client session and returns a guid (globally unique identifier) for the stream
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Guid BeginStream();

        /// <summary>
        /// sends some text upstream; clients can call this API multiple times for the length of a session
        /// </summary>
        /// <param name="streamGuid"></param>
        /// <param name="text"></param>
        [OperationContract]
        void PutStreamData(Guid streamGuid, string[] text);

        /// <summary>
        /// Streams back the data accumulated so far, sorted ascending lexicographically, treating each line of text as a single word.
        /// </summary>
        /// <param name="streamGuid"></param>
        /// <returns></returns>
        [OperationContract]
        Stream GetSortedStream(Guid streamGuid);

        /// <summary>
        /// Ends a client session and closes the stream; subsequent calls to retrieve the stream data must fail.
        /// </summary>
        /// <param name="streamGuid"></param>
        [OperationContract]
        void EndStream(Guid streamGuid);
    }
}
