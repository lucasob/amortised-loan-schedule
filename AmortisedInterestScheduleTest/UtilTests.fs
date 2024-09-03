module AmortisedInterestScheduleTest.UtilTests

open Xunit
open AmortisedInterestSchedule.Util

[<Fact>]
let ``Can partition an even number of items into an even number of bueckts `` () =
    let input = [ 1..2 ]
    let output = List.partitionInto 2 input
    Assert.Equal<int list list>([ [ 1; 2 ] ], output)

[<Fact>]
let ``Can partition single overflow into the last bucket`` () =
    let input = [ 1..3 ]
    let output = List.partitionInto 2 input
    Assert.Equal<int list list>([ [ 1; 2; 3 ] ], output)

[<Fact>]
let ``Can handle empty lists`` () =
    let input = []
    let output = List.partitionInto 2 input
    Assert.Equal<int list>([], output)
