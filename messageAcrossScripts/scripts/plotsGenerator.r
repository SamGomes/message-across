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
longData = merge(longData1, longData2, by = c("playerId","version"))
longDataSplitted <- split(longData,longData$version)
longDataMean = data.frame(matrix(ncol = 0, nrow = 4))
j = 1
for(i in  c("Comp","Ind","M.Help","E.Altr")) {
  longDataMean$version[j] <- i
  longDataMean$focus[j] <- mean(longDataSplitted[[i]]$focus)
  longDataMean$intention[j] <- mean(longDataSplitted[[i]]$intention)
  j <- j+1
}

plot <- ggplot(longData) 
plot <- plot + geom_count(show.legend=F)  + facet_wrap(version ~ .)

# draw grid lines
plot <- plot + geom_hline(aes(yintercept = 3), color="gray", linetype="dashed")
plot <- plot + geom_hline(aes(yintercept = 5), color="gray", linetype="dashed")
plot <- plot + geom_vline(aes(xintercept = 3), color="gray", linetype="dashed")
plot <- plot + geom_vline(aes(xintercept = 5), color="gray", linetype="dashed")
plot <- plot + geom_hline(data= longDataMean, aes(yintercept = intention), color="red")
plot <- plot + geom_vline(data= longDataMean, aes(xintercept = focus), color="red")

plot <- plot + aes(x=longData$focus, y=longData$intention, color=longData$version, background=longData$version) + scale_color_npg()
plot <- plot + scale_x_continuous(longData$focus, name="Focus", labels = as.character(c("Me 1","2", "3", "4\nNeutral", "5", "6", "7\nThe \n Other \n  Player  ")), breaks = c(1,2,3,4,5,6,7))
plot <- plot + scale_y_continuous(longData$intention, name="Intention", labels = as.character(c("Help 1","2", "3", "Neutral 4", "5", "6", "Complicate 7")), breaks = c(1,2,3,4,5,6,7)) 

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

