suppressMessages(library(ez))
suppressMessages(library(ggplot2))
suppressMessages(library(multcomp))
suppressMessages(library(nlme))
suppressMessages(library(pastecs))
suppressMessages(library(pgirmess))
suppressMessages(library(reshape))
suppressMessages(library(WRS))
suppressMessages(library(e1071))
suppressMessages(library(afex))
suppressMessages(library(emmeans))
suppressMessages(library(rcompanion))
suppressMessages(library(psych))
suppressMessages(library(tidyverse))

#myData <- read.csv(file="input/dataThreeCategories.csv", header=TRUE, sep=",")
myData <- read.csv(file="output/meltedDataThreeCategories.csv", header=TRUE, sep=",")
#myData <- read.csv(file="input/messageAcrossData.csv", header=TRUE, sep=",")
#myData$preferredVersion <- unclass(myData$preferredVersion)

personalityVariables <- names(myData)[2:39]

# result <- kruskal.test(preferredVersion ~ A4, data = myData)
# print(result)
# posthoc <- pairwise.wilcox.test(myData$preferredVersion, myData$A4, p.adj = "none", exact=FALSE)
# print(posthoc)

# print(wilcox.test(preferredVersion ~ A4, data = myData))

# result <- fisher.test(x = myData$preferredVersion, y = myData$A4)
# print(result)

# invisible(lapply(personalityVariables, function(x) {
# 	print(x)
#     result <- fisher.test(x = myData$preferredVersion, eval(substitute(myData$personality, list(personality = as.name(x)))))
#     if(result <= 0.05){
# 		print(result)
# 		Matrix <- table(myData[, c(x, "preferredVersion")])
# 		print(Matrix)
# 		PT = pairwiseNominalIndependence(Matrix, fisher = TRUE, gtest  = FALSE, chisq  = FALSE, digits = 3)
# 		print(PT)
# 		#print(cldList(comparison = PT$Comparison, p.value = PT$p.adj.Fisher, threshold  = 0.05))
#     }
#   }))

#png('myPlot.png')
# personalityVariables <- (myData %>% select(playerId, N, E, O, A, C))
# data25th <- data.frame(x= c("1", "2", "3", "4", "5"), y = c(79, 95, 95, 112, 123))
# data75th <- data.frame(x= c("1", "2", "3", "4", "5"), y = c(106, 118, 119, 137, 143))
# plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value)) + geom_violin() + labs(x="Traits",y="Value") + geom_segment(data = data25th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2) + geom_segment(data = data75th, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 2)
# suppressMessages(ggsave(sprintf("%s.png", "Traits")))


# myData$N3 <- factor(myData$N3 , levels=c("Low", "Medium", "High"))
# myData$preferredVersion <- factor(myData$preferredVersion, levels=c("D", "C", "B", "A"))
# #ggplot(myData, aes(N3, ..count..)) + geom_bar(aes(fill = factor(preferredVersion)), position = "fill") + xlab("N3") + ylab("percentage") + scale_fill_manual(values=c("blue4", "red4", "green4", "pink2")) + coord_flip() + geom_text(aes(label=..count..), stat='count', position=position_fill(vjust=0.5), color="white")
# ggplot(myData,aes(x=factor(N3),fill=preferredVersion)) + geom_bar(position="fill") + geom_text(aes(label=..count..), stat='count', position=position_fill(vjust=0.5), color = "white") + xlab("N3") + ylab("percentage") + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3","#e7298a"), labels=c("D", "C", "B", "A"), guide = guide_legend(reverse = TRUE)) + labs(fill = "Version") + coord_flip()
# suppressMessages(ggsave("mainN3.png"))

#mapvalues(myData$ScoreSystem, from = c("D", "C", "B", "A"), to = c("Extreme Altruism", "Mutual Help", "Individualism", "Competitiveness"))
#revalue(myData$ScoreSystem, c("A"="Competitiveness", "B"="Individualism", "C"="Mutual Help", "D"="Extreme Altruism"))

levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "A"] <- "Competitiveness"
levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "B"] <- "Individualism"
levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "C"] <- "Mutual Help"
levels(myData$ScoreSystem)[levels(myData$ScoreSystem) == "D"] <- "Extreme Altruism"

myData$C4 <- factor(myData$C4 , levels=c("High", "Medium", "Low"))
ggplot(myData, aes(x=ScoreSystem, y=who, fill=C4)) + geom_boxplot() + labs(x="Reward-Based Version", y="focus", fill="Achievement-Striving") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(1,7)) + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3")) + scale_y_continuous("focus", labels = as.character(c("Me 1","2", "3", "4", "5", "6", "Other 7\n  Player  ")), breaks = c(1,2,3,4,5,6,7))
suppressMessages(ggsave("myPlot.png", height = 7, width = 14))

myData$A4 <- factor(myData$A4 , levels=c("High", "Medium", "Low"))
ggplot(myData, aes(x=ScoreSystem, y=what, fill=A4)) + geom_boxplot() + labs(x="Reward-Based Version", y="intention", fill="Cooperation") + theme(text = element_text(size=20)) + coord_cartesian(ylim=c(1,7)) + scale_fill_manual(values=c("#1b9e77", "#d95f02", "#7570b3")) + scale_y_continuous("intention", labels = as.character(c("Help 1","2", "3", "4", "5", "6", "Complicate 7")), breaks = c(1,2,3,4,5,6,7))
suppressMessages(ggsave("myPlot2.png", height = 7, width = 14))
#dev.off()



# print(describe.by(myData,myData$O6))

