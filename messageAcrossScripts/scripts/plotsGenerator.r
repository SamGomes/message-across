suppressMessages(library(ez))
suppressMessages(library(ggplot2))
suppressMessages(library(multcomp))
suppressMessages(library(nlme))
suppressMessages(library(pastecs))
suppressMessages(library(reshape))
suppressMessages(library(WRS))
suppressMessages(library(tidyverse))


myData <- read.csv(file="input/messageAcrossData.csv", header=TRUE, sep=",")
# print(myData)

print("Plotting game variables...")
processBoxPlot <- function(yVarPre, yVarPos, yLabel, plotName, labels, breaks){

  varsToProcess = c(sprintf("%sA%s",yVarPre,yVarPos),sprintf("%sB%s",yVarPre,yVarPos),sprintf("%sC%s",yVarPre,yVarPos),sprintf("%sD%s",yVarPre,yVarPos))

  keeps <- c("playerId", varsToProcess)
  data <- myData[, (names(myData) %in% keeps)]
  longData <- melt(data, id="playerId", measured=varsToProcess)

  names(longData)<-c("playerId", "scoreSystem", "yVar")
  longData$scoreSystem <- factor(longData$scoreSystem, labels=c("Comp","Ind","M_Help","P_Alt"))
  longData <- longData[order(longData$playerId),]

  hist <- ggplot(longData, aes(longData$scoreSystem, longData$yVar)) + theme(axis.text=element_text(size=18), axis.title=element_text(size=18,face="bold"))
  hist <- hist + geom_boxplot(fill='#c4d4ff', color="black")
  if( labels!=-1 && breaks!=-1){
    hist <- hist +  scale_y_continuous(yLabel, labels = as.character(labels), breaks = breaks)
  }
  hist <- hist + labs(x="Score Attribution System",y=yLabel) 
  suppressMessages(ggsave(sprintf("plots/%s.png",plotName)))
}
processBoxPlot("meanNumberOfGives_", "", "Mean Number of Gives", "meanNumberOfGives", -1, -1)
processBoxPlot("meanNumberOfTakes_", "", "Mean Number of Takes", "meanNumberOfTakes", -1, -1)
processBoxPlot("whoFocus_", "", "Interaction Focus", "interactionFocus", c("Me 1","2", "3", "4", "5", "6", "Other 7"), c(1,2,3,4,5,6,7))
processBoxPlot("whatFocus_", "", "Interaction Intention", "interactionIntention", c("Help 1","2", "3", "4", "5", "6", "Complicate 7"), c(1,2,3,4,5,6,7))
processBoxPlot("score_", "_7", "Final Score", "finalScore", -1, -1)

actionsVariables <- (myData %>% select(playerId, grandMeanTakes, grandMeanGives, ratioTakesGives))
plot <- ggplot(melt(actionsVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Actions",y="Value")
suppressMessages(ggsave(sprintf("plots/gameVariables/%s.png", "Actions")))



### Personality Variables
print("Plotting personality variables...")

personalityVariables <- (myData %>% select(playerId, N, E, O, A, C))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Traits",y="Value")
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Traits")))

personalityVariables <- (myData %>% select(playerId, N1, N2, N3, N4, N5, N6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Neuroticism",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_N")))

personalityVariables <- (myData %>% select(playerId, E1, E2, E3, E4, E5, E6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Extraversion",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_E")))

personalityVariables <- (myData %>% select(playerId, O1, O2, O3, O4, O5, O6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Openness to Experience",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_O")))

personalityVariables <- (myData %>% select(playerId, A1, A2, A3, A4, A5, A6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Agreeableness",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_A")))

personalityVariables <- (myData %>% select(playerId, C1, C2, C3, C4, C5, C6))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets from Conscientiousness",y="Value") + coord_cartesian(ylim = c(0, 36))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_C")))

personalityVariables <- (myData %>% select(playerId, Internal, PowerfulOthers, Chance))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Dimensions from Locus of Control",y="Value") + coord_cartesian(ylim = c(0, 48))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Dimensions")))

### Interaction Variables

#interactionVariables <- (myData %>% select(playerId, A1, grandMeanGives))
meltedData <- read.csv(file="output/meltedData.csv", header=TRUE, sep=",")
personalityVariables <- (meltedData %>% select( N1, N2, N3, N4, N5, N6,
                                                E1, E2, E3, E4, E5, E6,
                                                A1, A2, A3, A4, A5, A6,
                                                O1, O2, O3, O4, O5, O6,
                                                C1, C2, C3, C4, C5, C6,
                                                N, E, O, A, C,
                                                Internal, PowerfulOthers, Chance))

personalityNames <- names(meltedData)

interactionBoxplotFacets <- function(yVar){
  i<-1
  for (xText in personalityVariables){
      i<-i+1
      qplot(x = meltedData[[yVar]], y = xText, data = meltedData, facets = ~ScoreSystem, color = ScoreSystem) + geom_smooth(method = "lm") + geom_point(color = "grey", alpha = .7)
      suppressMessages(ggsave(sprintf("plots/interactionEffects/facets/%s/%s_%s.png", yVar, personalityNames[i], yVar)))
  }
}

interactionBoxplotJoin <- function(yVar){
  i<-1
  for (xText in personalityVariables){
      i<-i+1
      qplot(x = meltedData[[yVar]], y = xText, data = meltedData, color = ScoreSystem) + geom_smooth(method = "lm") + geom_point(color = "grey", alpha = .7)
      suppressMessages(ggsave(sprintf("plots/interactionEffects/join/%s/%s_%s.png", yVar, personalityNames[i], yVar)))
  }
}

print("Plotting interactions in facets...")
interactionBoxplotFacets("gives")
interactionBoxplotFacets("takes")
interactionBoxplotFacets("what")
interactionBoxplotFacets("who")

print("Plotting interactions in joint boxplots...")
interactionBoxplotJoin("gives")
interactionBoxplotJoin("takes")
interactionBoxplotJoin("what")
interactionBoxplotJoin("who")