module AmortisedInterestSchedule.Rate

type RateFrequency = Monthly | Yearly

type Rate = {
    Amount: float
    Frequency: RateFrequency
}
