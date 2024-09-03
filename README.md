# Amortised Loan Schedule

## What?

* A way to generate loan schedules

## Why?

* Exposed to an issue currently, where we want to be able to dynamically restructure schedules.
* Mambu don't offer this, so we can approximate
* The best solution is to move to generating your own schedule
* This will unluck the ability to do whatever we want
* Finally, because this is my project, in my time, it can be done in F# 🫶

## Usage

> [!NOTE]
> I have been to lazy to bundle up CLI validation beyond type mapping and keys
>
> I would also recommend having jq installed

### Usage

```shell
dotnet run Main.fs --amount 1000.0 --rate 0.05 --term 12 | jq
```
