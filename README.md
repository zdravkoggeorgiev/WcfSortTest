# Solution WcfSortTest

### Small WCF Service and even smaller Console client to consume the Service.
>Implements WCF Service to store, sort and retrieve string streams.
Can have multiple streams at once, distinguished by UID.
Can have multiple calls to same stream to add new data.

### About sorting big data
* Planned to switch Sorting Items container, when items become too many, to leave In-Memory container, and to start use file system too.
* File-system store is not yet implemented.

### Exceptions handling
* The main reason to have exception is to get out of memory, which is unrecoverable, for application. So currently IMHO best is to terminate Application. 
* On defined Exception strategy, it can be easily added.