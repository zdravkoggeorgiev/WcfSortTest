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
    public class SortingService : ISortingService
    {
        public Guid BeginStream()
        {
            SortingStores stores = new SortingStores();
            return stores.BeginStream();
        }

        public void PutStreamData(Guid streamGuid, string[] text)
        {
            SortingStores stores = new SortingStores();
            stores.PutStreamData(streamGuid, text);
        }

        public Stream GetSortedStream(Guid streamGuid)
        {
            SortingStores stores = new SortingStores();
            return stores.GetSortedStream(streamGuid);
        }

        public void EndStream(Guid streamGuid)
        {
            SortingStores stores = new SortingStores();
            stores.EndStream(streamGuid);
        }
    }
}
