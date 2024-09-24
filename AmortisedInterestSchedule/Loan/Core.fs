module AmortisedInterestSchedule.Loan.Core

let amortisedPaymentAmount (principal: double) (rate: double) (periods: double) (totalTerm: double) =
    principal
    * (((rate / periods) * ((1.0 + (rate / periods)) ** totalTerm))
       / (((1.0 + (rate / periods)) ** totalTerm) - 1.0))
