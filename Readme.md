# ExpressionEvalService

.NET Core 3.1 assignment project

HTTP service that evaluates mathematical expression and returns it's result.

## Supported operators

Binary: + - * / % ^

Unary: + -

Grouping: Parenthesis ( )


## Requirements 

.Net Core 3.1 Runtime

## Service starting

```
cd  ExpressionEvalService
dotnet build
dotnet run
```

## Connecting to the service

Use HTTP GET request

Endpoint http://localhost:5000/compute

Parameter ?expr=

### Example

http://localhost:5000/compute?expr=1+1
