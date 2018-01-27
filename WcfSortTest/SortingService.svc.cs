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
            Guid newGuid = Guid.NewGuid();
            return newGuid;
        }

        public void EndStream(Guid streamGuid)
        {
            throw new NotImplementedException();
        }

        public Stream GetSortedStream(Guid streamGuid)
        {
            throw new NotImplementedException();
        }

        public void PutStreamData(Guid streamGuid, string[] text)
        {
            throw new NotImplementedException();
        }
    }
}
