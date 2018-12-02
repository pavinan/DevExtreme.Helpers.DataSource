# DevExtreme helpers remote datasource.

[![Build status](https://ci.appveyor.com/api/projects/status/yv6c00rckiqw39mo?svg=true)](https://ci.appveyor.com/project/pavinan/devextreme-helpers-datasource)
[![Build status](https://pavinan.visualstudio.com/Public/_apis/build/status/pavinan.DevExtreme.Helpers.DataSource)](https://pavinan.visualstudio.com/Public/_build/latest?definitionId=12)

Nuget link: https://www.nuget.org/packages/Bharat.DevExtreme.Helpers.DataSource/

```csharp
var Builder = new WhereClauseBuilder();
var json = "[\"CustomerID\",\"=\",\"ALFKI\"]";
var jarray = JArray.Parse(json);
var sqlWhereClause = Builder.BuildFor(jarray);
```

Outputs:
```
CustomerID = "ALFKI"
```

In sql you can write
```sql
Exec('Select * from customers where ' + @sqlWhereClause);
```

There is an overloaded version of `WhereClauseBuilder` you can provide your own implentation `ISQLColumnProvider` if you are using table aliases you can customize there.
