
suppressMessages(library(gridExtra))
suppressMessages(library(ggplot2))
suppressMessages(library(stringr))
suppressMessages(library(dplyr))
suppressMessages(library(envDocument))

resultsLog <- read.csv("messageAcross.csv", header=TRUE, sep=",")

meanNumberOfTakes <- unlist(resultsLog[c("meanNumberOfTakes_A", "meanNumberOfTakes_B", "meanNumberOfTakes_C", "meanNumberOfTakes_D")], use.names = FALSE) 
meanNumberOfTakes_Ranks <- rank(meanNumberOfTakes, ties.method = "average")

resultsLog$meanTakes_A_Ranked <- meanNumberOfTakes_Ranks[1:66]
resultsLog$meanTakes_B_Ranked <- meanNumberOfTakes_Ranks[67:132]
resultsLog$meanTakes_C_Ranked <- meanNumberOfTakes_Ranks[133:198]
resultsLog$meanTakes_D_Ranked <- meanNumberOfTakes_Ranks[199:264]


focus <- unlist(resultsLog[c("whoFocus_A", "whoFocus_B", "whoFocus_C", "whoFocus_D")], use.names = FALSE) 
focus_Ranks <- rank(focus, ties.method = "average")

resultsLog$focus_A_Ranked <- focus_Ranks[1:66]
resultsLog$focus_B_Ranked <- focus_Ranks[67:132]
resultsLog$focus_C_Ranked <- focus_Ranks[133:198]
resultsLog$focus_D_Ranked <- focus_Ranks[199:264]


challenge <- unlist(resultsLog[c("whatFocus_A", "whatFocus_B", "whatFocus_C", "whatFocus_D")], use.names = FALSE) 
challenge_Ranks <- rank(challenge, ties.method = "average")

resultsLog$challenge_A_Ranked <- challenge_Ranks[1:66]
resultsLog$challenge_B_Ranked <- challenge_Ranks[67:132]
resultsLog$challenge_C_Ranked <- challenge_Ranks[133:198]
resultsLog$challenge_D_Ranked <- challenge_Ranks[199:264]



resultsLog$RN <- rank(resultsLog$N, ties.method = "average")
resultsLog$RC <- rank(resultsLog$C, ties.method = "average")
resultsLog$RE <- rank(resultsLog$E, ties.method = "average")
resultsLog$RA <- rank(resultsLog$A, ties.method = "average")
resultsLog$RO <- rank(resultsLog$O, ties.method = "average")

write.csv(resultsLog, "messageAcross_ranked.csv", row.names=FALSE)
