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

processGameVar <- function(yVarPre, yVarPos, xlabel, ylabel){

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
  out <- pairwise.wilcox.test(longData$value, longData$variable, paired= TRUE, p.adj = "none", exact=FALSE)
  #print(out)
  capture.output(out, file = sprintf("results/mainEffects/scoreSystem/postHoc/friedmanTestPostHoc %s.messageAcrossData",yVarPre))
}
# processGameVar("meanNumberOfGives_","")
processGameVar("meanNumberOfTakes_","")
processGameVar("whoFocus_","")
processGameVar("whatFocus_","")
processGameVar("score_","_7")


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