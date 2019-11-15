suppressMessages(library(ez))
suppressMessages(library(ggplot2))
suppressMessages(library(multcomp))
suppressMessages(library(nlme))
suppressMessages(library(pastecs))
suppressMessages(library(reshape))
suppressMessages(library(WRS))
suppressMessages(library(tidyverse))


myData <- read.csv(file="input/messageAcrossData.csv", header=TRUE, sep=",")

processBoxPlot <- function(yVar){

  varsToProcess = c(paste(yVar,"_A",sep=""),paste(yVar,"_B",sep=""),paste(yVar,"_C",sep=""),paste(yVar,"_D",sep=""))

  keeps <- c("playerId", varsToProcess)
  givesData <- myData[, (names(myData) %in% keeps)]
  longGivesData <- melt(givesData, id="playerId", measured=varsToProcess)

  names(longGivesData)<-c("playerId", "scoreSystem", "yVar")
  longGivesData$scoreSystem <- factor(longGivesData$scoreSystem, labels=c("A","B","C","D"))
  longGivesData <- longGivesData[order(longGivesData$playerId),]

  givesMeanHist <- ggplot(longGivesData, aes(longGivesData$scoreSystem, longGivesData$yVar))
  givesMeanHist <- givesMeanHist + geom_boxplot()
  givesMeanHist <- givesMeanHist + labs(x="scoreSystem",y=yVar) 
  suppressMessages(ggsave(sprintf("plots/%s.png",yVar)))
}
processBoxPlot("meanNumberOfGives")
processBoxPlot("meanNumberOfTakes")
processBoxPlot("whoFocus")
processBoxPlot("whatFocus")

personalityVariables <- (myData %>% select(playerId, grandMeanTakes, grandMeanGives, ratioTakesGives))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Actions",y="Value")
suppressMessages(ggsave(sprintf("plots/%s.png", "Actions")))

personalityVariables <- (myData %>% select(playerId, N, E, O, A, C))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Traits",y="Value")
suppressMessages(ggsave(sprintf("plots/%s.png", "Traits")))

personalityVariables <- (myData %>% select(playerId, N1, N2, N3, N4, N5, N6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Neuroticism",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/%s.png", "Facets_N")))

personalityVariables <- (myData %>% select(playerId, E1, E2, E3, E4, E5, E6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Extraversion",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/%s.png", "Facets_E")))

personalityVariables <- (myData %>% select(playerId, O1, O2, O3, O4, O5, O6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Openness to Experience",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/%s.png", "Facets_O")))

personalityVariables <- (myData %>% select(playerId, A1, A2, A3, A4, A5, A6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Agreeableness",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/%s.png", "Facets_A")))

personalityVariables <- (myData %>% select(playerId, C1, C2, C3, C4, C5, C6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Conscientiousness",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/%s.png", "Facets_C")))

personalityVariables <- (myData %>% select(playerId, Internal, PowerfulOthers, Chance))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Dimensions from Locus of Control",y="Value") + coord_cartesian(ylim = c(0, 48))
suppressMessages(ggsave(sprintf("plots/%s.png", "Dimensions")))