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
#myData <- read.csv(file="output/meltedDataThreeCategories.csv", header=TRUE, sep=",")
myData <- read.csv(file="input/messageAcrossData.csv", header=TRUE, sep=",")
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
personalityVariables <- (myData %>% select(playerId, N, E, O, A, C))
dat2 <- data.frame(x= c("1", "2", "3", "4", "5"), y = c(100, 120, 140, 120, 100))
plot <- ggplot(melt(personalityVariables, id="playerId"), aes(x = variable, y = value))  + geom_boxplot() + labs(x="Traits",y="Value") + geom_segment(data = dat2, color = "red", aes(x = as.numeric(x) - 0.3, y = y, xend = as.numeric(x) + 0.3, yend = y), size = 3)
suppressMessages(ggsave(sprintf("%s.png", "Traits")))
#dev.off()

# print(describe.by(myData,myData$O6))

