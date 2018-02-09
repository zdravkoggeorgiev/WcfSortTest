# Solution WcfSortTest

### Small WCF Service **SortWcf** and even smaller Console client **SortClient** to consume the Service.
>Implements WCF Service to store, sort and retrieve string streams.
Can have multiple streams at once, distinguished by UID.
Can have multiple calls to same stream to add new data.

### The focus is on:
* Running in concurrent environment, where multiple users connect simultaneously to receive sorted data
* Optimise for speed and memory/resources used
* Speed on sorting operation

### This is achieved by:
* Data is stored as its arrived: unsorted, each packet (array) is saved as-is, in different unsorted array.
* There is a list with arrays with input data, to which list we just add new data. 
* Such way locking is fast and doesn't blocks reading.
* For sorting, we use separate BTree map, which points to data, in sorted way.
* BTree supports duplicates. Actually, it will work even faster, when there are a lot of duplicates, Because it collects all duplicates in a single Node.
* BTree implementation is tied to current task and data structures, no delete ( no need for task)
* On reading, we copy current sorted map as array, and data is read sorted by map. This allows us to avoid collisions between read and write operations.
* On sorting, we just add new items into BTree sort map
* There are simple nunit tests. I created them mostly for purpose on debugging. Each time to run WCF and Console Client was slow.
* The main logic is in the file **ConcurentArrays.cs** 

### TODO: Because I have no enough time to devote to the tasks, some things are not done.
* I had no time to make big data ( >16GB ) sorting
* My idea was to split incoming data, and to write (append) it to different files, unsorted. In first files all starting lines aaa to azz, in second baa to bzz, etc.
* I will track the size of files, when size hits a limit ( like 500MB), then file will be split in 2 files. Like starting  **aaa** to **azz** -> **aaa** to **ass** and **att** to **azz**. If it becomes too big, split again, etc.
* When there is no more new data, I would load first file into memory, sort it, write to disk. Then load second, and repeat.
* If there is low selectivity ( often repeats in the input data), this algorithm could not be so good.

### Style Convention is like one my last project
* It has rare-to-use in C# patterns, like underscored private members. This can easily be adjusted, when i see your code.
