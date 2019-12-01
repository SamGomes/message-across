suppressMessages(library(ez))
suppressMessages(library(ggplot2))
suppressMessages(library(multcomp))
suppressMessages(library(nlme))
suppressMessages(library(pastecs))
suppressMessages(library(reshape))
suppressMessages(library(WRS))
suppressMessages(library(tidyverse))
suppressMessages(library(sjPlot))
suppressMessages(library(sjmisc))


myData <- read.csv(file="input/messageAcrossData.csv", header=TRUE, sep=",")

print("Plotting game variables...")
processBoxPlot <- function(yVarPre, yVarPos, yLabel, plotName, labels, breaks){

  varsToProcess = c(sprintf("%sA%s",yVarPre,yVarPos),sprintf("%sB%s",yVarPre,yVarPos),sprintf("%sC%s",yVarPre,yVarPos),sprintf("%sD%s",yVarPre,yVarPos))

  keeps <- c("playerId", varsToProcess)
  data <- myData[, (names(myData) %in% keeps)]
  longData <- melt(data, id="playerId", measured=varsToProcess)

  names(longData)<-c("playerId", "scoreSystem", "yVar")
  longData$scoreSystem <- factor(longData$scoreSystem, labels=c("Comp","Ind","M.Help","E.Altr"))
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
processBoxPlot("whoFocus_", "", "Interaction Focus", "interactionFocus", c("Me 1","2", "3", "4", "5", "6", "The    \n Other 7\n  Player  "), c(1,2,3,4,5,6,7))
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

mainBoxplotFacets <- function(source, type, yVar){
  meltedData <- read.csv(file=source, header=TRUE, sep=",")
  personalityVariables <- (meltedData %>% select( N1, N2, N3, N4, N5, N6,
                                                  E1, E2, E3, E4, E5, E6,
                                                  A1, A2, A3, A4, A5, A6,
                                                  O1, O2, O3, O4, O5, O6,
                                                  C1, C2, C3, C4, C5, C6,
                                                  N, E, O, A, C,
                                                  Internal, PowerfulOthers, Chance))

  personalityNames <- names(meltedData)

  i<-1
  for (xText in personalityVariables){
      i<-i+1
      qplot(x = meltedData[[yVar]], y = xText, data = meltedData, na.rm=TRUE) + geom_smooth(method = "lm") + geom_point(color = "grey", alpha = .7) + xlab(yVar) + ylab(personalityNames[i])
      suppressMessages(ggsave(sprintf("plots/mainEffects/facets/%s/%s_%s_%s.png", yVar, type, personalityNames[i], yVar)))
  }
}

mainBoxplotJoin <- function(source, type, yVar){
  meltedData <- read.csv(file=source, header=TRUE, sep=",")
  personalityVariables <- (meltedData %>% select( N1, N2, N3, N4, N5, N6,
                                                  E1, E2, E3, E4, E5, E6,
                                                  A1, A2, A3, A4, A5, A6,
                                                  O1, O2, O3, O4, O5, O6,
                                                  C1, C2, C3, C4, C5, C6,
                                                  N, E, O, A, C,
                                                  Internal, PowerfulOthers, Chance))

  personalityNames <- names(meltedData)

  i<-1
  for (xText in personalityVariables){
      i<-i+1
      qplot(x = meltedData[[yVar]], y = xText, data = meltedData, na.rm=TRUE) + geom_smooth(method = "lm") + geom_point(color = "grey", alpha = .7) + xlab(yVar) + ylab(personalityNames[i])
      suppressMessages(ggsave(sprintf("plots/mainEffects/join/%s/%s_%s_%s.png", yVar, type, personalityNames[i], yVar)))
  }
}

interactionBoxplotFacets <- function(source, type, yVar){
  meltedData <- read.csv(file=source, header=TRUE, sep=",")
  personalityVariables <- (meltedData %>% select( N1, N2, N3, N4, N5, N6,
                                                  E1, E2, E3, E4, E5, E6,
                                                  A1, A2, A3, A4, A5, A6,
                                                  O1, O2, O3, O4, O5, O6,
                                                  C1, C2, C3, C4, C5, C6,
                                                  N, E, O, A, C,
                                                  Internal, PowerfulOthers, Chance))

  personalityNames <- names(meltedData)

	i<-1
	for (xText in personalityVariables){
  		i<-i+1
  		qplot(x = meltedData[[yVar]], y = xText, data = meltedData, facets = ~ScoreSystem, color = ScoreSystem, na.rm=TRUE) + geom_smooth(method = "lm") + geom_point(color = "grey", alpha = .7) + xlab(yVar) + ylab(personalityNames[i])
  		suppressMessages(ggsave(sprintf("plots/interactionEffects/facets/%s/%s_%s_%s.png", yVar, type, personalityNames[i], yVar)))
	}
}

interactionBoxplotJoin <- function(source, type, yVar){
  meltedData <- read.csv(file=source, header=TRUE, sep=",")
  personalityVariables <- (meltedData %>% select( N1, N2, N3, N4, N5, N6,
                                                  E1, E2, E3, E4, E5, E6,
                                                  A1, A2, A3, A4, A5, A6,
                                                  O1, O2, O3, O4, O5, O6,
                                                  C1, C2, C3, C4, C5, C6,
                                                  N, E, O, A, C,
                                                  Internal, PowerfulOthers, Chance))

  personalityNames <- names(meltedData)
	i<-1
	for (xText in personalityVariables){
  		i<-i+1
  		qplot(x = meltedData[[yVar]], y = xText, data = meltedData, color = ScoreSystem, na.rm=TRUE) + geom_smooth(method = "lm") + geom_point(color = "grey", alpha = .7) + xlab(yVar) + ylab(personalityNames[i])
  		suppressMessages(ggsave(sprintf("plots/interactionEffects/join/%s/%s_%s_%s.png", yVar, type, personalityNames[i], yVar)))
	}
}

oldw <- getOption("warn")
options(warn = -1)

print("Plotting main effects in facets...")
#mainBoxplotFacets("output/meltedDataFourCategories.csv", "four", "gives")
mainBoxplotFacets("input/dataFourCategories.csv", "four", "grandMeanTakes")
mainBoxplotFacets("input/dataFourCategories.csv", "four", "meanWhatFocus")
mainBoxplotFacets("input/dataFourCategories.csv", "four", "meanWhoFocus")
#mainBoxplotFacets("input/dataFourCategories.csv", "four", "preferredVersion")

myData <- read.csv(file="input/dataFourCategories.csv", header=TRUE, sep=",")

ggplot(myData, aes(N3, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge")
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/N3_Version.png"))

ggplot(myData, aes(N4, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge")
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/N4_Version.png"))

ggplot(myData, aes(E4, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge")
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/E4_Version.png"))

ggplot(myData, aes(C2, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge")
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/C2_Version.png"))

print("Plotting main effects in joint boxplots...")
#mainBoxplotJoin("output/meltedDataFourCategories.csv", "four", "gives")
mainBoxplotJoin("input/dataFourCategories.csv", "four", "grandMeanTakes")
mainBoxplotJoin("input/dataFourCategories.csv", "four", "meanWhatFocus")
mainBoxplotJoin("input/dataFourCategories.csv", "four", "meanWhoFocus")
#mainBoxplotJoin("input/dataFourCategories.csv", "four", "preferredVersion")

print("Plotting interactions in facets...")
# interactionBoxplotFacets("output/meltedData.csv", "integer", "gives")
# interactionBoxplotFacets("output/meltedData.csv", "integer", "takes")
# interactionBoxplotFacets("output/meltedData.csv", "integer", "what")
# interactionBoxplotFacets("output/meltedData.csv", "integer", "who")
# interactionBoxplotFacets("output/meltedDataTwoCategories.csv", "two", "gives")
# interactionBoxplotFacets("output/meltedDataTwoCategories.csv", "two", "takes")
# interactionBoxplotFacets("output/meltedDataTwoCategories.csv", "two", "what")
# interactionBoxplotFacets("output/meltedDataTwoCategories.csv", "two", "who")
# interactionBoxplotFacets("output/meltedDataThreeCategories.csv", "three", "gives")
# interactionBoxplotFacets("output/meltedDataThreeCategories.csv", "three", "takes")
# interactionBoxplotFacets("output/meltedDataThreeCategories.csv", "three", "what")
# interactionBoxplotFacets("output/meltedDataThreeCategories.csv", "three", "who")
#interactionBoxplotFacets("output/meltedDataFourCategories.csv", "four", "gives")
interactionBoxplotFacets("output/meltedDataFourCategories.csv", "four", "takes")
interactionBoxplotFacets("output/meltedDataFourCategories.csv", "four", "what")
interactionBoxplotFacets("output/meltedDataFourCategories.csv", "four", "who")

print("Plotting interactions in joint boxplots...")
# interactionBoxplotJoin("output/meltedData.csv", "integer", "gives")
# interactionBoxplotJoin("output/meltedData.csv", "integer", "takes")
# interactionBoxplotJoin("output/meltedData.csv", "integer", "what")
# interactionBoxplotJoin("output/meltedData.csv", "integer", "who")
# interactionBoxplotJoin("output/meltedDataTwoCategories.csv", "two", "gives")
# interactionBoxplotJoin("output/meltedDataTwoCategories.csv", "two", "takes")
# interactionBoxplotJoin("output/meltedDataTwoCategories.csv", "two", "what")
# interactionBoxplotJoin("output/meltedDataTwoCategories.csv", "two", "who")
# interactionBoxplotJoin("output/meltedDataThreeCategories.csv", "three", "gives")
# interactionBoxplotJoin("output/meltedDataThreeCategories.csv", "three", "takes")
# interactionBoxplotJoin("output/meltedDataThreeCategories.csv", "three", "what")
# interactionBoxplotJoin("output/meltedDataThreeCategories.csv", "three", "who")
# interactionBoxplotJoin("output/meltedDataFourCategories.csv", "four", "gives")
interactionBoxplotJoin("output/meltedDataFourCategories.csv", "four", "takes")
interactionBoxplotJoin("output/meltedDataFourCategories.csv", "four", "what")
interactionBoxplotJoin("output/meltedDataFourCategories.csv", "four", "who")

options(warn = oldw)

