suppressMessages(library(ez))
suppressMessages(library(ggplot2))
suppressMessages(library(multcomp))
suppressMessages(library(nlme))
suppressMessages(library(pastecs))
suppressMessages(library(pgirmess))
suppressMessages(library(reshape2))
suppressMessages(library(WRS))
suppressMessages(library(e1071))
suppressMessages(library(rstatix))

myData <- read.csv(file="input/messageAcrossData.csv", header=TRUE, sep=",")
myData$preferredVersion <- unclass(myData$preferredVersion)

print("Computing normality tests...")
print("Computing main effects based on the score system...")

processGameVar <- function(myData, yVarPre, yVarPos, xlabel, ylabel){

  a <- paste(yVarPre,"A",yVarPos,sep="")
  b <- paste(yVarPre,"B",yVarPos,sep="")
  c <- paste(yVarPre,"C",yVarPos,sep="")
  d <- paste(yVarPre,"D",yVarPos,sep="")
  varsToProcess = c(a,b,c,d)

  keeps <- c("playerId", varsToProcess)
  keepsData <- myData[, (names(myData) %in% keeps)]
  longData <- melt(keepsData, id.vars=c("playerId"))
  dietMyData <- na.omit(keepsData)

  a <- dietMyData[[a]]
  b <- dietMyData[[b]]
  c <- dietMyData[[c]]
  d <- dietMyData[[d]]
  
  
  out <- shapiro.test(a)
  #print(out)
  capture.output(out, file = sprintf("results/normality/scoreSystem/shapiroTest %s_a.messageAcrossData",yVarPre))
 
  out <- shapiro.test(b)
  #print(out)
  capture.output(out, file = sprintf("results/normality/scoreSystem/shapiroTest %s_b.messageAcrossData",yVarPre))

  out <- shapiro.test(c)
  #print(out)
  capture.output(out, file = sprintf("results/normality/scoreSystem/shapiroTest %s_c.messageAcrossData",yVarPre))

  out <- shapiro.test(d)
  #print(out)
  capture.output(out, file = sprintf("results/normality/scoreSystem/shapiroTest %s_d.messageAcrossData",yVarPre))


  out <- friedman.test(longData$value, longData$variable, longData$playerId)
  #print(out)
  capture.output(out, file = sprintf("results/mainEffects/scoreSystem/friedmanTest %s.messageAcrossData",yVarPre))

  out <- friedman_effsize(longData, value ~ variable | playerId)
  #print(out)
  capture.output(out, file = sprintf("results/mainEffects/scoreSystem/effSize/friedmanEffSize %s.messageAcrossData",yVarPre))

  # out <- friedmanmc(longData$value, longData$variable, longData$playerId)
  out <- pairwise.wilcox.test(longData$value, longData$variable, paired= TRUE, p.adj = "bonferroni", exact=FALSE)
  #print(out)
  capture.output(out, file = sprintf("results/mainEffects/scoreSystem/postHoc/friedmanTestPostHoc %s.messageAcrossData",yVarPre))
}
# processGameVar("meanNumberOfGives_","")
processGameVar(myData, "meanNumberOfTakes_","")
processGameVar(myData, "whoFocus_","")
processGameVar(myData, "whatFocus_","")


varsToProcess = c("score_A_7","score_B_7","score_C_7","score_D_7")
keeps <- c("playerId", varsToProcess)
data <- myData[, (names(myData) %in% keeps)]
data$playerId <- factor(data$playerId, levels = c("121","122","125","126","84","118","9","111","124","110","136","127","62","119","100","102","82","135","3","78","31","109","33","117","67","103","91","112","70","17","88","47","130","123","113","114","83","133","48","34","107","132","24","134","18","120","46","143","146","147","150","151","129","153","154","155","148","149","144","145","116","158","160","159","69","76","156","157","141","142","161","162","87","166","168","167","164","165"))
data <- data[order(data$playerId),]
scoreData = data.frame(matrix(ncol = 0, nrow = 33))

j <- 1
for(i in  seq(from=1, to=dim(data)[1], by=2)) {
  scoreData$playerId[j] <- data$playerId[i] #assume the id of the group is the first id of the players
  scoreData$scoreDiff_A[j] <- abs(data$score_A_7[i]-data$score_A_7[i+1])
  scoreData$scoreDiff_B[j] <- abs(data$score_B_7[i]-data$score_B_7[i+1])
  scoreData$scoreDiff_C[j] <- abs(data$score_C_7[i]-data$score_C_7[i+1])
  scoreData$scoreDiff_D[j] <- abs(data$score_D_7[i]-data$score_D_7[i+1])
  j <- j+1
}


processGameVar(scoreData, "scoreDiff_","")


# print("Computing main effects based on personality...")
# personalityVariables <- names(myData)[20:57]


# processGameVar <- function(varToTestText){
#   # print(paste("varToTest: ",varToTest,sep=""))

#   varToTest <- myData[[varToTestText]]
#   out <- shapiro.test(varToTest)
#   #print(out)
#   capture.output(out, file = sprintf("results/normality/derivedMeasures/shapiroTest_%s.messageAcrossData", varToTestText))

#   # print("#################HERE###################")

#   for (xText in personalityVariables){
#       x <- myData[[xText]]
#       out <- shapiro.test(x)
#       capture.output(out, file = sprintf("results/normality/personality/shapiroTest_%s.messageAcrossData", xText))
#       #print(sprintf("%s-> Median(%f);Mean(%f)", xText, median(x), mean(x)))

#       out <- (cor.test(x, varToTest, method=c("spearman"), exact=F))
#       # if(test$p.value <= 0.05){
#         #print(test)
#         capture.output(out, file = sprintf("results/mainEffects/personality/%s/spearmanPersonalityResults_%s.messageAcrossData",varToTestText, xText))
#       # }
#     }
# }

# processGameVar("meanWhoFocus")
# processGameVar("meanWhatFocus")
# #processGameVar("grandMeanGives")
# processGameVar("grandMeanTakes")
# #processGameVar("ratioTakesGives")
# processGameVar("preferredVersion")

# # print("Computing interaction effects...")

# meltedData <- read.csv(file="output/meltedData.csv", header=TRUE, sep=",")

# # Compute the analysis of variance
# res.aov <- lapply(personalityVariables, function(x) {
#   aov(eval(substitute(gives ~ personality * ScoreSystem, list(personality = as.name(x)))), data = meltedData)
#   }
# )
# text <- lapply(res.aov, summary)
# #print(text)
# capture.output(text, file = "results/interactionEffects/givesMixedAnovaResults.messageAcrossData")

# # Compute the analysis of variance
# res.aov <- lapply(personalityVariables, function(x) {
#   aov(eval(substitute(takes ~ personality * ScoreSystem, list(personality = as.name(x)))), data = meltedData)
#   }
# )
# text <- lapply(res.aov, summary)
# #print(text)
# capture.output(text, file = "results/interactionEffects/takesMixedAnovaResults.messageAcrossData")


# # Compute the analysis of variance
# res.aov <- lapply(personalityVariables, function(x) {
#   aov(eval(substitute(who ~ personality * ScoreSystem, list(personality = as.name(x)))), data = meltedData)
#   }
# )
# text <- lapply(res.aov, summary)
# #print(text)
# capture.output(text, file = "results/interactionEffects/whoMixedAnovaResults.messageAcrossData")


# # Compute the analysis of variance
# res.aov <- lapply(personalityVariables, function(x) {
#   aov(eval(substitute(what ~ personality * ScoreSystem, list(personality = as.name(x)))), data = meltedData)
#   }
# )
# text <- lapply(res.aov, summary)
# #print(text)
# capture.output(text, file = "results/interactionEffects/whatMixedAnovaResults.messageAcrossData")