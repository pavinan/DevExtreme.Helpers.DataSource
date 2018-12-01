# DevExtreme helpers remote datasource.


```csharp
var Builder = new WhereClauseBuilder();
var json = "[\"CustomerID\",\"=\",\"ALFKI\"]";
var jarray = JArray.Parse(json);
var sqlWhereClause = Builder.BuildFor(jarray);
```

Outputs:

CustomerID = "ALFKI"

in sql you can write.

```sql
Exec('Select * from customers where ' + @sqlWhereClause);
```
