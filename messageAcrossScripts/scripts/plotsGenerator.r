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
plot <- ggplot(myData, aes(myData$preferredVersion)) + geom_bar(fill='#c4d4ff', color="black") + labs(x="Preferred Version",y="Frequencies") + theme(axis.text=element_text(size=18), axis.title=element_text(size=18,face="bold")) +  scale_x_discrete(labels = as.character(c("Comp","Ind","M.Help","E.Altr"))) + geom_text(stat='count', aes(label=..count..), vjust=-1)
suppressMessages(ggsave(sprintf("plots/mainEffects/PreferredVersion.png")))

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
  suppressMessages(ggsave(sprintf("plots/mainEffects/%s.png",plotName)))
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
data25th <- data.frame(x= c("1", "2", "3", "4", "5"), y = c(79, 95, 95, 112, 123))
data75th <- data.frame(x= c("1", "2", "3", "4", "5"), y = c(106, 118, 119, 137, 143))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Traits",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Traits")))

personalityVariables <- (myData %>% select(playerId, N1, N2, N3, N4, N5, N6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(15, 11, 13, 13, 12, 9))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(22, 17, 20, 19, 18, 15))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Neuroticism",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_N")))

personalityVariables <- (myData %>% select(playerId, E1, E2, E3, E4, E5, E6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(19, 14, 11, 15, 15, 16))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(25, 20, 18, 19, 21, 21))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Extraversion",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_E")))

personalityVariables <- (myData %>% select(playerId, O1, O2, O3, O4, O5, O6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(13, 16, 17, 14, 14, 15))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(20, 23, 22, 19, 21, 20))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Openness to Experience",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_O")))

personalityVariables <- (myData %>% select(playerId, A1, A2, A3, A4, A5, A6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(16, 16, 20, 17, 18, 19))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(22, 25, 25, 25, 24, 24))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Agreeableness",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_A")))

personalityVariables <- (myData %>% select(playerId, C1, C2, C3, C4, C5, C6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(19, 17, 22, 18, 17, 21))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(23, 23, 27, 23, 23, 24))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Conscientiousness",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_C")))

personalityVariables <- (myData %>% select(playerId, Internal, PowerfulOthers, Chance))
data25th <- data.frame(x= c("1", "2", "3"), y = c(12, 12, 12))
data75th <- data.frame(x= c("1", "2", "3"), y = c(36, 36, 36))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Dimensions from Locus of Control",y="Value") + coord_cartesian(ylim = c(0, 48))  + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)
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
      xText <- factor(xText , levels=c("High", "Medium", "Low"))
      interaction.plot( x.factor     = meltedData$ScoreSystem,
                        trace.factor = xText,
                        response     = meltedData[[yVar]],
                        fun = mean,
                        type="b",
                        col=c("blue4", "red4", "green4"),  ### Colors for levels of trace var.
                        pch=c(19, 17, 15),             ### Symbols for levels of trace var.
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
        legend("topleft", legend=c("High", "Medium", "Low"),
             col=c("blue4", "red4", "green4"), lty=1, pch=c(19, 17, 15, 21), cex=1,
             title="Personality Group", text.font=1)
      }
      else{
        legend("topright", legend=c("High", "Medium", "Low"),
             col=c("blue4", "red4", "green4"), lty=1, pch=c(19, 17, 15, 21), cex=1,
             title="Personality Group", text.font=1)
      }
      dev.off()
	 }
}

oldw <- getOption("warn")
options(warn = -1)

# print("Plotting main effects in facets...")

myData <- read.csv(file="input/dataThreeCategories.csv", header=TRUE, sep=",")

myData$preferredVersion <- factor(myData$preferredVersion, levels=c("D", "C", "B", "A"))

myData$N3 <- factor(myData$N3 , levels=c("Low", "Medium", "High"))
ggplot(myData,aes(x=factor(N3),fill=preferredVersion)) + geom_bar(position="fill") + geom_text(aes(label=..count..), stat='count', position=position_fill(vjust=0.5), color = "white") + xlab("N3") + ylab("percentage") + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3","#e7298a"), labels=c("D", "C", "B", "A"), guide = guide_legend(reverse = TRUE)) + labs(fill = "Version") + coord_flip()
suppressMessages(ggsave("plots/personality/mainN3.png"))

myData$C2 <- factor(myData$C2 , levels=c("Low", "Medium", "High"))
ggplot(myData,aes(x=factor(C2),fill=preferredVersion)) + geom_bar(position="fill") + geom_text(aes(label=..count..), stat='count', position=position_fill(vjust=0.5), color = "white") + xlab("C2") + ylab("percentage") + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3","#e7298a"), labels=c("D", "C", "B", "A"), guide = guide_legend(reverse = TRUE)) + labs(fill = "Version") + coord_flip()
suppressMessages(ggsave("plots/personality/mainC2.png"))

myData$C <- factor(myData$C , levels=c("Low", "Medium", "High"))
ggplot(myData,aes(x=factor(C),fill=preferredVersion)) + geom_bar(position="fill") + geom_text(aes(label=..count..), stat='count', position=position_fill(vjust=0.5), color = "white") + xlab("C") + ylab("percentage") + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3","#e7298a"), labels=c("D", "C", "B", "A"), guide = guide_legend(reverse = TRUE)) + labs(fill = "Version") + coord_flip()
suppressMessages(ggsave("plots/personality/mainC.png"))

#print("Plotting main effects in joint boxplots...")
# mainBoxplotJoin("input/dataFourCategories.csv", "four", "grandMeanTakes")
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
interactionJoin("output/meltedDataThreeCategories.csv", "three", "takes")
interactionJoin("output/meltedDataThreeCategories.csv", "three", "what")
interactionJoin("output/meltedDataThreeCategories.csv", "three", "who")
# interactionJoin("output/meltedDataFourCategories.csv", "four", "takes")
# interactionJoin("output/meltedDataFourCategories.csv", "four", "what")
# interactionJoin("output/meltedDataFourCategories.csv", "four", "who")

options(warn = oldw)

