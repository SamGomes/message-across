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

mainBoxplotFacets <- function(source, type, yVar){
  meltedData <- read.csv(file=source, header=TRUE, sep=",")
  personalityVariables <- (meltedData %>% select( N1, N2, N3, N4, N5, N6,
                                                  E1, E2, E3, E4, E5, E6,
                                                  O1, O2, O3, O4, O5, O6,
                                                  A1, A2, A3, A4, A5, A6,
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

interactionBoxplotFacets <- function(source, type, yVar){
  meltedData <- read.csv(file=source, header=TRUE, sep=",")
  personalityVariables <- (meltedData %>% select( N1, N2, N3, N4, N5, N6,
                                                  E1, E2, E3, E4, E5, E6,
                                                  O1, O2, O3, O4, O5, O6,
                                                  A1, A2, A3, A4, A5, A6,
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

interactionJoin <- function(source, type, yVar){
  meltedData <- read.csv(file=source, header=TRUE, sep=",")
  personalityVariables <- (meltedData %>% select( N1, N2, N3, N4, N5, N6,
                                                  E1, E2, E3, E4, E5, E6,
                                                  O1, O2, O3, O4, O5, O6,
                                                  A1, A2, A3, A4, A5, A6,
                                                  C1, C2, C3, C4, C5, C6,
                                                  N, E, O, A, C,
                                                  Internal, PowerfulOthers, Chance))

  personalityNames <- names(meltedData)
	i<-1
	for (xText in personalityVariables){
  		i<-i+1
  		png(sprintf("plots/interactionEffects/join/%s/%s_%s_%s.png", yVar, type, personalityNames[i], yVar))
      xText <- factor(xText , levels=c("High", "Medium_High", "Medium_Low", "Low"))
      interaction.plot( x.factor     = meltedData$ScoreSystem,
                        trace.factor = xText,
                        response     = meltedData[[yVar]],
                        fun = mean,
                        type="b",
                        col=c("blue4", "red4", "green4", "pink2"),  ### Colors for levels of trace var.
                        pch=c(19, 17, 15, 21),             ### Symbols for levels of trace var.
                        fixed=TRUE,                    ### Order by factor order in data
                        ylab = sprintf("mean of %s", yVar),
                        xlab = "Score System",
                        xpd=TRUE,
                        lwd = 3,
                        legend=FALSE,
                        lty=1,
                        yaxt = "n")
      if(yVar == "takes"){
        axis(2, at=seq(0, 4, by = .25), labels=seq(0, 4, by = .25), las = 2)
      }
      else{
        axis(2, at=seq(0, 7, by = .25), labels=seq(0, 7, by = .25), las = 2)
      }
      if(yVar == "who"){
        legend("topleft", legend=c("High", "Medium_High", "Medium_Low", "Low"),
             col=c("blue4", "red4", "green4", "pink2"), lty=1, pch=c(19, 17, 15, 21), cex=1,
             title="Personality Group", text.font=1)
      }
      else{
        legend("topright", legend=c("High", "Medium_High", "Medium_Low", "Low"),
             col=c("blue4", "red4", "green4", "pink2"), lty=1, pch=c(19, 17, 15, 21), cex=1,
             title="Personality Group", text.font=1)
      }
      dev.off()
	 }
}

oldw <- getOption("warn")
options(warn = -1)

print("Plotting main effects in facets...")

myData <- read.csv(file="input/dataFourCategories.csv", header=TRUE, sep=",")

myData$N3 <- factor(myData$N3 , levels=c("High", "Medium_High", "Medium_Low", "Low"))
ggplot(myData, aes(N3, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge") + xlab("N3") + scale_fill_manual(values=c("blue4", "red4", "green4", "pink2"))
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/mainN3.png"))

myData$N4 <- factor(myData$N4 , levels=c("High", "Medium_High", "Medium_Low", "Low"))
ggplot(myData, aes(N4, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge") + xlab("N4") + scale_fill_manual(values=c("blue4", "red4", "green4", "pink2"))
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/mainN4.png"))

myData$E4 <- factor(myData$E4 , levels=c("High", "Medium_High", "Medium_Low", "Low"))
ggplot(myData, aes(E4, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge") + xlab("E4") + scale_fill_manual(values=c("blue4", "red4", "green4", "pink2"))
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/mainE4.png"))

myData$A4 <- factor(myData$A4 , levels=c("High", "Medium_High", "Medium_Low", "Low"))
ggplot(myData, aes(A4, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge") + xlab("A4") + scale_fill_manual(values=c("blue4", "red4", "green4", "pink2"))
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/mainA4.png"))

myData$C2 <- factor(myData$C2 , levels=c("High", "Medium_High", "Medium_Low", "Low"))
ggplot(myData, aes(C2, ..count..)) + geom_bar(aes(fill = preferredVersion), position = "dodge") + xlab("C2") + scale_fill_manual(values=c("blue4", "red4", "green4", "pink2"))
suppressMessages(ggsave("plots/mainEffects/join/preferredVersion/mainC2.png"))

#print("Plotting main effects in joint boxplots...")
#mainBoxplotJoin("input/dataFourCategories.csv", "four", "grandMeanTakes")
# mainBoxplotJoin("input/dataFourCategories.csv", "four", "meanWhatFocus")
# mainBoxplotJoin("input/dataFourCategories.csv", "four", "meanWhoFocus")

#print("Plotting interactions in facets...")
# interactionBoxplotFacets("output/meltedData.csv", "integer", "takes")
# interactionBoxplotFacets("output/meltedData.csv", "integer", "what")
# interactionBoxplotFacets("output/meltedData.csv", "integer", "who")
# interactionBoxplotFacets("output/meltedDataTwoCategories.csv", "two", "takes")
# interactionBoxplotFacets("output/meltedDataTwoCategories.csv", "two", "what")
# interactionBoxplotFacets("output/meltedDataTwoCategories.csv", "two", "who")
# interactionBoxplotFacets("output/meltedDataThreeCategories.csv", "three", "takes")
# interactionBoxplotFacets("output/meltedDataThreeCategories.csv", "three", "what")
# interactionBoxplotFacets("output/meltedDataThreeCategories.csv", "three", "who")
# interactionBoxplotFacets("output/meltedDataFourCategories.csv", "four", "takes")
# interactionBoxplotFacets("output/meltedDataFourCategories.csv", "four", "what")
# interactionBoxplotFacets("output/meltedDataFourCategories.csv", "four", "who")

print("Plotting interactions in joint plots...")
# interactionBoxplotJoin("output/meltedData.csv", "integer", "takes")
# interactionBoxplotJoin("output/meltedData.csv", "integer", "what")
# interactionBoxplotJoin("output/meltedData.csv", "integer", "who")
# interactionBoxplotJoin("output/meltedDataTwoCategories.csv", "two", "takes")
# interactionBoxplotJoin("output/meltedDataTwoCategories.csv", "two", "what")
# interactionBoxplotJoin("output/meltedDataTwoCategories.csv", "two", "who")
# interactionBoxplotJoin("output/meltedDataThreeCategories.csv", "three", "takes")
# interactionBoxplotJoin("output/meltedDataThreeCategories.csv", "three", "what")
# interactionBoxplotJoin("output/meltedDataThreeCategories.csv", "three", "who")
interactionJoin("output/meltedDataFourCategories.csv", "four", "takes")
interactionJoin("output/meltedDataFourCategories.csv", "four", "what")
interactionJoin("output/meltedDataFourCategories.csv", "four", "who")

options(warn = oldw)

