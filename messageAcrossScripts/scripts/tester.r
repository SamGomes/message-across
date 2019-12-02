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

myData <- read.csv(file="input/dataFourCategories.csv", header=TRUE, sep=",")
#myData <- read.csv(file="output/meltedDataFourCategories.csv", header=TRUE, sep=",")
#myData$preferredVersion <- unclass(myData$preferredVersion)

personalityVariables <- names(myData)[2:39]

# result <- kruskal.test(preferredVersion ~ A4, data = myData)
# print(result)
# posthoc <- pairwise.wilcox.test(myData$preferredVersion, myData$A4, p.adj = "none", exact=FALSE)
# print(posthoc)

# print(wilcox.test(preferredVersion ~ A4, data = myData))

# result <- fisher.test(x = myData$preferredVersion, y = myData$A4)
# print(result)

invisible(lapply(personalityVariables, function(x) {
	print(x)
    result <- fisher.test(x = myData$preferredVersion, eval(substitute(myData$personality, list(personality = as.name(x)))))
    if(result <= 0.05){
		print(result)
		Matrix <- table(myData[, c(x, "preferredVersion")])
		print(Matrix)
		PT = pairwiseNominalIndependence(Matrix, fisher = TRUE, gtest  = FALSE, chisq  = FALSE, digits = 3)
		print(PT)
		#print(cldList(comparison = PT$Comparison, p.value = PT$p.adj.Fisher, threshold  = 0.05))
    }
  }))

# png('myPlot.png')
# myData$A6 <- factor(myData$A6 , levels=c("High", "Medium_High", "Medium_Low", "Low"))
# interaction.plot(	x.factor     = myData$ScoreSystem,
# 	                trace.factor = myData$A6,
# 	                response     = myData$takes,
# 	                fun = mean,
# 	                type="b",
# 	                col=c("blue4", "red4", "green4", "pink2"),  ### Colors for levels of trace var.
# 	                pch=c(19, 17, 15, 21),             ### Symbols for levels of trace var.
# 	                fixed=TRUE,                    ### Order by factor order in data
# 	                ylab = sprintf("mean of takes"),
# 	                xlab = "Score System",
# 	                xpd=TRUE,
# 	                lwd = 3,
# 	                legend=FALSE,
# 	                lty=1,
# 	                yaxt = "n")
# axis(2, at=seq(0, 4, by = .25), labels=seq(0, 4, by = .25), las = 1)
# legend("topright", legend=c("High", "Medium_High", "Medium_Low", "Low"),
#        col=c("blue4", "red4", "green4", "pink2"), lty=1, pch=c(19, 17, 15, 21), cex=1,
#        title="Personality Group", text.font=1)
# dev.off()

# print(describe.by(myData,myData$O6))