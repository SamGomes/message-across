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

myData <- read.csv(file="input/messageAcrossData.csv", header=TRUE, sep=",")
myData$preferredVersion <- unclass(myData$preferredVersion)

personalityVariables <- names(myData)[20:57]

print("Computing interaction effects...")

meltedData <- read.csv(file="output/meltedData.csv", header=TRUE, sep=",")

# Compute the analysis of variance
#   aov(eval(substitute(gives ~ personality * ScoreSystem, list(personality = as.name(x)))), data = meltedData)

#Below is an example using our “Mixed_Data” dataset.
# After the aov_car command, we enter the formula for our ANOVA model.
# In this case, we are looking at DV by (“~”) Btw_Cond and Within_Cond.
# Also we know that we must specify the error term we wish to use.
# In this example, the error term is represented by subject error divided by the within-subjects error (Within_Cond).
# IMPORTANTLY: If you have any between subjects variables, they do not need to be included in the error term. 

# Mixed.aov.1<-lapply(personalityVariables, function(x) {
#   aov_car(eval(substitute(gives ~ personality * ScoreSystem + Error(playerId/ScoreSystem), list(personality = as.name(x)))), data=meltedData)
#   }
# )

# res.aov <- lapply(personalityVariables, function(x) {
#   aov(eval(substitute(gives ~ personality * ScoreSystem, list(personality = as.name(x)))), data = meltedData)
#   }
# )
# text <- lapply(res.aov, summary)

Mixed.aov.1<-aov_car(gives ~ C*ScoreSystem + Error(playerId/ScoreSystem), data=meltedData)

knitr::kable(nice(Mixed.aov.1))

print("#####################################################")

Old.aov.1<-aov(gives ~ C*ScoreSystem, data=meltedData)

print(summary(Old.aov.1))

print("#####################################################")

Bew.aov.1<-aov(gives ~ C*ScoreSystem + Error(playerId/ScoreSystem), data=meltedData)

print(summary(Bew.aov.1))

print("#####################################################")

New.aov.1<-aov(gives ~ C*ScoreSystem + Error(playerId/(C*ScoreSystem)), data=meltedData)

print(summary(New.aov.1))
#print(Mixed.aov.1)
# print(text)

# datafilename="http://personality-project.org/R/datasets/R.appendix2.data"
# data.ex2=read.table(datafilename,header=T)   #read the data into a table
# data.ex2                                      #show the data
# aov.ex2 = aov(Alertness~Gender*Dosage,data=data.ex2)         #do the analysis of variance
# summary(aov.ex2)                                    #show the summary table
# print(model.tables(aov.ex2,"means"),digits=3)      
# 	#report the means and the number of subjects/cell
# boxplot(Alertness~Dosage*Gender,data=data.ex2) 
# 	#graphical summary of means of the 4 cells
# attach(data.ex2)
# interaction.plot(Dosage,Gender,Alertness)  #another way to graph the means 
# detach(data.ex2)

# Post-hoc

Mixed_Fitted_StudyMethod<-emmeans(Mixed.aov.1, ~ScoreSystem)
print(Mixed_Fitted_StudyMethod)

Mixed_Fitted_Personality<-emmeans(Mixed.aov.1, ~C)
print(Mixed_Fitted_Personality)

Mixed_Fitted_Interaction<-emmeans(Mixed.aov.1, ~ScoreSystem|C)
print(Mixed_Fitted_Interaction)
print(pairs(Mixed_Fitted_Interaction))