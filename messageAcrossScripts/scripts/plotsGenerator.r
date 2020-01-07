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
suppressMessages(library(ggsci))


myData <- read.csv(file="input/messageAcrossData.csv", header=TRUE, sep=",")
plot <- ggplot(myData, aes(fill=myData$preferredVersion, y=((..count..)/sum(..count..)*100), x=1)) 
plot <- plot + geom_bar() + labs(x="", fill="Preferred Version", y="") 
plot <- plot + scale_x_discrete(breaks=c(0)) + scale_y_discrete(breaks=c(0))
plot <- plot + scale_fill_npg(labels = as.character(c("Comp","Ind","M.Help","E.Altr")))
plot <- plot + theme(legend.title=element_text(size=20), legend.text=element_text(size=18),axis.text=element_text(size=18), axis.title=element_text(size=24,face="bold"),panel.background = element_blank()) 
plot <- plot + geom_text(stat='count', size=8, aes(label=..count.., x=1, y=(..count../sum(..count..))*100), position = position_stack(vjust=0.5)) + coord_flip()
suppressMessages(ggsave(sprintf("plots/mainEffects/preferredVersion.png"), width = 8, height = 4))


print("Plotting game variables...")
processBoxPlot <- function(myData, yVarPre, yVarPos, yLabel, plotName, labels, breaks){

  varsToProcess = c(sprintf("%sA%s",yVarPre,yVarPos),sprintf("%sB%s",yVarPre,yVarPos),sprintf("%sC%s",yVarPre,yVarPos),sprintf("%sD%s",yVarPre,yVarPos))

  keeps <- c("playerId", varsToProcess)
  data <- myData[, (names(myData) %in% keeps)]
  longData <- melt(data, id="playerId", measured=varsToProcess)

  names(longData)<-c("playerId", "scoreSystem", "yVar")
  longData$scoreSystem <- factor(longData$scoreSystem, labels=c("Comp","Ind","M.Help","E.Altr"))
  longData <- longData[order(longData$playerId),]

  hist <- ggplot(longData, aes(longData$scoreSystem, longData$yVar, fill=longData$scoreSystem)) + theme(legend.position="none", axis.text=element_text(size=18), axis.title=element_text(size=18, face="bold"))
  hist <- hist + geom_boxplot()
  # hist <- hist + scale_fill_manual()
  if( labels!=-1 && breaks!=-1){
    hist <- hist +  scale_y_continuous(yLabel, labels = as.character(labels), breaks = breaks)
  }
  hist <- hist + labs(x="Score Attribution System", y=yLabel)  + scale_fill_npg()
  suppressMessages(ggsave(sprintf("plots/%s.png",plotName)))
}
processBoxPlot(myData, "meanNumberOfGives_", "", "Mean number of gives", "meanNumberOfGives", -1, -1)
processBoxPlot(myData, "meanNumberOfTakes_", "", "Mean number of takes", "meanNumberOfTakes", -1, -1)
processBoxPlot(myData, "whoFocus_", "", "Interaction focus", "interactionFocus", c("Me 1","2", "3", "Neutral 4", "5", "6", "The    \n Other 7\n  Player  "), c(1,2,3,4,5,6,7))
processBoxPlot(myData, "whatFocus_", "", "Interaction intention", "interactionIntention", c("Help   \n other 1\n player  ","2", "3", "Neutral 4", "5", "6", "Complicate   \n other 7\n player  "), c(1,2,3,4,5,6,7))



yVarPre="whoFocus_"
yVarPos=""
varsToProcess = c(sprintf("%sA%s",yVarPre,yVarPos),sprintf("%sB%s",yVarPre,yVarPos),sprintf("%sC%s",yVarPre,yVarPos),sprintf("%sD%s",yVarPre,yVarPos))

keeps <- c("playerId", varsToProcess)
data <- myData[, (names(myData) %in% keeps)]
longData1 <- melt(data, id="playerId", measured=varsToProcess)
colnames(longData1)[colnames(longData1)=="variable"] <- "version"
colnames(longData1)[colnames(longData1)=="value"] <- "focus"
longData1$version <- factor(longData1$version, labels=c("Comp","Ind","M.Help","E.Altr"))

yVarPre="whatFocus_"
yVarPos=""
varsToProcess = c(sprintf("%sA%s",yVarPre,yVarPos),sprintf("%sB%s",yVarPre,yVarPos),sprintf("%sC%s",yVarPre,yVarPos),sprintf("%sD%s",yVarPre,yVarPos))

keeps <- c("playerId", varsToProcess)
data <- myData[, (names(myData) %in% keeps)]
longData2 <- melt(data, id="playerId", measured=varsToProcess)
colnames(longData2)[colnames(longData2)=="variable"] <- "version"
colnames(longData2)[colnames(longData2)=="value"] <- "intention"
longData2$version <- factor(longData2$version, labels=c("Comp","Ind","M.Help","E.Altr"))


# focus and intention
longData = merge(longData1,longData2, by = c("playerId","version"))
plot <- ggplot(longData) 

# draw grid lines
plot <- plot + geom_hline(aes(yintercept = 3), color="gray", linetype="dashed")
plot <- plot + geom_hline(aes(yintercept = 5), color="gray", linetype="dashed")
plot <- plot + geom_vline(aes(xintercept = 3), color="gray", linetype="dashed")
plot <- plot + geom_vline(aes(xintercept = 5), color="gray", linetype="dashed")
plot <- plot + geom_hline(aes(yintercept=mean(longData$version ~ longData$focus), color="red"))
plot <- plot + geom_hline(aes(yintercept=mean(longData$version ~ longData$intention), color="red"))

plot <- plot + geom_count(show.legend=F)  + facet_wrap(longData$version ~ .)
plot <- plot + aes(x=longData$focus, y=longData$intention, color=longData$version, background=longData$version) + scale_color_npg()
plot <- plot + scale_x_continuous(longData$focus, name="Focus", labels = as.character(c("Me 1","2", "3", "Neutral 4", "5", "6", "7\nThe \n Other \n  Player  ")), breaks = c(1,2,3,4,5,6,7))
plot <- plot + scale_y_continuous(longData$intention, name="Intention", labels = as.character(c("Help 1","2", "3", "4\nNeutral", "5", "6", "Complicate 7")), breaks = c(1,2,3,4,5,6,7)) 

suppressMessages(ggsave(sprintf("plots/mainEffects/focusAndIntention.png"), width = 8, height = 4))


varsToProcess = c("score_A_7","score_B_7","score_C_7","score_D_7")
keeps <- c("playerId", varsToProcess)
data <- myData[, (names(myData) %in% keeps)]
data$playerId <- factor(data$playerId, levels = c("121","122","125","126","84","118","9","111","124","110","136","127","62","119","100","102","82","135","3","78","31","109","33","117","67","103","91","112","70","17","88","47","130","123","113","114","83","133","48","34","107","132","24","134","18","120","46","143","146","147","150","151","129","153","154","155","148","149","144","145","116","158","160","159","69","76","156","157","141","142","161","162","87","166","168","167","164","165"))
data <- data[order(data$playerId),]
scoreData = data.frame(matrix(ncol = 0, nrow = 33))

j <- 1
for(i in  seq(from=1, to=dim(data)[1], by=2)) {
  scoreData$playerId[j] <- data$playerId[i] #assume the id of the group is the first id of the players
  scoreData$score_A[j] <- abs(data$score_A_7[i]-data$score_A_7[i+1])
  scoreData$score_B[j] <- abs(data$score_B_7[i]-data$score_B_7[i+1])
  scoreData$score_C[j] <- abs(data$score_C_7[i]-data$score_C_7[i+1])
  scoreData$score_D[j] <- abs(data$score_D_7[i]-data$score_D_7[i+1])
  j <- j+1
}

processBoxPlot(scoreData, "score_", "", "Final Score Diff.", "scoreDiffs", -1, -1)


actionsVariables <- (myData %>% select(playerId, grandMeanTakes, grandMeanGives, ratioTakesGives))
plot <- ggplot(melt(actionsVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Actions",y="Value")
plot <- plot
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

