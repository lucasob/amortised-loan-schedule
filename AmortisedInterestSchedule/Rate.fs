module AmortisedInterestSchedule.Rate

type RateFrequency = Monthly | Yearly

type Rate = {
    Amount: double
    Frequency: RateFrequency
}
