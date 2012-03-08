## Introduction

CombineTxt is a fluent .NET API for joining child files to a parent file.

## Features

* Join multiple files to a single parent file.
* Support for multi-line records in parent and child files.
* Customized key functions for each file.
* Optionally take action on unmatched child records.
* Optionally format each line before joining.

## Example


### ParentFile.txt

```
A|1000|Acura|1990
B|1|Sub Record Information
A|1001|Nissan|1990
B|2|Sub Record Information
A|1002|Ford|1990
B|3|Sub Record Information
A|1003|Toyota|1990  
B|4|Sub Record Information
```

### ChildFile1.txt

```
C|1000|TSX
C|1000|TS
C|1000|MDX
C|1001|Maxima
C|1001|Pathfinder
C|1002|Focus
```


### Join Code

```c#
CombineTxt.With("ParentFile.txt")
          .DefineKeyBy(l => l.Split('|')[1])
          .RecordDelimitedByStartingWith("A")
          .JoinTo("ChildFile1.txt")
          .DefineKeyBy(l => l.Split('|')[1])
          .WriteResultTo("Output1.txt");
```

### Output1.txt

```
A|1000|Acura|1990
B|1|Sub Record Information
C|1000|TSX
C|1000|TS
C|1000|MDX
A|1001|Nissan|1990
B|2|Sub Record Information
C|1001|Maxima
C|1001|Pathfinder
A|1002|Ford|1990
B|3|Sub Record Information
C|1002|Focus
A|1003|Toyota|1990
B|4|Sub Record Information
```
