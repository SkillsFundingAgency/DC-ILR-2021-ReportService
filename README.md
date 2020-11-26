# DC-ILR-2021-ReportService
This code will create the sixth part of the conventional ILR pipeline. 
The report service will create the 25+ reports that are produced for each ILR submission. The code breaks down into types of report (CSV, Excel) and different data gathering mechanisms, most reports will work from teh JSON blobs that are persisted to the storage as they go. Some reports though folder in previous year data as well and so will have to link to databases (this is mostly removed now). 
Online is stateless service fabric, desktop build is a nuget package
