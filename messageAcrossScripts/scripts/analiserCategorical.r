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
suppressMessages(library(tidyverse))
suppressMessages(library(sjPlot))
suppressMessages(library(sjmisc))

processMainEffectCategory <- function(filename, n){
  data <- read.csv(file=filename, header=TRUE, sep=",")
  data$preferredVersion <- unclass(data$preferredVersion)
  personalityVariables <- names(data)[2:39]

  invisible(lapply(personalityVariables, function(x) {
    result <- kruskal.test(eval(substitute(grandMeanTakes ~ personality, list(personality = as.name(x)))), data = data)
    filename = sprintf("./results/mainEffects/personality/categories/%s/grandMeanTakes/%s.messageAcrossData", n, x)
    print(result)
    write.table(data.frame(unlist(result)), file=filename, row.names=TRUE, col.names=TRUE)
    if(result[3] <= 0.05){
      posthoc<-suppressMessages(pairwise.wilcox.test(data$grandMeanTakes, eval(substitute(data$personality, list(personality = as.name(x)))), p.adj = "bonferroni", exact=FALSE))
      write.table(data.frame(unlist(posthoc)), file=sprintf("./results/mainEffects/personality/categories/%s/grandMeanTakes/Post_%s.messageAcrossData", n, x), row.names=TRUE, col.names=TRUE)
      print(posthoc)
    }
  }))

  invisible(lapply(personalityVariables, function(x) {
    result <- kruskal.test(eval(substitute(meanWhoFocus ~ personality, list(personality = as.name(x)))), data = data)
    filename = sprintf("./results/mainEffects/personality/categories/%s/meanWhoFocus/%s.messageAcrossData", n, x)
    print(result)
    write.table(data.frame(unlist(result)), file=filename, row.names=TRUE, col.names=TRUE)
    if(result[3] <= 0.05){
      posthoc<-suppressMessages(pairwise.wilcox.test(data$meanWhoFocus, eval(substitute(data$personality, list(personality = as.name(x)))), p.adj = "bonferroni", exact=FALSE))
      write.table(data.frame(unlist(posthoc)), file=sprintf("./results/mainEffects/personality/categories/%s/meanWhoFocus/Post_%s.messageAcrossData", n, x), row.names=TRUE, col.names=TRUE)
      print(posthoc)
    }
  }))

  invisible(lapply(personalityVariables, function(x) {
    result <- kruskal.test(eval(substitute(meanWhatFocus ~ personality, list(personality = as.name(x)))), data = data)
    filename = sprintf("./results/mainEffects/personality/categories/%s/meanWhatFocus/%s.messageAcrossData", n, x)
    print(result)
    write.table(data.frame(unlist(result)), file=filename, row.names=TRUE, col.names=TRUE)
    if(result[3] <= 0.05){
      posthoc<-suppressMessages(pairwise.wilcox.test(data$meanWhatFocus, eval(substitute(data$personality, list(personality = as.name(x)))), p.adj = "bonferroni", exact=FALSE))
      write.table(data.frame(unlist(posthoc)), file=sprintf("./results/mainEffects/personality/categories/%s/meanWhatFocus/Post_%s.messageAcrossData", n, x), row.names=TRUE, col.names=TRUE)
      print(posthoc)
    }
  }))
}

processInteractionCategory <- function(filename, n){
  meltedData <- read.csv(file=filename, header=TRUE, sep=",")

  personalityVariables <- names(meltedData)[2:39]
  #print(personalityVariables)

  #Compute the analysis of variance

  invisible(lapply(personalityVariables, function(x) {
      Mixed.aov.1 <- suppressMessages(aov_car(eval(substitute(takes ~ personality * ScoreSystem + Error(playerId/ScoreSystem), list(personality = as.name(x)))), data = meltedData))
      
      filename = sprintf("./results/interactionEffects/categories/%s/takes/%s.messageAcrossData", n, x)
      write(knitr::kable(nice(Mixed.aov.1)),file=filename,append=TRUE)

      if(grepl("*", nice(Mixed.aov.1)[[4]][[3]], fixed=TRUE)){
        Mixed_Fitted_Interaction<-suppressMessages(emmeans(Mixed.aov.1, eval(substitute(~ScoreSystem|personality, list(personality = as.name(x))))))
        write.table(Mixed_Fitted_Interaction, file=sprintf("./results/interactionEffects/categories/%s/takes/Post_%s.messageAcrossData", n, x), row.names=TRUE, col.names=TRUE)
      }
    }
  ))

  invisible(lapply(personalityVariables, function(x) {
      Mixed.aov.1 <- suppressMessages(aov_car(eval(substitute(what ~ personality * ScoreSystem + Error(playerId/ScoreSystem), list(personality = as.name(x)))), data = meltedData))
      
      filename = sprintf("./results/interactionEffects/categories/%s/what/%s.messageAcrossData", n, x)
      write(knitr::kable(nice(Mixed.aov.1)),file=filename,append=TRUE)

      if(grepl("*", nice(Mixed.aov.1)[[4]][[3]], fixed=TRUE)){
        Mixed_Fitted_Interaction<-suppressMessages(emmeans(Mixed.aov.1, eval(substitute(~ScoreSystem|personality, list(personality = as.name(x))))))
        write.table(Mixed_Fitted_Interaction, file=sprintf("./results/interactionEffects/categories/%s/what/Post_%s.messageAcrossData", n, x), row.names=TRUE, col.names=TRUE)
      }
    }
  ))

  invisible(lapply(personalityVariables, function(x) {
      Mixed.aov.1 <- suppressMessages(aov_car(eval(substitute(who ~ personality * ScoreSystem + Error(playerId/ScoreSystem), list(personality = as.name(x)))), data = meltedData))
      
      filename = sprintf("./results/interactionEffects/categories/%s/who/%s.messageAcrossData", n, x)
      write(knitr::kable(nice(Mixed.aov.1)),file=filename,append=TRUE)

      if(grepl("*", nice(Mixed.aov.1)[[4]][[3]], fixed=TRUE)){
        Mixed_Fitted_Interaction<-suppressMessages(emmeans(Mixed.aov.1, eval(substitute(~ScoreSystem|personality, list(personality = as.name(x))))))
        write.table(Mixed_Fitted_Interaction, file=sprintf("./results/interactionEffects/categories/%s/who/Post_%s.messageAcrossData", n, x), row.names=TRUE, col.names=TRUE)
      }
    }
  ))
}

print("Computing main effects...")
# print("Computing for two groups [High, Low]...")
# processMainEffectCategory("input/dataTwoCategories.csv", "two")
print("Computing for three groups [High, Medium, Low]...")
processMainEffectCategory("input/dataThreeCategories.csv", "three")
# print("Computing for four groups [High, Medium_High, Medium_Low, Low]...")
# processMainEffectCategory("input/dataFourCategories.csv", "four")

print("Computing interaction effects...")
# print("Computing for two groups [High, Low]...")
# processInteractionCategory("output/meltedDataTwoCategories.csv", "two")
print("Computing for three groups [High, Medium, Low]...")
processInteractionCategory("output/meltedDataThreeCategories.csv", "three")
# print("Computing for four groups [High, Medium_High, Medium_Low, Low]...")
# processInteractionCategory("output/meltedDataFourCategories.csv", "four")
