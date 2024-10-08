﻿module AmortisedInterestSchedule.Loan.Quoted

open AmortisedInterestSchedule.Term
open AmortisedInterestSchedule.Rate
open AmortisedInterestSchedule.Loan.Instalment
open AmortisedInterestSchedule.Loan.Core

type QuotedLoan =
    { Amount: double
      Term: Term
      Rate: Rate }

    member this.GetTerm =
        let (Term t) = this.Term
        t |> double

    member this.GetRate = this.Rate.Amount / 12.0

    member this.AmortizedPaymentAmount =
        amortisedPaymentAmount this.Amount this.Rate.Amount 12.0 this.GetTerm

    member private this.IndicativeSchedule' (outstandingBalance: float) periods (rate: float) amortisedPaymentAmount instalments =
        if List.length instalments = periods then
            instalments
        else
            let interestAccruedInPeriod = rate * outstandingBalance
            let balanceWithInterestApplied = interestAccruedInPeriod + outstandingBalance

            let principalComponent =
                if List.length instalments = (periods - 1) then
                    balanceWithInterestApplied - interestAccruedInPeriod
                else
                    (amortisedPaymentAmount - interestAccruedInPeriod)

            let actualPaymentAmount = principalComponent + interestAccruedInPeriod
            let closingBalancePostPayment = balanceWithInterestApplied - actualPaymentAmount

            let nextInstalment =
                { OpeningBalance = outstandingBalance
                  Interest = interestAccruedInPeriod
                  Principal = principalComponent
                  ClosingBalance = closingBalancePostPayment }

            this.IndicativeSchedule' nextInstalment.ClosingBalance periods rate amortisedPaymentAmount (nextInstalment :: instalments)

    member this.IndicativeSchedule =
        this.IndicativeSchedule' this.Amount (this.GetTerm |> int) this.GetRate this.AmortizedPaymentAmount []
        |> List.rev

