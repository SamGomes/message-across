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
suppressMessages(ggsave(sprintf("plots/mainEffects/preferredVersion/preferredVersion.png")))

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
  suppressMessages(ggsave(sprintf("plots/mainEffects/gameVariables/%s.png",plotName)))
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
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Traits",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + theme(text = element_text(size=20))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Traits")))

personalityVariables <- (myData %>% select(playerId, N1, N2, N3, N4, N5, N6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(15, 11, 13, 13, 12, 9))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(22, 17, 20, 19, 18, 15))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Neuroticism",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)+ theme(text = element_text(size=20))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_N")))

personalityVariables <- (myData %>% select(playerId, E1, E2, E3, E4, E5, E6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(19, 14, 11, 15, 15, 16))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(25, 20, 18, 19, 21, 21))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Extraversion",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)+ theme(text = element_text(size=20))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_E")))

personalityVariables <- (myData %>% select(playerId, O1, O2, O3, O4, O5, O6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(13, 16, 17, 14, 14, 15))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(20, 23, 22, 19, 21, 20))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Openness to Experience",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)+ theme(text = element_text(size=20))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_O")))

personalityVariables <- (myData %>% select(playerId, A1, A2, A3, A4, A5, A6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(16, 16, 20, 17, 18, 19))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(22, 25, 25, 25, 24, 24))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Agreeableness",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)+ theme(text = element_text(size=20))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_A")))

personalityVariables <- (myData %>% select(playerId, C1, C2, C3, C4, C5, C6))
data25th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(19, 17, 22, 18, 17, 21))
data75th <- data.frame(x= c("1", "2", "3", "4", "5", "6"), y = c(23, 23, 27, 23, 23, 24))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Facets of Conscientiousness",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)+ theme(text = element_text(size=20))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Facets_C")))

personalityVariables <- (myData %>% select(playerId, Internal, PowerfulOthers, Chance))
data25th <- data.frame(x= c("1", "2", "3"), y = c(12, 12, 12))
data75th <- data.frame(x= c("1", "2", "3"), y = c(36, 36, 36))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Dimensions from Locus of Control",y="Value") + coord_cartesian(ylim = c(0, 48))  + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)+ theme(text = element_text(size=20))
suppressMessages(ggsave(sprintf("plots/personality/%s.png", "Dimensions")))

oldw <- getOption("warn")
options(warn = -1)

print("Plotting main effects in facets...")

myData <- read.csv(file="input/dataThreeCategories.csv", header=TRUE, sep=",")

myData$preferredVersion <- factor(myData$preferredVersion, levels=c("D", "C", "B", "A"))

myData$N3 <- factor(myData$N3 , levels=c("Low", "Medium", "High"))
ggplot(myData,aes(x=factor(N3),fill=preferredVersion)) + geom_bar(position="fill") + geom_text(aes(label=..count..), stat='count', position=position_fill(vjust=0.5), color = "white", size = 10) + xlab("Depression") + ylab("percentage") + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3","#e7298a"), labels=c("Extreme Altruism", "Mutual Help", "Individualism", "Competitiveness"), guide = guide_legend(reverse = TRUE)) + labs(fill = "Version") + coord_flip() + theme(text = element_text(size=20))
suppressMessages(ggsave("plots/mainEffects/preferredVersion/versionN3.png", height = 5, width = 10))

myData$C2 <- factor(myData$C2 , levels=c("Low", "Medium", "High"))
ggplot(myData,aes(x=factor(C2),fill=preferredVersion)) + geom_bar(position="fill") + geom_text(aes(label=..count..), stat='count', position=position_fill(vjust=0.5), color = "white", size = 10) + xlab("Orderliness") + ylab("percentage") + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3","#e7298a"), labels=c("Extreme Altruism", "Mutual Help", "Individualism", "Competitiveness"), guide = guide_legend(reverse = TRUE)) + labs(fill = "Version") + coord_flip() + theme(text = element_text(size=20))
suppressMessages(ggsave("plots/mainEffects/preferredVersion/versionC2.png", height = 5, width = 10))

myData$C <- factor(myData$C , levels=c("Low", "Medium", "High"))
ggplot(myData,aes(x=factor(C),fill=preferredVersion)) + geom_bar(position="fill") + geom_text(aes(label=..count..), stat='count', position=position_fill(vjust=0.5), color = "white", size = 10) + xlab("Conscientiousness") + ylab("percentage") + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3","#e7298a"), labels=c("Extreme Altruism", "Mutual Help", "Individualism", "Competitiveness"), guide = guide_legend(reverse = TRUE)) + labs(fill = "Version") + coord_flip() + theme(text = element_text(size=20))
suppressMessages(ggsave("plots/mainEffects/preferredVersion/versionC.png", height = 5, width = 10))

print("Plotting interactions in joint plots...")

myData <- read.csv(file="output/meltedDataThreeCategories.csv", header=TRUE, sep=",")

levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "A"] <- "Competitiveness"
levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "B"] <- "Individualism"
levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "C"] <- "Mutual Help"
levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "D"] <- "Extreme Altruism"

myData$A1 <- factor(myData$A1 , levels=c("High", "Medium", "Low"))
ggplot(myData, aes(x=ScoreSystem, y=takes, fill=A1)) + geom_boxplot() + labs(x="Reward-Based Version", y="mean number of takes", fill="Trust") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(0,4)) + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3"))
suppressMessages(ggsave("plots/interactionEffects/takes/interactionA1.png", height = 5, width = 10))

myData$A6 <- factor(myData$A6 , levels=c("High", "Medium", "Low"))
ggplot(myData, aes(x=ScoreSystem, y=takes, fill=A6)) + geom_boxplot() + labs(x="Reward-Based Version", y="mean number of takes", fill="Sympathy") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(0,4)) + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3"))
suppressMessages(ggsave("plots/interactionEffects/takes/interactionA6.png", height = 5, width = 10))

myData$E4 <- factor(myData$E4 , levels=c("High", "Medium", "Low"))
ggplot(myData, aes(x=ScoreSystem, y=takes, fill=E4)) + geom_boxplot() + labs(x="Reward-Based Version", y="mean number of takes", fill="Activity Level") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(0,4)) + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3"))
suppressMessages(ggsave("plots/interactionEffects/takes/interactionE4.png", height = 5, width = 10))

myData$C4 <- factor(myData$C4 , levels=c("High", "Medium", "Low"))
ggplot(myData, aes(x=ScoreSystem, y=who, fill=C4)) + geom_boxplot() + labs(x="Reward-Based Version", y="focus", fill="Achievement-Striving") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(1,7)) + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3")) + scale_y_continuous("focus", labels = as.character(c("Me 1","2", "3", "4", "5", "6", "Other 7\n  Player  ")), breaks = c(1,2,3,4,5,6,7))
suppressMessages(ggsave("plots/interactionEffects/who/interactionC4.png", height = 7, width = 14))

myData$A4 <- factor(myData$A4 , levels=c("High", "Medium", "Low"))
ggplot(myData, aes(x=ScoreSystem, y=what, fill=A4)) + geom_boxplot() + labs(x="Reward-Based Version", y="intention", fill="Cooperation") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(1,7)) + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3")) + scale_y_continuous("intention", labels = as.character(c("Help 1","2", "3", "4", "5", "6", "Complicate 7")), breaks = c(1,2,3,4,5,6,7))
suppressMessages(ggsave("plots/interactionEffects/what/interactionA4.png", height = 7, width = 14))

options(warn = oldw)

